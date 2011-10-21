/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Accord.Statistics;

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
		protected double rocEvaluationDelta;
		private Dictionary<string,double> possibleDefectFiles;
		private ProjectFile[] allFiles;
		private string[] defectFiles;
		private string[] predictedDefectFiles;
		
		public PostReleaseDefectFilesPrediction()
		{
			defaultCutOffValue = 0.5;
			UseFileEstimationMeanAsCutOffValue = false;
			FilePortionLimit = 1;
			rocEvaluationDelta = 0.01;
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);
			
			allFiles = null;
			defectFiles = null;
			predictedDefectFiles = null;
		}
		public virtual void Predict()
		{
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
		}
		public EvaluationResult Evaluate()
		{
			return Evaluate(PredictedDefectFiles);
		}
		public virtual ROCEvaluationResult EvaluateUsingROC()
		{
			List<EvaluationResult> results = new List<EvaluationResult>(101);
			
			for (int cutOffValue = 0; cutOffValue <= 100; cutOffValue++)
			{
				var predictedDefectFiles = possibleDefectFiles
					.Where(x => x.Value >= (double)cutOffValue * rocEvaluationDelta)
					.Select(x => x.Key)
					.ToArray();

				results.Add(Evaluate(predictedDefectFiles));
			}

			return new ROCEvaluationResult(results.ToArray(), rocEvaluationDelta);
		}
		public bool UseFileEstimationMeanAsCutOffValue
		{
			get; set;
		}
		public double FilePortionLimit
		{
			get; set;
		}

		public double[] FileEstimations
		{
			get { return possibleDefectFiles.Values.ToArray(); }
		}
		public ProjectFile[] AllFiles
		{
			get
			{
				if (allFiles == null)
				{
					allFiles = GetFilesInRevision(PredictionRelease);
				}
				return allFiles;
			}
		}
		public virtual string[] PredictedDefectFiles
		{
			get
			{
				if (predictedDefectFiles == null)
				{
					predictedDefectFiles = possibleDefectFiles
						.Where(x => x.Value >= CutOffValue)
						.TakeNoMoreThan((int)(possibleDefectFiles.Count * FilePortionLimit))
						.Select(x => x.Key)
						.ToArray();
				}
				return predictedDefectFiles;
			}
		}
		public virtual string[] DefectFiles
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
					return FileEstimations.Mean();
				}
				return defaultCutOffValue;
			}
		}
		protected EvaluationResult Evaluate(string[] predictedDefectFiles)
		{
			var allFiles = AllFiles.Select(x => x.Path).ToArray();
			
			return new EvaluationResult(
				DefectFiles,
				allFiles.Except(DefectFiles).ToArray(),
				predictedDefectFiles,
				allFiles.Except(predictedDefectFiles).ToArray()
			);
		}
		protected abstract double GetFileEstimation(ProjectFile file);
		protected ProjectFile[] GetFilesInRevision(string revision)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(revision)
					.ToArray();
		}
		
		private string[] GetPostReleaseDefectiveFiles()
		{
			return GetPostReleaseDefectiveFiles(
				repositories.SelectionDSL()
					.Commits()
						.TillRevision(PredictionRelease)
			);
		}
		private string[] GetPostReleaseDefectiveFilesFromTouchedInRelease()
		{
			return GetPostReleaseDefectiveFiles(
				repositories.SelectionDSL()
					.Commits()
						.AfterRevision(TrainReleases.Last())
						.TillRevision(PredictionRelease)
			);
		}
		private string[] GetPostReleaseDefectiveFiles(CommitSelectionExpression commits)
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
