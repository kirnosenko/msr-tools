/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.Entities.Mapping
{
	public class SkipPathByExtension : IPathSelector
	{
		private List<string> extensionsToSkip;
		
		public SkipPathByExtension(string[] extensionsToSkip)
		{
			this.extensionsToSkip = new List<string>(extensionsToSkip);
		}
		public bool InSelection(string path)
		{
			string ext = Path.GetExtension(path).ToLower();	
			
			if (extensionsToSkip.Contains(ext))
			{
				return false;
			}
			return true;
		}
	}
}
