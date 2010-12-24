/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using DbLinq.PostgreSql;
using Npgsql;

namespace MSR.Data.Persistent
{
	public class PostgresqlDataStore : PersistentDataStore
	{
		private string connectionString;

		public PostgresqlDataStore(string connectionString)
		{
			this.connectionString = connectionString;
		}
		protected override IDataContext CreateDataContext()
		{
			return new AlternativeDataContext(
				new PgsqlDataContext(
					new NpgsqlConnection(connectionString)
				)
			)
			{
				Logger = Logger
			};
		}
	}
}
