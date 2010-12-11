/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.Mapping;
using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.BugTracking;
using MSR.Data.VersionControl;

namespace MSR.Tools.Mapper
{
	public class MappingTool : Tool
	{
		protected IScmData scmData;
		protected IScmData scmDataNoCache;
		
		public MappingTool(string configFile)
			: base(configFile)
		{
			scmData = GetConfiguredType<IScmData>();
			scmDataNoCache = GetConfiguredType<IScmData>("nocache");
		}
		public void Map(bool createSchema, string tillRevision)
		{
			if (createSchema)
			{
				CreateSchema();
			}

			using (ConsoleTimeLogger.Start("mapping time"))
			{
				MappingController mapping = GetConfiguredType<MappingController>();

				int startRevision = MappingStartRevision(data);
				int stopRevision = Convert.ToInt32(tillRevision);

				for (int revision = startRevision; revision <= stopRevision; revision++)
				{
					Console.WriteLine("mapping of revision {0}", revision);
					mapping.RevisionNumber = revision;
					mapping.Map();
				}
			}
		}
		public void Truncate(int numberOfRevisionsToKeep)
		{		
			using (ConsoleTimeLogger.Start("truncating time"))
			using (var s = data.OpenSession())
			{
				var selectionDSL = new RepositorySelectionExpression(s);

				var addedCommits = selectionDSL.Commits().AfterRevision(numberOfRevisionsToKeep);
				var addedBugFixes = addedCommits.BugFixes().InCommits();
				var addedModifications = addedCommits.Modifications().InCommits();
				var addedFiles = addedCommits.Files().AddedInCommits();
				var deletedFiles = addedCommits.Files().DeletedInCommits();
				var addedCodeBlocks = addedModifications.CodeBlocks().InModifications();

				foreach (var codeBlock in addedCodeBlocks)
				{
					s.Repository<CodeBlock>().Delete(codeBlock);
				}
				foreach (var modification in addedModifications)
				{
					s.Repository<Modification>().Delete(modification);
				}
				foreach (var file in addedFiles)
				{
					s.Repository<ProjectFile>().Delete(file);
				}
				foreach (var file in deletedFiles)
				{
					file.DeletedInCommit = null;
				}
				foreach (var bugFix in addedBugFixes)
				{
					s.Repository<BugFix>().Delete(bugFix);
				}
				foreach (var commit in addedCommits)
				{
					s.Repository<Commit>().Delete(commit);
				}

				s.SubmitChanges();
			}
		}
		public void Check(int stopRevisionNumber)
		{
			string stopRevision = scmData.RevisionByNumber(stopRevisionNumber);
			
			using (ConsoleTimeLogger.Start("checking time"))
			using (var s = data.OpenSession())
			{
				string testRevision = stopRevision;
				if (s.Repository<Commit>().SingleOrDefault(c => c.Revision == testRevision) == null)
				{
					Console.WriteLine("Could not find revision {0}.", testRevision);
					return;
				}

				CheckEmptyCodeBlocks(s, testRevision);
				CheckCodeSizeForDeletedFiles(s, testRevision);
				CheckTargetsForCodeBlocks(s, testRevision);
				CheckDeletedCodeBlocksVsAdded(s, testRevision);
				CheckLinesContent(s, scmDataNoCache, testRevision);
			}
		}

