/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.Mapping;
using MSR.Data.Entities.Mapping.PathSelectors;
using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.BugTracking;
using MSR.Data.VersionControl;

namespace MSR.Tools.Mapper
{
	public class MappingTool : Tool
	{
		private IScmData scmData;
		private IScmData scmDataWithoutCache;
		
		private bool automaticallyFixDiffErrors;
		private Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> pathFilter;
		
		public MappingTool(string configFile)
			: base(configFile, "mappingtool")
		{
			scmData = GetScmData();
			scmDataWithoutCache = GetScmDataWithoutCache();
		}
		public void Info()
		{
			Console.WriteLine("Database info:");
			using (var s = data.OpenSession())
			{
				Console.WriteLine("Revisions: {0}",
					s.Queryable<Commit>().Count()
				);
				Console.WriteLine("Last revision: {0}", s.LastRevision());
				Console.WriteLine("Period: {0}-{1}",
					s.Queryable<Commit>().Min(c => c.Date),
					s.Queryable<Commit>().Max(c => c.Date)
				);
				Console.WriteLine("LOC: {0}",
					s.SelectionDSL().CodeBlocks().CalculateLOC()
				);
				Console.WriteLine("Releases: {0}",
					s.Queryable<Release>().Count()
				);
				Console.WriteLine("Files: {0}",
					s.Queryable<ProjectFile>().Count()
				);
				Console.WriteLine("Fixes: {0}",
					s.Queryable<BugFix>().Count()
				);
			}
		}
		public void Map(bool createSchema, int stopRevisionNumber)
		{
			Map(createSchema, scmData.RevisionByNumber(stopRevisionNumber));
		}
		public void Map(bool createDataBase, string stopRevision)
		{
			MappingController mapping = GetConfiguredType<MappingController>();
			mapping.CreateDataBase = createDataBase;
			mapping.StopRevision = stopRevision;
			
			Map(mapping);
		}
		public void PartialMap(int startRevisionNumber, IPathSelector[] pathSelectors)
		{
			PartialMap(scmData.RevisionByNumber(startRevisionNumber), pathSelectors);
		}
		public void PartialMap(string startRevision, IPathSelector[] pathSelectors)
		{
			MappingController mapping = GetConfiguredType<MappingController>();
			mapping.StartRevision = startRevision;
			mapping.RegisterMapper(GetConfiguredType<CommitMapperForExistentRevision>());
			var fileMapper = GetConfiguredType<ProjectFileMapper>();
			fileMapper.PathSelectors = pathSelectors;
			mapping.RegisterMapper(fileMapper);
			mapping.KeepOnlyMappers(new Type[]
			{
				typeof(Commit),
				typeof(ProjectFile),
				typeof(Modification),
				typeof(CodeBlock)
			});

			Map(mapping);
		}
		public void MapReleaseEntity(string revision, string tag)
		{
			using (ConsoleTimeLogger.Start("entity mapping time"))
			using (var s = data.OpenSession())
			{
				s.Add(new Release()
				{
					Commit = s.Queryable<Commit>().Single(c =>
						c.Revision == revision
					),
					Tag = tag
				});
				s.SubmitChanges();
			}
		}
		public void Truncate(int numberOfRevisionsToKeep)
		{
			Truncate(scmData.RevisionByNumber(numberOfRevisionsToKeep));
		}
		public void Truncate(string lastRevisionToKeep)
		{
			using (ConsoleTimeLogger.Start("truncating time"))
			using (var s = data.OpenSession())
			{
				var selectionDSL = s.SelectionDSL();

				var addedCommits = selectionDSL.Commits().AfterRevision(lastRevisionToKeep);
				var addedBugFixes = addedCommits.BugFixes().InCommits();
				var addedModifications = addedCommits.Modifications().InCommits();
				var addedFiles = addedCommits.Files().AddedInCommits();
				var deletedFiles = addedCommits.Files().DeletedInCommits();
				var addedCodeBlocks = addedModifications.CodeBlocks().InModifications();

				foreach (var codeBlock in addedCodeBlocks)
				{
					s.Delete(codeBlock);
				}
				foreach (var modification in addedModifications)
				{
					s.Delete(modification);
				}
				foreach (var file in addedFiles)
				{
					s.Delete(file);
				}
				foreach (var file in deletedFiles)
				{
					file.DeletedInCommit = null;
				}
				foreach (var bugFix in addedBugFixes)
				{
					s.Delete(bugFix);
				}
				foreach (var commit in addedCommits)
				{
					s.Delete(commit);
				}

				s.SubmitChanges();
			}
		}
		public void MapReleases(IDictionary<string,string> releases)
		{
			using (ConsoleTimeLogger.Start("releases mapping time"))
			using (var s = data.OpenSession())
			{
				foreach (var release in releases)
				{
					s.MappingDSL()
						.Commit(release.Key).IsRelease(release.Value);
				}
				
				s.SubmitChanges();
			}
		}
		public void MapSyntheticReleases(int count)
		{
			MapSyntheticReleases(count, 0.9);
		}
		public void MapSyntheticReleases(int count, double stabilizationProbability)
		{
			Dictionary<string,string> releases = new Dictionary<string,string>();

			using (var s = data.OpenSession())
			{
				double stabilizationPeriod = s.SelectionDSL().BugFixes()
					.CalculateStabilizationPeriod(stabilizationProbability);
				
				DateTime from = s.Queryable<Commit>().Single(c => c.Revision == s.FirstRevision()).Date;
				DateTime to = s.Queryable<Commit>().Single(c => c.Revision == s.LastRevision()).Date.AddDays(- stabilizationPeriod);
				
				int duration = (to - from).Days;
				int delta = duration / count;
				DateTime releaseDate = from;
				
				for (int i = 1; i <= count; i++)
				{
					releaseDate = releaseDate.AddDays(delta);
					
					var q = s.Queryable<Commit>()
							.Where(x => x.Date <= releaseDate)
							.OrderByDescending(x => x.Date).ToList();
					
					releases.Add(
						s.Queryable<Commit>()
							.Where(x => x.Date <= releaseDate)
							.OrderByDescending(x => x.Date)
							.First().Revision,
						"Synthetic Release # " + i.ToString()
					);
				}
			}
			
			MapReleases(releases);
		}
		public void Check(int stopRevisionNumber)
		{
			Check(stopRevisionNumber, null, false);
		}
		public void Check(int stopRevisionNumber, string path, bool automaticallyFixDiffErrors)
		{
			Check(
				scmData.RevisionByNumber(stopRevisionNumber),
				path,
				automaticallyFixDiffErrors
			);
		}
		public void Check(string stopRevision, string path, bool automaticallyFixDiffErrors)
		{
			pathFilter = e => path == null ? e : e.PathIs(path);
			this.automaticallyFixDiffErrors = automaticallyFixDiffErrors;
			
			using (ConsoleTimeLogger.Start("checking time"))
			using (var s = data.OpenSession())
			{
				string testRevision = stopRevision;
				if (s.Queryable<Commit>().SingleOrDefault(c => c.Revision == testRevision) == null)
				{
					Console.WriteLine("Could not find revision {0}.", testRevision);
					return;
				}

				CheckEmptyCodeBlocks(s, testRevision);
				CheckCodeSizeForDeletedFiles(s, testRevision);
				CheckTargetsForCodeBlocks(s, testRevision);
				CheckDeletedCodeBlocksVsAdded(s, testRevision);
				CheckLinesContent(s, scmDataWithoutCache, testRevision);
				if (automaticallyFixDiffErrors)
				{
					s.SubmitChanges();
				}
			}
		}

