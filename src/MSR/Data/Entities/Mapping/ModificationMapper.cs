/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class ModificationMapper : EntityMapper<ProjectFileMappingExpression,ModificationMappingExpression>
	{
		public ModificationMapper(IScmData scmData)
			: base(scmData)
		{
		}
		public override IEnumerable<ModificationMappingExpression> Map(ProjectFileMappingExpression expression)
		{
			return new ModificationMappingExpression[]
			{
				expression.Modified()
			};
		}
	}
}
