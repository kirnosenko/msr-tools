/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class RepositoryResolverHelper
	{
		public static RepositorySelectionExpression SelectionDSL(this IRepository repository)
		{
			return new RepositorySelectionExpression(repository);
		}
	}
}
