/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MSR.Data.Entities;
using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public interface IPredictorModel
	{
		void OpenConfig(string fileName);
		void Predict(IEnumerable<string> releases, int modelNumber, bool evaluate, bool showFiles);
		IEnumerable<string> Releases { get; }
		IEnumerable<string> Models { get; }
		
		string Report { get; }
	}
	
	public class PredictorModel : IPredictorModel
	{
		private PredictionTool tool;
		
		public void OpenConfig(string fileName)
		{
			tool = new PredictionTool(fileName);
		}
		public IEnumerable<string> Releases
		{
			get
			{
				using (var s = tool.Data.OpenSession())
				{
					return (from r in s.Repository<Release>() select r.Tag).ToList();
				}
			}
		}
		public IEnumerable<string> Models
		{
			get { return tool.Models.Models().Select(x => x.Title); }
		}
		public void Predict(IEnumerable<string> releases, int modelNumber, bool evaluate, bool showFiles)
		{
			StringBuilder report = new StringBuilder();
			string evaluationResult = null;
			
			var model = tool.Models.Models()[modelNumber];
			using (var s = tool.Data.OpenSession())
			{
				var releaseRevisions = 
					from r in s.Repository<Release>().Where(x => releases.Contains(x.Tag))
					join c in s.Repository<Commit>() on r.CommitID equals c.ID
					select c.Revision;
				model.Init(s, releaseRevisions);
				model.Predict();
				if (evaluate)
				{
					evaluationResult = model.Evaluate().ToString();
				}
			}

			report.AppendLine(model.Title);
			report.Append("Releases: ");
			foreach (var r in releases)
			{
				report.Append(r + ", ");
			}
			report.AppendLine();
			if (showFiles || (! evaluate))
			{
				report.AppendLine("Predicted defect files:");
				foreach (var f in model.PredictedDefectFiles)
				{
					report.AppendLine(
						string.Format("{0} {1}", f, evaluate ? model.DefectFiles.Contains(f) ? "+" : "-" : "")
					);
				}
			}
			if (evaluate)
			{
				if (showFiles)
				{
					report.AppendLine("Defect files:");
					foreach (var f in model.DefectFiles.OrderBy(x => x))
					{
						report.AppendLine(string.Format("{0} {1}", f, model.PredictedDefectFiles.Contains(f) ? "+" : "-"));
					}
				}
				report.AppendLine(evaluationResult);
			}
			Report = report.ToString();
		}
		public string Report
		{
			get; private set;
		}
	}
}
