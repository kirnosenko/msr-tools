/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace MSR.Data.Persistent
{
	public class PersistentSession : ISession
	{
		private IDataContext context;
		private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

		public PersistentSession(IDataContext context)
		{
			this.context = context;
			ReadOnly = false;
		}
		public void Dispose()
		{
			if (context != null)
			{
				context.Dispose();
				context = null;
			}
		}
		public void Add<T>(T entity) where T : class
		{
			context.Add(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			context.AddRange(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			context.Delete(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return context.Queryable<T>();
		}
		public void SubmitChanges()
		{
			if (! ReadOnly)
			{
				context.SubmitChanges();
			}
		}
		public int QueryTimeout
		{
			get { return context.CommandTimeout; }
			set { context.CommandTimeout = value; }
		}
		public bool ReadOnly
		{
			get; set;
		}
	}
}
