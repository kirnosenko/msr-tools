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
			int totalFiles = repositories.SelectionDSL()
				.Files().InDirectory(TargetDir).Exist()
				.Count();

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
					.Fixed(),
				TouchedFiles = repositories.SelectionDSL()
					.Commits().AuthorIs(author)
					.Files().InDirectory(TargetDir).Exist().TouchedInCommits()
			}).ToList();
			
			var statByAuthor =
				from a in codeByAuthor
				let authorCommits = a.AddedCode.Commits().Again().Count()
				let authorFixes = a.AddedCode.Commits().Again().AreBugFixes().Count()
				let authorRefactorings = a.AddedCode.Commits().Again().AreRefactorings().Count()
				let authorAddedLoc = a.AddedCode.CalculateLOC()
				let authorCurrentLoc = authorAddedLoc + a.AddedCode.ModifiedBy().CalculateLOC()
				let authorTouchedFiles = a.TouchedFiles.Count()
				let authorFilesTouchedByOtherAuthors = a.TouchedFiles
					.Commits().AuthorIsNot(a.Name)
					.Files().Again().TouchedInCommits().Count()
				select new
				{
					name = a.Name,
					commits = string.Format("{0} ({1}%)", authorCommits, (((double)authorCommits / commits) * 100).ToString("F02")),
					fixes = string.Format("{0} ({1}%)", authorFixes, (((double)authorFixes / authorCommits) * 100).ToString("F02")),
					refactorings = string.Format("{0} ({1}%)", authorRefactorings, (((double)authorRefactorings / authorCommits) * 100).ToString("F02")),
					dd = a.AddedCode.CalculateDefectDensity().ToString("F03"),
					added = a.AddedCode.CalculateLOC(),
					deleted = - a.RemovedCode.CalculateLOC(),
					current = authorCurrentLoc,
					contribution = ((authorCurrentLoc / totalLoc) * 100).ToString("F02"),
					specialization = ((double)authorTouchedFiles / totalFiles * 100).ToString("F02"),
					uniqueSpecialization = (authorTouchedFiles > 0 ?
						((double)(authorTouchedFiles - authorFilesTouchedByOtherAuthors) / totalFiles * 100)
						:
						0).ToString("F02"),
					efficiency = (authorAddedLoc > 0 ?
						((authorCurrentLoc / authorAddedLoc) * 100)
						:
						0).ToString("F02")
				};

			result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());
			
			return result;
		}
	}
}
