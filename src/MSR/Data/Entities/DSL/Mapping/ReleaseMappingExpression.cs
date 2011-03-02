/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.DSL.Mapping
{
	public static class ReleaseMappingExtension
	{
		public static ReleaseMappingExpression IsRelease(this ICommitMappingExpression exp, string tag)
		{
			return new ReleaseMappingExpression(exp, tag);
		}
	}

	public interface IReleaseMappingExpression : ICommitMappingExpression
	{}

	public class ReleaseMappingExpression : EntityMappingExpression<Release>, IBugFixMappingExpression
	{
		public ReleaseMappingExpression(IRepositoryMappingExpression parentExp, string tag)
			: base(parentExp)
		{
			entity = new Release()
			{
				Commit = CurrentEntity<Commit>(),
				Tag = tag
			};
			AddEntity();
		}
	}
}
