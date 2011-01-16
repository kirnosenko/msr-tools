/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Data.VersionControl.Svn
{
	public interface ISvnClient
	{
		string RepositoryPath { get; }
		Stream Info();
		Stream Log(string revision);
		Stream DiffSum(string revision);
		Stream List(string revision, string dirPath);
		Stream Diff(string revision);
		Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision);
		Stream Blame(string revision, string filePath);
	}
}
