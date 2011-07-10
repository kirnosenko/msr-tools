/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.VersionControl.Git
{
	public class GitData : IScmData
	{
		private IGitClient git;
		
		public List<string> revisions;
		
		public GitData(IGitClient git)
		{
			this.git = git;
		}
		public ILog Log(string revision)
		{
			using (var log = git.Log(revision))
			{
				return new GitLog(log);
			}
		}
		public IDiff Diff(string revision, string filePath)
		{
			using (var diff = git.Diff(revision, filePath))
			{
				return new FileUniDiff(diff);
			}
		}
		public IDiff Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			using (var diff = git.Diff(newPath, newRevision, oldPath, oldRevision))
			{
				return new FileUniDiff(diff);
			}
		}
		public IBlame Blame(string revision, string filePath)
		{
			using (var blame = git.Blame(revision, filePath))
			{
				return GitBlame.Parse(blame);
			}
		}
		public string RevisionByNumber(int revisionNumber)
		{
			if (revisions == null)
			{
				GetAllRevisions();
			}
			if (revisionNumber - 1 < revisions.Count)
			{
				return revisions[revisionNumber - 1];
			}
			else
			{
				return null;
			}
		}
		public string NextRevision(string revision)
		{
			if (revisions == null)
			{
				GetAllRevisions();
			}
			int revisionNumber = revisions.IndexOf(revision) + 1;
			return RevisionByNumber(revisionNumber + 1);
		}
		public string PreviousRevision(string revision)
		{
			if (revisions == null)
			{
				GetAllRevisions();
			}
			int revisionNumber = revisions.IndexOf(revision) + 1;
			return RevisionByNumber(revisionNumber - 1);
		}
		private void GetAllRevisions()
		{
			revisions = new List<string>();
			
			using (var revlist = git.RevList())
			{
				TextReader reader = new StreamReader(revlist);
				
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					revisions.Add(line);
				}
			}
		}
	}
}
