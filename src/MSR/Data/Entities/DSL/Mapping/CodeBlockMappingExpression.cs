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
	public static class CodeBlockMappingExtension
	{
		public static CodeBlockMappingExpression Code(this IModificationMappingExpression exp, double size)
		{
			return new CodeBlockMappingExpression(exp, size);
		}
		public static CodeBlockMappingExpression CopyCode(this IModificationMappingExpression exp)
		{
			CodeBlockMappingExpression lastCodeBlockExp = null;
			
			foreach (var codeByAddedCode in (
				from cb in exp.Queryable<CodeBlock>()
				join m in exp.Queryable<Modification>() on cb.ModificationID equals m.ID
				join f in exp.Queryable<ProjectFile>() on m.FileID equals f.ID
				join c in exp.Queryable<Commit>() on m.CommitID equals c.ID
					let addedCodeID = cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID
				where
					f.ID == exp.CurrentEntity<ProjectFile>().SourceFile.ID &&
					c.OrderedNumber <= exp.CurrentEntity<ProjectFile>().SourceCommit.OrderedNumber
				group cb.Size by addedCodeID
				)
			)
			{
				double currentCodeSize = codeByAddedCode.Sum();
				
				if (currentCodeSize != 0)
				{
					if (lastCodeBlockExp == null)
					{
						lastCodeBlockExp = exp.Code(currentCodeSize);
					}
					else
					{
						lastCodeBlockExp = lastCodeBlockExp.Code(currentCodeSize);
					}
					lastCodeBlockExp.CopiedFrom(
						RevisionCodeBlockWasInitiallyAddedIn(exp, codeByAddedCode.Key ?? 0)
					);
				}
			}
				
			return lastCodeBlockExp;
		}
		public static CodeBlockMappingExpression DeleteCode(this IModificationMappingExpression exp)
		{
			CodeBlockMappingExpression lastCodeBlockExp = null;

			foreach (var codeByAddedCode in (
				from cb in exp.Queryable<CodeBlock>()
				join m in exp.Queryable<Modification>() on cb.ModificationID equals m.ID
				join f in exp.Queryable<ProjectFile>() on m.FileID equals f.ID
					let addedCodeID = cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID
				where
					f.ID == exp.CurrentEntity<ProjectFile>().ID
				group cb.Size by addedCodeID
				)
			)
			{
				double currentCodeSize = codeByAddedCode.Sum();
				
				if (currentCodeSize != 0)
				{
					if (lastCodeBlockExp == null)
					{
						lastCodeBlockExp = exp.Code(- currentCodeSize);
					}
					else
					{
						lastCodeBlockExp = lastCodeBlockExp.Code(- currentCodeSize);
					}
					lastCodeBlockExp.ForCodeAddedInitiallyInRevision(
						RevisionCodeBlockWasInitiallyAddedIn(exp, codeByAddedCode.Key ?? 0)
					);
				}
			}
			
			return lastCodeBlockExp;
		}
		private static string RevisionCodeBlockWasInitiallyAddedIn(IRepository repository, int codeBlockID)
		{
			return repository.Queryable<Commit>()
				.Single(c => c.ID == repository.Queryable<CodeBlock>()
					.Single(cb => cb.ID == codeBlockID).AddedInitiallyInCommitID
				).Revision;
		}
	}

	public interface ICodeBlockMappingExpression : IModificationMappingExpression
	{}

	public class CodeBlockMappingExpression : EntityMappingExpression<CodeBlock>, ICodeBlockMappingExpression
	{
		public CodeBlockMappingExpression(IRepositoryMappingExpression parentExp, double size)
			: base(parentExp)
		{
			entity = new CodeBlock()
			{
				Size = size,
				Modification = CurrentEntity<Modification>(),
			};
			if (size > 0)
			{
				entity.AddedInitiallyInCommit = CurrentEntity<Commit>();
			}
			AddEntity();
		}
		public ICodeBlockMappingExpression CopiedFrom(string revision)
		{
			entity.AddedInitiallyInCommit = Queryable<Commit>()
				.Single(c => c.Revision == revision);
			return this;
		}
		public ICodeBlockMappingExpression ForCodeAddedInitiallyInRevision(string revision)
		{
			try
			{
				entity.TargetCodeBlock = this.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(CurrentEntity<Modification>().File.ID)
					.Modifications().InFiles()
					.CodeBlocks().InModifications().AddedInitiallyInCommits().Single();
			}
			catch
			{
				throw new MsrMappingDslException(string.Format(
					"Could not found the code block added initially in revision {0} to file {1}.",
					revision,
					CurrentEntity<Modification>().File.Path
				));
			}
			
			return this;
		}
	}
}
