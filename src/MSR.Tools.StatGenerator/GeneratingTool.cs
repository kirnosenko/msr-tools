/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;

namespace MSR.Tools.StatGenerator
{
	public class GeneratingTool : Tool
	{
		public GeneratingTool(string configFile)
			: base(configFile)
		{
			data.ReadOnly = true;
		}
		public void GenerateStat(string outputDir, string templateDir)
		{
			VelocityContext context = new VelocityContext();
			Velocity.Init();
			
			CompositeStatBuilder builder = new CompositeStatBuilder(new IStatBuilder[]
			{
				new GeneralStatBuilder(),
				new AuthorStatBuilder(),
				new FileStatBuilder()
			});
			builder.AddData(data, context);

			File.Copy(templateDir + "/stats.css", outputDir + "/stats.css", true);
			File.Copy(templateDir + "/sortable.js", outputDir + "/sortable.js", true);

			using (TextWriter writer = new StreamWriter(outputDir + "/general.html"))
			{
				Velocity.MergeTemplate(
					templateDir + "/general.html",
					Encoding.UTF8.WebName,
					context,
					writer
				);
			}
			using (TextWriter writer = new StreamWriter(outputDir + "/authors.html"))
			{
				Velocity.MergeTemplate(
					templateDir + "/authors.html",
					Encoding.UTF8.WebName,
					context,
					writer
				);
			}
			using (TextWriter writer = new StreamWriter(outputDir + "/files.html"))
			{
				Velocity.MergeTemplate(
					templateDir + "/files.html",
					Encoding.UTF8.WebName,
					context,
					writer
				);
			}
		}
	}
}
