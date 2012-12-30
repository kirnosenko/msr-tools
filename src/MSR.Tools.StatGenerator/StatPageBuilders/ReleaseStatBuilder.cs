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
		public override IDictionary<string,object> BuildData(IRepository repository)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();
			
			double totalLoc = repository.SelectionDSL()
				.Files().InDirectory(TargetDir)
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateLOC();

			List<object> releaseObjects = new List<object>();

			var releases = repository.AllReleases();
			releases.Add(repository.LastRevision(), "upcoming");
			
			DateTime statFrom = repository.Queryable<Commit>().Min(x => x.Date);
			
			string prevRelease = null;
			foreach (var release in releases)
			{
				var releaseCommits = repository.SelectionDSL()
					.Commits()
						.AfterRevision(prevRelease)
						.TillRevision(release.Key)
					.Fixed();
				var releaseCode = releaseCommits
					.Files()
						.InDirectory(TargetDir)
					.Modifications()
						.InCommits()
						.InFiles()
					.CodeBlocks()
						.InModifications()
					.Fixed();
				var totalReleaseCommits = repository.SelectionDSL()
					.Commits()
						.TillRevision(release.Key)
					.Fixed();
				var totalReleaseCode = totalReleaseCommits
					.Files()
						.InDirectory(TargetDir)
					.Modifications()
						.InCommits()
						.InFiles()
					.CodeBlocks()
						.InModifications()
					.Fixed();

				DateTime releaseStatFrom = releaseCommits.Min(x => x.Date);
				DateTime releaseStatTo = releaseCommits.Max(x => x.Date);
				
				int releaseCommitsCount = releaseCommits.Count();
				int releaseAuthorsCount = releaseCommits.Select(c => c.Author).Distinct().Count();
				int releaseFixesCount = releaseCommits.AreBugFixes().Count();
				int releaseFilesCount = repository.SelectionDSL()
					.Files().ExistInRevision(release.Key).Count();
				int releaseTouchedFilesCount = releaseCommits
					.Files()
						.ExistInRevision(release.Key)
						.TouchedInCommits()
					.Count();
				int releaseDefectiveFilesCount = releaseCode
					.DefectiveFiles(prevRelease, null)
						.ExistInRevision(release.Key)
					.Count();
				int totalReleaseCommitsCount = totalReleaseCommits.Count();
				int totalReleaseAuthorsCount = totalReleaseCommits.Select(c => c.Author).Distinct().Count();
				int totalReleaseFixesCount = totalReleaseCommits.AreBugFixes().Count();
				
				var totalReleaseLoc = totalReleaseCode.CalculateLOC();
				var releaseRemainLoc = releaseCode.Added().CalculateLOC() + releaseCode.ModifiedBy().CalculateLOC();
				var totalReleaseAddedLoc = totalReleaseCode.Added().CalculateLOC();
				var releaseAddedLoc = releaseCode.Added().CalculateLOC();
				
				double releaseDD = releaseCode.CalculateDefectDensity();
				double totalReleaseDD = totalReleaseCode.CalculateDefectDensity();
				double postReleaseDD = releaseDD - releaseCode.CalculateDefectDensity(release.Key);
				
				releaseObjects.Add(new
				{
					tag = release.Value,
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
					files = releaseFilesCount,
					files_changed = string.Format(
						"{0} ({1}%)",
						releaseTouchedFilesCount,
						(((double)releaseTouchedFilesCount / releaseFilesCount) * 100).ToString("F02")
					),
					files_defective = string.Format(
						"{0} ({1}%)",
						releaseDefectiveFilesCount,
						(((double)releaseDefectiveFilesCount / releaseFilesCount) * 100).ToString("F02")
					),
					dd = string.Format("{0} ({1})",
						releaseDD.ToString("F03"),
						totalReleaseDD.ToString("F03")
					),
					post_release_dd = string.Format(
						"{0} ({1}%)",
						postReleaseDD.ToString("F02"),
						((releaseDD > 0 ? postReleaseDD / releaseDD : 0) * 100).ToString("F02")
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
					contribution = ((releaseRemainLoc / totalLoc) * 100).ToString("F02") + "%",
					demand_for_code = (releaseAddedLoc > 0 ?
						((releaseRemainLoc / releaseAddedLoc) * 100)
						:
						0).ToString("F02") + "%"
				});
				
				prevRelease = release.Key;
			}
			
			result.Add("releases", releaseObjects);
			return result;
		}
	}
}
