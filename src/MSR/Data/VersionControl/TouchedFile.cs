/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.VersionControl
{
	/// <summary>
	/// Touched file in commit.
	/// </summary>
	public class TouchedFile
	{
		/// <summary>
		/// Action on touched path.
		/// </summary>
		public enum TouchedFileAction
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
		/// Path to the touched file.
		/// </summary>
		public string Path;
		/// <summary>
		/// Action on the touched file.
		/// </summary>
		public TouchedFileAction Action;
		/// <summary>
		/// The source path for a copied file.
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
