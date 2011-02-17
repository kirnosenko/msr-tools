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
		private List<Func<IEnumerable<TouchedFile>,IRepositoryResolver,IEnumerable<TouchedFile>>> fileFilters =
			new List<Func<IEnumerable<TouchedFile>,IRepositoryResolver,IEnumerable<TouchedFile>>>();
		
		public ProjectFileMapper(IScmData scmData)
			: this(scmData, new IPathSelector[] {})
		{
		}
		public ProjectFileMapper(IScmData scmData, IPathSelector[] pathSelectors)
			: base(scmData)
		{
			if (pathSelectors.Length > 0)
			{
				fileFilters.Add((f,r) => f.Where(x =>
				{
					foreach (var selector in pathSelectors)
					{
						if (! selector.InSelection(x.Path))
						{
							return false;
						}
					}
					return true;
				}));
			}
		}
		public override IEnumerable<ProjectFileMappingExpression> Map(CommitMappingExpression expression)
		{
			List<ProjectFileMappingExpression> fileExpressions = new List<ProjectFileMappingExpression>();
			
			ILog log = scmData.Log(expression.CurrentEntity<Commit>().Revision);
			var touchedFiles = FilterTouchedFiles(log.TouchedFiles, expression);
			
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
		private IEnumerable<TouchedFile> FilterTouchedFiles(IEnumerable<TouchedFile> touchedFiles, IRepositoryResolver repositories)
		{
			var files = touchedFiles;
			foreach (var filter in fileFilters)
			{
				files = filter(files, repositories);
			}
			return files;
		}
	}
}
