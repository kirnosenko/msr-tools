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
			
			string prevRelease = null;
			foreach (var release in releases)
			{
				var code = repositories.SelectionDSL()
					.Commits()
						.AfterRevision(prevRelease)
						.TillRevision(release.Revision)
					.Files().InDirectory(TargetDir)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();
				var currentLoc = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC();
				
				releaseObjects.Add(new
				{
					tag = release.Tag,
					dd = code.CalculateDefectDensity().ToString("F03"),
					added = code.Added().CalculateLOC(),
					deleted = -code.Deleted().CalculateLOC(),
					current = currentLoc,
					contribution = ((currentLoc / totalLoc) * 100).ToString("F02"),
				});
				
				prevRelease = release.Revision;
			}
			
			result.Add("releases", releaseObjects);
			return result;
		}
	}
}
