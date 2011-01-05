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
	/// File in version control system.
	/// </summary>
	[Table(Name = "Files")]
	public class ProjectFile
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		/// <summary>
		/// UNIX-formated path of the file with leading slash.
		/// </summary>
		[Column(DbType = "NVarChar(MAX) NOT NULL")]
		public string Path { get; set; }
		/// <summary>
		/// Commit the file was added in.
		/// </summary>
		[Column(CanBeNull = false)]
		public int AddedInCommitID { get; set; }
		/// <summary>
		/// Commit the file was deleted in.
		/// Null if the file exists so far.
		/// </summary>
		[Column(CanBeNull = true)]
		public int? DeletedInCommitID { get; set; }
		/// <summary>
		/// The source file of the file.
		/// Null if the file was created from scratch.
		/// </summary>
		[Column(CanBeNull = true)]
		public int? SourceFileID { get; set; }
		/// <summary>
		/// The revision the source file was taken from.
		/// </summary>
		[Column(CanBeNull = true)]
		public int? SourceCommitID { get; set; }
		
		private EntityRef<Commit> _addedInCommit;
		[Association(Storage = "_addedInCommit", ThisKey = "AddedInCommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit AddedInCommit
		{
			get { return this._addedInCommit.Entity; }
			set { this._addedInCommit.Entity = value; }
		}

		private EntityRef<Commit> _deletedInCommit;
		[Association(Storage = "_deletedInCommit", ThisKey = "DeletedInCommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit DeletedInCommit
		{
			get { return this._deletedInCommit.Entity; }
			set { this._deletedInCommit.Entity = value; }
		}

		private EntityRef<ProjectFile> _sourceFile;
		[Association(Storage = "_sourceFile", ThisKey = "SourceFileID", OtherKey = "ID", IsForeignKey = true)]
		public ProjectFile SourceFile
		{
			get { return this._sourceFile.Entity; }
			set { this._sourceFile.Entity = value; }
		}

		private EntityRef<Commit> _sourceCommit;
		[Association(Storage = "_sourceCommit", ThisKey = "SourceCommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit SourceCommit
		{
			get { return this._sourceCommit.Entity; }
			set { this._sourceCommit.Entity = value; }
		}
	}
}
