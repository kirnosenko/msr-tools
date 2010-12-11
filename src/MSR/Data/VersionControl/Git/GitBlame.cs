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
	/// <summary>
	/// Keeps the revision last modified each line.
	/// </summary>
	public class GitBlame : Dictionary<int,string>, IBlame
	{
		/// <summary>
		/// Parse blame stream.
		/// </summary>
		/// <param name="blameData">Git blame in incremental format.</param>
		/// <returns>Dictionary of line numbers (from 1) with revisions.</returns>
		public static GitBlame Parse(Stream blameData)
		{
			TextReader reader = new StreamReader(blameData);
			GitBlame blame = new GitBlame();
			string line;
			
			while ((line = reader.ReadLine()) != null)
			{
				if ((line.Length >= 46) && (line.Length < 100))
				{
					string[] parts = line.Split(' ');
					if ((parts.Length == 4) && (parts[0].Length == 40))
					{
						int lines = Convert.ToInt32(parts[3]);
						int startLine = Convert.ToInt32(parts[2]);
						for (int i = 0; i < lines; i++)
						{
							blame.Add(startLine + i, parts[0]);
						}
					}
				}
			}
			
			return blame;
		}
	}
}
