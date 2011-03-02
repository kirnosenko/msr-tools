/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping
{
	public class NullReleaseDetector : IReleaseDetector
	{
		public string TagForCommit(Commit commit)
		{
			return null;
		}
	}
}
