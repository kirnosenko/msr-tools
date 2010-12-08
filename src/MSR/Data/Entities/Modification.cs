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
	[Table(Name = "Modifications")]
	public class Modification
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		[Column(CanBeNull = false)]
		public int CommitID { get; set; }
		[Column(CanBeNull = false)]
		public int FileID { get; set; }

		private EntityRef<Commit> _commit;
		[Association(Storage = "_commit", ThisKey = "CommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit Commit
		{
			get { return this._commit.Entity; }
			set { this._commit.Entity = value; }
		}
		private EntityRef<ProjectFile> _file;
		[Association(Storage = "_file", ThisKey = "FileID", OtherKey = "ID", IsForeignKey = true)]
		public ProjectFile File
		{
			get { return this._file.Entity; }
			set { this._file.Entity = value; }
		}
	}
}
