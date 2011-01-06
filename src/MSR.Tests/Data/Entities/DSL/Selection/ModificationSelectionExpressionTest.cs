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

namespace MSR.Data.Entities.DSL.Selection
{
	[TestFixture]
	public class ModificationSelectionExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_select_modifications_for_revision()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();
			
			selectionDSL
				.Commits().RevisionIs("1")
				.Modifications().InCommits()
				.Count()
					.Should().Be(1);

			selectionDSL
				.Commits().RevisionIs("2")
				.Modifications().InCommits()
				.Count()
					.Should().Be(2);
		}
		[Test]
		public void Should_select_modifications_for_files()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
					.AddFile("file2").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file3").Modified()
			.Submit()
				.AddCommit("3")
					.File("file1").Modified()
					.File("file2").Modified()
			.Submit();

			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.Count()
					.Should().Be(3);
			selectionDSL
				.Files().PathIs("file2")
				.Modifications().InFiles()
				.Count()
					.Should().Be(2);
			selectionDSL
				.Files().PathIs("file3")
				.Modifications().InFiles()
				.Count()
					.Should().Be(1);
		}
		[Test]
		public void Should_select_unique_commits_that_contain_modifications()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();
			
			selectionDSL
				.Files().PathIs("file1")
				.Modifications().InFiles()
				.Commits().ContainModifications()
				.Select(x => x.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "1", "2" });

			selectionDSL
				.Files().PathIs("file2")
				.Modifications().InFiles()
				.Commits().ContainModifications()
				.Select(x => x.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "2" });

			selectionDSL
				.Commits().ContainModifications()
				.Select(x => x.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "1", "2" });
		}
		[Test]
		public void Should_select_unique_files_that_contain_modifications()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();
			
			selectionDSL
				.Files().ContainModifications()
				.Select(x => x.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1", "file2" });
		}
		[Test]
		public void Should_get_unique_files_touched_in_specified_commits()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();

			selectionDSL
				.Commits().Reselect(s => s.Where(c => c.Revision == "1"))
				.Files().TouchedInCommits()
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1" });

			selectionDSL
				.Commits().Reselect(s => s.Where(c => c.Revision == "2"))
				.Files().TouchedInCommits()
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1", "file2" });

			selectionDSL
				.Files().TouchedInCommits()
				.Select(f => f.Path).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "file1", "file2" });
		}
		[Test]
		public void Should_get_unique_commits_touch_specified_files()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();

			selectionDSL
				.Files().PathIs("file1")
				.Commits().TouchFiles()
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameValuesAs(new string[] { "1", "2" });

			selectionDSL
				.Files().PathIs("file2")
				.Commits().TouchFiles()
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameValuesAs(new string[] { "2" });

			selectionDSL
				.Files()
				.Commits().TouchFiles()
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameValuesAs(new string[] { "1", "2" });
		}
	}
}
