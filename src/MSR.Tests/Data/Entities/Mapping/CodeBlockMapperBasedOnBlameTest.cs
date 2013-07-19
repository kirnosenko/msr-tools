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
	public class CodeBlockMapperBasedOnBlameTest : BaseMapperTest
	{
		private CodeBlockMapperBasedOnBlame mapper;

		private IBlame blameStub;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			
			mapper = new CodeBlockMapperBasedOnBlame(scmData);
		}
		[Test]
		public void Should_map_added_lines()
		{
			blameStub = new BlameStub();
			blameStub[1] = "abc";
			blameStub[2] = "abc";
			blameStub[3] = "abc";
			
			scmData.Stub(x => x.Blame("abc", "file1"))
				.Return(blameStub);

			mapper.Map(
				mappingDSL.AddCommit("abc")
					.AddFile("file1").Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>().Count()
				.Should().Be(1);
			Queryable<CodeBlock>().Single()
				.Satisfy(x =>
					x.Size == 3
					&&
					x.AddedInitiallyInCommit.Revision == "abc"
				);
		}
		[Test]
		public void Should_not_take_blame_for_deleted_file()
		{
			mappingDSL
				.AddCommit("ab")
					.AddFile("file1").Modified()
						.Code(100)
				.Submit();
			mapper.Map(
				mappingDSL.AddCommit("abc")
					.File("file1").Delete().Modified()
			);
			SubmitChanges();

			Queryable<CodeBlock>()
				.Select(cb => cb.Size).ToArray()
					.Should().Have.SameSequenceAs(new double[]
					{
						100, -100
					});
			scmData.AssertWasNotCalled(x => x.Blame("abc", "file1"));
		}
		[Test]
		public void Should_remove_code_that_no_more_exists()
		{
			mappingDSL
				.AddCommit("a")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit()
				.AddCommit("ab")
					.File("file1").Modified()
						.Code(20)
			.Submit();

			blameStub = new BlameStub();
			for (int i = 1; i <= 10; i++)
			{
				blameStub[i] = "abc";
			}
			for (int i = 11; i <= 25; i++)
			{
				blameStub[i] = "ab";
			}
			scmData.Stub(x => x.Blame("abc", "file1"))
				.Return(blameStub);
			
			mapper.Map(
				mappingDSL.AddCommit("abc")
					.File("file1").Modified()
			);
			SubmitChanges();

			var code = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "abc");
			
			code.Select(cb => cb.Size).ToArray()
				.Should().Have.SameValuesAs(new double[]
				{
					10, -10, -5
				});
			code.Where(cb => cb.Size < 0)
				.Select(cb => cb.TargetCodeBlock.Size).ToArray()
				.Should().Have.SameValuesAs(new double[]
				{
					10, 20
				});
		}
		[Test]
		public void Should_map_all_code_as_is_for_copied_file()
		{
			mappingDSL
				.AddCommit("a")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit()
				.AddCommit("ab")
					.File("file1").Modified()
						.Code(5)
			.Submit();

			blameStub = new BlameStub();
			for (int i = 1; i <= 10; i++)
			{
				blameStub[i] = "a";
			}
			for (int i = 11; i <= 15; i++)
			{
				blameStub[i] = "ab";
			}
			scmData.Stub(x => x.Blame("abc", "file2"))
				.Return(blameStub);

			mapper.Map(
				mappingDSL.AddCommit("abc")
					.AddFile("file2").CopiedFrom("file1", "ab").Modified()
			);
			SubmitChanges();

			var code = Queryable<CodeBlock>()
				.Where(cb => cb.Modification.Commit.Revision == "abc");
				
			code.Select(cb => cb.Size).ToArray()
				.Should().Have.SameValuesAs(new double[]
				{
					10, 5
				});
			code.Select(cb => cb.AddedInitiallyInCommit.Revision).ToArray()
				.Should().Have.SameValuesAs(new string[]
				{
					"a", "ab"
				});
		}
		[Test]
		public void Should_map_new_code_in_copied_file_as_new()
		{
			mappingDSL
				.AddCommit("a")
					.AddFile("file1").Modified()
						.Code(10)
			.Submit();

			blameStub = new BlameStub();
			for (int i = 1; i <= 10; i++)
			{
				blameStub[i] = "a";
			}
			for (int i = 11; i <= 15; i++)
			{
				blameStub[i] = "abc";
			}
			scmData.Stub(x => x.Blame("abc", "file2"))
				.Return(blameStub);
			
			mapper.Map(
				mappingDSL.AddCommit("abc")
					.AddFile("file2").CopiedFrom("file1", "a").Modified()
			);
			SubmitChanges();

			var code = Queryable<CodeBlock>()
				.Single(cb => cb.Size == 5)
				.AddedInitiallyInCommit.Revision
					.Should().Be("abc");
		}
	}
}
