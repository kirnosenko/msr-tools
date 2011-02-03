/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class DefectCodeDensity
	{
		public static double CalculateDefectCodeDensity(this CodeBlockSelectionExpression code)
		{
			code = code.Added().Fixed();
			
			return CalculateDefectCodeDensity(
				code.CalculateLOC(),
				- code.ModifiedBy().InBugFixes().CalculateLOC()
			);
		}
		public static double CalculateDefectCodeDensityAtRevision(this CodeBlockSelectionExpression code, string revision)
		{
			code = code
				.Commits().TillRevision(revision)
				.CodeBlocks().Again().AddedInitiallyInCommits().Fixed();

			return CalculateDefectCodeDensity(
				code.CalculateLOC(),
				- code
					.BugFixes().InCommits()
					.CodeBlocks().ModifiedBy().InBugFixes().CalculateLOC()
			);
		}
		private static double CalculateDefectCodeDensity(double addedCodeSize, double defectCodeSize)
		{
			if (addedCodeSize == 0)
			{
				return 0;
			}
			return defectCodeSize / addedCodeSize;
		}
	}
}
