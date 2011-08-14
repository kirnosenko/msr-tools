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
	/// <summary>
	/// Calculates defect density as number of defects
	/// per 1000 LOC.
	/// </summary>
	public static class DefectDensity
	{
		public const double KLOC = 1000;
		
		/// <summary>
		/// Calculates traditional defect density for specified code.
		/// Total code size is LOC at the moment 
		/// (added code minus removed code).
		/// </summary>
		/// <param name="code">Code set to calc metric for.</param>
		/// <returns>Defect density.</returns>
		public static double CalculateTraditionalDefectDensity(this CodeBlockSelectionExpression code)
		{
			code = code.Added().Fixed();
			
			return CalculateDefectDensity(
				code.CalculateLOC() + code.ModifiedBy().Deleted().CalculateLOC(),
				code.CalculateNumberOfDefects()
			);
		}
		public static double CalculateTraditionalDefectDensity(this CodeBlockSelectionExpression code, string revision)
		{
			code = code
				.Commits().TillRevision(revision)
				.CodeBlocks().Again().AddedInitiallyInCommits().Fixed();
			
			return CalculateDefectDensity(
				code.CalculateLOC()
				+
				code
					.Modifications().InCommits()
					.CodeBlocks().Again().ModifiedBy().Deleted().InModifications().CalculateLOC(),
				code.CalculateNumberOfDefects(revision)
			);
		}
		/// <summary>
		/// Calculates alternative defect density for specified code.
		/// Total code size is added LOC at the moment.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static double CalculateDefectDensity(this CodeBlockSelectionExpression code)
		{
			code = code.Added().Fixed();

			return CalculateDefectDensity(
				code.CalculateLOC(),
				code.CalculateNumberOfDefects()
			);
		}
		public static double CalculateDefectDensity(this CodeBlockSelectionExpression code, string revision)
		{
			code = code
				.Commits().TillRevision(revision)
				.CodeBlocks().Again().AddedInitiallyInCommits().Fixed();

			return CalculateDefectDensity(
				code.CalculateLOC(),
				code.CalculateNumberOfDefects(revision)
			);
		}
		private static double CalculateDefectDensity(double codeSize, double numberOfDefects)
		{
			if (codeSize == 0)
			{
				return 0;
			}
			return numberOfDefects / (codeSize / KLOC);
		}
	}
}
