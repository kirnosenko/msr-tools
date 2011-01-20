/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Diagnostics;

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
	}
}
