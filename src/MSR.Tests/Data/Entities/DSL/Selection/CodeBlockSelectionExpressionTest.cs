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
	public class CodeBlockSelectionExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_select_codeblocks_for_modifications()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(+100)
					.AddFile("file2").Modified()
						.Code(+50)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(+10)
						.Code(-5)
			.Submit();
			
			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.CodeBlocks().InModifications()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { 100, 10, -5 });
			
			selectionDSL
				.Files().PathIs("file2")
				.Modifications().InFiles()
				.CodeBlocks().InModifications()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { 50 });
		}
		[Test]
		public void Should_select_codeblocks_modified_by_specified_codeblocks()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(+100)
					.AddFile("file2").Modified()
						.Code(+50)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(+10)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(+20)
						.Code(-2).ForCodeAddedInitiallyInRevision("1")
						.Code(-3).ForCodeAddedInitiallyInRevision("2")
					.File("file2").Modified()
						.Code(-4).ForCodeAddedInitiallyInRevision("1")
			.Submit();
			
			selectionDSL
				.Commits().RevisionIs("3")
				.Files().PathIs("file1")
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().Modify()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { 100, 10 });
			
			selectionDSL
				.Commits().RevisionIs("1")
				.Modifications().InCommits()
				.CodeBlocks().InModifications().ModifiedBy()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { -5, -2, -4 });
		}
		[Test]
		public void Should_select_code_in_bugfixes()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2").IsBugFix()
					.File("file1").Modified()
						.Code(-5)
						.Code(5)
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(-10)
						.Code(20)
			.Submit();
			
			selectionDSL.CodeBlocks()
				.InBugFixes()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { -5, 5 });
		}
		[Test]
		public void Should_select_code_added_initially_in_commit()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
						.CopyCode()
			.Submit();
			
			selectionDSL
				.Commits().RevisionIs("1")
				.CodeBlocks().AddedInitiallyInCommits()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { 100, 100 });
		}
		[Test]
		public void Should_select_unique_modifications_that_contain_codeblocks()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-10)
						.Code(15)
			.Submit();
			
			selectionDSL
				.CodeBlocks().Added()
				.Modifications().ContainCodeBlocks().Count()
					.Should().Be(2);

			selectionDSL
				.CodeBlocks().Deleted()
				.Modifications().ContainCodeBlocks().Count()
					.Should().Be(1);
			
			selectionDSL
				.Modifications().ContainCodeBlocks().Count()
					.Should().Be(2);
		}
	}
}
