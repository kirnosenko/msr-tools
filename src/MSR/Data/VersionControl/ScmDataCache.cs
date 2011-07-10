/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data.VersionControl
{
	public class ScmDataCache : IScmData
	{
		private IScmData innerScmData;
		private SmartDictionary<string,ILog> logs;
		private SmartDictionary<string,SmartDictionary<string,IDiff>> diffs;
		private SmartDictionary<string,IBlame> blames;
		
		public ScmDataCache(IScmData innerScmData)
		{
			this.innerScmData = innerScmData;
			
			logs = new FixedSizeDictionary<string,ILog>(1, x => innerScmData.Log(x));
			diffs = new FixedSizeDictionary<string,SmartDictionary<string,IDiff>>(1, x =>
				new SmartDictionary<string,IDiff>(y => innerScmData.Diff(x, y))
			);
			blames = new FixedSizeDictionary<string,IBlame>(100);
		}
		public ILog Log(string revision)
		{
			return logs[revision];
		}
		public IDiff Diff(string revision, string filePath)
		{
			return diffs[revision][filePath];
		}
		public IDiff Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			return innerScmData.Diff(newPath, newRevision, oldPath, oldRevision);
		}
		public IBlame Blame(string revision, string filePath)
		{
			string key = revision + filePath;
			if (! blames.ContainsKey(key))
			{
				blames.Add(key, innerScmData.Blame(revision, filePath));
			}
			return blames[key];
		}
		public string RevisionByNumber(int revisionNumber)
		{
			return innerScmData.RevisionByNumber(revisionNumber);
		}
		public string NextRevision(string revision)
		{
			return innerScmData.NextRevision(revision);
		}
		public string PreviousRevision(string revision)
		{
			return innerScmData.PreviousRevision(revision);
		}
		public int BlamesToCache
		{
			set
			{
				blames = new FixedSizeDictionary<string, IBlame>(value);
			}
		}
	}
}
