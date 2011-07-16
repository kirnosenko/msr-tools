/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public abstract class SelectPathByExtension : IPathSelector
	{
		public SelectPathByExtension()
		{
			IgnoreCase = true;
		}
		public bool InSelection(string path)
		{
			if (Extensions != null)
			{
				if (Extensions.Any(x => string.Compare(x, Path.GetExtension(path), IgnoreCase) == 0))
				{
					return SelectMatchedPath();
				}
			}
			
			return ! SelectMatchedPath();
		}
		public string[] Extensions
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
