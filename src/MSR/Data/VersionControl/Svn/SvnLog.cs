/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace MSR.Data.VersionControl.Svn
{
	public class SvnLog : Log
	{
		private ISvnClient svn;
		private XElement logXml;
		
		public SvnLog(ISvnClient svn, string revision)
		{
			this.svn = svn;
			using (var log = svn.Log(revision))
			{
				logXml = XElement.Load(new StreamReader(log)).Element("logentry");
			}
			ParseCommitInfo();
		}
		public override IEnumerable<TouchedFile> TouchedFiles
		{
			get
			{
				if (touchedFiles == null)
				{
					ParseTouchedPathsInfo();
				}
				return touchedFiles;
			}
		}
		
		private void ParseCommitInfo()
		{
			Revision = logXml.Attribute("revision").Value;
			Author = (logXml.Element("author") != null) ? logXml.Element("author").Value : "";
			Date = DateTime.Parse(logXml.Element("date").Value).ToUniversalTime();
			Message = logXml.Element("msg").Value;
		}
		private void ParseTouchedPathsInfo()
		{
			touchedFiles = new List<TouchedFile>();
			foreach (var touchedPath in SvnTouchedPaths())
			{
				TouchPath(touchedPath);
			}
			touchedFiles.Sort((x, y) => string.CompareOrdinal(x.Path.ToLower(), y.Path.ToLower()));
		}
		private IEnumerable<SvnTouchedPath> SvnTouchedPaths()
		{
			List<SvnTouchedPath> svnTouchedPaths = new List<SvnTouchedPath>();

			XElement diffSumXml;
			using (var diffSum = svn.DiffSum(Revision))
			{
				diffSumXml = XElement.Load(new StreamReader(diffSum));
			}
			int repositoryPathLength = svn.RepositoryPath.Length;

			IEnumerable<string> replacedPaths = logXml.Descendants("path")
				.Where(x => x.Attribute("action").Value == "R")
				.Select(x => x.Value);

			foreach (var diffPath in diffSumXml.Descendants("path"))
			{
				var touchedPath = new SvnTouchedPath()
				{
					Path = diffPath.Value.Substring(repositoryPathLength),
					IsFile = diffPath.Attribute("kind").Value == "file",
					Action = ParsePathAction(diffPath.Attribute("item").Value)
				};

				if (touchedPath.Action == SvnTouchedPath.SvnTouchedPathAction.MODIFIED && replacedPaths.Contains(touchedPath.Path))
				{
					touchedPath.Action = SvnTouchedPath.SvnTouchedPathAction.REPLACED;
				}

				svnTouchedPaths.Add(touchedPath);
			}
			foreach (var logPath in logXml.Descendants("path")
				.Where(x => x.Attribute("copyfrom-path") != null)
				.OrderBy(x => x.Value)
			)
			{
				var touchedPath = svnTouchedPaths.Single(x =>
					x.Path == logPath.Value
				);
				if (! touchedPath.IsFile)
				{
					foreach (var copiedFile in svnTouchedPaths
						.Where(x => x.Path.StartsWith(logPath.Value + "/"))
					)
					{
						copiedFile.SourcePath = copiedFile.Path.Replace(logPath.Value, logPath.Attribute("copyfrom-path").Value);
						copiedFile.SourceRevision = logPath.Attribute("copyfrom-rev").Value;
					}
				}
				else
				{
					touchedPath.SourcePath = touchedPath.Path.Replace(logPath.Value, logPath.Attribute("copyfrom-path").Value);
					touchedPath.SourceRevision = logPath.Attribute("copyfrom-rev").Value;
				}
			}
			
			return svnTouchedPaths;
		}
		private void TouchPath(SvnTouchedPath touchedPath)
		{
			if (touchedPath.IsFile)
			{
				switch (touchedPath.Action)
				{
					case SvnTouchedPath.SvnTouchedPathAction.MODIFIED:
						TouchFile(TouchedFile.TouchedFileAction.MODIFIED, touchedPath.Path);
						break;
					case SvnTouchedPath.SvnTouchedPathAction.ADDED:
						TouchFile(TouchedFile.TouchedFileAction.ADDED, touchedPath.Path, touchedPath.SourcePath, touchedPath.SourceRevision);
						break;
					case SvnTouchedPath.SvnTouchedPathAction.DELETED:
						TouchFile(TouchedFile.TouchedFileAction.DELETED, touchedPath.Path);
						break;
					case SvnTouchedPath.SvnTouchedPathAction.REPLACED:
						TouchFile(TouchedFile.TouchedFileAction.DELETED, touchedPath.Path);
						TouchFile(TouchedFile.TouchedFileAction.ADDED, touchedPath.Path, touchedPath.SourcePath, touchedPath.SourceRevision);
						break;
					default:
						break;
				}
			}
			else if (touchedPath.Action == SvnTouchedPath.SvnTouchedPathAction.DELETED)
			{
				foreach (var deletedFile in PathList(touchedPath.Path, PreviousRevision()))
				{
					TouchFile(TouchedFile.TouchedFileAction.DELETED, deletedFile);
				}
			}
		}
		private void TouchFile(TouchedFile.TouchedFileAction action, string path)
		{
			TouchFile(action, path, null, null);
		}
		private void TouchFile(TouchedFile.TouchedFileAction action, string path, string sourcePath, string sourceRevision)
		{
			touchedFiles.Add(new TouchedFile()
			{
				Action = action,
				Path = path,
				SourcePath = sourcePath,
				SourceRevision = sourceRevision
			});
		}
		private SvnTouchedPath.SvnTouchedPathAction ParsePathAction(string action)
		{
			switch (action.Substring(0,1).ToUpper())
			{
				case "M": return SvnTouchedPath.SvnTouchedPathAction.MODIFIED;
				case "A": return SvnTouchedPath.SvnTouchedPathAction.ADDED;
				case "D": return SvnTouchedPath.SvnTouchedPathAction.DELETED;
				case "R": return SvnTouchedPath.SvnTouchedPathAction.REPLACED;
				case "N": return SvnTouchedPath.SvnTouchedPathAction.NONE;
			}
			throw new ApplicationException(string.Format("{0} - is invalid path action", action));
		}
		private IEnumerable<string> PathList(string path, string revision)
		{
			List<string> paths = new List<string>();
			
			using (var list = svn.List(revision, path))
			using (TextReader reader = new StreamReader(list))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.EndsWith("/"))
					{
						paths.AddRange(PathList(path + "/" + line.Remove(line.Length - 1), revision));
					}
					else
					{
						paths.Add(path + "/" + line);
					}
				}
			}
			
			return paths;
		}
		private string PreviousRevision()
		{
			return (Convert.ToInt32(Revision) - 1).ToString();
		}
	}
}
