/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NVelocity;
using NVelocity.App;

using MSR.Data;

namespace MSR.Tools.StatGenerator
{
	public class StatBuilder
	{
		private IStatPageBuilder[] builders;
		
		public StatBuilder(IStatPageBuilder[] builders)
		{
			this.builders = builders;
			SharedPageTemplate = "page.html";
			FilesToCopy = new string[]
			{
				"stats.css", "sortable.js"
			};
		}
		public void GenerateStat(IDataStore data, string dir, string outputDir, string templateDir)
		{
			Velocity.Init();
			
			var menu = builders.Select(x => new
			{
				url = x.PageTemplate,
				name = x.PageName
			});
			
			foreach (var builder in builders)
			{
				VelocityContext context = new VelocityContext();
				context.Put("menu", menu);
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
						templateDir + "/" + builder.PageTemplate,
						Encoding.UTF8.WebName,
						context,
						writer
					);
					writer.Flush();

					pageContent.Seek(0, SeekOrigin.Begin);
					context.Put("content", new StreamReader(pageContent).ReadToEnd());
					using (TextWriter writer2 = new StreamWriter(outputDir + "/" + builder.PageTemplate))
					{
						Velocity.MergeTemplate(
							templateDir + "/" + SharedPageTemplate,
							Encoding.UTF8.WebName,
							context,
							writer2
						);
					}
				}
			}
			foreach (var fileToCopy in FilesToCopy)
			{
				File.Copy(templateDir + "/" + fileToCopy, outputDir + "/" + fileToCopy, true);
			}
		}
		public string SharedPageTemplate
		{
			get; set;
		}
		public string[] FilesToCopy
		{
			get; set;
		}
	}
}
