/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.DSL.Mapping
{
	public class MsrMappingDslException : MsrException
	{
		public MsrMappingDslException(string message)
			: base(message)
		{
		}
	}
}
