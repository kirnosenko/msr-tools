/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.Statistics;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Models.Regressions;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public abstract class PostReleaseDefectFilesPrediction : Prediction
	{
		public event Action<PostReleaseDefectFilesPrediction,double> CallBack;
		
		protected double defaultCutOffValue;
		private Dictionary<string,double> possibleDefectFiles;
		private IEnumerable<string> defectFiles;
		
		public PostReleaseDefectFilesPrediction()
		{
			defaultCutOffValue = 0.5;
			UseFileEstimationMeanAsCutOffValue = false;
			FilePortionLimit = 1;
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);
			
			defectFiles = null;
		}
		public virtual void Predict()
		{
			AllFiles = GetFilesInRevision(PredictionRelease);
			
			int allFilesCount = AllFiles.Count();
			int processedFilesCount = 0;
			
			possibleDefectFiles = new Dictionary<string,double>();
			foreach (var file in AllFiles)
			{
				possibleDefectFiles.Add(
					file.Path,
					GetFileEstimation(file)
				);
				
				if (CallBack != null)
				{
					processedFilesCount++;
					CallBack(this, (double)processedFilesCount / allFilesCount);
				}
			}
			
			PredictedDefectFiles = possibleDefectFiles
				.Where(x => x.Value >= CutOffValue)
				.TakeNoMoreThan((int)(allFilesCount * FilePortionLimit))
				.Select(x => x.Key)
				.ToArray();
		}
		public EvaluationResult Evaluate()
		{
			return Evaluate(PredictedDefectFiles);
		}
		public ROCEvaluationResult EvaluateUsingROC()
		{
			List<EvaluationResult> results = new List<EvaluationResult>(101);
			
			for (int cutOffValue = 0; cutOffValue <= 100; cutOffValue++)
			{
				var predictedDefectFiles = possibleDefectFiles
					.Where(x => x.Value >= (double)cutOffValue * 0.01)
					.Select(x => x.Key)
					.ToArray();
				
				results.Add(Evaluate(predictedDefectFiles));
			}
			
			return new ROCEvaluationResult(results.ToArray());
		}
		public bool UseFileEstimationMeanAsCutOffValue
		{
			get; set;
		}
		public double FilePortionLimit
		{
			get; set;
		}

		public double FileEstimationMean
		{
			get { return possibleDefectFiles.Values.Mean(); }
		}
		public double FileEstimationMax
		{
			get { return possibleDefectFiles.Values.Max(); }
		}
		public double FileEstimationMin
		{
			get { return possibleDefectFiles.Values.Min(); }
		}
		public IEnumerable<ProjectFile> AllFiles
		{
			get; protected set;
		}
		public IEnumerable<string> PredictedDefectFiles
		{
			get; protected set;
		}
		public IEnumerable<string> DefectFiles
		{
			get
			{
				if (defectFiles == null)
				{
					defectFiles = GetPostReleaseDefectiveFiles();
				}
				return defectFiles;
			}
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		
		protected double CutOffValue
		{
			get
			{
				if (UseFileEstimationMeanAsCutOffValue)
				{
					return FileEstimationMean;
				}
				return defaultCutOffValue;
			}
		}
		protected EvaluationResult Evaluate(
			IEnumerable<string> predictedDefectFiles
		)
		{
			var allFiles = AllFiles.Select(x => x.Path).ToArray();
			
			IEnumerable<string> predictedNonDefectFiles = allFiles.Except(predictedDefectFiles);
			
			IEnumerable<string> P = DefectFiles;
			IEnumerable<string> N = allFiles.Except(DefectFiles);
			int TP = predictedDefectFiles.Intersect(P).Count();
			int TN = predictedNonDefectFiles.Intersect(N).Count();
			int FP = predictedDefectFiles.Count() - TP;
			int FN = predictedNonDefectFiles.Count() - TN;

			return new EvaluationResult(TP, TN, FP, FN);
		}
		protected abstract double GetFileEstimation(ProjectFile file);
		protected IEnumerable<ProjectFile> GetFilesInRevision(string revision)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(revision)
					.ToArray();
		}
		
		private IEnumerable<string> GetPostReleaseDefectiveFiles()
		{
			return GetPostReleaseDefectiveFiles(
				repositories.SelectionDSL()
					.Commits()
						.TillRevision(PredictionRelease)
			);
		}
		private IEnumerable<string> GetPostReleaseDefectiveFilesFromTouchedInRelease()
		{
			return GetPostReleaseDefectiveFiles(
				repositories.SelectionDSL()
					.Commits()
						.AfterRevision(TrainReleases.Last())
						.TillRevision(PredictionRelease)
			);
		}
		private IEnumerable<string> GetPostReleaseDefectiveFiles(CommitSelectionExpression commits)
		{
			return commits
				.Modifications()
					.InCommits()
				.CodeBlocks()
					.InModifications()
				.DefectiveFiles(PredictionRelease, null)
					.Reselect(FileSelector)
					.ExistInRevision(PredictionRelease)
				.Select(x => x.Path)
				.ToArray();
		}
	}
}
