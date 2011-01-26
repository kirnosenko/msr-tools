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

namespace MSR.Models
{
	public class PostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;
		private Dictionary<object,Type> predictors = new Dictionary<object,Type>();
		
		public PostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
			FilePortionLimit = 0.2;

			AddPredictor((Func<ProjectFileSelectionExpression,double>)(files =>
			{
				return files
					.Commits().Again().TouchFiles().Count();
			}));
			AddPredictor((Func<CodeBlockSelectionExpression,double>)(code =>
			{
				return code.CalculateLOC();
			}));
			AddPredictor((Func<CodeBlockSelectionExpression,double>)(code =>
			{
				return code.CalculateNumberOfDefects();
			}));
		}
		public void AddPredictor<Exp>(Func<Exp,double> predictor)
		{
			predictors.Add(predictor, typeof(Exp));
		}
		public IEnumerable<string> Predict(string[] previousReleaseRevisions, string releaseRevision)
		{
			LogisticRegression lr = new LogisticRegression();
			
			foreach (var oldRelease in previousReleaseRevisions)
			{
				foreach (var file in FilesInRelease(oldRelease))
				{
					lr.AddTrainingData(
						GetPredictorValues(file.ID, oldRelease),
						Defects(file.ID, oldRelease) > 0 ? 1 : 0
					);
				}
			}
			
			lr.Train();

			var files = FilesInRelease(releaseRevision);
			int filesInRelease = files.Count();
			var faultProneFiles =
				(
					from f in files
					select new
					{
						Path = f.Path,
						FaultProneProbability = lr.Predict(
							GetPredictorValues(f.ID, releaseRevision)
						)
					}
				).Where(x => x.FaultProneProbability > 0.5)
				.OrderByDescending(x => x.FaultProneProbability);

			return faultProneFiles
				.Select(x => x.Path)
				.TakeNoMoreThan((int)(filesInRelease * FilePortionLimit));
		}
		public Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
		public double FilePortionLimit
		{
			get; set;
		}
		private IEnumerable<double> GetPredictorValuesFor<Exp>(Exp exp)
		{
			return predictors
				.Where(p => exp.GetType() == p.Value)
				.Select(p => (p.Key as Func<Exp,double>)(exp));
		}
		private double[] GetPredictorValues(int fileID, string revision)
		{
			List<double> predictorValues = new List<double>();
			
			predictorValues.AddRange(
				GetPredictorValuesFor(
					repositories.SelectionDSL()
						.Commits().TillRevision(revision)
						.Files().IdIs(fileID)
				)
			);
			var code = repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits().TillRevision(revision)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();
			predictorValues.AddRange(GetPredictorValuesFor(code));
			
			return predictorValues.ToArray();
		}
		private IEnumerable<ProjectFile> FilesInRelease(string release)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(release)
					.ToList();
		}
		private double Defects(int fileID, string release)
		{
			return repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits().TillRevision(release)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects();
		}
	}
}
