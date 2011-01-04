/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSR.Data.VersionControl.Git
{
	public class GitLog : Log
	{
		public GitLog(Stream log)
		{
			TextReader reader = new StreamReader(log);
			
			Revision = reader.ReadLine();
			Author = reader.ReadLine();
			Date = DateTime.Parse(reader.ReadLine());
			Message = reader.ReadLine();
			
			string line;
			string[] blocks;
			TouchedPathGitAction action;
			
			while ((line = reader.ReadLine()) != null)
			{
				blocks = line.Split('	');
				action = ParsePathAction(blocks[0]);
				
				switch (action)
				{
					case TouchedPathGitAction.MODIFIED:
						TouchPath(TouchedPath.TouchedPathAction.MODIFIED, blocks[1]);
						break;
					case TouchedPathGitAction.ADDED:
						TouchPath(TouchedPath.TouchedPathAction.ADDED, blocks[1]);
						break;
					case TouchedPathGitAction.DELETED:
						TouchPath(TouchedPath.TouchedPathAction.DELETED, blocks[1]);
						break;
					case TouchedPathGitAction.RENAMED:
						TouchPath(TouchedPath.TouchedPathAction.DELETED, blocks[1]);
						TouchPath(TouchedPath.TouchedPathAction.ADDED, blocks[2], blocks[1]);
						break;
					case TouchedPathGitAction.COPIED:
						TouchPath(TouchedPath.TouchedPathAction.ADDED, blocks[2], blocks[1]);
						break;
					default:
						break;
				}
			}
			touchedPaths.Sort((x,y) => string.CompareOrdinal(x.Path.ToLower(), y.Path.ToLower()));
		}
		private void TouchPath(TouchedPath.TouchedPathAction action, string path)
		{
			TouchPath(action, path, null);
		}
		private void TouchPath(TouchedPath.TouchedPathAction action, string path, string sourcePath)
		{
			path = "/" + path;
			if (sourcePath != null)
			{
				sourcePath = "/" + sourcePath;
			}
			touchedPaths.Add(new TouchedPath()
			{
				Path = path,
				IsFile = true,
				Action = action,
				SourcePath = sourcePath
			});
		}
		private TouchedPathGitAction ParsePathAction(string action)
		{
			switch (action.Substring(0, 1).ToUpper())
			{
				case "M": return TouchedPathGitAction.MODIFIED;
				case "A": return TouchedPathGitAction.ADDED;
				case "D": return TouchedPathGitAction.DELETED;
				case "R": return TouchedPathGitAction.RENAMED;
				case "C": return TouchedPathGitAction.COPIED;
			}
			throw new ApplicationException(string.Format("{0} - is invalid path action", action));
		}
	}
}
