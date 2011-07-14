/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public abstract class SelectPathByList : IPathSelector
	{
		public bool InSelection(string path)
		{
			if (Paths != null)
			{
				if (Paths.Any(x => x == path))
				{
					return SelectMatchedPath();
				}
			}
			if (Dirs != null)
			{
				if (Dirs.Any(x => path.StartsWith(x + "/")))
				{
					return SelectMatchedPath();
				}
			}
			return ! SelectMatchedPath();
		}
		public string[] Paths
		{
			get; set;
		}
		public string[] Dirs
		{
			get; set;
		}
		protected abstract bool SelectMatchedPath();
	}
}
