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
    public class CommitPerMonthGraphBuilder : StatPageBuilder
    {
        public CommitPerMonthGraphBuilder() 
        {
            PageName = "Commits per Month Graph";
            PageTemplate = "commitspermonthgraph.html";
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
            }).ToList();

            var statByAuthor =
                from a in codeByAuthor
            select new
            {
                name = a.Name,
            };

            result.Add("authors", statByAuthor.OrderBy(x => x.name).ToArray());

            List<object> monthObjs = new List<object>();
            List<object> fixObjs = new List<object>();
            List<object> refactObjs = new List<object>();
            List<object> restObjs = new List<object>();

			DateTime statFrom = repositories.Queryable<Commit>().Min(x => x.Date);
			DateTime statTo = repositories.Queryable<Commit>().Max(x => x.Date);
			
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

                var monthCommits = repositories.SelectionDSL()
                    .Commits()
                        .DateIsGreaterOrEquelThan(month)
                        .DateIsLesserThan(nextMonth)
                        .Fixed();

                int monthFixCommitsCount = monthCommits.AreBugFixes().Count();
                int monthRefactCommitsCount = monthCommits.AreRefactorings().Count();
                int monthRestCommitsCount = monthCommits.Count() - monthFixCommitsCount - monthRefactCommitsCount;

                foreach (var author in authors)
                {
                    int monthAuthFixCommits = monthCommits.AuthorIs(author).AreBugFixes().Count();
                    int monthAuthRefactCommits = monthCommits.AuthorIs(author).AreRefactorings().Count();
                    int monthRestAuthCommits = monthCommits.AuthorIs(author).Count() - monthAuthFixCommits - monthAuthRefactCommits;

                    double monthAuthFixCommitsCount = monthFixCommitsCount == 0 ? 0 : (monthAuthFixCommits / monthFixCommitsCount * 100);
                    double monthAuthRefactCommitsCount = monthRefactCommitsCount == 0 ? 0 : (monthAuthRefactCommits / monthRefactCommitsCount * 100);
                    double monthAuthRestCommitsCount = monthRestCommitsCount == 0 ? 0 : (monthRestAuthCommits / monthRestCommitsCount * 100);

                    fixObjs.Add(new
                    {
                        fix = monthAuthFixCommitsCount
                    });

                    refactObjs.Add(new
                    {
                        refact = monthAuthRefactCommitsCount
                    });

                    restObjs.Add(new 
                    {
                        rest = monthAuthRestCommitsCount
                    });
                }

                monthObjs.Add(new
                {
                    month = month.Year.ToString() + "-" + String.Format("{0:00}", month.Month)
                });
            }

            result.Add("monthes", monthObjs);

            result.Add("fixes",fixObjs);

            result.Add("refactorings",refactObjs);

            result.Add("rests",restObjs);

            return result;
        }
    }
}
