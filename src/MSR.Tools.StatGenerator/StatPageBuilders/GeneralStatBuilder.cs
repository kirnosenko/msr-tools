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
		public override IDictionary<string,object> BuildData(IRepositoryResolver repositories)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();

			int commits_count = repositories.Repository<Commit>().Count();
			int commits_fix_count = repositories.SelectionDSL().Commits().AreBugFixes().Count();
			string commits_fix_percent = ((double)commits_fix_count / commits_count * 100).ToString("F02");
			DateTime statfrom = repositories.Repository<Commit>().Min(x => x.Date);
			DateTime statto = repositories.Repository<Commit>().Max(x => x.Date);

			result.Add("stat_date", DateTime.Now.ToString());
			result.Add("stat_period_from", statfrom);
			result.Add("stat_period_to", statto);
			result.Add("stat_period_days", (statto - statfrom).Days);
			result.Add("authors_count",
				repositories.Repository<Commit>().Select(x => x.Author).Distinct().Count()
			);
			result.Add("commits_count", commits_count);
			result.Add("commits_fix_count", commits_fix_count);
			result.Add("commits_fix_percent", commits_fix_percent);
			result.Add("files_current",
				repositories.SelectionDSL().Files().Exist().Count()
			);
			result.Add("files_added",
				repositories.Repository<ProjectFile>().Count()
			);
			result.Add("files_removed",
				repositories.SelectionDSL().Files().Deleted().Count()
			);
			result.Add("loc_current",
				repositories.SelectionDSL().CodeBlocks().CalculateLOC()
			);
			result.Add("loc_added",
				repositories.SelectionDSL().CodeBlocks().Added().CalculateLOC()
			);
			result.Add("loc_removed",
				- repositories.SelectionDSL().CodeBlocks().Deleted().CalculateLOC()
			);
			result.Add("dd",
				repositories.SelectionDSL()
					.Files().Exist()
					.Modifications().InFiles()
					.CodeBlocks().InModifications().CalculateTraditionalDefectDensity().ToString("F02")
			);
			
			
			return result;
		}
	}
}
