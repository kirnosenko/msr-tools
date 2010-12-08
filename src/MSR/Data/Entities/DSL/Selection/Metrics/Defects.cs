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
	public static class Defects
	{
		/// <summary>
		/// Calculate number of bugs found for specified code
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
	}
}
