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
		private double defaultCutOffValue;
		
		public PostReleaseDefectFilesPrediction()
		{
			defaultCutOffValue = 0.5;
		}
		public virtual void Predict()
		{
			var files = GetFilesInRevision(PredictionRelease);

			PredictedDefectFiles =
				(
					from f in files
					select new
					{
						Path = f.Path,
						FaultProneProbability = PredictDefectFileProbability(f)
					}
				).Where(x => x.FaultProneProbability > defaultCutOffValue)
				.Select(x => x.Path).ToArray();
		}
		public EvaluationResult Evaluate()
		{
			var allFiles = GetFilesInRevision(PredictionRelease)
				.Select(x => x.Path);
			DefectFiles = GetPostReleaseDefectFiles();
			
			IEnumerable<string> predictedNonDefectFiles = allFiles.Except(PredictedDefectFiles);

			IEnumerable<string> P = DefectFiles;
			IEnumerable<string> N = allFiles.Except(DefectFiles);
			int TP = PredictedDefectFiles.Intersect(P).Count();
			int TN = predictedNonDefectFiles.Intersect(N).Count();
			int FP = PredictedDefectFiles.Count() - TP;
			int FN = predictedNonDefectFiles.Count() - TN;

			return new EvaluationResult(TP, TN, FP, FN);
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
		
		protected abstract double PredictDefectFileProbability(ProjectFile file);
		protected IEnumerable<ProjectFile> GetFilesInRevision(string revision)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(revision)
					.ToList();
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
