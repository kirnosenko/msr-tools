/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl.Svn
{
	public enum TouchedPathSvnAction
	{
		NONE,
		ADDED,
		MODIFIED,
		DELETED,
		REPLACED
	}
}
