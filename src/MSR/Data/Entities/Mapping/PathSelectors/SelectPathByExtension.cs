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
		public bool InSelection(string path)
		{
			if (Extensions != null)
			{	
				if (Extensions.Any(x => x == Path.GetExtension(path).ToLower()))
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
		protected abstract bool SelectMatchedPath();
	}
}
