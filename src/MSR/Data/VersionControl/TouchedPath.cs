/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl
{
	/// <summary>
	/// Touched path in commit.
	/// </summary>
	public class TouchedPath
	{
		/// <summary>
		/// Action on touched path.
		/// </summary>
		public enum TouchedPathAction
		{
			/// <summary>
			/// Addition of a new file.
			/// </summary>
			ADDED,
			/// <summary>
			/// Modify of an existent file.
			/// </summary>
			MODIFIED,
			/// <summary>
			/// Removing of an existent file.
			/// </summary>
			DELETED
		}
		
		/// <summary>
		/// Touched path.
		/// </summary>
		public string Path;
		/// <summary>
		/// Action on touched path.
		/// </summary>
		public TouchedPathAction Action;
		/// <summary>
		/// The source path for a copied path.
		/// Null for a file created from scratch.
		/// </summary>
		public string SourcePath;
		/// <summary>
		/// Revision source path was taken from.
		/// Null for a file created from scratch.
		/// If null for path which has source path,
		/// mapping will fill this with the revision
		/// of the last mapped commit.
		/// </summary>
		public string SourceRevision;
	}
}
