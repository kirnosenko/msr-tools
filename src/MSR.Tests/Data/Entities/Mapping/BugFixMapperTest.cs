/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class BugFixMapperTest : BaseMapperTest
	{
		private BugFixMapper mapper;
		private IBugFixDetector bugFixDetector;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();		
			bugFixDetector = MockRepository.GenerateStub<IBugFixDetector>();
			mapper = new BugFixMapper(scmData, bugFixDetector);
		}
		[TestCase(true)]
		[TestCase(false)]
		public void Should_add_bugfix_for_fix_commit(bool isBugFix)
		{
			bugFixDetector.Stub(x => x.IsBugFix(null))
				.IgnoreArguments()
				.Return(isBugFix);
			
			mapper.Map(
				mappingDSL.AddCommit("1")
			);
			SubmitChanges();
			
			if (! isBugFix)
			{
				Queryable<BugFix>().Count()
					.Should().Be(0);
			}
			else
			{
				Queryable<BugFix>().Count()
					.Should().Be(1);
				Queryable<BugFix>().Single().Commit.Revision
					.Should().Be("1");
			}
		}
	}
}
