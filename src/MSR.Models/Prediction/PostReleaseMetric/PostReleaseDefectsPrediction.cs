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

namespace MSR.Models.Prediction.PostReleaseMetric
{
	public class PostReleaseDefectsPrediction : PostReleaseMetricPrediction
	{
		public PostReleaseDefectsPrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
		}
		public override double PostReleaseMetric(string previousReleaseRevision, string releaseRevision)
		{
			return repositories.SelectionDSL()
				.Commits()
					.AfterRevision(previousReleaseRevision)
					.TillRevision(releaseRevision)
				.Files()
					.Reselect(FileSelector)
					.ExistInRevision(releaseRevision)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects();
		}
	}
}
