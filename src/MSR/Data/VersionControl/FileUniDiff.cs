/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MSR.Data.VersionControl
{
	public class FileUniDiff : IDiff
	{
		public class LineContext
		{
			public int AddedLineNumber = 0;
			public int RemovedLineNumber = 0;
			public bool HunkStarted = false;
		}
		
		public static FileUniDiff Empty = new FileUniDiff("".ToStream());
		
		private static Regex HunkHeader = new Regex(
			@"^@@ (\-(?<OldNumber>\d+)(,\d+)?) (\+(?<NewNumber>\d+)(,\d+)?) @@",
			RegexOptions.Compiled
		);
		
		public List<int> removedLines = new List<int>();
		public List<int> addedLines = new List<int>();
		
		public FileUniDiff(Stream fileDiff)
			: this(new StreamReader(fileDiff), null)
		{
		}
		public FileUniDiff(TextReader reader, Func<string, LineContext, LineContext> additionalParsing)
		{
			string line;
			LineContext c = new LineContext();

			while (((line = reader.ReadLine()) != null) && (c != null))
			{
				if (line.StartsWith("+") && c.HunkStarted)
				{
					addedLines.Add(c.AddedLineNumber);
					c.AddedLineNumber++;
				}
				else if (line.StartsWith("-") && c.HunkStarted)
				{
					removedLines.Add(c.RemovedLineNumber);
					c.RemovedLineNumber++;
				}
				else if (line.StartsWith("@@"))
				{
					c.RemovedLineNumber = Convert.ToInt32(HunkHeader.Match(line).Result("${OldNumber}"));
					c.AddedLineNumber = Convert.ToInt32(HunkHeader.Match(line).Result("${NewNumber}"));
					c.HunkStarted = true;
				}
				else if (line.StartsWith("\\"))
				{
				}
				else
				{
					c.AddedLineNumber++;
					c.RemovedLineNumber++;

					if (additionalParsing != null)
					{
						c = additionalParsing(line, c);
					}
				}
			}
		}
		public IEnumerable<int> RemovedLines
		{
			get { return removedLines; }
		}
		public IEnumerable<int> AddedLines
		{
			get { return addedLines; }
		}
		public bool IsEmpty
		{
			get
			{
				return RemovedLines.Count() == 0 && AddedLines.Count() == 0;
			}
		}
	}
}
