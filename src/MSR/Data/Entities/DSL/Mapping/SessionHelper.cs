/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.DSL.Mapping
{
	public static class SessionHelper
	{
		public static RepositoryMappingExpression MappingDSL(this ISession session)
		{
			return new RepositoryMappingExpression(session);
		}
	}
}
