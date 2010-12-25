/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.IO;

namespace MSR.Data.Persistent
{
	public class DefaultDataContext : DataContext, IDataContext
	{
		public DefaultDataContext(string connectionString)
			: base(connectionString)
		{
		}
		public void CreateSchema(params Type[] tables)
		{
			if (DatabaseExists())
			{
				DeleteDatabase();
			}
			foreach (var table in tables)
			{
				GetTable(table);
			}
			CreateDatabase();
		}
		public void Add<T>(T entity) where T : class
		{
			GetTable<T>().InsertOnSubmit(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			GetTable<T>().InsertAllOnSubmit(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			GetTable<T>().DeleteOnSubmit(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return GetTable<T>();
		}
		public IEnumerable<T> Enumerable<T>() where T : class
		{
			return GetTable<T>();
		}
	}
}
