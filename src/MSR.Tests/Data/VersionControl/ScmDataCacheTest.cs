/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace MSR.Data.VersionControl
{
	[TestFixture]
	public class ScmDataCacheTest
	{
		private ScmDataCache cache;
		private IScmData scmDataStub;
		private List<string> calls;
		
		[SetUp]
		public void SetUp()
		{
			scmDataStub = MockRepository.GenerateStub<IScmData>();
			calls = new List<string>();
			scmDataStub.Stub(x => x.Log(null))
				.IgnoreArguments()
				.Do((Func<string,ILog>)(r =>
				{
					calls.Add("log" + r);
					return null;
				}));
			scmDataStub.Stub(x => x.Diff(null,null))
				.IgnoreArguments()
				.Do((Func<string,string,IDiff>)((r,f) =>
				{
					calls.Add("diff" + r + f);
					return null;
				}));
			scmDataStub.Stub(x => x.Blame(null, null))
				.IgnoreArguments()
				.Do((Func<string,string,IBlame>)((r, f) =>
				{
					calls.Add("blame" + r + f);
					return null;
				}));
			cache = new ScmDataCache(scmDataStub);
		}
		[Test]
		public void Should_keep_single_log()
		{
			cache.Log("1");
			cache.Log("1");
			cache.Log("2");
			
			VerifyCalls("log1", "log2");
		}
		[Test]
		public void Should_keep_diffs_for_single_revision()
		{
			cache.Diff("2", "file1");
			cache.Diff("2", "file2");
			cache.Diff("2", "file1");
			cache.Diff("3", "file1");
			cache.Diff("2", "file1");

			VerifyCalls("diff2file1", "diff2file2", "diff3file1", "diff2file1");
		}
		[Test]
		public void Should_keep_specified_number_of_blames()
		{
			cache.BlamesToCache = 2;
			
			cache.Blame("2", "file1");
			cache.Blame("3", "file1");
			cache.Blame("2", "file1");
			cache.Blame("2", "file2");
			cache.Blame("3", "file2");
			cache.Blame("3", "file1");

			VerifyCalls("blame2file1", "blame3file1", "blame2file2", "blame3file2", "blame3file1");
		}
		private void VerifyCalls(params string[] expectedCalls)
		{
			calls.ToArray()
				.Should().Have.SameSequenceAs(expectedCalls);
		}
	}
}
