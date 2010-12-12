/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Tools.Debugger
{
	public class DebuggingTool : Tool
	{
		public DebuggingTool(string configFile)
			: base(configFile)
		{
		}
		public void Blame(string path, string revision)
		{
			var blame = scmDataNoCache.Blame(revision, path);
			
			SmartDictionary<string, IEnumerable<int>> output =
				new SmartDictionary<string,IEnumerable<int>>((x) => new List<int>());
			for (int i = 1; i <= blame.Count; i++)
			{
				(output[blame[i]] as List<int>).Add(i);
			}
			foreach (var rev in output.OrderBy(x => x.Key))
			{
				Console.WriteLine("{0} {1}", rev.Key, rev.Value.Count());
			}
		}
	}
}
