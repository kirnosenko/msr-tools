/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using MSR.Data.Entities;
using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public abstract class ReleaseSetGettingAlgo
	{
		public static readonly ReleaseSetGettingAlgo All = new ReleaseSetGettingAlgoAll();
		public static readonly ReleaseSetGettingAlgo IncrementalGrowth = new ReleaseSetGettingAlgoIncrementalGrowth();
		public static readonly ReleaseSetGettingAlgo Fixed = new ReleaseSetGettingAlgoFixed();
		
		public abstract List<IDictionary<string,string>> ReleaseSets(IPredictorModel model);
	}
	public class ReleaseSetGettingAlgoAll : ReleaseSetGettingAlgo
	{
		public override List<IDictionary<string,string>> ReleaseSets(IPredictorModel model)
		{
			return new List<IDictionary<string,string>>() { model.SelectedReleases };
		}
	}
	public class ReleaseSetGettingAlgoIncrementalGrowth : ReleaseSetGettingAlgo
	{
		public override List<IDictionary<string,string>> ReleaseSets(IPredictorModel model)
		{
			List<IDictionary<string,string>> list = new List<IDictionary<string,string>>();
			
			for (int i = 1; i <= model.SelectedReleases.Count; i++)
			{
				list.Add(
					model.SelectedReleases.Take(i).ToDictionary(x => x.Key, x => x.Value)
				);
			}
			
			return list;
		}
	}
	public class ReleaseSetGettingAlgoFixed : ReleaseSetGettingAlgo
	{
		public override List<IDictionary<string,string>> ReleaseSets(IPredictorModel model)
		{
			List<IDictionary<string,string>> list = new List<IDictionary<string,string>>();

			if (model.SelectedReleases.Count <= model.ReleaseSetSize)
			{
				list.Add(model.SelectedReleases);
			}
			else
			{
				for (int i = 0; i <= model.SelectedReleases.Count - model.ReleaseSetSize; i++)
				{
					list.Add(
						model.SelectedReleases.Skip(i).Take(model.ReleaseSetSize)
							.ToDictionary(x => x.Key, x => x.Value)
					);
				}
			}

			return list;
		}
	}
	
	public interface IPredictorModel
	{
		event Action<string> OnTitleUpdated;
		event Action OnClearReport;
		event Action<string> OnAddReport;
		event Action<bool> OnReadyStateChanged;
		event Action<string> OnProgressStateChanged;
		event Action<string> OnError;
		
		void OpenConfig(string fileName);
		void Predict();
		IDictionary<string,string> Releases { get; }
		IDictionary<string,string> SelectedReleases { get; set; }
		PostReleaseDefectFilesPrediction[] Models { get; }
		IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels { get; set; }
		
		bool ShowFiles { get; set; }
		bool Evaluate { get; set; }
		bool EvaluateUsingROC { get; set; }
		ReleaseSetGettingAlgo ReleaseSetGetting { get; set; }
		int ReleaseSetSize { get; set; }
	}
	
	public class PredictorModel : IPredictorModel
	{
		public event Action<string> OnTitleUpdated;
		public event Action OnClearReport;
		public event Action<string> OnAddReport;
		public event Action<bool> OnReadyStateChanged;
		public event Action<string> OnProgressStateChanged;
		public event Action<string> OnError;
		
		private PredictionTool predictor;
		
		public PredictorModel()
		{
			ShowFiles = false;
			Evaluate = true;
			EvaluateUsingROC = true;
			ReleaseSetGetting = ReleaseSetGettingAlgo.IncrementalGrowth;
			ReleaseSetSize = 3;
		}
		public void OpenConfig(string fileName)
		{
			predictor = new PredictionTool(fileName);
			OnTitleUpdated(string.Format("Predictor - {0}", fileName));
			foreach (var model in Models)
			{
				model.CallBack += (m,p) =>
				{
					OnProgressStateChanged(string.Format("{0} - {1:0}%",
						m.Title,
						p * 100
					));
				};
			}
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
			get { return predictor.Models.Models; }
		}
		public IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels
		{
			get; set;
		}
		public void Predict()
		{
			Thread thread = new Thread(PredictWork);
			thread.IsBackground = true;
			thread.Start();
		}
		public bool ShowFiles
		{
			get; set;
		}
		public bool Evaluate
		{
			get; set;
		}
		public bool EvaluateUsingROC
		{
			get; set;
		}
		public ReleaseSetGettingAlgo ReleaseSetGetting
		{
			get; set;
		}
		public int ReleaseSetSize
		{
			get; set;
		}
		
		private void PredictWork()
		{
			try
			{
				OnReadyStateChanged(false);
				OnClearReport();
				
				foreach (var releaseSet in ReleaseSetGetting.ReleaseSets(this))
				{
					Predict(releaseSet);
				}
			}
			catch (Exception e)
			{
				OnError(e.Message);
			}
			finally
			{
				OnReadyStateChanged(true);
			}
		}
		private void Predict(IDictionary<string,string> releases)
		{
			StringBuilder output = new StringBuilder();

			output.Append("Releases: ");
			foreach (var r in releases.Values)
			{
				output.Append(r + " ");
			}
			output.AppendLine();
			output.AppendLine();
			OnAddReport(output.ToString());

			foreach (var model in SelectedModels)
			{
				output = new StringBuilder();
				
				using (var s = predictor.Data.OpenSession())
				{
					model.Init(s, releases.Keys);
					model.Predict();

					output.AppendLine(model.Title);
					output.AppendLine();
					
					if (ShowFiles || (!Evaluate && !EvaluateUsingROC))
					{
						output.AppendLine("Predicted defect files:");
						foreach (var f in model.PredictedDefectFiles)
						{
							output.AppendLine(
								string.Format("{0} {1}", f, Evaluate ? model.DefectFiles.Contains(f) ? "+" : "-" : "")
							);
						}
					}
					if (Evaluate || EvaluateUsingROC)
					{
						string evaluateResult = Evaluate ?
							model.Evaluate().ToString()
							:
							"";
						string evaluateUsingROCResult = EvaluateUsingROC ?
							model.EvaluateUsingROC().ToString()
							:
							"";
						
						if (ShowFiles)
						{
							output.AppendLine("Defect files:");
							foreach (var f in model.DefectFiles.OrderBy(x => x))
							{
								output.AppendLine(
									string.Format("{0} {1}", f, model.PredictedDefectFiles.Contains(f) ? "+" : "-")
								);
							}
						}

						output.AppendLine(string.Format("{0}{1}{2}",
							evaluateResult,
							((evaluateResult != "") && (evaluateUsingROCResult != "")) ? ", " : "",
							evaluateUsingROCResult
						));
					}
					output.AppendLine();

					OnAddReport(output.ToString());
				}
			}
		}
	}
}
