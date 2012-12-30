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

namespace MSR.Data.Entities.DSL.Mapping
{
	[TestFixture]
	public class ReleaseMappingExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_add_release()
		{
			mappingDSL
				.AddCommit("1")
				.AddCommit("2").IsRelease("1.0.0")
				.AddCommit("3")
			.Submit();

			Queryable<Release>().Count()
				.Should().Be(1);
			Queryable<Release>().Single()
				.Satisfy(x =>
					x.Tag == "1.0.0"
					&&
					x.Commit.Revision == "2"
				);
		}
	}
}
