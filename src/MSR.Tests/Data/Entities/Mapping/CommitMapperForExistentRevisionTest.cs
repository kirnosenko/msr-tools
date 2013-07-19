/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
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
	public class CommitMapperForExistentRevisionTest : BaseMapperTest
	{
		private CommitMapperForExistentRevision mapper;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			mapper = new CommitMapperForExistentRevision(scmData);
		}
		[Test]
		public void Should_not_add_new_commit()
		{
			mappingDSL
				.AddCommit("1")
			.Submit();
			
			mappingDSL.Revision = "1";
			mapper.Map(mappingDSL).Count()
				.Should().Be(1);
			Queryable<Commit>().Count()
				.Should().Be(1);
		}
	}
}
