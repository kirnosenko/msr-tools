/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public interface IRepositorySelectionExpression : IRepositoryResolver
	{
		IQueryable<T> Selection<T>() where T : class;
	}

	public class RepositorySelectionExpression : IRepositorySelectionExpression
	{
		private IRepositoryResolver repositories;
		
		public RepositorySelectionExpression(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IRepository<T> Repository<T>() where T : class
		{
			return repositories.Repository<T>();
		}
		public IQueryable<T> Selection<T>() where T : class
		{
			return Repository<T>();
		}
	}
}
