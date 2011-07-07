/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class CommitMapperForExistentRevision : CommitMapper
	{
		public CommitMapperForExistentRevision(IScmData scmData)
			: base(scmData)
		{
		}
		protected override CommitMappingExpression ExpressionFor(RepositoryMappingExpression expression)
		{
			return expression.Commit(expression.Revision);
		}
	}
}
