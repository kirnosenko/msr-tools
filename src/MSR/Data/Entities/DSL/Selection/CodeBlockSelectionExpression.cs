/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
				join tcb in Repository<CodeBlock>() on cb.TargetCodeBlockID equals tcb.ID
				select tcb
			);
		}
		public CodeBlockSelectionExpression ModifiedBy()
		{
			return Reselect(s =>
				from tcb in s
				join cb in Repository<CodeBlock>() on tcb.ID equals cb.TargetCodeBlockID
				select cb
			);
		}
		public CodeBlockSelectionExpression InBugFixes()
		{
			return Reselect(s =>
				from cb in s
				join m in Repository<Modification>() on cb.ModificationID equals m.ID
				join c in Repository<Commit>() on m.CommitID equals c.ID
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
