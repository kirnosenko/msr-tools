/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MSR.Data.Entities
{
	/// <summary>
	/// Bug fix.
	/// </summary>
	[Table(Name = "BugFixes")]
	public class BugFix
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		/// <summary>
		/// Commit that contains code that fixes a bug.
		/// </summary>
		[Column(CanBeNull = false)]
		public int CommitID { get; set; }

		private EntityRef<Commit> _commit;
		[Association(Storage = "_commit", ThisKey = "CommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit Commit
		{
			get { return this._commit.Entity; }
			set { this._commit.Entity = value; }
		}
	}
}
