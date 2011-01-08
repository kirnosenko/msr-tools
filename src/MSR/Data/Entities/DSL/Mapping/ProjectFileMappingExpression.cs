/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

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
				exp.Repository<ProjectFile>().Single(x =>
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
			Commit sourceCommit = Repository<Commit>().Single(x => x.Revision == sourceRevision);
			
			entity.SourceCommit = sourceCommit;
			try
			{
				entity.SourceFile = Repository<ProjectFile>().Single(x =>
					x.Path == sourseFilePath
					&&
					Repository<Commit>()
						.Single(c => c.ID == x.AddedInCommitID)
						.OrderedNumber <= sourceCommit.OrderedNumber
					&&
					(
						(x.DeletedInCommitID == null)
						||
						Repository<Commit>()
							.Single(c => c.ID == x.DeletedInCommitID)
							.OrderedNumber > sourceCommit.OrderedNumber
					)
				);
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
