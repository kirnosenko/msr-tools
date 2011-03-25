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
		event Action<string> OnTitleUpdated;
		event Action OnClearReport;
		event Action<string> OnAddReport;
		
		void OpenConfig(string fileName);
		void Predict(bool evaluate);
		IDictionary<string,string> Releases { get; }
		IDictionary<string,string> SelectedReleases { get; set; }
		PostReleaseDefectFilesPrediction[] Models { get; }
		IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels { get; set; }
		
		bool ShowFiles { get; set; }
		int MaxReleaseSetSize { get; set; }
	}
	
	public class PredictorModel : IPredictorModel
	{
		public event Action<string> OnTitleUpdated;
		public event Action OnClearReport;
		public event Action<string> OnAddReport;
		
		private PredictionTool predictor;
		
		public PredictorModel()
		{
			ShowFiles = true;
			MaxReleaseSetSize = 3;
		}
		public void OpenConfig(string fileName)
		{
			predictor = new PredictionTool(fileName);
			OnTitleUpdated(string.Format("Predictor - {0}", fileName));
		}
		public IDictionary<string,string> Releases
		{
			get { return predictor.Releases; }
		}
		public IDictionary<string,string> SelectedReleases
		{
			get; set;
		}
		public PostReleaseDefectFilesPrediction[] Models
		{
			get { return predictor.Models.Models(); }
		}
		public IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels
		{
			get; set;
		}
		public void Predict(bool evaluate)
		{
			OnClearReport();
			
			List<IDictionary<string,string>> releaseSets = new List<IDictionary<string,string>>();
			if (SelectedReleases.Count <= MaxReleaseSetSize)
			{
				releaseSets.Add(SelectedReleases);
			}
			else
			{
				for (int i = 0; i <= SelectedReleases.Count - MaxReleaseSetSize; i++)
				{
					releaseSets.Add(
						SelectedReleases.Skip(i).Take(MaxReleaseSetSize)
							.ToDictionary(x => x.Key, x => x.Value)
					);
				}
			}
			
			foreach (var releaseSet in releaseSets)
			{
				Predict(evaluate, releaseSet);
			}
		}
		public void Predict(bool evaluate, IDictionary<string,string> releases)
		{
			StringBuilder report = new StringBuilder();
			string evaluationResult = null;

			report.Append("Releases: ");
			foreach (var r in releases.Keys)
			{
				report.Append(r + " ");
			}
			report.AppendLine();
			
			foreach (var model in SelectedModels)
			{
				using (var s = predictor.Data.OpenSession())
				{
					model.Init(s, releases.Values);
					model.Predict();
					if (evaluate)
					{
						evaluationResult = model.Evaluate().ToString();
					}
				}

				report.AppendLine(model.Title);
				if (ShowFiles || (!evaluate))
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
					if (ShowFiles)
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
		public bool ShowFiles
		{
			get; set;
		}
		public int MaxReleaseSetSize
		{
			get; set;
		}
	}
}
