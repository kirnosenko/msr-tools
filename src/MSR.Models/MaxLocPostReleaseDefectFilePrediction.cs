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
	public class MaxLocPostReleaseDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;
		
		public MaxLocPostReleaseDefectFilePrediction(IRepositoryResolver repositories)
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
			
			var code = selectionDSL
				.Commits()
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(fileSelector)
					.ExistInRevision(releaseRevision)
						.Do(e => filesInRelease = e.Count())
				.Modifications()
					.InCommits()
					.InFiles()
				.CodeBlocks()
					.InModifications();
			
			var files = 
				from cb in code
				join m in code.Selection<Modification>() on cb.ModificationID equals m.ID
				join f in code.Selection<ProjectFile>() on m.FileID equals f.ID
				group cb by f into g
				select new
				{
					Path = g.Key.Path,
					Size = g.Sum(x => x.Size)
				};
			
			return files
				.OrderByDescending(x => x.Size)
				.Select(x => x.Path)
				.TakeNoMoreThan((int)(filesInRelease * 0.2));
		}
	}
}
