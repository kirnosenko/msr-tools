/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

using MSR.Data.Entities;
using MSR.Models.Prediction;
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
		event Action OnClearRocList;
		event Action<string,string,int> OnRocAdded;
		event Action<bool> OnReadyStateChanged;
		event Action<string> OnProgressStateChanged;
		event Action<string> OnError;
		
		void OpenConfig(string fileName);
		void Predict();
		void ShowROC(int number);
		IDictionary<string,string> Releases { get; }
		IDictionary<string,string> SelectedReleases { get; set; }
		PostReleaseDefectFilesPrediction[] Models { get; }
		IEnumerable<PostReleaseDefectFilesPrediction> SelectedModels { get; set; }
		
		bool ShowFiles { get; set; }
		bool Evaluate { get; set; }
		bool EvaluateUsingROC { get; set; }
		bool EvaluateRanking { get; set; }
		bool ShowEstimationStats { get; set; }
		bool ShowTotalResult { get; set; }
		ReleaseSetGettingAlgo ReleaseSetGetting { get; set; }
		int ReleaseSetSize { get; set; }
	}
	
	struct ModelResult
	{
		public string ModelTitle { get; set; }
		public EvaluationResult ER { get; set; }
		public ROCEvaluationResult RoER { get; set; }
		public RankingEvaluationResult RaER { get; set; }
		public double[] FileEstimations { get; set; }
		public string[] PredictedDefectFiles { get; set; }
		public string[] DefectFiles { get; set; }
	}
	
	public class PredictorModel : IPredictorModel
	{
		public event Action<string> OnTitleUpdated;
		public event Action OnClearReport;
		public event Action<string> OnAddReport;
		public event Action OnClearRocList;
		public event Action<string,string,int> OnRocAdded;
		public event Action<bool> OnReadyStateChanged;
		public event Action<string> OnProgressStateChanged;
		public event Action<string> OnError;
		
		private PredictionTool predictor;
		private List<ROCEvaluationResult> rocs = new List<ROCEvaluationResult>();
		
		public PredictorModel()
		{
			bool justPredictionIsPreferred = true;

			ShowFiles = justPredictionIsPreferred;
			Evaluate = !justPredictionIsPreferred;
			EvaluateUsingROC = !justPredictionIsPreferred;
			EvaluateRanking = !justPredictionIsPreferred;
			ShowEstimationStats = !justPredictionIsPreferred;
			ShowTotalResult = !justPredictionIsPreferred;

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
		public void ShowROC(int number)
		{
			ROCEvaluationResult roc = rocs[number];
			
			string tempfile = Path.GetTempFileName();
			using (TextWriter w = new StreamWriter(tempfile))
			{
				w.WriteLine("l");
				w.WriteLine("{0} {1}", 0, 0);
				w.WriteLine("{0} {1}", 1, 1);
				w.WriteLine("l ROC");
				for (int i = 0; i < roc.Count; i++)
				{
					w.WriteLine("{0} {1}", roc.Pf[i], roc.Se[i]);
				}
				w.WriteLine("l Precision");
				for (int i = 0; i < roc.Count; i++)
				{
					w.WriteLine("{0} {1}", (double)i * 0.01, roc.P[i]);
				}
				w.WriteLine("l Sensitivity (Recall)");
				for (int i = 0; i < roc.Count; i++)
				{
					w.WriteLine("{0} {1}", (double)i * 0.01, roc.Se[i]);
				}
				w.WriteLine("l Specificity");
				for (int i = 0; i < roc.Count; i++)
				{
					w.WriteLine("{0} {1}", (double)i * 0.01, roc.Sp[i]);
				}
			}
			Shell.Run("MSR.Tools.Visualizer.exe", tempfile);
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
		public bool EvaluateRanking
		{
			get; set;
		}
		public bool ShowEstimationStats
		{
			get; set;
		}
		public bool ShowTotalResult
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
				rocs.Clear();
				OnClearRocList();
				
				List<List<ModelResult>> results = new List<List<ModelResult>>();
				foreach (var releaseSet in ReleaseSetGetting.ReleaseSets(this))
				{
					ShowReleases(releaseSet.Values);
					results.Add(new List<ModelResult>(Predict(releaseSet)));
				}
				if ((ShowTotalResult) && (Evaluate || EvaluateUsingROC || EvaluateRanking || ShowEstimationStats))
				{
					ShowTotalModelResult(results);
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
		private IEnumerable<ModelResult> Predict(IDictionary<string,string> releases)
		{
			ModelResult modelResult;
			
			foreach (var model in SelectedModels)
			{
				modelResult = new ModelResult();
				modelResult.ModelTitle = model.Title;
				
				using (var s = predictor.Data.OpenSession())
				{
					model.Init(s, releases.Keys);
					model.Predict();
					
					if (ShowFiles)
					{
						modelResult.PredictedDefectFiles = model.PredictedDefectFiles.ToArray();
					}
					if (Evaluate || EvaluateUsingROC)
					{
						modelResult.DefectFiles = model.DefectFiles.ToArray();
					}
					if (Evaluate)
					{
						modelResult.ER = model.Evaluate();
					}
					if (EvaluateUsingROC)
					{
						modelResult.RoER = model.EvaluateUsingROC();
						rocs.Add(modelResult.RoER);
						OnRocAdded(model.Title, releases.Last().Value, rocs.Count - 1);
					}
					if (EvaluateRanking)
					{
						modelResult.RaER = model.EvaluateRanking();
					}
					if (ShowEstimationStats)
					{
						modelResult.FileEstimations = model.FileEstimations;
					}
				}
				
				ShowModelResult(modelResult);
				yield return modelResult;
			}
		}
		private void ShowReleases(IEnumerable<string> releases)
		{
			StringBuilder output = new StringBuilder();

			output.Append("Releases: ");
			foreach (var r in releases)
			{
				output.Append(r + " ");
			}
			output.AppendLine();
			output.AppendLine();
			OnAddReport(output.ToString());
		}
		private void ShowModelResult(ModelResult modelResult)
		{
			StringBuilder output = new StringBuilder();
			output.AppendLine(modelResult.ModelTitle);
			output.AppendLine();
			
			if (modelResult.PredictedDefectFiles != null)
			{
				bool evaluated = modelResult.DefectFiles != null;
				
				output.AppendLine("Predicted defect files:");
				foreach (var f in modelResult.PredictedDefectFiles)
				{
					output.AppendLine(
						string.Format("{0} {1}", f, evaluated ? modelResult.DefectFiles.Contains(f) ? "+" : "-" : "")
					);
				}
				
				if (evaluated)
				{
					output.AppendLine("Defect files:");
					foreach (var f in modelResult.DefectFiles.OrderBy(x => x))
					{
						output.AppendLine(
							string.Format("{0} {1}", f, modelResult.PredictedDefectFiles.Contains(f) ? "+" : "-")
						);
					}
				}
			}
			
			if (modelResult.ER != null)
			{
				output.AppendLine(EvaluationResultToString(modelResult.ER));
			}
			if (modelResult.RoER != null)
			{
				output.AppendLine(RocEvaluationResultToString(modelResult.RoER));
			}
			if (modelResult.RaER != null)
			{
				output.AppendLine(RankingEvaluationResultToString(modelResult.RaER));
			}
			if (modelResult.FileEstimations != null)
			{
				output.AppendLine(FileEstimationsToString(
					Accord.Statistics.Tools.Mean(modelResult.FileEstimations),
					Accord.Statistics.Tools.Median(modelResult.FileEstimations),
					modelResult.FileEstimations.Max(),
					modelResult.FileEstimations.Min()
				));
			}
			output.AppendLine();
			OnAddReport(output.ToString());
		}
		private void ShowTotalModelResult(List<List<ModelResult>> results)
		{
			int releaseCount = results.Count;
			if (releaseCount == 0)
			{
				return;
			}
			int modelCount = results[0].Count;
			if (modelCount == 0)
			{
				return;
			}
			for (int i = 0; i < modelCount; i++)
			{
				var modelResults = results.Select(x => x[i]);
				
				StringBuilder output = new StringBuilder();
				output.AppendLine(modelResults.First().ModelTitle + " " + "TOTAL");
				output.AppendLine();

				if (modelResults.First().ER != null)
				{
					output.AppendLine(EvaluationResultToString(
						modelResults.Average(x => x.ER.Precision),
						modelResults.Average(x => x.ER.Recall),
						modelResults.Average(x => x.ER.Accuracy),
						modelResults.Average(x => x.ER.Fmeasure),
						modelResults.Average(x => x.ER.NegPos)
					));
				}
				if (modelResults.First().RoER != null)
				{
					output.AppendLine(RocEvaluationResultToString(
						modelResults.Average(x => x.RoER.AUC),
						modelResults.Average(x => x.RoER.MaxPoint),
						modelResults.Average(x => x.RoER.BalancePoint),
						modelResults.Average(x => x.RoER.OptimalPoint)
					));
				}
				if (modelResults.First().RaER != null)
				{
					output.AppendLine(RankingEvaluationResultToString(
						modelResults.Average(x => x.RaER.DefectCodeSize),
						modelResults.Average(x => x.RaER.DefectCodeSizeInSelection),
						modelResults.Average(x => x.RaER.DefectCodeSizeInSelectionPercent)
					));
				}
				if (modelResults.First().FileEstimations != null)
				{
					output.AppendLine(FileEstimationsToString(
						modelResults.Average(x => Accord.Statistics.Tools.Mean(x.FileEstimations)),
						modelResults.Average(x => Accord.Statistics.Tools.Median(x.FileEstimations)),
						modelResults.Average(x => x.FileEstimations.Max()),
						modelResults.Average(x => x.FileEstimations.Min())
					));
				}
				output.AppendLine();
				OnAddReport(output.ToString());
			}
		}
		private string EvaluationResultToString(EvaluationResult er)
		{
			return EvaluationResultToString(er.Precision, er.Recall, er.Accuracy, er.Fmeasure, er.NegPos);
		}
		private string EvaluationResultToString(double P, double R, double A, double F, double NP)
		{
			return string.Format("Precision = {0:0.00}, Recall = {1:0.00}, Accuracy = {2:0.00}, F-measure = {3:0.00}, Neg/Pos = {4:0.00}",
				P, R, A, F, NP
			);
		}
		private string RocEvaluationResultToString(ROCEvaluationResult roer)
		{
			return RocEvaluationResultToString(roer.AUC, roer.MaxPoint, roer.BalancePoint, roer.OptimalPoint);
		}
		private string RocEvaluationResultToString(double AUC, double MP, double BP, double OP)
		{
			return string.Format("AUC = {0:0.00}, MaxPoint = {1:0.00}, BalancePoint = {2:0.00}, OptimalPoint = {3:0.00}",
				AUC, MP, BP, OP
			);
		}
		private string RankingEvaluationResultToString(RankingEvaluationResult raer)
		{
			return RankingEvaluationResultToString(
				raer.DefectCodeSize,
				raer.DefectCodeSizeInSelection,
				raer.DefectCodeSizeInSelectionPercent
			);
		}
		private string RankingEvaluationResultToString(double DCS, double SDCS, double SDCSP)
		{
			return string.Format("Defect code size = {0:0.00}, Defect code size in selection = {1:0.00} ({2:0.00} %)",
				DCS, SDCS, SDCSP
			);
		}
		private string FileEstimationsToString(double mean, double median, double max, double min)
		{
			return string.Format("File estimations: Mean = {0:0.00}, Median = {1:0.00}, Max = {2:0.00}, Min = {3:0.00}",
				mean, median, max, min
			);
		}
	}
}