		private void Map(MappingController mapping)
		{
			using (ConsoleTimeLogger.Start("mapping time"))
			{
				mapping.OnRevisionMapping += (r, n) => Console.WriteLine(
					"mapping of revision {0}{1}",
					r,
					r != n ? string.Format(" ({0})", n) : ""
				);

				mapping.Map(data);
			}
		}
		private void CheckEmptyCodeBlocks(IRepository repository, string testRevision)
		{
			foreach (var zeroCodeBlock in
				from cb in repository.Queryable<CodeBlock>()
				join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
				join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
				join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
				let testRevisionNumber = repository.Queryable<Commit>()
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
		private void CheckTargetsForCodeBlocks(IRepository repository, string testRevision)
		{
			foreach (var codeBlockWithWrongTarget in
				from cb in repository.Queryable<CodeBlock>()
				join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
				join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
				join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
				let testRevisionNumber = repository.Queryable<Commit>()
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
		private void CheckCodeSizeForDeletedFiles(IRepository repository, string testRevision)
		{
			foreach (var codeSizeForDeletedFile in
				(
					from cb in repository.Queryable<CodeBlock>()
					join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
					join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
					join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
					let testRevisionNumber = repository.Queryable<Commit>()
						.Single(x => x.Revision == testRevision)
						.OrderedNumber
					where
						c.OrderedNumber <= testRevisionNumber
						&&
						f.DeletedInCommitID != null
						&&
						repository.Queryable<Commit>()
							.Single(x => x.ID == f.DeletedInCommitID)
							.OrderedNumber <= testRevisionNumber
					group cb by f into g
					select new
					{
						Path = g.Key.Path,
						DeletedInRevision = repository.Queryable<Commit>()
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
		private void CheckDeletedCodeBlocksVsAdded(IRepository repository, string testRevision)
		{
			foreach (var codeBlockWithWrongTarget in
				(
					from cb in repository.Queryable<CodeBlock>()
					join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
					join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
					join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
					let testRevisionNumber = repository.Queryable<Commit>()
						.Single(x => x.Revision == testRevision)
						.OrderedNumber
					let addedCodeID = cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID
					where
						c.OrderedNumber <= testRevisionNumber
					group cb.Size by addedCodeID into g
					select new
					{
						CodeSize = g.Sum(),
						Path = repository.Queryable<ProjectFile>()
							.Single(f => f.ID == repository.Queryable<Modification>()
								.Single(m => m.ID == repository.Queryable<CodeBlock>()
									.Single(cb => cb.ID == g.Key).ModificationID
								).FileID
							).Path,
						AddedCodeSize = repository.Queryable<CodeBlock>()
							.Single(cb => cb.ID == g.Key)
							.Size,
						Revision = repository.Queryable<Commit>()
							.Single(c => c.ID == repository.Queryable<Modification>()
								.Single(m => m.ID == repository.Queryable<CodeBlock>()
									.Single(cb => cb.ID == g.Key).ModificationID
								).CommitID
							).Revision
					}
				).Where(x => x.CodeSize < 0)
			)
			{
				Console.WriteLine("There are too many deleted code blocks for code block in revision {0} for file {1} for code with size {2}. Code size {3} should be greater or equal to 0",
					codeBlockWithWrongTarget.Revision,
					codeBlockWithWrongTarget.Path,
					codeBlockWithWrongTarget.AddedCodeSize,
					codeBlockWithWrongTarget.CodeSize
				);
			}
		}
		private void CheckLinesContent(IRepository repository, IScmData scmData, string testRevision)
		{
			var existentFiles = repository.SelectionDSL()
				.Files()
					.Reselect(pathFilter)
					.ExistInRevision(testRevision);

			foreach (var existentFile in existentFiles)
			{
				CheckLinesContent(repository, scmData, testRevision, existentFile, false);
			}
		}
		private bool CheckLinesContent(IRepository repository, IScmData scmData, string testRevision, ProjectFile file, bool resultOnly)
		{
			IBlame fileBlame = null;
			try
			{
				fileBlame = scmData.Blame(testRevision, file.Path);
			}
			catch
			{
			}
			if (fileBlame == null)
			{
				if (! resultOnly)
				{
					Console.WriteLine("File {0} does not exist.", file.Path);
				}
				return false;
			}
			
			double currentLOC = repository.SelectionDSL()
					.Commits().TillRevision(testRevision)
					.Files().IdIs(file.ID)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications()
					.CalculateLOC();

			bool correct = currentLOC == fileBlame.Count;
			
			if (! correct)
			{
				if (! resultOnly)
				{
					Console.WriteLine("Incorrect number of lines in file {0}. {1} should be {2}",
						file.Path, currentLOC, fileBlame.Count
					);
				}
				else
				{
					return false;
				}
			}

			SmartDictionary<string, int> linesByRevision = new SmartDictionary<string, int>(x => 0);
			foreach (var line in fileBlame)
			{
				linesByRevision[line.Value]++;
			}

			var codeBySourceRevision =
			(
				from f in repository.Queryable<ProjectFile>()
				join m in repository.Queryable<Modification>() on f.ID equals m.FileID
				join cb in repository.Queryable<CodeBlock>() on m.ID equals cb.ModificationID
				join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
				let addedCodeBlock = repository.Queryable<CodeBlock>()
					.Single(x => x.ID == (cb.Size < 0 ? cb.TargetCodeBlockID : cb.ID))
				let codeAddedInitiallyInRevision = repository.Queryable<Commit>()
					.Single(x => x.ID == addedCodeBlock.AddedInitiallyInCommitID)
					.Revision
				let testRevisionNumber = repository.Queryable<Commit>()
					.Single(x => x.Revision == testRevision)
					.OrderedNumber
				where
					f.ID == file.ID
					&&
					c.OrderedNumber <= testRevisionNumber
				group cb.Size by codeAddedInitiallyInRevision into g
				select new
				{
					FromRevision = g.Key,
					CodeSize = g.Sum()
				}
			).Where(x => x.CodeSize != 0).ToList();
			
			var errorCode =
				(
					from codeFromRevision in codeBySourceRevision
					where
						codeFromRevision.CodeSize != linesByRevision[codeFromRevision.FromRevision]
					select new
					{
						SourceRevision = codeFromRevision.FromRevision,
						CodeSize = codeFromRevision.CodeSize,
						RealCodeSize = linesByRevision[codeFromRevision.FromRevision]
					}
				).ToList();

			correct =
				correct
				&&
				codeBySourceRevision.Count() == linesByRevision.Count
				&&
				errorCode.Count == 0;
			
			if (! resultOnly)
			{
				if (codeBySourceRevision.Count() != linesByRevision.Count)
				{
					Console.WriteLine("Number of revisions file {0} contains code from is incorrect. {1} should be {2}",
						file.Path, codeBySourceRevision.Count(), linesByRevision.Count
					);
				}
				foreach (var error in errorCode)
				{
					Console.WriteLine("Incorrect number of lines in file {0} from revision {1}. {2} should be {3}",
						file.Path,
						error.SourceRevision,
						error.CodeSize,
						error.RealCodeSize
					);
				}
				if ((! correct) && (errorCode.Count > 0))
				{
					string latestCodeRevision = repository.LastRevision(errorCode.Select(x => x.SourceRevision));

					var commitsFileTouchedIn = repository.SelectionDSL()
						.Files().IdIs(file.ID)
						.Commits().FromRevision(latestCodeRevision).TouchFiles()
						.OrderBy(c => c.OrderedNumber);
					
					foreach (var commit in commitsFileTouchedIn)
					{
						if (!CheckLinesContent(repository, scmData, commit.Revision, file, true))
						{
							Console.WriteLine("{0} - incorrectly mapped commit.", commit.Revision);
							if ((automaticallyFixDiffErrors) && (errorCode.Sum(x => x.CodeSize - x.RealCodeSize) == 0))
							{
								var incorrectDeleteCodeBlocks =
									from cb in repository.SelectionDSL()
										.Commits().RevisionIs(commit.Revision)
										.Files().PathIs(file.Path)
										.Modifications().InCommits().InFiles()
										.CodeBlocks().InModifications().Deleted()
									join tcb in repository.Queryable<CodeBlock>() on cb.TargetCodeBlockID equals tcb.ID
									join m in repository.Queryable<Modification>() on tcb.ModificationID equals m.ID
									join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
									where
										errorCode.Select(x => x.SourceRevision).Contains(c.Revision)
									select new
									{
										Code = cb,
										TargetRevision = c.Revision
									};
								
								foreach (var error in errorCode)
								{
									var incorrectDeleteCodeBlock = incorrectDeleteCodeBlocks.SingleOrDefault(x => x.TargetRevision == error.SourceRevision);
									var codeBlock = incorrectDeleteCodeBlock == null ? null : incorrectDeleteCodeBlock.Code;
									double difference = error.CodeSize - error.RealCodeSize;
									if (codeBlock == null)
									{
										codeBlock = new CodeBlock()
										{
											Size = 0,
											Modification = repository.SelectionDSL()
												.Commits().RevisionIs(commit.Revision)
												.Files().PathIs(file.Path)
												.Modifications().InCommits().InFiles().Single(),
										};
										repository.Add(codeBlock);
									}
									Console.WriteLine("Fix code block size for file {0} in revision {1}:", file.Path, commit.Revision);
									Console.Write("Was {0}", codeBlock.Size);
									codeBlock.Size -= difference;
									if (codeBlock.Size == 0)
									{
										repository.Delete(codeBlock);
									}
									else if ((codeBlock.Size > 0) && (codeBlock.AddedInitiallyInCommitID == null))
									{
										codeBlock.AddedInitiallyInCommit = commit;
									}
									else if ((codeBlock.Size < 0) && (codeBlock.TargetCodeBlockID == null))
									{
										codeBlock.TargetCodeBlock = repository.SelectionDSL()
											.Commits().RevisionIs(error.SourceRevision)
											.Files().PathIs(file.Path)
											.Modifications().InFiles()
											.CodeBlocks().InModifications().AddedInitiallyInCommits().Single();
									}
									Console.WriteLine(", now {0}", codeBlock.Size);
								}
							}
							break;
						}
					}					
				}
			}
			
			return correct;
		}
	}
}
