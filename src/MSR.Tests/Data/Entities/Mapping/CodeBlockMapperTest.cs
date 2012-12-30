/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class CodeBlockMapperTest : BaseMapperTest
	{
		private CodeBlockMapper mapper;
		
		private List<int> addedLines;
		private List<int> removedLines;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			addedLines = new List<int>();
			removedLines = new List<int>();
			diffStub.Stub(x => x.AddedLines)
				.Return(addedLines);
			diffStub.Stub(x => x.RemovedLines)
				.Return(removedLines);
			scmData.Stub(x => x.PreviousRevision(null))
				.IgnoreArguments()
				.Do((Func<string,string>)(r =>
				{
					return (Convert.ToInt32(r) - 1).ToString();
				}));
			
			mapper = new CodeBlockMapper(scmData);
		}
		[Test]
		public void Should_map_nothing_if_there_are_no_added_or_removed_lines()
		{
			scmData.Stub(x => x.Diff("10", "file1"))
				.Return(diffStub);
			
			mapper.Map(
				mappingDSL.AddCommit("10")
					.AddFile("file1").Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>().Count()
				.Should().Be.EqualTo(0);
		}
		[Test]
		public void Should_add_separate_blocks_for_added_and_removed_code()
		{
			mappingDSL
				.AddCommit("9")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();
			
			scmData.Stub(x => x.Diff("10", "file1"))
				.Return(diffStub);
			addedLines.AddRange(new int[] { 20, 21, 22 });
			removedLines.Add(5);
			scmData.Stub(x => x.Blame("9", "file1"))
				.Return(new BlameStub(x => "9"));

			mapper.Map(
				mappingDSL.AddCommit("10")
					.File("file1").Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>().Count()
				.Should().Be.EqualTo(3);
			Queryable<CodeBlock>()
				.Select(x => x.Size).ToArray()
					.Should().Have.SameSequenceAs(
						new double[] { 100, 3, -1 }
					);
		}
		[Test]
		public void Should_set_commit_code_block_was_added_in()
		{
			mappingDSL
				.AddCommit("9")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();

			scmData.Stub(x => x.Diff("10", "file1"))
				.Return(diffStub);
			removedLines.Add(10);
			addedLines.Add(10);
			scmData.Stub(x => x.Blame("9", "file1"))
				.Return(new BlameStub(x => "9"));

			mapper.Map(
				mappingDSL.AddCommit("10")
					.File("file1").Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>().Single(x => x.Size == 1).AddedInitiallyInCommit.Revision
				.Should().Be("10");
			Queryable<CodeBlock>().Single(x => x.Size == -1).AddedInitiallyInCommit
				.Should().Be.Null();
		}
		[Test]
		public void Should_set_target_code_block_for_deleted_code_block()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(50)
			.Submit()
				.AddCommit("9")
			.Submit();

			scmData.Stub(x => x.Diff("10", "file1"))
				.Return(diffStub);
			removedLines.Add(5);
			scmData.Stub(x => x.Blame("9", "file1"))
				.Return(new BlameStub()
				{
					{ 5, "1" }
				});

			mapper.Map(
				mappingDSL.AddCommit("10")
					.File("file1").Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>().Single(x => x.Modification.Commit.Revision == "10").TargetCodeBlock.Size
				.Should().Be(100);
		}
		[Test]
		public void Should_add_codeblocks_by_source_commit_for_copied_file()
		{
			mappingDSL
				.AddCommit("6")
					.AddFile("file1").Modified()
						.Code(20)
			.Submit()
				.AddCommit("7")
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("6")
						.Code(3)
			.Submit()
				.AddCommit("8")
					.File("file1").Modified()
						.Code(5)
			.Submit();
			
			scmData.Stub(x => x.Diff("10", "file2"))
				.Return(diffStub);
			addedLines.AddRange(Enumerable.Range(1,18));
			scmData.Stub(x => x.Blame("7", "file1"))
				.Return(new BlameStub(x => "6")
				{
					{ 10, "7" },
					{ 11, "7" },
					{ 12, "7" },
				});
			scmData.Stub(x => x.Diff("file2", "10", "file1", "7"))
				.Return(FileUniDiff.Empty);
			
			mapper.Map(
				mappingDSL.AddCommit("10")
					.AddFile("file2").CopiedFrom("file1", "7").Modified()
			);
			SubmitChanges();

			var code = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "10");
			
			code.Count()
				.Should().Be(2);
			code.Select(cb => cb.Size).ToArray()
				.Should().Have.SameSequenceAs(new double[] { 15, 3 });
			code.Select(cb => cb.AddedInitiallyInCommit.Revision).ToArray()
				.Should().Have.SameSequenceAs(new string[] { "6", "7" });
		}
		[Test]
		public void Should_add_codeblocks_for_file_during_partial_mapping()
		{
			mappingDSL
				.AddCommit("6")
					.AddFile("file1").Modified()
						.Code(20)
					.AddFile("file2").Modified()
						.Code(25)
			.Submit()
				.AddCommit("7")
					.File("file1").Modified()
						.Code(30)
			.Submit()
				.AddCommit("8")
					.File("file1").Modified()
						.Code(40)
			.Submit();

			scmData.Stub(x => x.Diff("7", "file2"))
				.Return(diffStub);
			addedLines.AddRange(new int[] { 10, 11 });
			removedLines.AddRange(new int[] { 10, 11 });
			scmData.Stub(x => x.Blame("6", "file2"))
				.Return(new BlameStub(x => "6"));
			
			mapper.Map(
				mappingDSL.Commit("7")
					.File("file2").Modified()
			);
			SubmitChanges();

			var code = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "7");

			code.Count()
				.Should().Be(3);
			code.Select(cb => cb.Size).ToArray()
				.Should().Have.SameSequenceAs(new double[] { 30, 2, -2 });
		}
		
		[Test]
		public void Should_not_take_diff_for_copied_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();
			
			scmData.Stub(x => x.Diff("file2", "10", "file1", "1"))
				.Return(FileUniDiff.Empty);

			mapper.Map(
				mappingDSL.AddCommit("10")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
			);
			SubmitChanges();
			
			scmData.AssertWasNotCalled(x => x.Diff("10", "file2"));
		}
		[Test]
		public void Should_use_diff_and_blame_for_copied_and_modified_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(20)
			.Submit();
			
			scmData.Stub(x => x.Diff("file2", "10", "file1", "2"))
				.Return(diffStub);
			removedLines.Add(120);
			addedLines.Add(120);
			
			IBlame blame = new BlameStub();
			for (int i = 1; i <= 100; i++)
			{
				blame.Add(i, "1");
			}
			for (int i = 101; i <= 119; i++)
			{
				blame.Add(i, "2");
			}
			blame.Add(120, "10");
			scmData.Stub(x => x.Blame("10", "file2"))
				.Return(blame);
			
			mapper.Map(
				mappingDSL.AddCommit("10")
					.AddFile("file2").CopiedFrom("file1", "2").Modified()
			);
			SubmitChanges();

			var codeBlocks = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "10");
			
			codeBlocks.Count()
				.Should().Be(3);
			codeBlocks.Select(cb => cb.Size)
				.Should().Have.SameSequenceAs(
					new double[] { 100, 19, 1 }
				);
			codeBlocks.Select(cb => cb.AddedInitiallyInCommit.Revision)
				.Should().Have.SameSequenceAs(
					new string[] { "1", "2", "10" }
				);
		}
		[Test]
		public void Should_not_take_blame_for_copied_and_modified_file_without_deleted_code()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();

			scmData.Stub(x => x.Diff("file2", "10", "file1", "1"))
				.Return(diffStub);
			addedLines.Add(10);

			mapper.Map(
				mappingDSL.AddCommit("10")
					.AddFile("file2").CopiedFrom("file1", "1").Modified()
			);
			SubmitChanges();

			scmData.AssertWasNotCalled(x => x.Blame("10", "file2"));
		}
		[Test]
		public void Should_not_take_diff_for_deleted_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();

			mapper.Map(
				mappingDSL.AddCommit("10")
					.File("file1").Delete().Modified()
			);
			SubmitChanges();

			scmData.AssertWasNotCalled(x => x.Diff("10", "file1"));
		}
		[Test]
		public void Should_not_take_blame_for_file_without_removed_code()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit();
			
			scmData.Stub(x => x.Diff("10", "file1"))
				.Return(diffStub);
			addedLines.AddRange(Enumerable.Range(10,20));
			
			mapper.Map(
				mappingDSL.AddCommit("10")
					.File("file1").Modified()
			);
			SubmitChanges();

			scmData.AssertWasNotCalled(x => x.Blame("9","file1"));
		}
	}
}
