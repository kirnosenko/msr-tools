/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping
{
	public interface IReleaseDetector
	{
		/// <summary>
		/// Return tag for specified commit.
		/// </summary>
		/// <param name="commit">Commit</param>
		/// <returns>Tag if exists and null otherwise.</returns>
		string TagForCommit(Commit commit);
	}
}
