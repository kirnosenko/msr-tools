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
	public class AuthorStatBuilder : StatPageBuilder
	{
		public AuthorStatBuilder()
		{
			PageName = "Authors";
			PageTemplate = "authors.html";
		}
		public override IDictionary<string,object> BuildData(IRepositoryResolver repositories)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();

			int commits = repositories.Repository<Commit>().Count();
			var authors = repositories.Repository<Commit>()
				.Select(x => x.Author)
				.Distinct().ToList();
			double totalLoc = repositories.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();

			var codeByAuthor = (from author in authors select new
			{
				Name = author,
				AddedCode = repositories.SelectionDSL()
					.Commits().AuthorIs(author)
					.Files().InDirectory(TargetDir)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().AddedInitiallyInCommits()
					.Fixed(),
				RemovedCode = repositories.SelectionDSL()
					.Commits().AuthorIs(author)
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Deleted()
					.Fixed()
			}).ToList();
			
			var statByAuthor =
				from a in codeByAuthor
				let authorCommits = a.AddedCode.Commits().Again().Count()
				let authorFixes = a.AddedCode.Commits().Again().AreBugFixes().Count()
				let authorRefactorings =
					(
						from c in a.AddedCode.Commits().Again().AreNotBugFixes()
						join m in repositories.Repository<Modification>() on c.ID equals m.CommitID
						join cb in repositories.Repository<CodeBlock>() on m.ID equals cb.ModificationID
						group cb by c into g
						select new
						{
							Added = g.Where(x => x.Size > 0).Sum(x => x.Size),
							Removed = - g.Where(x => x.Size < 0).Sum(x => x.Size)
						}
					).Where(x => x.Removed / x.Added >= 1/2).Count()
				let authorLoc = a.AddedCode.CalculateLOC() + a.AddedCode.ModifiedBy().CalculateLOC()
				select new
				{
					name = a.Name,
					commits = string.Format("{0} ({1}%)", authorCommits, (((double)authorCommits / commits) * 100).ToString("F02")),
					fixes = string.Format("{0} ({1}%)", authorFixes, (((double)authorFixes / authorCommits) * 100).ToString("F02")),
					refactorings = string.Format("{0} ({1}%)", authorRefactorings, (((double)authorRefactorings / authorCommits) * 100).ToString("F02")),
					dd = a.AddedCode.CalculateTraditionalDefectDensity().ToString("F02"),
					added = a.AddedCode.CalculateLOC(),
					deleted = - a.RemovedCode.CalculateLOC(),
					current = authorLoc,
					contribution = ((authorLoc / totalLoc) * 100).ToString("F02")
				};

			result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());
			
			return result;
		}
	}
}
