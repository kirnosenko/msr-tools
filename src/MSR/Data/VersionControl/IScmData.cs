/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
		/// <summary>
		/// Return next revision after specified.
		/// </summary>
		/// <param name="revision">SCM revision.</param>
		/// <returns>Next revision or null if there is no one.</returns>
		string NextRevision(string revision);
		string PreviousRevision(string revision);
	}
}
