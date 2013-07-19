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

namespace MSR.Data.Entities.DSL.Mapping
{
	[TestFixture]
	public class BugFixMappingExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_add_bugfix()
		{
			mappingDSL
				.AddCommit("1")
				.AddCommit("2").IsBugFix()
				.AddCommit("3")
			.Submit();

			Queryable<BugFix>().Count()
				.Should().Be(1);
			Queryable<BugFix>().Single().Commit.Revision
				.Should().Be("2");
		}
	}
}
