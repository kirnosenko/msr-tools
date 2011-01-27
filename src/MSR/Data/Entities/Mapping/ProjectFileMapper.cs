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
		private IEnumerable<IPathSelector> pathSelectors;
		
		public ProjectFileMapper(IScmData scmData)
			: this(scmData, new IPathSelector[] {})
		{
		}
		public ProjectFileMapper(IScmData scmData, IPathSelector[] pathSelectors)
			: base(scmData)
		{
			this.pathSelectors = pathSelectors;
		}
		public override IEnumerable<ProjectFileMappingExpression> Map(CommitMappingExpression expression)
		{
			List<ProjectFileMappingExpression> fileExpressions = new List<ProjectFileMappingExpression>();
			
			ILog log = scmData.Log(expression.CurrentEntity<Commit>().Revision);
			
			foreach (var touchedFile in log.TouchedFiles)
			{
				if (! ShouldProcessPath(touchedFile.Path))
				{
					continue;
				}
				
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
						touchedFile.SourceRevision = expression.LastRevision();
					}
					fileExp.CopiedFrom(touchedFile.SourcePath, touchedFile.SourceRevision);
				}
				fileExpressions.Add(
					fileExp
				);
			}
			
			return fileExpressions;
		}
		private bool ShouldProcessPath(string path)
		{
			foreach (var selector in pathSelectors)
			{
				if (! selector.InSelection(path))
				{
					return false;
				}
			}
			return true;
		}
		private IEnumerable<string> ExistentFilesInDir(IRepositoryResolver repositories, string path)
		{
			return repositories.SelectionDSL()
				.Files().Exist().InDirectory(path)
				.Select(x => x.Path);
		}
	}
}
