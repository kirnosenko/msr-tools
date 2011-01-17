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
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.Mapping
{
	public class CodeBlockMapper : EntityMapper<CodeBlock,ModificationMappingExpression,CodeBlockMappingExpression>
	{
		public CodeBlockMapper(IScmData scmData)
			: base(scmData)
		{
		}
		public override IEnumerable<CodeBlockMappingExpression> Map(ModificationMappingExpression expression)
		{
			List<CodeBlockMappingExpression> codeBlockExpressions = new List<CodeBlockMappingExpression>();
			string revision = expression.CurrentEntity<Commit>().Revision;
			ProjectFile file = expression.CurrentEntity<ProjectFile>();
			bool fileIsNew = (file.AddedInCommit != null) && (file.AddedInCommit.Revision == revision);
			bool fileCopied = fileIsNew && (file.SourceFile != null);
			bool fileDeleted = file.DeletedInCommit != null;
			
			if (fileCopied)
			{
				IDiff diff = scmData.Diff(file.Path, revision, file.SourceFile.Path, file.SourceCommit.Revision);
				int addedLines = diff.AddedLines.Count();
				int removedLines = diff.RemovedLines.Count();
				
				if (removedLines == 0)
				{
					codeBlockExpressions.Add(
						expression.CopyCode()
					);
					if (addedLines > 0)
					{
						codeBlockExpressions.Add(
							expression.Code(addedLines)
						);
					}
				}
				else
				{
					var blame = scmData.Blame(revision, file.Path);
					foreach (var lines in LinesByRevisionTheyWereAddedIn(
							blame.Keys, blame
						)
					)
					{
						codeBlockExpressions.Add(
							expression.Code(lines.Value.Count())
						);
						if (lines.Key != revision)
						{
							codeBlockExpressions.Last().CopiedFrom(lines.Key);
						}
					}
				}
			}
			else if (fileDeleted)
			{
				codeBlockExpressions.Add(
					expression.DeleteCode()
				);
			}
			else
			{
				IDiff diff = scmData.Diff(revision, file.Path);
				int addedLines = diff.AddedLines.Count();
				int removedLines = diff.RemovedLines.Count();

				if (addedLines > 0)
				{
					codeBlockExpressions.Add(
						expression.Code(addedLines)
					);
				}
				if (removedLines > 0)
				{
					foreach (var lines in LinesByRevisionTheyWereAddedIn(
							diff.RemovedLines, scmData.Blame(expression.LastRevision(), file.Path)
						)
					)
					{
						codeBlockExpressions.Add(
							expression.Code(-lines.Value.Count())
						);
						codeBlockExpressions.Last().ForCodeAddedInitiallyInRevision(lines.Key);
					}
				}
			}
			
			return codeBlockExpressions;
		}
		private IDictionary<string, IEnumerable<int>> LinesByRevisionTheyWereAddedIn(
			IEnumerable<int> lines,
			IBlame blame
		)
		{
			SmartDictionary<string, IEnumerable<int>> result =
				new SmartDictionary<string, IEnumerable<int>>((x) => new List<int>());
			foreach (int line in lines)
			{
				(result[blame[line]] as List<int>).Add(line);
			}
			return result;
		}
	}
}
