/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.VersionControl.Svn
{
	public class SvnBlame : Dictionary<int,string>, IBlame
	{
		private static char[] blameSeparators = { ' ', '	' };
		
		public static SvnBlame Parse(Stream blameData)
		{
			TextReader reader = new StreamReader(blameData);
			int lineNumber = 1;
			SvnBlame blame = new SvnBlame();
			string line;

			while ((line = reader.ReadLine()) != null)
			{
				if (line != string.Empty)
				{
					blame.Add(
						lineNumber,
						line.Split(blameSeparators, StringSplitOptions.RemoveEmptyEntries)[0]
					);
				}
				lineNumber++;
			}
			
			return blame;
		}
	}
}
