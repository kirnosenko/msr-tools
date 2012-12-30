/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities
{
	public static class RepositoryHelper
	{
		public static string FirstRevision(this IRepository repository)
		{
			return
				repository.Queryable<Commit>().Single(
					c => c.OrderedNumber == repository.Queryable<Commit>().Min(x => x.OrderedNumber)
				).Revision;
		}
		public static string LastRevision(this IRepository repository)
		{
			return
				repository.Queryable<Commit>().Single(
					c => c.OrderedNumber == repository.Queryable<Commit>().Max(x => x.OrderedNumber)
				).Revision;
		}
		public static string LastRevision(this IRepository repository, IEnumerable<string> revisions)
		{
			if (revisions.Count() == 0)
			{
				throw new ArgumentException("revisions");
			}
			return
				repository.Queryable<Commit>().Single(
					c => c.OrderedNumber == repository.Queryable<Commit>()
						.Where(x => revisions.Contains(x.Revision))
						.Max(x => x.OrderedNumber)
				).Revision;
		}
		public static TimeSpan DevelopmentPeriod(this IRepository repository)
		{
			return
				repository.Queryable<Commit>().Max(c => c.Date)
				-
				repository.Queryable<Commit>().Min(c => c.Date);
		}
		public static IDictionary<string, string> AllReleases(this IRepository repository)
		{
			var releases =
				from r in repository.Queryable<Release>()
				join c in repository.Queryable<Commit>() on r.CommitID equals c.ID
				orderby c.OrderedNumber
				select new
				{
					Revision = c.Revision,
					Tag = r.Tag
				};
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (var r in releases)
			{
				result.Add(r.Revision, r.Tag);
			}
			return result;
		}
	}
}
