/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NVelocity;

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
		public override void AddData(IDataStore data, VelocityContext context)
		{
			using (var s = data.OpenSession())
			{
				int commits_count = s.Repository<Commit>().Count();
				int commits_fix_count = s.SelectionDSL().Commits().AreBugFixes().Count();
				string commits_fix_percent = ((double)commits_fix_count / commits_count * 100).ToString("F02");
				DateTime statfrom = s.Repository<Commit>().Min(x => x.Date);
				DateTime statto = s.Repository<Commit>().Max(x => x.Date);
				
				context.Put("stat_date", DateTime.Now.ToString());
				context.Put("stat_period_from", statfrom);
				context.Put("stat_period_to", statto);
				context.Put("stat_period_days", (statto - statfrom).Days);
				context.Put("authors_count",
					s.Repository<Commit>().Select(x => x.Author).Distinct().Count()
				);
				context.Put("commits_count", commits_count);
				context.Put("commits_fix_count", commits_fix_count);
				context.Put("commits_fix_percent", commits_fix_percent);
				context.Put("files_current",
					s.SelectionDSL().Files().Exist().Count()
				);
				context.Put("files_added",
					s.Repository<ProjectFile>().Count()
				);
				context.Put("files_removed",
					s.SelectionDSL().Files().Deleted().Count()
				);
				context.Put("loc_current",
					s.SelectionDSL().CodeBlocks().CalculateLOC()
				);
				context.Put("loc_added",
					s.SelectionDSL().CodeBlocks().Added().CalculateLOC()
				);
				context.Put("loc_removed",
					- s.SelectionDSL().CodeBlocks().Deleted().CalculateLOC()
				);
				context.Put("dd",
					s.SelectionDSL()
						.Files().Exist()
						.Modifications().InFiles()
						.CodeBlocks().InModifications().CalculateTraditionalDefectDensity().ToString("F02")
				);
			}
		}
	}
}
