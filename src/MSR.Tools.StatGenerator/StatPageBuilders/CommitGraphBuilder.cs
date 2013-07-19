/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
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
    public class CommitGraphBuilder : StatPageBuilder
    {
        public CommitGraphBuilder()
        {
            PageName = "Commit Graph";
            PageTemplate = "commitgraph.html";
        }
        public override IDictionary<string, object> BuildData(IRepository repositories)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

			var authors = repositories.Queryable<Commit>()
				.Select(x => x.Author)
				.Distinct().ToList();

            var codeByAuthor = (from author in authors select new
			{
                Name = author,
				AddedCode = repositories.SelectionDSL()
					.Commits().AuthorIs(author)
					.Files().InDirectory(TargetDir)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().AddedInitiallyInCommits()
					.Fixed(),
            }).ToList();

            var statByAuthor =
                from a in codeByAuthor
                let authorCommits = a.AddedCode.Commits().Again().Count()
                let authorFixes = a.AddedCode.Commits().Again().AreBugFixes().Count()
                let authorRefactorings = a.AddedCode.Commits().Again().AreRefactorings().Count()
            select new
                {
                    name = a.Name,
                    fixes = (double) authorFixes / authorCommits * 100,
                    refactorings = (double) authorRefactorings / authorCommits * 100
                };

            result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());

            return result;
        }
    }
}
