/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Globalization;
using System.Linq;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public abstract class SelectPathByList : IPathSelector
	{
		public SelectPathByList()
		{
			IgnoreCase = true;
		}
		public bool InSelection(string path)
		{
			if (Paths != null)
			{
				if (Paths.Any(x => string.Compare(x, path, IgnoreCase) == 0))
				{
					return SelectMatchedPath();
				}
			}
			if (Dirs != null)
			{
				if (Dirs.Any(x => path.StartsWith(x + "/", IgnoreCase, CultureInfo.InvariantCulture)))
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
		public bool IgnoreCase
		{
			get; set;
		}
		protected abstract bool SelectMatchedPath();
	}
}
