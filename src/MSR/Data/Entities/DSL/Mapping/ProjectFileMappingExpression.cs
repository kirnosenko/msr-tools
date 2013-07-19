/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.DSL.Mapping
{
	public static class ProjectFileMappingExtension
	{
		public static ProjectFileMappingExpression AddFile(this ICommitMappingExpression exp, string filePath)
		{
			return new ProjectFileMappingExpression(exp, filePath);
		}
		public static ProjectFileMappingExpression File(this ICommitMappingExpression exp, string filePath)
		{
			return new ProjectFileMappingExpression(
				exp,
				exp.Queryable<ProjectFile>().Single(x =>
					x.Path == filePath && x.DeletedInCommitID == null
				)
			);
		}
	}

	public interface IProjectFileMappingExpression : ICommitMappingExpression
	{}

	public class ProjectFileMappingExpression : EntityMappingExpression<ProjectFile>, IProjectFileMappingExpression
	{
		public ProjectFileMappingExpression(IRepositoryMappingExpression parentExp, string filePath)
			: base(parentExp)
		{
			entity = new ProjectFile()
			{
				Path = filePath,
				AddedInCommit = CurrentEntity<Commit>()
			};
			AddEntity();
		}
		public ProjectFileMappingExpression(IRepositoryMappingExpression parentExp, ProjectFile file)
			: base(parentExp)
		{
			entity = file;
		}
		public IProjectFileMappingExpression Delete()
		{
			entity.DeletedInCommit = CurrentEntity<Commit>();
			return this;
		}
		public IProjectFileMappingExpression CopiedFrom(string sourseFilePath, string sourceRevision)
		{
			entity.SourceCommit = Queryable<Commit>()
				.Single(x => x.Revision == sourceRevision);
			try
			{
				entity.SourceFile = this.SelectionDSL()
					.Files().PathIs(sourseFilePath).ExistInRevision(sourceRevision).Single();
			}
			catch
			{
				throw new MsrMappingDslException(
					string.Format("Could not find file {0} in revision {1}.", sourseFilePath, sourceRevision)
				);
			}
			return this;
		}
	}
}
