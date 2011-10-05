/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class StabilizationPeriod
	{
		public static double CalculateStabilizationPeriod(this BugFixSelectionExpression bugFixes, double stabilizationProbability)
		{
			var bugLifetimes = bugFixes.CalculateAvarageBugLifetime().OrderByDescending(x => x);
			int bugLifetimesCount = bugLifetimes.Count();
			double stabilizationPeriod = bugLifetimes.First();

			foreach (var bugLifetime in bugLifetimes)
			{
				double lessOrEqualToTotal = (double)bugLifetimes.Where(x => x <= bugLifetime).Count() / bugLifetimes.Count();
				if (lessOrEqualToTotal >= stabilizationProbability)
				{
					stabilizationPeriod = bugLifetime;
				}
				else
				{
					break;
				}
			}
			
			return stabilizationPeriod;
		}
	}
}
