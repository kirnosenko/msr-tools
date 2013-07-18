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
    public class ActivityGraphBuilder : StatPageBuilder
    {
        public ActivityGraphBuilder()
        {
            PageName = "Activity Graph";
            PageTemplate = "activitygraph.html";
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
				RemovedCode = repositories.SelectionDSL()
					.Commits().AuthorIs(author)
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Deleted()
					.Fixed()
            }).ToList();

            var statByAuthor =
                from a in codeByAuthor
                select new 
                {
                    name = a.Name,
                    added_loc = a.AddedCode.CalculateLOC() / (a.AddedCode.CalculateLOC() - a.RemovedCode.CalculateLOC()) * 100,
                    removed_loc = -a.RemovedCode.CalculateLOC() / (a.AddedCode.CalculateLOC() - a.RemovedCode.CalculateLOC()) * 100
                };
            result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());

            return result;
        }
    }
}
