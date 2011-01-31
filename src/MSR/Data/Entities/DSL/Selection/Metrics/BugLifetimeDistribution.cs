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
	public static class BugLifetimeDistribution
	{
		public static IDictionary<double,double> CalculateBugLifetimeDistribution(
			this BugFixSelectionExpression bugFixes,
			Func<BugFixSelectionExpression,IEnumerable<double>> lifetimesResolver
		)
		{
			Dictionary<double,double> distribution = new Dictionary<double,double>();
			
			IEnumerable<double> bugLifetimes = lifetimesResolver(bugFixes);
			foreach (var bugLifetime in bugLifetimes)
			{
				distribution.Add(
					bugLifetime,
					(double)bugLifetimes.Where(x => x <= bugLifetime).Count() / bugLifetimes.Count()
				);
			}
			
			return distribution;
		}
	}
}
