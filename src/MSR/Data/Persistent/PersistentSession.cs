/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace MSR.Data.Persistent
{
	public class PersistentSession : ISession
	{
		private DataContext context;
		private Dictionary<Type, object> repositories = new Dictionary<Type, object>();

		public PersistentSession(DataContext context)
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
		public IRepository<T> Repository<T>() where T : class
		{
			if (! repositories.ContainsKey(typeof(T)))
			{
				repositories[typeof(T)] = new PersistentRepository<T>(context);
			}
			return (IRepository<T>)repositories[typeof(T)];
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
