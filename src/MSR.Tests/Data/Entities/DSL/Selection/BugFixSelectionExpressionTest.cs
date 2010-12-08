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

using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.DSL.Selection
{
	[TestFixture]
	public class BugFixSelectionExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_select_bugfixes_for_commits()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2").IsBugFix()
			.Submit()
				.AddCommit("3").IsBugFix()
			.Submit();
			
			selectionDSL
				.Commits().RevisionIs("1")
				.BugFixes().InCommits().Count()
					.Should().Be(0);
			selectionDSL
				.Commits().RevisionIs("2")
				.BugFixes().InCommits().Count()
					.Should().Be(1);
			selectionDSL
				.Commits().RevisionIs("3")
				.BugFixes().InCommits().Count()
					.Should().Be(1);
		}
		[Test]
		public void Should_select_commits_are_bugfixes()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2").IsBugFix()
			.Submit()
				.AddCommit("3").IsBugFix()
			.Submit();
			
			selectionDSL
				.Commits().AreBugFixes()
				.Select(x => x.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "2", "3" });
		}
	}
}
