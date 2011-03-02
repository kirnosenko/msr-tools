/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MSR.Data.Entities
{
	/// <summary>
	/// Releases of system.
	/// </summary>
	[Table(Name = "Releases")]
	public class Release
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		/// <summary>
		/// Commit in which the release was done.
		/// </summary>
		[Column(CanBeNull = false)]
		public int CommitID { get; set; }
		/// <summary>
		/// The tag of the release.
		/// </summary>
		[Column(DbType = "NVarChar(50) NOT NULL")]
		public string Tag { get; set; }

		private EntityRef<Commit> _commit;
		[Association(Storage = "_commit", ThisKey = "CommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit Commit
		{
			get { return this._commit.Entity; }
			set { this._commit.Entity = value; }
		}
	}
}
