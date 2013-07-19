/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data
{
	public interface IRepositoryResolver
	{
		InMemoryRepository<T> Repository<T>() where T : class;
	}

	public class InMemoryDataStore : IDataStore, IRepositoryResolver
	{
		private Dictionary<Type,object> repositories = new Dictionary<Type,object>();
		
		public void CreateSchema(params Type[] tables)
		{
		}
		public ISession OpenSession()
		{
			return new InMemorySession(this);
		}
		public InMemoryRepository<T> Repository<T>() where T : class
		{
			if (! repositories.ContainsKey(typeof(T)))
			{
				repositories[typeof(T)] = new InMemoryRepository<T>();
			}
			return (InMemoryRepository<T>)repositories[typeof(T)];
		}
	}
}
