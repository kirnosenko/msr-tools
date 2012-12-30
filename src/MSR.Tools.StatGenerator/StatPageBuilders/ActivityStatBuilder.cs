/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.StatGenerator.StatPageBuilders
{
	public class ActivityStatBuilder : StatPageBuilder
	{
		public ActivityStatBuilder()
		{
			PageName = "Activity";
			PageTemplate = "activity.html";
		}
		public override IDictionary<string,object> BuildData(IRepository repository)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

			double totalLoc = repository.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();

			List<object> monthObjects = new List<object>();

			DateTime statFrom = repository.Queryable<Commit>().Min(x => x.Date);
			DateTime statTo = repository.Queryable<Commit>().Max(x => x.Date);
			
			List<DateTime> monthes = new List<DateTime>();
			DateTime m = new DateTime(statFrom.Year, statFrom.Month, 1);
			while (m < statTo)
			{
				monthes.Add(m);
				m = m.AddMonths(1);
			}
			
			string lastRevision = null;
			
			foreach (var month in monthes)
			{
				DateTime nextMonth = month.AddMonths(1);
				
				var monthCommits = repository.SelectionDSL()
					.Commits()
						.DateIsGreaterOrEquelThan(month)
						.DateIsLesserThan(nextMonth)
						.Fixed();
				var monthCode = monthCommits
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();
				var totalMonthCommits = repository.SelectionDSL()
					.Commits()
						.DateIsLesserThan(nextMonth)
						.Fixed();
				var totalMonthCode = totalMonthCommits
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();

				int monthCommitsCount = monthCommits.Count();
				int totalMonthCommitsCount = totalMonthCommits.Count();
				int monthAuthorsCount = monthCommits.Select(c => c.Author).Distinct().Count();
				int totalMonthAuthorsCount = totalMonthCommits.Select(c => c.Author).Distinct().Count();
				int monthFixesCount = monthCommits.AreBugFixes().Count();
				int totalMonthFixesCount = totalMonthCommits.AreBugFixes().Count();
				if (monthCommitsCount > 0)
				{
					lastRevision = monthCommits
						.Single(c => c.OrderedNumber == monthCommits.Max(x => x.OrderedNumber))
						.Revision;
				}
				
				monthObjects.Add(new
				{
					month = month.Year.ToString() + "-" + String.Format("{0:00}", month.Month),
					commits = string.Format("{0} ({1})",
						monthCommitsCount,
						totalMonthCommitsCount
					),
					authors = string.Format("{0} ({1})",
						monthAuthorsCount,
						totalMonthAuthorsCount
					),
					files = repository.SelectionDSL().Files()
						.ExistInRevision(lastRevision).Count(),
					fixed_defects = string.Format("{0} ({1})",
						monthFixesCount,
						totalMonthFixesCount
					),
					added_loc = string.Format("{0} ({1})",
						monthCode.Added().CalculateLOC(),
						totalMonthCode.Added().CalculateLOC()
					),
					removed_loc = string.Format("{0} ({1})",
						-monthCode.Deleted().CalculateLOC(),
						-totalMonthCode.Deleted().CalculateLOC()
					),
					loc = totalMonthCode.CalculateLOC()
				});
			}

			result.Add("monthes", monthObjects);
			return result;
		}
	}
}
