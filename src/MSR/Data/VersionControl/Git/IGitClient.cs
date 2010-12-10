/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Data.VersionControl.Git
{
	public interface IGitClient
	{
		/// <summary>
		/// Get list of all revisions in reverse topological order
		/// (i.e. descendant commits are after their parents).
		/// </summary>
		/// <returns>Resulting stream.</returns>
		Stream RevList();
		Stream Log(string revision);
		Stream Diff(string revision, string path);
		Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision);
		Stream Blame(string revision, string path);
	}
}
