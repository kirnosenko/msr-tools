/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data
{
	public class InMemorySession : ISession
	{
		private IRepositoryResolver repositories;
		private event Action OnSubmit;
		
		public InMemorySession(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public void Dispose()
		{
		}
		public void Add<T>(T entity) where T : class
		{
			Repository<T>().Add(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			Repository<T>().AddRange(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			Repository<T>().Delete(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return Repository<T>();
		}
		public InMemoryRepository<T> Repository<T>() where T : class
		{
			InMemoryRepository<T> repository = repositories.Repository<T>();
			OnSubmit += repository.SubmitChanges;
			return repository;
		}
		public void SubmitChanges()
		{
			if (OnSubmit != null)
			{
				OnSubmit();
			}
		}
	}
}
