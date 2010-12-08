/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
	public class ProjectFileMapper : EntityMapper<CommitMappingExpression, ProjectFileMappingExpression>
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
			
			foreach (var touchedFile in log.TouchedPaths.Where(f => f.IsFile))
			{
				if (! ShouldProcessPath(touchedFile.Path))
				{
					continue;
				}
				
				ProjectFileMappingExpression fileExp = null;
				
				switch (touchedFile.Action)
				{
					case TouchedPath.TouchedPathAction.MODIFIED:
						fileExp = expression.File(touchedFile.Path);
						break;
					case TouchedPath.TouchedPathAction.ADDED:
						fileExp = expression.AddFile(touchedFile.Path);
						break;
					case TouchedPath.TouchedPathAction.DELETED:
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
			foreach (var deletedDir in log.TouchedPaths
				.Where(p => !p.IsFile && p.Action == TouchedPath.TouchedPathAction.DELETED)
			)
			{
				foreach (var deletedFilePath in ExistentFilesInDir(expression, deletedDir.Path))
				{
					ProjectFileMappingExpression fileExp = null;
					
					fileExp = expression.File(deletedFilePath);
					fileExp.Delete();
					fileExpressions.Add(
						fileExp
					);
				}
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
			RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(repositories);
			
			return selectionDSL
				.Files().Exist().InDirectory(path)
				.Select(x => x.Path);
		}
	}
}
