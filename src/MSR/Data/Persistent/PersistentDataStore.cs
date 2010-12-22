/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Data.Linq;
using System.IO;

namespace MSR.Data.Persistent
{
	public abstract class PersistentDataStore : IDataStore
	{
		public PersistentDataStore()
		{
			QueryTimeout = 60;
		}
		public ISession OpenSession()
		{
			return new PersistentSession(CreateDataContext())
			{
				QueryTimeout = this.QueryTimeout,
				ReadOnly = this.ReadOnly
			};
		}
		public void CreateSchema(params Type[] tables)
		{
			using (var context = CreateDataContext())
			{
				context.CreateSchema(tables);
			}
		}
		public TextWriter Logger
		{
			get; set;
		}
		public int QueryTimeout
		{
			get; set;
		}
		public bool ReadOnly
		{
			get; set;
		}
		protected abstract IDataContext CreateDataContext();
	}
}
