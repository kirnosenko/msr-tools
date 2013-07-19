/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class ReleaseSelectionExtensions
	{
		public static ReleaseSelectionExpression Releases(this IRepositorySelectionExpression parentExp)
		{
			return new ReleaseSelectionExpression(parentExp);
		}
		public static CommitSelectionExpression AreReleases(this CommitSelectionExpression parentExp)
		{
			return parentExp.Reselect(s =>
				from c in s
				join r in parentExp.Queryable<Release>() on c.ID equals r.CommitID
				select c
			);
		}
	}

	public class ReleaseSelectionExpression : EntitySelectionExpression<Release,ReleaseSelectionExpression>
	{
		public ReleaseSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		protected override ReleaseSelectionExpression Recreate()
		{
			return new ReleaseSelectionExpression(this);
		}
	}

}
