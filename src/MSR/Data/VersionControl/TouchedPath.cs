/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl
{
	public class TouchedPath
	{
		public enum TouchedPathAction
		{
			ADDED,
			MODIFIED,
			DELETED
		}

		public string Path;
		public bool IsFile;
		public TouchedPathAction Action;
		public string SourcePath;
		public string SourceRevision;
	}
}
