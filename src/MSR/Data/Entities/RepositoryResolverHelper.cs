/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities
{
	public static class RepositoryResolverHelper
	{
		public static string LastRevision(this IRepositoryResolver repositories)
		{
			return
				repositories.Repository<Commit>().Single(
					c => c.OrderedNumber == repositories.Repository<Commit>().Max(x => x.OrderedNumber)
				).Revision;
		}
		public static string LastRevision(this IRepositoryResolver repositories, IEnumerable<string> revisions)
		{
			if (revisions.Count() == 0)
			{
				throw new ArgumentException("revisions");
			}
			return
				repositories.Repository<Commit>().Single(
					c => c.OrderedNumber == repositories.Repository<Commit>()
						.Where(x => revisions.Contains(x.Revision))
						.Max(x => x.OrderedNumber)
				).Revision;
		}
		public static TimeSpan DevelopmentPeriod(this IRepositoryResolver repositories)
		{
			return
				repositories.Repository<Commit>().Max(c => c.Date)
				-
				repositories.Repository<Commit>().Min(c => c.Date);
		}
	}
}
