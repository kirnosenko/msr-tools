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
	public class PersistentDataStore : IDataStore
	{
		private string connectionString;

		public PersistentDataStore(string connectionString)
		{
			this.connectionString = connectionString;
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
		private DataContext CreateDataContext()
		{
			return new DataContext(connectionString)
			{
				Log = Logger
			};
		}
	}
}
