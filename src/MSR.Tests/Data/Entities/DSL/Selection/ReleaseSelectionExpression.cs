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

using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.DSL.Selection
{
	[TestFixture]
	public class ReleaseSelectionExpression : BaseRepositoryTest
	{
		[Test]
		public void Can_select_release_commits()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2").IsRelease("1.0")
			.Submit()
				.AddCommit("3")
			.Submit();
			
			selectionDSL
				.Commits().AreReleases().Count()
					.Should().Be(1);
			selectionDSL
				.Commits().AreReleases()
				.Select(x => x.Revision).ToArray()
					.Should().Have.SameValuesAs(new string[] { "2" });
		}
	}
}
