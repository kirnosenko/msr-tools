/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NVelocity;
using NVelocity.App;

using MSR.Data;

namespace MSR.Tools.StatGenerator
{
	public class StatBuilder
	{
		public StatBuilder()
		{
			PageBuilders = new IStatPageBuilder[] {};
			SharedPageTemplate = "page.html";
			FilesToCopy = new string[]
			{
				"stats.css",
				"sortable.js",
				"g.bar-min.js",
				"g.dot-min.js",
				"g.line-min.js",
				"g.pie-min.js",
				"g.raphael-min.js",
				"raphael-min.js",
			};
			TargetDir = "/";
			OutputDir = Environment.CurrentDirectory;
			TemplateDir = "./templates/html";
		}
		public void GenerateStat(IDataStore data)
		{
			Velocity.Init();
			
			var menu = PageBuilders.Select(x => new
			{
				url = x.PageTemplate,
				name = x.PageName
			});
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			version = version.Substring(0, version.LastIndexOf('.'));
			
			foreach (var builder in PageBuilders)
			{
				if (builder.TargetDir == null)
				{
					builder.TargetDir = TargetDir;
				}
				
				VelocityContext context = new VelocityContext();
				context.Put("menu", menu);
				context.Put("version", version);
				using (var s = data.OpenSession())
				{
					foreach (var obj in builder.BuildData(s))
					{
						context.Put(obj.Key, obj.Value);
					}
				}
				
				using (MemoryStream pageContent = new MemoryStream())
				using (TextWriter writer = new StreamWriter(pageContent))
				{
					Velocity.MergeTemplate(
						TemplateDir + "/" + builder.PageTemplate,
						Encoding.UTF8.WebName,
						context,
						writer
					);
					writer.Flush();

					pageContent.Seek(0, SeekOrigin.Begin);
					context.Put("content", new StreamReader(pageContent).ReadToEnd());
					using (TextWriter writer2 = new StreamWriter(OutputDir + "/" + builder.PageTemplate))
					{
						Velocity.MergeTemplate(
							TemplateDir + "/" + SharedPageTemplate,
							Encoding.UTF8.WebName,
							context,
							writer2
						);
					}
				}
			}
			foreach (var fileToCopy in FilesToCopy)
			{
				File.Copy(TemplateDir + "/" + fileToCopy, OutputDir + "/" + fileToCopy, true);
			}
		}
		public IStatPageBuilder[] PageBuilders
		{
			get; set;
		}
		public string SharedPageTemplate
		{
			get; set;
		}
		public string[] FilesToCopy
		{
			get; set;
		}
		public string TargetDir
		{
			get; set;
		}
		public string OutputDir
		{
			get; set;
		}
		public string TemplateDir
		{
			get; set;
		}
	}
}
