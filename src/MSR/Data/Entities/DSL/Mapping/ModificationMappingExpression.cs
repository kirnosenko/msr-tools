/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Mapping
{
	public static class ModificationMappingExtension
	{
		public static ModificationMappingExpression Modified(this IProjectFileMappingExpression exp)
		{
			return new ModificationMappingExpression(exp);
		}
	}

	public interface IModificationMappingExpression : IProjectFileMappingExpression
	{}

	public class ModificationMappingExpression : EntityMappingExpression<Modification>, IModificationMappingExpression
	{
		public ModificationMappingExpression(IRepositoryMappingExpression parentExp)
			: base(parentExp)
		{
			entity = new Modification()
			{
				Commit = CurrentEntity<Commit>(),
				File = CurrentEntity<ProjectFile>()
			};
			AddEntity();
		}
	}
}
