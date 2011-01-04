/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NVelocity;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Tools.StatGenerator
{
	public class FileStatBuilder : IStatBuilder
	{
		public void AddData(IDataStore data, VelocityContext context)
		{
			using (var s = data.OpenSession())
			{
				var paths = s.SelectionDSL()
					.Files().Exist()
					.Select(f => f.Path);
				var files_count = paths.Count();
				
				List<string> extensions = new List<string>();
				foreach (var path in paths)
				{
					string ext = Path.GetExtension(path);
					if (! extensions.Contains(ext))
					{
						extensions.Add(ext);
					}
				}
				
				List<object> extObjects = new List<object>();
				foreach (var ext in extensions)
				{
					var code = s.SelectionDSL()
						.Files().Exist().PathEndsWith(ext)
						.Modifications().InFiles()
						.CodeBlocks().InModifications().Fixed();
					int ext_files_count = code.Files().Again().Count();
					
					extObjects.Add(new {
						ext = ext,
						files = ext_files_count,
						files_percent = ((double)ext_files_count / files_count * 100).ToString("F02"),
						dd = code.CalculateTraditionalDefectDensity().ToString("F02"),
						added = code.Added().CalculateLOC(),
						addedInFixes = code.Added().InBugFixes().CalculateLOC(),
						deleted = - code.Deleted().CalculateLOC(),
						deletedInFixes = - code.Deleted().InBugFixes().CalculateLOC(),
						current = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC()
					});
				}
				context.Put("file_types", extObjects);
			}
		}
	}
}
