/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Data.Linq.Mapping;

namespace MSR.Data.Entities
{
	[Table(Name = "Commits")]
	public class Commit
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		[Column(CanBeNull = false)]
		public int OrderedNumber { get; set; }
		[Column(DbType = "NVarChar(50) NOT NULL")]
		public string Revision { get; set; }
		[Column(DbType = "NVarChar(50) NOT NULL")]
		public string Author { get; set; }
		[Column(CanBeNull = false)]
		public DateTime Date { get; set; }
		[Column(DbType = "NVarChar(MAX) NOT NULL")]
		public string Message { get; set; }
	}
}
