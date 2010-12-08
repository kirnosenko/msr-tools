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
	[Table(Name = "CodeBlocks")]
	public class CodeBlock
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		[Column(CanBeNull = false)]
		public double Size { get; set; }
		[Column(CanBeNull = true)]
		public int? AddedInitiallyInCommitID { get; set; }
		[Column(CanBeNull = false)]
		public int ModificationID { get; set; }
		[Column(CanBeNull = true)]
		public int? TargetCodeBlockID { get; set; }

		private EntityRef<Commit> _addedInitiallyInCommit;
		[Association(Storage = "_addedInitiallyInCommit", ThisKey = "AddedInitiallyInCommitID", OtherKey = "ID", IsForeignKey = true)]
		public Commit AddedInitiallyInCommit
		{
			get { return this._addedInitiallyInCommit.Entity; }
			set { this._addedInitiallyInCommit.Entity = value; }
		}
		private EntityRef<Modification> _modification;
		[Association(Storage = "_modification", ThisKey = "ModificationID", OtherKey = "ID", IsForeignKey = true)]
		public Modification Modification
		{
			get { return this._modification.Entity; }
			set { this._modification.Entity = value; }
		}
		private EntityRef<CodeBlock> _targetCodeBlock;
		[Association(Storage = "_targetCodeBlock", ThisKey = "TargetCodeBlockID", OtherKey = "ID", IsForeignKey = true)]
		public CodeBlock TargetCodeBlock
		{
			get { return this._targetCodeBlock.Entity; }
			set { this._targetCodeBlock.Entity = value; }
		}
	}
}
