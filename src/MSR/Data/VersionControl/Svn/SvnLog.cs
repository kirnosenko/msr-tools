/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
		private XElement logXml;
		private Func<Stream> diffSum;
		private string repositoryPath;
		private bool touchedPathsInfoParsed = false;
		
		public SvnLog(Stream log, Func<Stream> diffSum, string repositoryPath)
		{
			this.logXml = XElement.Load(new StreamReader(log)).Element("logentry");
			this.diffSum = diffSum;
			this.repositoryPath = repositoryPath;
			ParseCommitInfo();
		}
		public override IEnumerable<TouchedPath> TouchedPaths
		{
			get
			{
				if (! touchedPathsInfoParsed)
				{
					ParseTouchedPathsInfo();
					touchedPathsInfoParsed = true;
				}
				return touchedPaths;
			}
		}
		
		private void ParseCommitInfo()
		{
			Revision = logXml.Attribute("revision").Value;
			Author = (logXml.Element("author") != null) ? logXml.Element("author").Value : "";
			Date = DateTime.Parse(logXml.Element("date").Value, CultureInfo.InvariantCulture);
			Message = logXml.Element("msg").Value;
		}
		private void ParseTouchedPathsInfo()
		{
			XElement diffSumXml = XElement.Load(new StreamReader(diffSum()));
			int repositoryPathLength = repositoryPath.Length;

			TouchedPathSvnAction action;
			string path;
			bool isFile;
			IEnumerable<string> replacedPaths = logXml.Descendants("path")
				.Where(x => x.Attribute("action").Value == "R")
				.Select(x => x.Value);
			
			foreach (var diffPath in diffSumXml.Descendants("path"))
			{
				path = diffPath.Value.Substring(repositoryPathLength);
				isFile = diffPath.Attribute("kind").Value == "file";
				action = ParsePathAction(diffPath.Attribute("item").Value);
				if (action == TouchedPathSvnAction.MODIFIED && replacedPaths.Contains(path))
				{
					action = TouchedPathSvnAction.REPLACED;
				}
				
				switch (action)
				{
					case TouchedPathSvnAction.MODIFIED:
						TouchPath(TouchedPath.TouchedPathAction.MODIFIED, path, isFile);
						break;
					case TouchedPathSvnAction.ADDED:
						TouchPath(TouchedPath.TouchedPathAction.ADDED, path, isFile);
						break;
					case TouchedPathSvnAction.DELETED:
						TouchPath(TouchedPath.TouchedPathAction.DELETED, path, isFile);
						break;
					case TouchedPathSvnAction.REPLACED:
						TouchPath(TouchedPath.TouchedPathAction.DELETED, path, isFile);
						TouchPath(TouchedPath.TouchedPathAction.ADDED, path, isFile);
						break;
					default:
						break;
				}
			}
			foreach (var logPath in logXml.Descendants("path")
				.Where(x => x.Attribute("copyfrom-path") != null)
				.OrderBy(x => x.Value)
			)
			{
				var touchedPath = touchedPaths.SingleOrDefault(x =>
					x.Action == TouchedPath.TouchedPathAction.ADDED
					&&
					x.Path == logPath.Value
				);
				if (! touchedPath.IsFile)
				{
					foreach (var copiedFile in touchedPaths
						.Where(x => x.Action == TouchedPath.TouchedPathAction.ADDED && x.Path.StartsWith(logPath.Value + "/"))
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

			touchedPaths.Sort((x, y) => string.CompareOrdinal(x.Path.ToLower(), y.Path.ToLower()));
		}
		private void TouchPath(TouchedPath.TouchedPathAction action, string path, bool isFile)
		{
			touchedPaths.Add(new TouchedPath()
			{
				Action = action,
				Path = path,
				IsFile = isFile
			});
		}
		private TouchedPathSvnAction ParsePathAction(string action)
		{
			switch (action.Substring(0,1).ToUpper())
			{
				case "M": return TouchedPathSvnAction.MODIFIED;
				case "A": return TouchedPathSvnAction.ADDED;
				case "D": return TouchedPathSvnAction.DELETED;
				case "R": return TouchedPathSvnAction.REPLACED;
				case "N": return TouchedPathSvnAction.NONE;
			}
			throw new ApplicationException(string.Format("{0} - is invalid path action", action));
		}
	}
}
