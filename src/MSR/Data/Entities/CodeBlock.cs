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
	/// Piece of code. Added or removed.
	/// </summary>
	[Table(Name = "CodeBlocks")]
	public class CodeBlock
	{
		[Column(DbType = "Int NOT NULL IDENTITY", AutoSync = AutoSync.OnInsert, IsPrimaryKey = true, IsDbGenerated = true)]
		public int ID { get; set; }
		/// <summary>
		/// A size of the code block. Can be LOC or something else.
		/// A positive value means code addition.
		/// A negative value means code removing.
		/// </summary>
		[Column(CanBeNull = false)]
		public double Size { get; set; }
		/// <summary>
		/// Commit in which code was added in.
		/// Null for code removing.
		/// </summary>
		[Column(CanBeNull = true)]
		public int? AddedInitiallyInCommitID { get; set; }
		/// <summary>
		/// Modification code block was created in.
		/// Null for code removing.
		/// </summary>
		[Column(CanBeNull = false)]
		public int ModificationID { get; set; }
		/// <summary>
		/// A code block being changed by this one.
		/// Now code removing block keeps code block from which
		/// code being removed.
		/// </summary>
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
