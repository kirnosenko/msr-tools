/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	[TestFixture]
	public class RemainingCodeTest : BaseRepositoryTest
	{
		[Test]
		public void Should_give_remaining_code_by_codeblock_which_added_the_code()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
						.Code(5)
					.AddFile("file2").Modified()
						.Code(50)
			.Submit()
				.AddCommit("3")
					.File("file2").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("2")
						.Code(10)
			.Submit();

			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.CodeBlocks().InModifications().CalculateRemainingCodeSize("2").Count
					.Should().Be(2);
			
			var code = selectionDSL.CodeBlocks().CalculateRemainingCodeSize("3");
			selectionDSL.CodeBlocks().Where(cb => code.Keys.Contains(cb.ID))
				.Select(cb => cb.Size).ToArray()
					.Should().Have.SameValuesAs(new double[]
					{
						100, 5, 50, 10
					});
			code.Values.Should().Have.SameValuesAs(new double[]
			{
				90, 5, 40, 10
			});
		}
		[Test]
		public void Should_ignore_fully_deleted_code()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-50).ForCodeAddedInitiallyInRevision("1")
						.Code(5)
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(-50).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
			.Submit();

			selectionDSL.CodeBlocks().CalculateRemainingCodeSize("3").Count
				.Should().Be(2);
		}
	}
}
