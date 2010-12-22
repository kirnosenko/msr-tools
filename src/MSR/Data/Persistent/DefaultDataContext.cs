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
	public class DefaultDataContext : IDataContext
	{
		private DataContext context;
		
		public DefaultDataContext(string connectionString)
		{
			context = new DataContext(connectionString);
		}
		public void Dispose()
		{
			context.Dispose();
		}
		public void CreateSchema(params Type[] tables)
		{
			if (context.DatabaseExists())
			{
				context.DeleteDatabase();
			}
			foreach (var table in tables)
			{
				context.GetTable(table);
			}
			context.CreateDatabase();
		}
		public void Add<T>(T entity) where T : class
		{
			Table<T>().InsertOnSubmit(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			Table<T>().InsertAllOnSubmit(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			Table<T>().DeleteOnSubmit(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return context.GetTable<T>();
		}
		public IEnumerable<T> Enumerable<T>() where T : class
		{
			return context.GetTable<T>();
		}
		public void SubmitChanges()
		{
			context.SubmitChanges();
		}

		public int CommandTimeout
		{
			get { return context.CommandTimeout; }
			set { context.CommandTimeout = value; }
		}
		public TextWriter Logger
		{
			get { return context.Log; }
			set { context.Log = value; }
		}
		private Table<T> Table<T>() where T : class
		{
			return context.GetTable<T>();
		}
	}
}
