/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class DefectCodeSize
	{
		public static double CalculateDefectCodeSize(this CodeBlockSelectionExpression code)
		{
			return - code.ModifiedBy().InBugFixes().CalculateLOC();
		}
		public static double CalculateDefectCodeSize(this CodeBlockSelectionExpression code, string revision)
		{
			return - code
				.Commits().TillRevision(revision)
				.Modifications().InCommits()
				.CodeBlocks().Again().ModifiedBy().InModifications().InBugFixes().CalculateLOC();
		}
	}
}
