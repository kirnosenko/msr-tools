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
			touchedFiles = new List<TouchedFile>();
			
			Revision = reader.ReadLine();
			Author = reader.ReadLine();
			Date = DateTime.Parse(reader.ReadLine());
			Message = reader.ReadLine();
			
			string line;
			string[] blocks;
			TouchedFileGitAction action;
			
			while ((line = reader.ReadLine()) != null)
			{
				blocks = line.Split('	');
				action = ParsePathAction(blocks[0]);
				
				switch (action)
				{
					case TouchedFileGitAction.MODIFIED:
						TouchFile(TouchedFile.TouchedFileAction.MODIFIED, blocks[1]);
						break;
					case TouchedFileGitAction.ADDED:
						TouchFile(TouchedFile.TouchedFileAction.ADDED, blocks[1]);
						break;
					case TouchedFileGitAction.DELETED:
						TouchFile(TouchedFile.TouchedFileAction.DELETED, blocks[1]);
						break;
					case TouchedFileGitAction.RENAMED:
						TouchFile(TouchedFile.TouchedFileAction.DELETED, blocks[1]);
						TouchFile(TouchedFile.TouchedFileAction.ADDED, blocks[2], blocks[1]);
						break;
					case TouchedFileGitAction.COPIED:
						TouchFile(TouchedFile.TouchedFileAction.ADDED, blocks[2], blocks[1]);
						break;
					default:
						break;
				}
			}
			touchedFiles.Sort((x,y) => string.CompareOrdinal(x.Path.ToLower(), y.Path.ToLower()));
		}
		private void TouchFile(TouchedFile.TouchedFileAction action, string path)
		{
			TouchFile(action, path, null);
		}
		private void TouchFile(TouchedFile.TouchedFileAction action, string path, string sourcePath)
		{
			path = "/" + path;
			if (sourcePath != null)
			{
				sourcePath = "/" + sourcePath;
			}
			touchedFiles.Add(new TouchedFile()
			{
				Path = path,
				Action = action,
				SourcePath = sourcePath
			});
		}
		private TouchedFileGitAction ParsePathAction(string action)
		{
			switch (action.Substring(0, 1).ToUpper())
			{
				case "M": return TouchedFileGitAction.MODIFIED;
				case "A": return TouchedFileGitAction.ADDED;
				case "D": return TouchedFileGitAction.DELETED;
				case "R": return TouchedFileGitAction.RENAMED;
				case "C": return TouchedFileGitAction.COPIED;
			}
			throw new ApplicationException(string.Format("{0} - is invalid path action", action));
		}
	}
}
