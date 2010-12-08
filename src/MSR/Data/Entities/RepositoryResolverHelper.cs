/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
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
	}
}
