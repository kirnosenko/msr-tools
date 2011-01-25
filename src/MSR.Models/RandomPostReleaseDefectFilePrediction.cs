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
	public class RandomPostReleaseDefectFilePrediction : IPostReleaseDefectFilePrediction
	{
		private IRepositoryResolver repositories;
		
		public RandomPostReleaseDefectFilePrediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IEnumerable<string> Predict(string previousReleaseRevision, string releaseRevision)
		{
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);

			int filesInRelease = 0;
			
			var files = selectionDSL
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
						.Do(e => filesInRelease = e.Count())
				.Select(f => f.Path).ToList();
			
			return files.TakeRandomly((int)(filesInRelease * 0.2));
		}
		public Func<ProjectFileSelectionExpression, ProjectFileSelectionExpression> FileSelector
		{
			get; set;
		}
	}
}
