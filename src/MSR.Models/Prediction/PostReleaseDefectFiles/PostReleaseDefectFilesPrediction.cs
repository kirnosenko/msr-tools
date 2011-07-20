/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

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
		
		private double defaultCutOffValue;
		private Dictionary<string,double> possibleDefectFiles;
		
		public PostReleaseDefectFilesPrediction()
		{
			defaultCutOffValue = 0.5;
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
					FileFaultProneProbability(file)
				);
				
				if (CallBack != null)
				{
					processedFilesCount++;
					CallBack(this, (double)processedFilesCount / allFilesCount);
				}
			}

			PredictedDefectFiles = possibleDefectFiles
				.Where(x => x.Value >= defaultCutOffValue)
				.Select(x => x.Key)
				.ToArray();
		}
		public EvaluationResult Evaluate()
		{
			DefectFiles = GetPostReleaseDefectFiles();
			
			return Evaluate(PredictedDefectFiles);
		}
		public double EvaluateUsingROC()
		{
			if (DefectFiles == null)
			{
				DefectFiles = GetPostReleaseDefectFiles();
			}
			
			List<double> xlist = new List<double>(100);
			List<double> ylist = new List<double>(100);
			
			for (int cutOffValue = 0; cutOffValue <= 100; cutOffValue++)
			{
				var predictedDefectFiles = possibleDefectFiles
					.Where(x => x.Value >= (double)cutOffValue * 0.01)
					.Select(x => x.Key)
					.ToArray();
				
				var er = Evaluate(predictedDefectFiles);
				
				xlist.Add(1 - er.Specificity);
				ylist.Add(er.Sensitivity);
			}
			
			double sum = 0;
			for (int i = 0; i < xlist.Count-1; i++)
			{
				sum += ((ylist[i+1] + ylist[i]) / 2) * (xlist[i] - xlist[i+1]);
			}
			return sum;
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
			get; protected set;
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
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
		protected abstract double FileFaultProneProbability(ProjectFile file);
		protected IEnumerable<ProjectFile> GetFilesInRevision(string revision)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(revision)
					.ToArray();
		}
		
		private IEnumerable<string> GetPostReleaseDefectFiles()
		{
			return repositories.SelectionDSL()
				.Commits()
					.TillRevision(PredictionRelease)
				.Modifications().InCommits()
				.CodeBlocks().InModifications().ModifiedBy()
				.Modifications().ContainCodeBlocks()
				.Commits()
					.AfterRevision(PredictionRelease)
					.AreBugFixes()
					.ContainModifications()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(PredictionRelease)
					.TouchedInCommits()
				.Select(x => x.Path)
				.ToArray();
		}
	}
}
