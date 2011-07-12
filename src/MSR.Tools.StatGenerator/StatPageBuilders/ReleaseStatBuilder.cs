/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
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
	public class ReleaseStatBuilder : StatPageBuilder
	{
		public ReleaseStatBuilder()
		{
			PageName = "Releases";
			PageTemplate = "releases.html";
		}
		public override IDictionary<string,object> BuildData(IRepositoryResolver repositories)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();
			
			double totalLoc = repositories.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();

			List<object> releaseObjects = new List<object>();
			
			var releases =
				(
					from r in repositories.Repository<Release>()
					join c in repositories.Repository<Commit>() on r.CommitID equals c.ID
					select new
					{
						Tag = r.Tag,
						Revision = c.Revision
					}
				).ToList();
			releases.Add(new
			{
				Tag = "upcoming",
				Revision = repositories.LastRevision()
			});

			DateTime statFrom = repositories.Repository<Commit>().Min(x => x.Date);
			DateTime statTo = repositories.Repository<Commit>().Max(x => x.Date);
			
			string prevRelease = null;
			foreach (var release in releases)
			{
				var releaseCommits = repositories.SelectionDSL()
					.Commits()
						.AfterRevision(prevRelease)
						.TillRevision(release.Revision)
						.Fixed();
				var releaseCode = releaseCommits
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();
				var totalReleaseCommits = repositories.SelectionDSL()
					.Commits()
						.TillRevision(release.Revision)
						.Fixed();
				var totalReleaseCode = totalReleaseCommits
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();

				DateTime releaseStatFrom = releaseCommits.Min(x => x.Date);
				DateTime releaseStatTo = releaseCommits.Max(x => x.Date);
				
				int releaseCommitsCount = releaseCommits.Count();
				int releaseAuthorsCount = releaseCommits.Select(c => c.Author).Distinct().Count();
				int releaseFixesCount = releaseCommits.AreBugFixes().Count();
				int totalReleaseCommitsCount = totalReleaseCommits.Count();
				int totalReleaseAuthorsCount = totalReleaseCommits.Select(c => c.Author).Distinct().Count();
				int totalReleaseFixesCount = totalReleaseCommits.AreBugFixes().Count();
				
				var totalReleaseLoc = totalReleaseCode.CalculateLOC();
				var releaseRemainLoc = releaseCode.Added().CalculateLOC() + releaseCode.ModifiedBy().CalculateLOC();
				var totalReleaseAddedLoc = totalReleaseCode.Added().CalculateLOC();
				var releaseAddedLoc = releaseCode.Added().CalculateLOC();
				
				double releaseDD = releaseCode.CalculateDefectDensity();
				double totalReleaseDD = totalReleaseCode.CalculateDefectDensity();
				double postReleaseDD = releaseDD - releaseCode.CalculateDefectDensityAtRevision(release.Revision);
				
				releaseObjects.Add(new
				{
					tag = release.Tag,
					commits = string.Format("{0} ({1})",
						releaseCommitsCount,
						totalReleaseCommitsCount
					),
					days = string.Format("{0} ({1})",
						(releaseStatTo - releaseStatFrom).Days,
						(releaseStatTo - statFrom).Days
					),
					authors = string.Format("{0} ({1})",
						releaseAuthorsCount,
						totalReleaseAuthorsCount
					),
					files = repositories.SelectionDSL().Files()
						.ExistInRevision(release.Revision).Count(),
					dd = string.Format("{0} ({1})",
						releaseDD.ToString("F03"),
						totalReleaseDD.ToString("F03")
					),
					post_release_dd = string.Format(
						"{0} ({1}%)",
						postReleaseDD.ToString("F02"),
						((postReleaseDD / releaseDD) * 100).ToString("F02")
					),
					fixed_defects = string.Format("{0} ({1})",
						releaseFixesCount,
						totalReleaseFixesCount
					),
					added_loc = string.Format("{0} ({1})",
						releaseAddedLoc,
						totalReleaseAddedLoc
					),
					removed_loc = string.Format("{0} ({1})",
						- releaseCode.Deleted().CalculateLOC(),
						- totalReleaseCode.Deleted().CalculateLOC()
					),
					loc = totalReleaseLoc,
					remain_loc = releaseRemainLoc,
					contribution = ((releaseRemainLoc / totalLoc) * 100).ToString("F02"),
					demand_for_code = (releaseAddedLoc > 0 ?
						((releaseRemainLoc / releaseAddedLoc) * 100)
						:
						0).ToString("F02")
				});
				
				prevRelease = release.Revision;
			}
			
			result.Add("releases", releaseObjects);
			return result;
		}
	}
}
