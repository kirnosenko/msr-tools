/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.StatGenerator.StatPageBuilders
{
	public class GeneralStatBuilder : StatPageBuilder
	{
		public GeneralStatBuilder()
		{
			PageName = "General";
			PageTemplate = "general.html";
		}
		public override IDictionary<string,object> BuildData(IRepository repository)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();

			int commits_count = repository.Queryable<Commit>().Count();
			int commits_fix_count = repository.SelectionDSL().Commits().AreBugFixes().Count();
			string commits_fix_percent = ((double)commits_fix_count / commits_count * 100).ToString("F02");
			int commits_refactoring_count = repository.SelectionDSL().Commits().AreRefactorings().Count();
			string commits_refactoring_percent = ((double)commits_refactoring_count / commits_count * 100).ToString("F02");
			
			DateTime statfrom = repository.Queryable<Commit>().Min(x => x.Date);
			DateTime statto = repository.Queryable<Commit>().Max(x => x.Date);

			result.Add("stat_date", DateTime.Now.ToString());
			result.Add("stat_period_from", statfrom);
			result.Add("stat_period_to", statto);
			result.Add("stat_period_days", (statto - statfrom).Days);
			result.Add("stat_period_years", ((statto - statfrom).TotalDays / 365).ToString("F01"));
			result.Add("authors_count",
				repository.Queryable<Commit>().Select(x => x.Author).Distinct().Count()
			);
			result.Add("commits_count", commits_count);
			result.Add("commits_fix_count", commits_fix_count);
			result.Add("commits_fix_percent", commits_fix_percent);
			result.Add("commits_refactoring_count", commits_refactoring_count);
			result.Add("commits_refactoring_percent", commits_refactoring_percent);
			var files = repository.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Fixed();
			result.Add("files_current",
				files.Exist().Count()
			);
			result.Add("files_added",
				files.Count()
			);
			result.Add("files_removed",
				files.Deleted().Count()
			);
			var code = repository.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Modifications().InFiles()
				.CodeBlocks().InModifications()
				.Fixed();
			result.Add("loc_current",
				code.CalculateLOC()
			);
			result.Add("loc_added",
				code.Added().CalculateLOC()
			);
			result.Add("loc_removed",
				- code.Deleted().CalculateLOC()
			);
			result.Add("tdd",
				code.CalculateTraditionalDefectDensity().ToString("F03")
			);
			result.Add("dd",
				code.CalculateDefectDensity().ToString("F03")
			);
			
			return result;
		}
	}
}
