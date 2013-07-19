/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class ModificationSelectionExtensions
	{
		public static ModificationSelectionExpression Modifications(this IRepositorySelectionExpression parentExp)
		{
			return new ModificationSelectionExpression(parentExp);
		}
		public static CommitSelectionExpression ContainModifications(this CommitSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				(
					from c in s
					join m in parentExp.Selection<Modification>() on c.ID equals m.CommitID
					select c
				).Distinct()
			);
		}
		public static ProjectFileSelectionExpression ContainModifications(this ProjectFileSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				(
					from f in s
					join m in parentExp.Selection<Modification>() on f.ID equals m.FileID
					select f
				).Distinct()
			);
		}
		public static CommitSelectionExpression TouchFiles(this CommitSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				(
					from c in s
					join m in parentExp.Queryable<Modification>() on c.ID equals m.CommitID
					join f in parentExp.Selection<ProjectFile>() on m.FileID equals f.ID
					select c
				).Distinct()
			);
		}
		public static ProjectFileSelectionExpression TouchedInCommits(this ProjectFileSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				(
					from f in s
					join m in parentExp.Queryable<Modification>() on f.ID equals m.FileID
					join c in parentExp.Selection<Commit>() on m.CommitID equals c.ID
					select f
				).Distinct()
			);
		}
	}

	public class ModificationSelectionExpression : EntitySelectionExpression<Modification,ModificationSelectionExpression>
	{
		public ModificationSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		public ModificationSelectionExpression InCommits()
		{
			return Reselect((s) =>
				from m in s
				join c in Selection<Commit>() on m.CommitID equals c.ID
				select m
			);
		}
		public ModificationSelectionExpression InFiles()
		{
			return Reselect((s) =>
				from m in s
				join f in Selection<ProjectFile>() on m.FileID equals f.ID
				select m
			);
		}
		protected override ModificationSelectionExpression Recreate()
		{
			return new ModificationSelectionExpression(this);
		}
	}
}
