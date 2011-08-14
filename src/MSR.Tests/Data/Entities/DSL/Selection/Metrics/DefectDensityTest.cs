/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
	public class DefectDensityTest : BaseRepositoryTest
	{
		[Test]
		public void Should_be_zero_for_empty_code()
		{
			selectionDSL
				.CodeBlocks().CalculateTraditionalDefectDensity()
					.Should().Be(0);
			selectionDSL
				.CodeBlocks().CalculateDefectDensity()
					.Should().Be(0);
		}
		[Test]
		public void Should_be_zero_for_code_without_bugs()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
						.Code(15)
			.Submit();

			selectionDSL
				.CodeBlocks().CalculateTraditionalDefectDensity()
					.Should().Be(0);
			selectionDSL
				.CodeBlocks().CalculateDefectDensity()
					.Should().Be(0);
		}
		[Test]
		public void Traditional_defect_density_is_ratio_of_number_of_defects_to_current_code_size()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(3000)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-100).ForCodeAddedInitiallyInRevision("1")
						.Code(5000)
			.Submit()
				.AddCommit("3").IsBugFix()
					.File("file1").Modified()
						.Code(-2).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
			.Submit()
				.AddCommit("4").IsBugFix()
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("2")
			.Submit()
				.AddCommit("5").IsBugFix()
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("2")
						.Code(10)
			.Submit();
			
			selectionDSL
				.Commits().RevisionIs("1")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.CalculateTraditionalDefectDensity()
					.Should().Be(1d / ((3000 - 100 - 2) / DefectDensity.KLOC));
			selectionDSL
				.Commits().RevisionIs("2")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.CalculateTraditionalDefectDensity()
					.Should().Be(2d / ((5000 - 5 - 10) / DefectDensity.KLOC));
			selectionDSL
				.CodeBlocks()
				.CalculateTraditionalDefectDensity()
					.Should().Be(3d / (7903 / DefectDensity.KLOC));
		}
		[Test]
		public void Defect_density_is_ratio_of_number_of_defects_to_added_code_size()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(3000)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-100).ForCodeAddedInitiallyInRevision("1")
						.Code(5000)
			.Submit()
				.AddCommit("3").IsBugFix()
					.File("file1").Modified()
						.Code(-2).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
			.Submit()
				.AddCommit("4").IsBugFix()
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("2")
			.Submit()
				.AddCommit("5").IsBugFix()
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("2")
						.Code(10)
			.Submit();

			selectionDSL
				.Commits().RevisionIs("1")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.CalculateDefectDensity()
					.Should().Be(1d / (3000 / DefectDensity.KLOC));
			selectionDSL
				.Commits().RevisionIs("2")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.CalculateDefectDensity()
					.Should().Be(2d / (5000 / DefectDensity.KLOC));
			selectionDSL
				.CodeBlocks()
				.CalculateDefectDensity()
					.Should().Be(3d / (8020 / DefectDensity.KLOC));
		}
		[Test]
		public void Should_ignore_fixes_after_specified_revision()
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
				.AddCommit("3").IsBugFix()
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
						.Code(-1).ForCodeAddedInitiallyInRevision("2")
						.Code(5)
			.Submit();

			var code = selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.CodeBlocks().InModifications();

			code.CalculateTraditionalDefectDensity("1")
				.Should().Be(0);
			code.CalculateTraditionalDefectDensity("2")
				.Should().Be(1d / (100 / DefectDensity.KLOC));
			code.CalculateTraditionalDefectDensity("3")
				.Should().Be(2d / (99 / DefectDensity.KLOC));

			code.CalculateDefectDensity("1")
				.Should().Be(0);
			code.CalculateDefectDensity("2")
				.Should().Be(1d / (105 / DefectDensity.KLOC));
			code.CalculateDefectDensity("3")
				.Should().Be(2d / (110 / DefectDensity.KLOC));
		}
	}
}
