/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class CommitSelectionExtensions
	{
		public static CommitSelectionExpression Commits(this IRepositorySelectionExpression parentExp)
		{
			return new CommitSelectionExpression(parentExp);
		}
	}

	public class CommitSelectionExpression : EntitySelectionExpression<Commit,CommitSelectionExpression>
	{
		public CommitSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		public CommitSelectionExpression AuthorIs(string author)
		{
			return Reselect(s => s.Where(x => x.Author == author));
		}
		public CommitSelectionExpression AuthorIsNot(string author)
		{
			return Reselect(s => s.Where(x => x.Author != author));
		}
		public CommitSelectionExpression RevisionIs(string revision)
		{
			return Reselect(s => s.Where(x => x.Revision == revision));
		}
		public CommitSelectionExpression DateIsGreaterThan(DateTime date)
		{
			return Reselect(s => s.Where(x => x.Date > date));
		}
		public CommitSelectionExpression DateIsGreaterOrEquelThan(DateTime date)
		{
			return Reselect(s => s.Where(x => x.Date >= date));
		}
		public CommitSelectionExpression DateIsLesserThan(DateTime date)
		{
			return Reselect(s => s.Where(x => x.Date < date));
		}
		public CommitSelectionExpression DateIsLesserOrEquelThan(DateTime date)
		{
			return Reselect(s => s.Where(x => x.Date <= date));
		}
		public CommitSelectionExpression BeforeRevision(int revisionOrderedNumber)
		{
			return Reselect(s =>
				from c in s
				where
					c.OrderedNumber < revisionOrderedNumber
				select c
			);
		}
		public CommitSelectionExpression BeforeRevision(string revision)
		{
			if (revision == null)
			{
				return this;
			}
			return Reselect(s =>
				from c in s
				let revisionNumber = Queryable<Commit>()
					.Single(x => x.Revision == revision)
					.OrderedNumber
				where
					c.OrderedNumber < revisionNumber
				select c
			);
		}
		public CommitSelectionExpression TillRevision(int revisionOrderedNumber)
		{
			return Reselect(s =>
				from c in s
				where
					c.OrderedNumber <= revisionOrderedNumber
				select c
			);
		}
		public CommitSelectionExpression TillRevision(string revision)
		{
			if (revision == null)
			{
				return this;
			}
			return Reselect(s =>
				from c in s
				let revisionNumber = Queryable<Commit>()
					.Single(x => x.Revision == revision)
					.OrderedNumber
				where
					c.OrderedNumber <= revisionNumber
				select c
			);
		}
		public CommitSelectionExpression FromRevision(int revisionOrderedNumber)
		{
			return Reselect(s =>
				from c in s
				where
					c.OrderedNumber >= revisionOrderedNumber
				select c
			);
		}
		public CommitSelectionExpression FromRevision(string revision)
		{
			if (revision == null)
			{
				return this;
			}
			return Reselect(s =>
				from c in s
				let revisionNumber = Queryable<Commit>()
					.Single(x => x.Revision == revision)
					.OrderedNumber
				where
					c.OrderedNumber >= revisionNumber
				select c
			);
		}
		public CommitSelectionExpression AfterRevision(int revisionOrderedNumber)
		{
			return Reselect(s =>
				from c in s
				where
					c.OrderedNumber > revisionOrderedNumber
				select c
			);
		}
		public CommitSelectionExpression AfterRevision(string revision)
		{
			if (revision == null)
			{
				return this;
			}
			return Reselect(s =>
				from c in s
				let revisionNumber = Queryable<Commit>()
					.Single(x => x.Revision == revision)
					.OrderedNumber
				where
					c.OrderedNumber > revisionNumber
				select c
			);
		}
		protected override CommitSelectionExpression Recreate()
		{
			return new CommitSelectionExpression(this);
		}
	}
}
