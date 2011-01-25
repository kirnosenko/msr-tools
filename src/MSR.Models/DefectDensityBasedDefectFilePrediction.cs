/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Models
{
	public class DefectDensityBasedDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;

		public DefectDensityBasedDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IEnumerable<string> Predict(string previousReleaseRevision, string releaseRevision)
		{
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);

			var DefectDensityForCodeBeforePreviousRelease = selectionDSL
				.Commits()
					.TillRevision(previousReleaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(previousReleaseRevision)
				.Modifications()
					.InCommits()
					.InFiles()
				.CodeBlocks()
					.InModifications()
					.CalculateTraditionalDefectDensity();

			var releaseCode = selectionDSL
				.Commits()
					.AfterRevision(previousReleaseRevision)
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
				.Modifications()
					.InCommits()
					.InFiles()
				.CodeBlocks()
					.InModifications()
					.Added();
			
			var files = 
				from cb in releaseCode
				join m in releaseCode.Selection<Modification>() on cb.ModificationID equals m.ID
				join f in releaseCode.Selection<ProjectFile>() on m.FileID equals f.ID
				group cb.Size by f.Path into g
				select new
				{
					path = g.Key,
					predictedDD = (g.Sum() / TraditionalDefectDensity.KLOC) * DefectDensityForCodeBeforePreviousRelease
				};
			
			return files
				.Where(x => x.predictedDD >= DefectDensityForCodeBeforePreviousRelease / 100)
				.Select(x => x.path);
		}
		public Func<ProjectFileSelectionExpression, ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
	}
}
