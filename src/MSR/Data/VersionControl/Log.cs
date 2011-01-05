/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data.VersionControl
{
	public abstract class Log : ILog
	{
		protected List<TouchedFile> touchedFiles;
		
		public string Revision
		{
			get; protected set;
		}
		public string Author
		{
			get; protected set;
		}
		public DateTime Date
		{
			get; protected set;
		}
		public string Message
		{
			get; protected set;
		}
		public virtual IEnumerable<TouchedFile> TouchedFiles
		{
			get { return touchedFiles; }
		}
	}
}
