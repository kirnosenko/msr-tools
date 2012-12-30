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
	public class ModificationMappingExpressionTest : BaseRepositoryTest
	{
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
		}
		[Test]
		public void Should_add_modification()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit();

			Queryable<Modification>().Count()
				.Should().Be(1);
			Queryable<Modification>().Single()
				.Satisfy(m =>
					m.Commit == Queryable<Commit>().Single(c => c.Revision == "1") &&
					m.File == Queryable<ProjectFile>().Single(f => f.Path == "file1")
				);
		}
		[Test]
		public void Should_link_modification_with_specified_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
					.AddFile("file2").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
			.Submit();

			Queryable<Modification>().Select(m => m.File.Path).ToArray()
				.Should().Have.SameSequenceAs(
					new string[] { "file1", "file2", "file1" }
				);
		}
	}
}
