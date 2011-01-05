/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl.Svn
{
	public class SvnTouchedPath
	{
		public enum SvnTouchedPathAction
		{
			NONE,
			ADDED,
			MODIFIED,
			DELETED,
			REPLACED
		}

		public string Path;
		public bool IsFile;
		public SvnTouchedPathAction Action;
		public string SourcePath;
		public string SourceRevision;
	}
}
