/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class Loc
	{
		public static double CalculateLOC(this CodeBlockSelectionExpression code)
		{
			return code.Sum(x => (double?)x.Size) ?? 0;
		}
	}
}
