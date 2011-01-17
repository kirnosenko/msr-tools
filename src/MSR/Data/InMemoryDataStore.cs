/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data
{
	public class InMemoryDataStore : IDataStore, IRepositoryResolver
	{
		private Dictionary<Type, object> repositories = new Dictionary<Type, object>();
		
		public void CreateSchema(params Type[] tables)
		{
		}
		public ISession OpenSession()
		{
			return new InMemorySession(this);
		}
		public IRepository<T> Repository<T>() where T : class
		{
			if (! repositories.ContainsKey(typeof(T)))
			{
				repositories[typeof(T)] = new InMemoryRepository<T>();
			}
			return (IRepository<T>)repositories[typeof(T)];
		}
	}
}
