/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.StatGenerator
{
	public class GeneratingTool : Tool
	{
		public GeneratingTool(string configFile)
			: base(configFile, "generatingtool")
		{
			data.ReadOnly = true;
		}
		public void GenerateStat(string targetDir, string outputDir, string templateDir)
		{
			using (ConsoleTimeLogger.Start("generating statistics time"))
			{
				StatBuilder builder = GetConfiguredType<StatBuilder>();
				if (targetDir != null)
				{
					builder.TargetDir = targetDir;
				}
				if (outputDir != null)
				{
					builder.OutputDir = outputDir;
				}
				if (templateDir != null)
				{
					builder.TargetDir = templateDir;
				}
				builder.GenerateStat(data);
			}
		}
		public void ShowBlame(string targetDir, string targetPath)
		{
			using (ConsoleTimeLogger.Start("getting blame time"))
			using (var s = data.OpenSession())
			{
				Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> fileSelector = e =>
				{
					return targetDir != null ?
						e.InDirectory(targetDir)
						:
						e.PathIs(targetPath);
				};

				var authors = s.SelectionDSL()
					.Files().Reselect(fileSelector)
					.Commits().TouchFiles()
					.Select(x => x.Author).Distinct().OrderBy(x => x)
					.ToList();
				
				int maxAuthorLen = authors.Select(x => x.Length).Max();
				if (maxAuthorLen < 6)
				{
					maxAuthorLen = 6;
				}
				string format = "{0,X}{1,13}{2,13}{3,13}{4,15}".Replace("X", maxAuthorLen.ToString());
				string line = "-".Repeat(maxAuthorLen + 13 + 13 + 13 + 15);
				Console.WriteLine(format, "Author", "Added LOC", "Removed LOC", "Current LOC", "Max & Min Age");
				Console.WriteLine(line);
				foreach (var author in authors)
				{
					var addedCode = s.SelectionDSL()
						.Commits().AuthorIs(author)
						.Files().Reselect(fileSelector)
						.Modifications().InFiles()
						.CodeBlocks().InModifications().AddedInitiallyInCommits()
						.Fixed();
					var removedCode = s.SelectionDSL()
						.Commits().AuthorIs(author)
						.Files().Reselect(fileSelector)
						.Modifications().InCommits().InFiles()
						.CodeBlocks().InModifications().Deleted()
						.Fixed();

					DateTime now = DateTime.Now;
					double addedLoc = addedCode.CalculateLOC();
					var remainingCode = addedCode.CalculateRemainingCodeSize(s.LastRevision());
					var remainingCodeSize = remainingCode.Sum(x => x.Value);

					var remainingCodeAge = 
						(
							from cb in remainingCode
							let CommitID = s.Queryable<CodeBlock>()
								.Single(x => x.ID == cb.Key)
								.AddedInitiallyInCommitID
							from c in s.Queryable<Commit>()
								where c.ID == CommitID
							select (now - c.Date).Days
						).ToList();
					
					string age = remainingCodeAge.Count() > 0 ?
						remainingCodeAge.Max().ToString() + "-" + remainingCodeAge.Min().ToString()
						:
						"";
					
					Console.WriteLine(format,
						author,
						addedLoc,
						- removedCode.CalculateLOC(),
						remainingCode.Sum(x => x.Value),
						age
					);
				}
				Console.WriteLine(line);
			}
		}
	}
}
