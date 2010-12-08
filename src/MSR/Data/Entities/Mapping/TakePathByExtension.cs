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
	public class TakePathByExtension : IPathSelector
	{
		private List<string> extensionsToTake;
		
		public TakePathByExtension(string[] extensionsToTake)
		{
			this.extensionsToTake = new List<string>(extensionsToTake);
		}
		public bool InSelection(string path)
		{
			string ext = Path.GetExtension(path).ToLower();	
			
			if (extensionsToTake.Contains(ext))
			{
				return true;
			}
			return false;
		}
	}
}
