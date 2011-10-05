/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class Defects
	{
		/// <summary>
		/// Calculate number of fixed bugs for specified code
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static int CalculateNumberOfDefects(this CodeBlockSelectionExpression code)
		{
			return code.ModifiedBy()
				.Modifications().ContainCodeBlocks()
				.Commits().ContainModifications()
				.BugFixes().InCommits().Count();
		}
		/// <summary>
		/// Calculate number of fixed bugs were fixed in revision or before it.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="revision"></param>
		/// <returns></returns>
		public static int CalculateNumberOfDefects(this CodeBlockSelectionExpression code, string revision)
		{
			return code.ModifiedBy()
				.Modifications().ContainCodeBlocks()
				.Commits().TillRevision(revision).ContainModifications()
				.BugFixes().InCommits().Count();
		}
	}
}
