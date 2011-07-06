/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class SkipPathByDirectory : IPathSelector
	{
		private string[] dirsToSkip;
		
		public SkipPathByDirectory(string[] dirsToSkip)
		{
			this.dirsToSkip = dirsToSkip;
		}
		public bool InSelection(string path)
		{
			foreach (var dir in dirsToSkip)
			{
				if (path.StartsWith(dir))
				{
					return false;
				}
			}
			return true;
		}
	}
}
