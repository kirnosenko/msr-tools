/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

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
