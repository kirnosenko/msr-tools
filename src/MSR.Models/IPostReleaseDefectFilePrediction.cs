/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data.Entities.DSL.Selection;

namespace MSR.Models
{
	public interface IPostReleaseDefectFilePrediction
	{
		IEnumerable<string> Predict(string previousReleaseRevision, string releaseRevision);
		Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> FileSelector { get; set; }
	}
}
