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
		
		public PostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
			FilePortionLimit = 0.2;
		}
		public void AddPredictor()
		{
		
		}
		public IEnumerable<string> Predict(string[] previousReleaseRevisions, string releaseRevision)
		{
			LogisticRegression lr = new LogisticRegression();
			
			foreach (var oldRelease in previousReleaseRevisions)
			{
				foreach (var file in FilesInRelease(oldRelease))
				{
					lr.AddTrainingData(
						new double[]
						{
							Loc(file.ID, oldRelease),
							Touches(file.ID, oldRelease)
						},
						Defects(file.ID, oldRelease) > 0 ? 1 : 0);
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
						FaultProneProbability = lr.Predict(new double[]
						{
							Loc(f.ID, releaseRevision),
							Touches(f.ID, releaseRevision)
						})
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
		private IEnumerable<ProjectFile> FilesInRelease(string release)
		{
			return repositories.SelectionDSL()
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(release)
					.ToList();
		}
		private double Touches(int fileID, string release)
		{
			return repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits().TillRevision(release).TouchFiles().Count();
		}
		private double Loc(int fileID, string release)
		{
			return repositories.SelectionDSL()
				.Files().IdIs(fileID)
				.Commits().TillRevision(release)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();
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
