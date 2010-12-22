/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Persistent
{
	public class MsSqlServerDataStore : PersistentDataStore
	{
		private string connectionString;
		
		public MsSqlServerDataStore(string connectionString)
		{
			this.connectionString = connectionString;
		}
		protected override IDataContext CreateDataContext()
		{
			return new DefaultDataContext(connectionString)
			{
				Logger = Logger
			};
		}
	}
}
