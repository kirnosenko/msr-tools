/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data.VersionControl
{
	public interface ILog
	{
		string Revision { get; }
		string Author { get; }
		DateTime Date { get; }
		string Message { get; }

		/// <summary>
		/// Returns alphabetically sorted list of touched files.
		/// </summary>
		IEnumerable<TouchedFile> TouchedFiles { get; }
	}
}
