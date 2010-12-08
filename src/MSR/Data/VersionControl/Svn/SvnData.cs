/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MSR.Data.VersionControl.Svn
{
	public class SvnData : IScmData
	{
		private ISvnClient svn;
		private string fullDiffRevision;
		private SvnFullUniDiff fullDiff;
		
		public SvnData(ISvnClient svn)
		{
			this.svn = svn;
		}
		public ILog Log(string revision)
		{
			using (var log = svn.Log(revision))
			{
				return new SvnLog(
					log,
					() => svn.DiffSum(revision),
					svn.RepositoryPath
				);
			}
		}
		public IDiff Diff(string revision, string filePath)
		{
			if (fullDiffRevision != revision)
			{
				using (var diff = svn.Diff(revision))
				{
					fullDiff = new SvnFullUniDiff(diff);
				}
				fullDiffRevision = revision;
			}
			return fullDiff[filePath];
		}
		public IDiff Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			using (var diff = svn.Diff(newPath, newRevision, oldPath, oldRevision))
			{
				return new FileUniDiff(diff);
			}
		}
		public IBlame Blame(string revision, string filePath)
		{
			using (var blame = svn.Blame(revision, filePath))
			{
				return SvnBlame.Parse(blame);
			}
		}
		public string RevisionByNumber(int revisionNumber)
		{
			return revisionNumber.ToString();
		}
	}
}
