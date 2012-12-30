/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class BugFixSelectionExtensions
	{
		public static BugFixSelectionExpression BugFixes(this IRepositorySelectionExpression parentExp)
		{
			return new BugFixSelectionExpression(parentExp);
		}
		public static CommitSelectionExpression AreBugFixes(this CommitSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				from c in s
				join bf in parentExp.Queryable<BugFix>() on c.ID equals bf.CommitID
				select c
			);
		}
		public static CommitSelectionExpression AreNotBugFixes(this CommitSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				from c in s
				join bf in parentExp.Queryable<BugFix>() on c.ID equals bf.CommitID into j
				from x in j.DefaultIfEmpty()
				where
					x == null
				select c
			);
		}
	}
	
	public class BugFixSelectionExpression : EntitySelectionExpression<BugFix,BugFixSelectionExpression>
	{
		public BugFixSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		public BugFixSelectionExpression InCommits()
		{
			return Reselect((s) =>
				from bf in s
				join c in Selection<Commit>() on bf.CommitID equals c.ID
				select bf
			);
		}
		protected override BugFixSelectionExpression Recreate()
		{
			return new BugFixSelectionExpression(this);
		}
	}
}
