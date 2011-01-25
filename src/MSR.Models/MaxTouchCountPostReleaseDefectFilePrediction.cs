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
	public class MaxTouchCountPostReleaseDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;
		
		public MaxTouchCountPostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IEnumerable<string> Predict(string previousReleaseRevision, string releaseRevision)
		{
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);

			int filesInRelease = 0;
			
			var modifications = selectionDSL
				.Commits()
					.AfterRevision(previousReleaseRevision)
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
						.Do(e => filesInRelease = e.Count())
				.Modifications()
					.InCommits()
					.InFiles();

			var files =
				from m in modifications
				join f in modifications.Selection<ProjectFile>() on m.FileID equals f.ID
				group m by f.Path into g
				select new
				{
					Path = g.Key,
					TouchCount = g.Count()
				};

			return files
				.OrderByDescending(x => x.TouchCount)
				.Select(x => x.Path)
				.TakeNoMoreThan((int)(filesInRelease * 0.2));
		}
		public Func<ProjectFileSelectionExpression, ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
	}
}
