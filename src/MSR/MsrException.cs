/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR
{
	public class MsrException : ApplicationException
	{
		public MsrException(string message)
			: base(message)
		{}
	}
}
