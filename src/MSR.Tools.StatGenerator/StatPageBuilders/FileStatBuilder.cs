/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
	public class FileStatBuilder : StatPageBuilder
	{
		private static char[] pathSeparators = new char[] { '/' };
		
		public FileStatBuilder()
		{
			PageName = "Files";
			PageTemplate = "files.html";
			PathDepthToKeep = new int[] { 1, 2, 3 };
		}
		public override IDictionary<string,object> BuildData(IRepository repository)
		{
			Dictionary<string,object> result = new Dictionary<string,object>();

			var paths = repository.SelectionDSL()
				.Files().InDirectory(TargetDir).Exist()
				.Select(f => f.Path);
			var files_count = paths.Count();
			
			List<string> extensions = new List<string>();
			foreach (var path in paths)
			{
				string ext = Path.GetExtension(path);
				if ((ext != "") && (! extensions.Contains(ext)))
				{
					extensions.Add(ext);
				}
			}
			
			List<object> extObjects = new List<object>();
			foreach (var ext in extensions)
			{
				var code = repository.SelectionDSL()
					.Files().InDirectory(TargetDir).Exist().PathEndsWith(ext)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().Fixed();
				int ext_files_count = code.Files().Again().Count();
				
				extObjects.Add(new {
					name = ext,
					files = string.Format("{0} ({1}%)",
						ext_files_count,
						((double)ext_files_count / files_count * 100).ToString("F02")
					),
					dd = code.CalculateDefectDensity().ToString("F03"),
					added_loc = code.Added().CalculateLOC(),
					removed_loc = - code.Deleted().CalculateLOC(),
					remain_loc = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC()
				});
			}
			result.Add("exts", extObjects);

			List<string> dirs = new List<string>();
			foreach (var path in paths)
			{
				string dir = Path.GetDirectoryName(path).Replace("\\", "/");
				if ((PathDepthToKeep.Contains(dir.Split(pathSeparators).Length - 1)) && (! dirs.Contains(dir)))
				{
					dirs.Add(dir);
				}
			}
			
			List<object> dirObjects = new List<object>();
			foreach (var dir in dirs)
			{
				var code = repository.SelectionDSL()
					.Files().InDirectory(dir).Exist()
					.Modifications().InFiles()
					.CodeBlocks().InModifications().Fixed();
				int dir_files_count = code.Files().Again().Count();
				
				dirObjects.Add(new
				{
					name = dir,
					files = string.Format("{0} ({1}%)",
						dir_files_count,
						((double)dir_files_count / files_count * 100).ToString("F02")
					),
					dd = code.CalculateDefectDensity().ToString("F03"),
					added_loc = code.Added().CalculateLOC(),
					removed_loc = - code.Deleted().CalculateLOC(),
					remain_loc = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC()
				});
			}
			result.Add("dirs", dirObjects);
			
			return result;
		}
		public int[] PathDepthToKeep
		{
			get; set;
		}
	}
}
