/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public abstract class SelectPathByExtension : IPathSelector
	{
		private List<string> extensions;
		
		public SelectPathByExtension(string[] extensions)
		{
			this.extensions = new List<string>(extensions);
		}
		public bool InSelection(string path)
		{
			string ext = Path.GetExtension(path).ToLower();	
			
			if (extensions.Contains(ext))
			{
				return SelectMatchedPath();
			}
			return ! SelectMatchedPath();
		}
		protected abstract bool SelectMatchedPath();
	}
}