		private void CreateSchema()
		{
			data.CreateSchema(
				typeof(Commit),
				typeof(BugFix),
				typeof(ProjectFile),
				typeof(Modification),
				typeof(CodeBlock)
			);
		}
		private int MappingStartRevision(IDataStore data)
		{
			using (var s = data.OpenSession())
			{
				if (s.Repository<Commit>().Count() == 0)
				{
					return 1;
				}
				return s.Repository<Commit>().Count() + 1;
			}
		}
		private void CheckEmptyCodeBlocks(IRepositoryResolver repositories, string testRevision)
		{
			foreach (var zeroCodeBlock in
				from cb in repositories.Repository<CodeBlock>()
				join m in repositories.Repository<Modification>() on cb.ModificationID equals m.ID
				join f in repositories.Repository<ProjectFile>() on m.FileID equals f.ID
				join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
				let testRevisionNumber = repositories.Repository<Commit>()
					.Single(x => x.Revision == testRevision)
					.OrderedNumber
				where
					c.OrderedNumber <= testRevisionNumber
					&&
					cb.Size == 0
				select new { path = f.Path, revision = c.Revision }
			)
			{
				Console.WriteLine("Empty code block in file {0} in revision {1}!",
					zeroCodeBlock.path, zeroCodeBlock.revision
				);
			}
		}
		private void CheckTargetsForCodeBlocks(IRepositoryResolver repositories, string testRevision)
		{
			foreach (var codeBlockWithWrongTarget in
				from cb in repositories.Repository<CodeBlock>()
				join m in repositories.Repository<Modification>() on cb.ModificationID equals m.ID
				join f in repositories.Repository<ProjectFile>() on m.FileID equals f.ID
				join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
				let testRevisionNumber = repositories.Repository<Commit>()
					.Single(x => x.Revision == testRevision)
					.OrderedNumber
				where
					c.OrderedNumber <= testRevisionNumber
					&&
					(
						(cb.Size > 0 && cb.TargetCodeBlockID != null)
						||
						(cb.Size < 0 && cb.TargetCodeBlockID == null)
					)
				select new
				{
					CodeSize = cb.Size,
					Path = f.Path,
					Revision = c.Revision
				}
			)
			{
				Console.WriteLine("Code block in revision {0} with size {1} for file {2} should{3} have target!",
					codeBlockWithWrongTarget.Revision,
					codeBlockWithWrongTarget.CodeSize,
					codeBlockWithWrongTarget.Path,
					codeBlockWithWrongTarget.CodeSize < 0 ? "" : " not"
				);
			}
		}
		private void CheckCodeSizeForDeletedFiles(IRepositoryResolver repositories, string testRevision)
		{
			foreach (var codeSizeForDeletedFile in
				(
					from cb in repositories.Repository<CodeBlock>()
					join m in repositories.Repository<Modification>() on cb.ModificationID equals m.ID
					join f in repositories.Repository<ProjectFile>() on m.FileID equals f.ID
					join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
					let testRevisionNumber = repositories.Repository<Commit>()
						.Single(x => x.Revision == testRevision)
						.OrderedNumber
					where
						c.OrderedNumber <= testRevisionNumber
						&&
						f.DeletedInCommitID != null
						&&
						repositories.Repository<Commit>()
							.Single(x => x.ID == f.DeletedInCommitID)
							.OrderedNumber <= testRevisionNumber
					group cb by f into g
					select new
					{
						Path = g.Key.Path,
						DeletedInRevision = repositories.Repository<Commit>()
							.Single(x => x.ID == g.Key.DeletedInCommitID)
							.Revision,
						CodeSize = g.Sum(x => x.Size)
					}
				).Where(x => x.CodeSize != 0)
			)
			{
				Console.WriteLine("For file {0} deleted in revision {1} code size is not 0. {2} should be 0.",
					codeSizeForDeletedFile.Path,
					codeSizeForDeletedFile.DeletedInRevision,
					codeSizeForDeletedFile.CodeSize
				);
			}
		}
		private void CheckDeletedCodeBlocksVsAdded(IRepositoryResolver repositories, string testRevision)
		{
			foreach (var codeBlockWithWrongTarget in
				(
					from cb in repositories.Repository<CodeBlock>()
					join m in repositories.Repository<Modification>() on cb.ModificationID equals m.ID
					join f in repositories.Repository<ProjectFile>() on m.FileID equals f.ID
					join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
					let testRevisionNumber = repositories.Repository<Commit>()
						.Single(x => x.Revision == testRevision)
						.OrderedNumber
					let addedCodeID = cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID
					where
						c.OrderedNumber <= testRevisionNumber
					group cb.Size by addedCodeID into g
					select new
					{
						CodeSize = g.Sum(),
						Path = repositories.Repository<ProjectFile>()
							.Single(f => f.ID == repositories.Repository<Modification>()
								.Single(m => m.ID == repositories.Repository<CodeBlock>()
									.Single(cb => cb.ID == g.Key).ModificationID
								).FileID
							).Path,
						Revision = repositories.Repository<Commit>()
							.Single(c => c.ID == repositories.Repository<Modification>()
								.Single(m => m.ID == repositories.Repository<CodeBlock>()
									.Single(cb => cb.ID == g.Key).ModificationID
								).CommitID
							).Revision
					}
				).Where(x => x.CodeSize < 0)
			)
			{
				Console.WriteLine("There are too many deleted code blocks for code block in revision {0} for file {1}. Code size {2} should be greater or equal to 0",
					codeBlockWithWrongTarget.Revision,
					codeBlockWithWrongTarget.Path,
					codeBlockWithWrongTarget.CodeSize
				);
			}
		}
		private void CheckLinesContent(IRepositoryResolver repositories, IScmData scmData, string testRevision)
		{
			var selectionDSL = new RepositorySelectionExpression(repositories);

			var existentFiles = selectionDSL.Files()
				//.InDirectory("/trunk")
				.ExistInRevision(testRevision);

			foreach (var existentFile in existentFiles)
			{
				IBlame fileBlame = scmData.Blame(testRevision, existentFile.Path);

				double currentLOC = selectionDSL
					.Commits().TillRevision(testRevision)
					.Files().PathIs(existentFile.Path).ExistInRevision(testRevision)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications()
					.CalculateLOC();

				if ((int)currentLOC != fileBlame.Count)
				{
					Console.WriteLine("Incorrect number of lines in file {0}. {1} should be {2}",
						existentFile.Path, currentLOC, fileBlame.Count
					);

					SmartDictionary<string, int> linesByRevision = new SmartDictionary<string, int>(x => 0);
					foreach (var line in fileBlame)
					{
						linesByRevision[line.Value]++;
					}

					var codeBySourceRevision =
					(
						from f in repositories.Repository<ProjectFile>()
						join m in repositories.Repository<Modification>() on f.ID equals m.FileID
						join cb in repositories.Repository<CodeBlock>() on m.ID equals cb.ModificationID
						join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
						let addedCodeBlock = repositories.Repository<CodeBlock>()
							.Single(x => x.ID == (cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID))
						let codeAddedInitiallyInRevision = repositories.Repository<Commit>()
							.Single(x => x.ID == addedCodeBlock.AddedInitiallyInCommitID)
							.Revision
						let testRevisionNumber = repositories.Repository<Commit>()
							.Single(x => x.Revision == testRevision)
							.OrderedNumber
						where
							f.ID == existentFile.ID
							&&
							c.OrderedNumber <= testRevisionNumber
						group cb.Size by codeAddedInitiallyInRevision into g
						select new { FromRevision = g.Key, CodeSize = g.Sum() }
					).Where(x => x.CodeSize != 0);

					if (codeBySourceRevision.Count() != linesByRevision.Count)
					{
						Console.WriteLine("Number of revisions file {0} contains code from is incorrect. {1} should be {2}",
							existentFile.Path, codeBySourceRevision.Count(), linesByRevision.Count
						);
					}
					foreach (var codeForRevision in codeBySourceRevision)
					{
						if (codeForRevision.CodeSize != linesByRevision[codeForRevision.FromRevision])
						{
							Console.WriteLine("Incorrect number of lines in file {0} from revision {1}. {2} should be {3}",
								existentFile.Path,
								codeForRevision.FromRevision,
								codeForRevision.CodeSize,
								linesByRevision[codeForRevision.FromRevision]
							);
						}
					}
				}
			}
		}
	}
}
