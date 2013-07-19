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
	public class CommitSelectionExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_select_commits_by_author()
		{
			mappingDSL
				.AddCommit("1").By("alan")
			.Submit()
				.AddCommit("2").By("bob")
			.Submit()
				.AddCommit("3").By("alan")
			.Submit();

			selectionDSL
				.Commits().AuthorIs("alan")
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "1", "3" });
			selectionDSL
				.Commits().AuthorIs("bob")
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "2", });
			selectionDSL
				.Commits().AuthorIsNot("alan")
				.Select(c => c.Revision).ToArray()
					.Should().Have.SameSequenceAs(new string[] { "2", });
		}
		[Test]
		public void Should_select_commits_by_date()
		{
			mappingDSL
				.AddCommit("1").At(DateTime.Today.AddDays(-9))
			.Submit()
				.AddCommit("2").At(DateTime.Today.AddDays(-8))
			.Submit()
				.AddCommit("5").At(DateTime.Today.AddDays(-5))
			.Submit();

			selectionDSL
				.Commits().DateIsGreaterThan(DateTime.Today.AddDays(-8)).Count()
					.Should().Be(1);
			selectionDSL
				.Commits().DateIsGreaterOrEquelThan(DateTime.Today.AddDays(-8)).Count()
					.Should().Be(2);
			selectionDSL
				.Commits().DateIsLesserThan(DateTime.Today.AddDays(-8)).Count()
					.Should().Be(1);
			selectionDSL
				.Commits().DateIsLesserOrEquelThan(DateTime.Today.AddDays(-8)).Count()
					.Should().Be(2);
		}
		[Test]
		public void Should_select_commits_relatively_specified()
		{
			mappingDSL
				.AddCommit("abc")
			.Submit()
				.AddCommit("abcd")
			.Submit()
				.AddCommit("abcde")
			.Submit();

			selectionDSL
				.Commits().BeforeRevision(1).Count()
					.Should().Be(0);
			selectionDSL
				.Commits().BeforeRevision("abc").Count()
					.Should().Be(0);
			selectionDSL
				.Commits().BeforeRevision(2).Count()
					.Should().Be(1);
			selectionDSL
				.Commits().BeforeRevision("abcd").Count()
					.Should().Be(1);
			selectionDSL
				.Commits().TillRevision(1).Count()
					.Should().Be(1);
			selectionDSL
				.Commits().TillRevision("abc").Count()
					.Should().Be(1);
			selectionDSL
				.Commits().TillRevision(3).Count()
					.Should().Be(3);
			selectionDSL
				.Commits().TillRevision("abcde").Count()
					.Should().Be(3);
			selectionDSL
				.Commits().FromRevision(2).Count()
					.Should().Be(2);
			selectionDSL
				.Commits().FromRevision("abcd").Count()
					.Should().Be(2);
			selectionDSL
				.Commits().FromRevision(3).Count()
					.Should().Be(1);
			selectionDSL
				.Commits().FromRevision("abcde").Count()
					.Should().Be(1);
			selectionDSL
				.Commits().AfterRevision(1).Count()
					.Should().Be(2);
			selectionDSL
				.Commits().AfterRevision("abc").Count()
					.Should().Be(2);
			selectionDSL
				.Commits().AfterRevision(3).Count()
					.Should().Be(0);
			selectionDSL
				.Commits().AfterRevision("abcde").Count()
					.Should().Be(0);
		}
		[Test]
		public void Should_ignore_null_values_for_commit_relative_selection()
		{
			mappingDSL
				.AddCommit("abc")
			.Submit()
				.AddCommit("abcd")
			.Submit()
				.AddCommit("abcde")
			.Submit();
			
			selectionDSL
				.Commits()
					.AfterRevision(null)
					.FromRevision(null)
					.TillRevision(null)
					.BeforeRevision(null)
					.Count()
						.Should().Be(3);
		}
		[Test]
		public void Should_take_specified_selection()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit()
				.AddCommit("4")
			.Submit();
			
			selectionDSL
				.Commits()
				.Are(
					from c in Queryable<Commit>()
					where c.Revision != "3"
					select c
				)
				.AfterRevision("2")
				.Count()
					.Should().Be(1);
		}
		[Test]
		public void Should_keep_selection_for_fixed_expression()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-5)
						.Code(20)
			.Submit();
			
			var r2code = selectionDSL
				.Commits().RevisionIs("2")
				.Modifications().InCommits()
				.CodeBlocks().InModifications()
				.Fixed();
			
			r2code.Count()
				.Should().Be(2);
			r2code.Added().Count()
				.Should().Be(1);
			r2code.Deleted().Count()
				.Should().Be(1);
			r2code.Reselect(e => e.Added()).Count()
				.Should().Be(1);
			r2code.Are(mappingDSL.Queryable<CodeBlock>()).Count()
				.Should().Be(3);
			r2code.Added().Again().Count()
				.Should().Be(2);
			r2code.Count()
				.Should().Be(2);
		}
		[Test]
		public void Can_restore_selection_from_previous_expression_of_the_same_type()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit()
				.AddCommit("4")
			.Submit();
			
			selectionDSL.Commits()
				.AfterRevision("2")
				.Modifications().InCommits()
				.Commits().Count()
					.Should().Be(4);
			
			selectionDSL.Commits()
				.AfterRevision("2")
				.Modifications().InCommits()
				.Commits().Again().Count()
					.Should().Be(2);
		}
		[Test]
		public void Can_do_something_with_dsl_subproduct()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit()
				.AddCommit("4")
			.Submit();
			
			selectionDSL.Commits()
				.AfterRevision("1")
				.Do(x => x.Count().Should().Be(3))
				.BeforeRevision("3")
				.Do(x => x.Count().Should().Be(1));
		}
		[Test]
		public void Can_reselect_using_func()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit()
				.AddCommit("4")
			.Submit();
			
			Func<CommitSelectionExpression, CommitSelectionExpression> selector = 
				e => e.AfterRevision("1").BeforeRevision("4");
			
			selectionDSL.Commits()
				.Reselect(selector)
				.Count()
					.Should().Be(2);
		}
		[Test]
		public void Can_continue_expression_after_null_reselector()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit()
				.AddCommit("4")
			.Submit();

			selectionDSL.Commits()
				.Reselect((Func<CommitSelectionExpression, CommitSelectionExpression>)null)
				.BeforeRevision("3")
				.Count()
					.Should().Be(2);
		}
	}
}
