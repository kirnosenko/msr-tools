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
		event Action OnClearReport;
		event Action<string> OnAddReport;
		
		void OpenConfig(string fileName);
		void Predict(bool evaluate, bool showFiles);
		IDictionary<string,string> Releases { get; }
		IDictionary<string,string> SelectedReleases { get; set; }
		PostReleaseDefectFilesPrediction[] Models { get; }
		IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels { get; set; }

		
	}
	
	public class PredictorModel : IPredictorModel
	{
		public event Action OnClearReport;
		public event Action<string> OnAddReport;
		
		private PredictionTool tool;
		
		public PredictorModel()
		{
		}
		public void OpenConfig(string fileName)
		{
			tool = new PredictionTool(fileName);
		}
		public IDictionary<string,string> Releases
		{
			get { return tool.Releases; }
		}
		public IDictionary<string,string> SelectedReleases
		{
			get; set;
		}
		public PostReleaseDefectFilesPrediction[] Models
		{
			get { return tool.Models.Models(); }
		}
		public IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels
		{
			get; set;
		}
		public void Predict(bool evaluate, bool showFiles)
		{
			StringBuilder report = new StringBuilder();
			string evaluationResult = null;
			
			OnClearReport();
			foreach (var model in SelectedModels)
			{
				using (var s = tool.Data.OpenSession())
				{
					model.Init(s, SelectedReleases.Values);
					model.Predict();
					if (evaluate)
					{
						evaluationResult = model.Evaluate().ToString();
					}
				}

				report.AppendLine(model.Title);
				report.Append("Releases: ");
				foreach (var r in SelectedReleases.Keys)
				{
					report.Append(r + " ");
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
				report.AppendLine();
			}
			OnAddReport(report.ToString());
		}
	}
}
