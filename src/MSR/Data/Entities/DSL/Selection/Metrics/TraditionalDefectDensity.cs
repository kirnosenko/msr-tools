/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	/// <summary>
	/// Calculates defect density as number of defects
	/// per 1000 LOC.
	/// Total code size is LOC at the moment 
	/// (added code minus removed code)
	/// </summary>
	public static class TraditionalDefectDensity
	{
		public const double KLOC = 1000;
		
		/// <summary>
		/// Calculates defect density for specified code set.
		/// </summary>
		/// <param name="code">Code set to calc metric for.</param>
		/// <returns>Defect density.</returns>
		public static double CalculateTraditionalDefectDensity(this CodeBlockSelectionExpression code)
		{
			code = code.Added().Fixed();
			
			return CalculateTraditionalDefectDensity(
				code.CalculateLOC() + code.ModifiedBy().Deleted().CalculateLOC(),
				code.CalculateNumberOfDefects()
			);
		}
		public static double CalculateTraditionalDefectDensityAtRevision(this CodeBlockSelectionExpression code, string revision)
		{
			code = code
				.Commits().TillRevision(revision)
				.CodeBlocks().Again().AddedInitiallyInCommits().Fixed();
			
			return CalculateTraditionalDefectDensity(
				code.CalculateLOC()
				+
				code
					.Modifications().InCommits()
					.CodeBlocks().Again().ModifiedBy().Deleted().InModifications().CalculateLOC(),
				code.CalculateNumberOfDefectsAtRevision(revision)
			);
		}
		private static double CalculateTraditionalDefectDensity(double codeSize, double numberOfDefects)
		{
			if (codeSize == 0)
			{
				return 0;
			}
			return numberOfDefects / (codeSize / KLOC);
		}
	}
}
