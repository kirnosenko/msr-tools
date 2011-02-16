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

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	[TestFixture]
	public class DefectCodeSizeTest : BaseRepositoryTest
	{
		[Test]
		public void Should_return_zero_for_empty_code()
		{
			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize()
					.Should().Be(0);
		}
		[Test]
		public void Should_calc_size_of_defect_code()
		{
			AddCode();

			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize()
					.Should().Be(7);
			selectionDSL
				.Commits().TillRevision("1")
				.Modifications().InCommits()
				.CodeBlocks().InModifications().CalculateDefectCodeSize()
					.Should().Be(6);
			selectionDSL
				.Commits().AfterRevision("1")
				.Modifications().InCommits()
				.CodeBlocks().InModifications().CalculateDefectCodeSize()
					.Should().Be(1);
		}
		[Test]
		public void Should_calc_size_of_defect_code_in_revision()
		{
			AddCode();

			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize("1")
					.Should().Be(0);
			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize("2")
					.Should().Be(5);
			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize("3")
					.Should().Be(5);
			selectionDSL
				.CodeBlocks().CalculateDefectCodeSize("4")
					.Should().Be(7);
		}
		private void AddCode()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2").IsBugFix()
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
						.Code(5)
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
			.Submit()
				.AddCommit("4").IsBugFix()
					.File("file1").Modified()
						.Code(-1).ForCodeAddedInitiallyInRevision("1")
						.Code(-1).ForCodeAddedInitiallyInRevision("2")
						.Code(1)
			.Submit();
		}
	}
}
