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
	public class AuthorStatBuilder : StatPageBuilder
	{
		public AuthorStatBuilder()
		{
			PageName = "Authors";
			PageTemplate = "authors.html";
		}
		public override void AddData(IDataStore data, VelocityContext context)
		{
			using (var s = data.OpenSession())
			{
				int commits = s.Repository<Commit>().Count();
				var authors = s.Repository<Commit>()
					.Select(x => x.Author)
					.Distinct().ToList();
				double totalLoc = s.SelectionDSL().CodeBlocks().CalculateLOC();

				var codeByAuthor = (from author in authors select new
				{
					Name = author,
					AddedCode = s.SelectionDSL()
						.Commits().AuthorIs(author)
						.CodeBlocks().AddedInitiallyInCommits()
						.Fixed(),
					RemovedCode = s.SelectionDSL()
						.Commits().AuthorIs(author)
						.Modifications().InCommits()
						.CodeBlocks().InModifications().Deleted()
						.Fixed()
				}).ToList();
				
				var statByAuthor =
					from a in codeByAuthor
					let authorCommits = a.AddedCode.Commits().Again().Count()
					let authorLoc = a.AddedCode.CalculateLOC() + a.AddedCode.ModifiedBy().CalculateLOC()
					select new
					{
						name = a.Name,
						commits = string.Format("{0} ({1}%)", authorCommits, (((double)authorCommits / commits) * 100).ToString("F02")),
						dd = a.AddedCode.CalculateTraditionalDefectDensity().ToString("F02"),
						added = a.AddedCode.CalculateLOC(),
						addedInFixes = a.AddedCode.InBugFixes().CalculateLOC(),
						deleted = - a.RemovedCode.CalculateLOC(),
						deletedInFixes = - a.RemovedCode.InBugFixes().CalculateLOC(),
						current = authorLoc,
						contribution = ((authorLoc / totalLoc) * 100).ToString("F02")
					};

				context.Put("authors", statByAuthor.OrderBy(x => x.name).ToArray());
			}
		}
	}
}
