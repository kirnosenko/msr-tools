using System;
using System.Linq;
using DbLinq.Data.Linq;
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
			return new AlternativeDataContext(new DataContext
			(
				new NpgsqlConnection(connectionString),
				new PgsqlVendor()
			))
			{
				Logger = Logger
			};
		}
	}
}
