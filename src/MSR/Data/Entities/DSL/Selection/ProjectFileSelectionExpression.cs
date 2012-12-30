/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection
{
	public static class ProjectFileSelectionExtension
	{
		public static ProjectFileSelectionExpression Files(this IRepositorySelectionExpression parentExp)
		{
			return new ProjectFileSelectionExpression(parentExp);
		}
		public static ProjectFileSelectionExpression DefectiveFiles(
			this CodeBlockSelectionExpression parentExp,
			string detectAfterRevision,
			string detectTillRevision)
		{
			return parentExp.ModifiedBy()
				.Modifications()
					.ContainCodeBlocks()
				.Commits()
					.AfterRevision(detectAfterRevision)
					.TillRevision(detectTillRevision)
					.AreBugFixes()
					.ContainModifications()
				.Files()
					.TouchedInCommits()
				.Reselect(x => x.Distinct());
		}
	}

	public class ProjectFileSelectionExpression : EntitySelectionExpression<ProjectFile,ProjectFileSelectionExpression>
	{
		public ProjectFileSelectionExpression(IRepositorySelectionExpression parentExp)
			: base(parentExp)
		{
		}
		public ProjectFileSelectionExpression AddedInCommits()
		{
			return Reselect(s =>
				from f in s
				where
					Selection<Commit>().Any(c => c.ID == f.AddedInCommitID)
				select f
			);
		}
		public ProjectFileSelectionExpression Deleted()
		{
			return Reselect(s =>
				from f in s
				where
					f.DeletedInCommitID != null
				select f
			);
		}
		public ProjectFileSelectionExpression DeletedInCommits()
		{
			return Reselect(s =>
				from f in s
				where
					Selection<Commit>().Any(c => c.ID == f.DeletedInCommitID)
				select f
			);
		}
		public ProjectFileSelectionExpression IdIs(int id)
		{
			return Reselect(s =>
				s.Where(x => x.ID == id)
			);
		}
		public ProjectFileSelectionExpression IdIn(int[] ids)
		{
			return Reselect(s =>
				s.Where(x => ids.Contains(x.ID))
			);
		}
		public ProjectFileSelectionExpression PathIs(string filePath)
		{
			return Reselect(s =>
				s.Where(x => x.Path == filePath)
			);
		}
		public ProjectFileSelectionExpression InDirectory(string dirPath)
		{
			if (! dirPath.EndsWith("/"))
			{
				dirPath += "/";
			}
			return PathStartsWith(dirPath);
		}
		public ProjectFileSelectionExpression PathStartsWith(string pathBeginning)
		{
			return Reselect(s =>
				s.Where(x => x.Path.StartsWith(pathBeginning))
			);
		}
		public ProjectFileSelectionExpression PathEndsWith(string pathEnding)
		{
			return Reselect(s =>
				s.Where(x => x.Path.EndsWith(pathEnding))
			);
		}
		public ProjectFileSelectionExpression Exist()
		{
			return Reselect(s =>
				from f in s
				where f.DeletedInCommitID == null
				select f
			);
		}
		public ProjectFileSelectionExpression ExistInRevision(string revision)
		{
			return Reselect(s =>
				from f in s
				let revisionNumber = Queryable<Commit>().Single(x => x.Revision == revision).OrderedNumber
				where
					Queryable<Commit>().Single(x => x.ID == f.AddedInCommitID).OrderedNumber <= revisionNumber
					&&
					(
						(f.DeletedInCommitID == null)
						||
						(Queryable<Commit>().Single(x => x.ID == f.DeletedInCommitID).OrderedNumber > revisionNumber)
					)
				select f
			);
		}
		protected override ProjectFileSelectionExpression Recreate()
		{
			return new ProjectFileSelectionExpression(this);
		}
	}
}
