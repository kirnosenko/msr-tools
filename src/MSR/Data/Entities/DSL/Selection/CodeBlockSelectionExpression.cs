/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class CodeBlockSelectionExtensions
	{
		public static CodeBlockSelectionExpression CodeBlocks(this IRepositorySelectionExpression parentExp)
		{
			return new CodeBlockSelectionExpression(parentExp);
		}
		public static ModificationSelectionExpression ContainCodeBlocks(this ModificationSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				(
					from m in s
					join cb in parentExp.Selection<CodeBlock>() on m.ID equals cb.ModificationID
					select m
				).Distinct()
			);
		}
		public static CommitSelectionExpression AreRefactorings(this CommitSelectionExpression parentExp)
		{
			return parentExp.AreNotBugFixes().Reselect(s =>
				(
					from c in s
					join m in parentExp.Queryable<Modification>() on c.ID equals m.CommitID
					join cb in parentExp.Queryable<CodeBlock>() on m.ID equals cb.ModificationID
					group cb by c into g
					select new
					{
						Commit = g.Key,
						Added = g.Where(x => x.Size > 0).Sum(x => x.Size),
						Removed = -g.Where(x => x.Size < 0).Sum(x => x.Size)
					}
				).Where(x => x.Removed / x.Added >= 1d / 2).Select(x => x.Commit)
			);
		}
	}

	public class CodeBlockSelectionExpression : EntitySelectionExpression<CodeBlock,CodeBlockSelectionExpression>
	{
		public CodeBlockSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		public CodeBlockSelectionExpression InModifications()
		{
			return Reselect(s =>
				from cb in s
				join m in Selection<Modification>() on cb.ModificationID equals m.ID
				select cb
			);
		}
		public CodeBlockSelectionExpression AddedInitiallyInCommits()
		{
			return Reselect(s =>
				from cb in s
				join c in Selection<Commit>() on cb.AddedInitiallyInCommitID equals c.ID
				select cb
			);
		}
		public CodeBlockSelectionExpression Added()
		{
			return Reselect(s => s.Where(x => x.Size > 0));
		}
		public CodeBlockSelectionExpression Deleted()
		{
			return Reselect(s => s.Where(x => x.Size < 0));
		}
		public CodeBlockSelectionExpression Modify()
		{
			return Reselect(s =>
				from cb in s
				join tcb in Queryable<CodeBlock>() on cb.TargetCodeBlockID equals tcb.ID
				select tcb
			);
		}
		public CodeBlockSelectionExpression ModifiedBy()
		{
			return Reselect(s =>
				from tcb in s
				join cb in Queryable<CodeBlock>() on tcb.ID equals cb.TargetCodeBlockID
				select cb
			);
		}
		public CodeBlockSelectionExpression InBugFixes()
		{
			return Reselect(s =>
				from cb in s
				join m in Queryable<Modification>() on cb.ModificationID equals m.ID
				join c in Queryable<Commit>() on m.CommitID equals c.ID
				join bf in Selection<BugFix>() on c.ID equals bf.CommitID
				select cb
			);
		}
		protected override CodeBlockSelectionExpression Recreate()
		{
			return new CodeBlockSelectionExpression(this);
		}
	}
}
