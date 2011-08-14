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
	public class DefectsTest : BaseRepositoryTest
	{
		[Test]
		public void Should_return_zero_for_empty_code()
		{
			selectionDSL
				.CodeBlocks().CalculateNumberOfDefects()
					.Should().Be(0);
		}
		[Test]
		public void Should_calc_number_of_fixed_defects_for_code()
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
						.Code(10)
					.AddFile("file2").Modified()
						.Code(50)
			.Submit()
				.AddCommit("4").IsBugFix()
					.File("file1").Modified()
						.Code(-1).ForCodeAddedInitiallyInRevision("1")
						.Code(1)
					.File("file2").Modified()
						.Code(-1).ForCodeAddedInitiallyInRevision("3")
						.Code(1)
			.Submit();
			
			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects()
					.Should().Be(2);
			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects("3")
					.Should().Be(1);
			selectionDSL
				.Files().PathIs("file2")
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateNumberOfDefects()
					.Should().Be(1);
		}
	}
}
