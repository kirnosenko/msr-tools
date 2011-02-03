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
	public class DefectCodeDensityTest : BaseRepositoryTest
	{
		[Test]
		public void Should_be_zero_for_empty_code()
		{
			selectionDSL
				.CodeBlocks().CalculateDefectCodeDensity()
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
				.CodeBlocks().CalculateDefectCodeDensity()
					.Should().Be(0);
		}
		[Test]
		public void Defect_code_density_is_ratio_of_defect_code_size_to_added_code_size()
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
				.CalculateDefectCodeDensity()
					.Should().Be(2d / 3000);
			selectionDSL
				.Commits().RevisionIs("2")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.CalculateDefectCodeDensity()
					.Should().Be(15d / 5000);
			selectionDSL
				.CodeBlocks()
				.CalculateDefectCodeDensity()
					.Should().Be(17d / 8020);
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

			code.CalculateDefectCodeDensityAtRevision("1")
				.Should().Be(0);
			code.CalculateDefectCodeDensityAtRevision("2")
				.Should().Be(5d / 105);
			code.CalculateDefectCodeDensityAtRevision("3")
				.Should().Be(11d / 110);
		}
	}
}
