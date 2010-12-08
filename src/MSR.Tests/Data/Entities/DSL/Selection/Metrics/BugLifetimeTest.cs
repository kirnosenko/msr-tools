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

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	[TestFixture]
	public class BugLifetimeTest : BaseRepositoryTest
	{
		[Test]
		public void Should_calc_bug_lifetime_as_time_between_fix_and_adding_of_code_was_fixed()
		{
			mappingDSL
				.AddCommit("1").At(DateTime.Today.AddDays(-9))
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("3").At(DateTime.Today.AddDays(-7))
					.File("file1").Modified()
						.Code(50)
			.Submit()
				.AddCommit("4").At(DateTime.Today.AddDays(-6)).IsBugFix()
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("3")
						.Code(10)
			.Submit()
				.AddCommit("8").At(DateTime.Today.AddDays(-2)).IsBugFix()
					.File("file1").Modified()
						.Code(-1).ForCodeAddedInitiallyInRevision("1")
						.Code(-1).ForCodeAddedInitiallyInRevision("3")
						.Code(5)
			.Submit();
			
			selectionDSL
				.BugFixes()
				.CalculateMaxBugLifetime().ToArray()
					.Should().Have.SameSequenceAs(new double[] { 1, 7 });
			selectionDSL
				.BugFixes()
				.CalculateMinBugLifetime().ToArray()
					.Should().Have.SameSequenceAs(new double[] { 1, 5 });
			selectionDSL
				.BugFixes()
				.CalculateAvarageBugLifetime().ToArray()
					.Should().Have.SameSequenceAs(new double[] { 1, 6 });
			selectionDSL
				.BugFixes()
				.CalculateBugLifetimeSpread().ToArray()
					.Should().Have.SameSequenceAs(new double[] { 0, 2 });
		}
	}
}
