/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
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
    public class LocGraphBuilder : StatPageBuilder
    {
        public LocGraphBuilder()
        {
            PageName = "LOC Graph";
            PageTemplate = "locgraph.html";
        }

        public override IDictionary<string, object> BuildData(IRepository repositories)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            var authors = repositories.Queryable<Commit>()
                .Select(x => x.Author)
                .Distinct().ToList();

            var codeByAuthor = (from author in authors
                                select new
                                {
                                    Name = author,
                                }).ToList();

            var statByAuthor =
                from a in codeByAuthor
                select new
                {
                    name = a.Name,
                };

            result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());

            DateTime statFrom = repositories.Queryable<Commit>().Min(x => x.Date);
            DateTime statTo = repositories.Queryable<Commit>().Max(x => x.Date);

            List<object> monthObjs = new List<object>();
            List<object> locObjs = new List<object>();

            List<DateTime> monthes = new List<DateTime>();
            DateTime m = new DateTime(statFrom.Year, statFrom.Month, 1);
            while (m < statTo)
            {
                monthes.Add(m);
                m = m.AddMonths(1);
            }

            foreach (var month in monthes)
            {
                DateTime nextMonth = month.AddMonths(1);

                var totalMonthCommits = repositories.SelectionDSL()
                    .Commits()
                        .DateIsLesserThan(nextMonth)
                        .Fixed();
                
                foreach (var author in authors)
                {
                    double authorLoc = totalMonthCommits
                        .AuthorIs(author).Files().InDirectory(TargetDir)
                        .Modifications().InCommits().InFiles()
                        .CodeBlocks().InModifications().Fixed().CalculateLOC();

                    locObjs.Add(new
                    {
                        loc = authorLoc
                    });
                }

                monthObjs.Add(new
                {
                    month = String.Format("{0:00}", month.Month) + "-" + month.Year.ToString()
                });
            }

            result.Add("monthes", monthObjs);

            result.Add("locs", locObjs);

            return result;
        }
    }
}
