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

namespace MSR.Data.Entities.DSL.Mapping
{
	[TestFixture]
	public class CodeBlockMappingExpressionTest : BaseRepositoryTest
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
		}
		[Test]
		public void Should_add_code_block()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit();

			Queryable<CodeBlock>().Count()
				.Should().Be(1);
			Queryable<CodeBlock>().Single()
				.Satisfy(cb =>
					cb.Size == 10 &&
					cb.Modification != null &&
					cb.AddedInitiallyInCommit.Revision == "1"
				);
		}
		[Test]
		public void Can_set_target_codeblock_for_new_one()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
					.AddFile("file2").Modified()
						.Code(40)
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(50)
			.Submit()
				.AddCommit("4")
					.File("file1").Modified()
						.Code(10)
						.Code(-3).ForCodeAddedInitiallyInRevision("2")
						.Code(-2).ForCodeAddedInitiallyInRevision("3")
			.Submit();

			var targetModification =
				from cb in Queryable<CodeBlock>().Where(cb => cb.Size < 0)
				join tcb in Queryable<CodeBlock>() on cb.TargetCodeBlockID equals tcb.ID
				join m in Queryable<Modification>() on tcb.ModificationID equals m.ID
				select m;

			(
				from m in targetModification
				join f in Queryable<ProjectFile>() on m.FileID equals f.ID
				select f.Path
			).Should().Have.SameSequenceAs(Enumerable.Repeat("file1", 3));
			(
				from m in targetModification
				join c in Queryable<Commit>() on m.CommitID equals c.ID
				select c.Revision
			).Should().Have.SameSequenceAs(new string[] { "1", "2", "3" });
		}
		[Test]
		public void Should_set_correct_target_code_block_for_copied_files()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
						.CopyCode()
			.Submit()
				.AddCommit("3")
					.File("file2").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
			.Submit();

			Queryable<CodeBlock>()
				.Single(cb => cb.Size == -10)
				.TargetCodeBlock.Modification.Commit.Revision
					.Should().Be("2");
		}
		[Test]
		public void Can_set_commit_code_was_added_in()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("3")
					.AddFile("file2").CopiedFrom("file1", "2").Modified()
						.Code(95).CopiedFrom("1")
						.Code(20).CopiedFrom("2")
			.Submit()
				.AddCommit("4")
					.AddFile("file3").CopiedFrom("file1", "2").Modified()
						.CopyCode()
			.Submit();

			Queryable<CodeBlock>()
				.Single(cb => cb.Modification.Commit.Revision == "1")
				.AddedInitiallyInCommit.Revision
					.Should().Be("1");
			Queryable<CodeBlock>()
				.Single(cb => cb.Modification.Commit.Revision == "2" && cb.Size > 0)
				.AddedInitiallyInCommit.Revision
					.Should().Be("2");
			Queryable<CodeBlock>()
				.Single(cb => cb.Modification.Commit.Revision == "2" && cb.Size < 0)
				.AddedInitiallyInCommit
					.Should().Be.Null();
			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "3")
				.Select(cb => cb.AddedInitiallyInCommit.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[]
					{
						"1", "2"
					});
			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "4")
				.Select(cb => cb.AddedInitiallyInCommit.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[]
					{
						"1", "2"
					});
		}
		[Test]
		public void Should_copy_code_for_copied_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(10)
						.Code(-2).ForCodeAddedInitiallyInRevision("2")
			.Submit()
				.AddCommit("4")
					.AddFile("file2").CopiedFrom("file1", "2").Modified()
						.CopyCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "4")
				.Select(cb => cb.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[]
					{
						95, 20
					});
		}
		[Test]
		public void Should_not_add_empty_code_blocks_for_copied_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(5)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("4")
					.AddFile("file2").CopiedFrom("file1", "3").Modified()
						.CopyCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "4")
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { 20, 5 });
		}
		[Test]
		public void Should_find_target_codeblock_for_copied_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
						.CopyCode()
			.Submit()
				.AddCommit("3")
					.File("file2").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit();

			Queryable<CodeBlock>().Single(cb => cb.Size == -5)
				.TargetCodeBlock.AddedInitiallyInCommit.Revision
					.Should().Be("1");
		}
		[Test]
		public void Should_copy_code_from_specified_file_only()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
					.AddFile("file2").Modified()
						.Code(50)
			.Submit()
				.AddCommit("2")
					.AddFile("file3").CopiedFrom("file2", "1").Modified()
						.CopyCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "2")
				.Select(cb => cb.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[]
					{
						50
					});
		}
		[Test]
		public void Should_copy_copied_code_with_correct_initial_commit()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
						.Code(20)
			.Submit()
				.AddCommit("3")
					.AddFile("file2").CopiedFrom("file1", "2").Modified()
						.CopyCode()
			.Submit()
				.AddCommit("4")
					.AddFile("file3").CopiedFrom("file2", "3").Modified()
						.CopyCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "4")
				.Select(cb => cb.AddedInitiallyInCommit.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[]
					{
						"1", "2"
					});
		}
		[Test]
		public void Should_delete_code_for_deleted_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
			.Submit()
				.AddCommit("3")
					.File("file1").Delete().Modified()
						.DeleteCode()
			.Submit();

			var codeBlocks = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "3");
			
			codeBlocks.Count()
				.Should().Be(2);
			codeBlocks.Select(cb => cb.Size).ToArray()
				.Should().Have.SameSequenceAs(new double[]
				{
					-95, -10
				});
			codeBlocks.Select(cb => cb.TargetCodeBlock).ToArray()
				.Should().Not.Contain(null);
			codeBlocks.Select(cb => cb.TargetCodeBlock.Size).ToArray()
				.Should().Have.SameSequenceAs(new double[]
				{
					100, 10
				});
		}
		[Test]
		public void Should_not_add_empty_code_blocks_for_deleted_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
						.Code(5)
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
			.Submit()
				.AddCommit("4")
					.File("file1").Delete().Modified()
						.DeleteCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "4")
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[] { -20, -5 });
		}
		[Test]
		public void Can_delete_code_for_copied_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
						.CopyCode()
			.Submit()
				.AddCommit("3")
					.File("file2").Delete().Modified()
						.DeleteCode()
			.Submit();

			Queryable<CodeBlock>()
				.Where(cb => cb.Modification.File.Path == "file2")
				.Sum(x => x.Size)
					.Should().Be(0);
		}
	}
}
