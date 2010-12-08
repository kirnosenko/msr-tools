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
	public class MaxAddedLocPostReleaseDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;

		public MaxAddedLocPostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IEnumerable<string> Predict(
			string previousReleaseRevision,
			string releaseRevision,
			Func<ProjectFileSelectionExpression, ProjectFileSelectionExpression> fileSelector
		)
		{
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);

			int filesInRelease = 0;
			
			var releaseCode = selectionDSL
				.Commits()
					.AfterRevision(previousReleaseRevision)
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(fileSelector)
					.ExistInRevision(releaseRevision)
						.Do(e => filesInRelease = e.Count())
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
					addedLoc = g.Sum()
				};
			
			return files
				.OrderByDescending(x => x.addedLoc)
				.Select(x => x.path)
				.TakeNoMoreThan((int)(filesInRelease * 0.2));
		}
	}
}
