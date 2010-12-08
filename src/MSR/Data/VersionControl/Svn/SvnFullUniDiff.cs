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
	public class SvnFullUniDiff : Dictionary<string,FileUniDiff>
	{
		public SvnFullUniDiff(Stream fileDiff)
			: base()
		{
			Queue<string> fileNames = new Queue<string>();
			TextReader reader = new StreamReader(fileDiff);
			
			do
			{
				FileUniDiff diff = new FileUniDiff(reader, (line,c) =>
				{
					if (line.StartsWith("Index: "))
					{
						fileNames.Enqueue(line.Replace("Index: ", "/"));
						if (fileNames.Count > 1)
						{
							return null;
						}
					}
					return c;
				});
				Add(fileNames.Dequeue(), diff);
			} while (fileNames.Count > 0);
		}
	}
}
