/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl.Git
{
	public enum TouchedFileGitAction
	{
		ADDED,
		MODIFIED,
		DELETED,
		RENAMED,
		COPIED
	}
}
