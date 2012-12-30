/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.Mapping
{
	public class ProjectFileMapper : EntityMapper<ProjectFile,CommitMappingExpression,ProjectFileMappingExpression>
	{
		public ProjectFileMapper(IScmData scmData)
			: base(scmData)
		{
		}
		public override IEnumerable<ProjectFileMappingExpression> Map(CommitMappingExpression expression)
		{
			List<ProjectFileMappingExpression> fileExpressions = new List<ProjectFileMappingExpression>();
			
			string revision = expression.CurrentEntity<Commit>().Revision;
			ILog log = scmData.Log(revision);
			var touchedFiles = FilterTouchedFiles(log.TouchedFiles);
			
			foreach (var touchedFile in touchedFiles)
			{
				ProjectFileMappingExpression fileExp = null;
				
				switch (touchedFile.Action)
				{
					case TouchedFile.TouchedFileAction.MODIFIED:
						fileExp = expression.File(touchedFile.Path);
						break;
					case TouchedFile.TouchedFileAction.ADDED:
						fileExp = expression.AddFile(touchedFile.Path);
						break;
					case TouchedFile.TouchedFileAction.DELETED:
						fileExp = expression.File(touchedFile.Path);
						fileExp.Delete();
						break;
					default:
						break;
				}
				if (touchedFile.SourcePath != null)
				{
					if (touchedFile.SourceRevision == null)
					{
						touchedFile.SourceRevision = scmData.PreviousRevision(revision);
					}
					fileExp.CopiedFrom(touchedFile.SourcePath, touchedFile.SourceRevision);
				}
				fileExpressions.Add(
					fileExp
				);
			}
			
			return fileExpressions;
		}
		public IPathSelector[] PathSelectors
		{
			get; set;
		}
		private IEnumerable<TouchedFile> FilterTouchedFiles(IEnumerable<TouchedFile> touchedFiles)
		{
			if (PathSelectors != null)
			{
				foreach (var selector in PathSelectors)
				{
					touchedFiles = touchedFiles.Where(x => selector.InSelection(x.Path)).ToArray();
				}
			}
			
			return touchedFiles;
		}
	}
}
