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
	public class ProjectFileMappingExpressionTest : BaseRepositoryTest
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
		}
		[Test]
		public void Should_add_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1")
			.Submit();

			Queryable<ProjectFile>().Count()
				.Should().Be(1);
			Queryable<ProjectFile>().Single()
				.Satisfy(f =>
					f.Path == "file1" &&
					f.AddedInCommit == Queryable<Commit>().Single()
				);
		}
		[Test]
		public void Should_copy_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1")
			.Submit()
				.AddCommit("2")
					.File("file1").Delete()
			.Submit()
				.AddCommit("3")
					.AddFile("file1")
			.Submit()
				.AddCommit("10")
					.AddFile("file2").CopiedFrom("file1", "1")
					.AddFile("file3").CopiedFrom("file1", "3")
			.Submit();

			var files = Queryable<ProjectFile>().ToList();

			Queryable<ProjectFile>().Single(x => x.Path == "file2")
				.Satisfy(x =>
					x.SourceCommit.Revision == "1" &&
					x.SourceFile.Path == "file1" &&
					x.SourceFile.DeletedInCommitID != null
				);
			Queryable<ProjectFile>().Single(x => x.Path == "file3")
				.Satisfy(x =>
					x.SourceCommit.Revision == "3" &&
					x.SourceFile.Path == "file1" &&
					x.SourceFile.DeletedInCommitID == null
				);
		}
		[Test]
		public void Should_mark_file_as_deleted()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1")
			.Submit()
				.AddCommit("2")
					.File("file1").Delete()
			.Submit();

			Queryable<ProjectFile>().Single().DeletedInCommit
				.Should().Be(
					Queryable<Commit>().Single(c => c.Revision == "2")
				);
		}
	}
}
