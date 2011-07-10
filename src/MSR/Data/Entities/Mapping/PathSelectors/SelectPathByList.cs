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
			if (PathList != null)
			{
				if (PathList.Any(x => x == path))
				{
					return SelectMatchedPath();
				}
			}
			if (DirList != null)
			{
				if (DirList.Any(x => path.StartsWith(x + "/")))
				{
					return SelectMatchedPath();
				}
			}
			return !SelectMatchedPath();
		}
		public string[] PathList
		{
			get; set;
		}
		public string[] DirList
		{
			get; set;
		}
		protected abstract bool SelectMatchedPath();
	}
}
