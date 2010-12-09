/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Data.Linq.Mapping;

namespace MSR.Data.Entities
{
	/// <summary>
	/// Commit of version control system.
	/// </summary>
	[Table(Name = "Commits")]
	public class Commit
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		/// <summary>
		/// Number of commit in commit list ordered in
		/// topological order and/or by date.
		/// It is necessary to be able to say for
		/// any commit pair which of them happened 
		/// first.
		/// </summary>
		[Column(CanBeNull = false)]
		public int OrderedNumber { get; set; }
		/// <summary>
		/// Unique identifier of commit in VCS.
		/// </summary>
		[Column(DbType = "NVarChar(50) NOT NULL")]
		public string Revision { get; set; }
		/// <summary>
		/// The name of the commit's author.
		/// Person had commited changes of the commit.
		/// </summary>
		[Column(DbType = "NVarChar(50) NOT NULL")]
		public string Author { get; set; }
		/// <summary>
		/// The date a commit had taken place.
		/// </summary>
		[Column(CanBeNull = false)]
		public DateTime Date { get; set; }
		/// <summary>
		/// Comments to commit.
		/// </summary>
		[Column(DbType = "NVarChar(MAX) NOT NULL")]
		public string Message { get; set; }
	}
}
