/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
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
				string format = "{0,X}{1,15}{2,15}{3,15}".Replace("X", maxAuthorLen.ToString());
				string line = "-".Repeat(maxAuthorLen + 15 + 15 + 15);
				Console.WriteLine(format, "Author", "Added LOC", "Removed LOC", "Current LOC");
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
					
					double addedLoc = addedCode.CalculateLOC();
					Console.WriteLine(format,
						author,
						addedLoc,
						- removedCode.CalculateLOC(),
						addedLoc + addedCode.ModifiedBy().CalculateLOC()
					);
				}
				Console.WriteLine(line);
			}
		}
	}
}
