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

namespace MSR.Tools.StatGenerator
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
					.Distinct();
				double totalLoc = s.SelectionDSL().CodeBlocks().CalculateLOC();

				Dictionary<string, CodeBlockSelectionExpression> codeByAuthor = new Dictionary<string, CodeBlockSelectionExpression>();
				foreach (var author in authors)
				{
					codeByAuthor.Add(
						author,
						s.SelectionDSL()
							.Commits().AuthorIs(author)
							.Modifications().InCommits()
							.CodeBlocks().InModifications()
							.Fixed()
					);
				}

				var statByAuthor =
					from a in codeByAuthor
					let author = a.Key
					let code = a.Value
					let authorCommits = code.Commits().Again().Count()
					let authorLoc = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC()
					select new
					{
						name = author,
						commits = string.Format("{0} ({1}%)", authorCommits, (((double)authorCommits / commits) * 100).ToString("F02")),
						dd = code.CalculateTraditionalDefectDensity().ToString("F02"),
						added = code.Added().CalculateLOC(),
						addedInFixes = code.Added().InBugFixes().CalculateLOC(),
						deleted = - code.Deleted().CalculateLOC(),
						deletedInFixes = - code.Deleted().InBugFixes().CalculateLOC(),
						current = authorLoc,
						contribution = ((authorLoc / totalLoc) * 100).ToString("F02")
					};

				context.Put("authors", statByAuthor.OrderBy(x => x.name).ToArray());
			}
		}
	}
}
