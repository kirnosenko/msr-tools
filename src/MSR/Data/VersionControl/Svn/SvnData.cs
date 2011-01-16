/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
		private int numberOfRevisions;
		
		public SvnData(ISvnClient svn)
		{
			this.svn = svn;
			numberOfRevisions = GetNumberOfRevisions();
		}
		public ILog Log(string revision)
		{
			return new SvnLog(svn, revision);
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
			if (revisionNumber <= numberOfRevisions)
			{
				return revisionNumber.ToString();
			}
			return null;
		}
		public string NextRevision(string revision)
		{
			return RevisionByNumber(Convert.ToInt32(revision) + 1);
		}
		private int GetNumberOfRevisions()
		{
			using (var info = svn.Info())
			{
				TextReader reader = new StreamReader(info);

				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("Revision: "))
					{
						return Convert.ToInt32(line.Replace("Revision: ", ""));
					}
				}
			}
			return 0;
		}
	}
}
