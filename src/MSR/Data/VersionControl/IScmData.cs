/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data.VersionControl
{
	public interface IScmData
	{
		ILog Log(string revision);
		IDiff Diff(string revision, string filePath);
		IDiff Diff(string newPath, string newRevision, string oldPath, string oldRevision);
		IBlame Blame(string revision, string filePath);
		string RevisionByNumber(int revisionNumber);
	}
}
