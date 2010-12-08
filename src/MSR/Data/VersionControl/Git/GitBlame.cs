/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.VersionControl.Git
{
	public class GitBlame : Dictionary<int,string>, IBlame
	{
		public static GitBlame Parse(Stream blameData)
		{
			TextReader reader = new StreamReader(blameData);
			int lineNumber = 1;
			GitBlame blame = new GitBlame();
			string line;
			
			while ((line = reader.ReadLine()) != null)
			{
				blame.Add(
					lineNumber,
					line.Remove(40)
				);
				lineNumber++;
			}

			return blame;
		}
	}
}
