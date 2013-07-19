/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Mapping
{
	public static class CommitMappingExtension
	{
		public static CommitMappingExpression AddCommit(this IRepositoryMappingExpression exp, string revision)
		{
			return new CommitMappingExpression(exp, revision);
		}
		public static CommitMappingExpression Commit(this IRepositoryMappingExpression exp, string revision)
		{
			return new CommitMappingExpression(
				exp,
				exp.Queryable<Commit>().Single(x =>
					x.Revision == revision
				)
			);
		}
	}

	public interface ICommitMappingExpression : IRepositoryMappingExpression
	{}

	public class CommitMappingExpression : EntityMappingExpression<Commit>, ICommitMappingExpression
	{
		public CommitMappingExpression(IRepositoryMappingExpression parentExp, string revision)
			: base(parentExp)
		{
			entity = new Commit();
			entity.OrderedNumber = Queryable<Commit>().Count() + 1;
			entity.Revision = revision;
			AddEntity();
		}
		public CommitMappingExpression(IRepositoryMappingExpression parentExp, Commit commit)
			: base(parentExp)
		{
			entity = commit;
		}
		public CommitMappingExpression By(string author)
		{
			entity.Author = author;
			return this;
		}
		public CommitMappingExpression WithMessage(string message)
		{
			entity.Message = message;
			return this;
		}
		public CommitMappingExpression At(DateTime date)
		{
			entity.Date = date;
			return this;
		}	
	}
}
