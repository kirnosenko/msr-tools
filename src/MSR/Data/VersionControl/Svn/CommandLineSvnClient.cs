/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Data.VersionControl.Svn
{
	public class CommandLineSvnClient : ISvnClient
	{
		public CommandLineSvnClient(string repositoryPath)
		{
			SvnCommand = "svn";
			RepositoryPath = repositoryPath;
			UseMergeHistory = true;
		}
		public Stream Info()
		{
			return RunSvnCommand("info \"{0}\"", RepositoryPath);
		}
		public Stream Log(string revision)
		{
			return RunSvnCommand("log \"{0}\"@{1} -c {1} -v --xml", RepositoryPath, revision);
		}
		public Stream DiffSum(string revision)
		{
			return RunSvnCommand("diff \"{0}\"@{1} -c {1} --summarize --xml", RepositoryPath, revision);
		}
		public Stream List(string revision, string dirPath)
		{
			return RunSvnCommand("list \"{0}{2}\"@{1}", RepositoryPath, revision, dirPath);
		}
		public Stream Diff(string revision)
		{
			return RunSvnCommand("diff \"{0}\"@{1} -c {1}", RepositoryPath, revision);
		}
		public Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			return RunSvnCommand(
				"diff \"{0}{3}\"@{4} \"{0}{1}\"@{2}",
				RepositoryPath, newPath, newRevision, oldPath, oldRevision
			);
		}
		public Stream Blame(string revision, string filePath)
		{
			return RunSvnCommand(
				"blame \"{0}{2}\"@{1}{3}",
				RepositoryPath, revision, filePath, UseMergeHistory ? " -g" : "");
		}
		public string SvnCommand
		{
			get; set;
		}
		public string RepositoryPath
		{
			get; private set;
		}
		public bool UseMergeHistory
		{
			get; set;
		}
		
		private Stream RunSvnCommand(string cmd, params object[] objects)
		{
			MemoryStream buf = new MemoryStream();
			
			Shell.RunAndWaitForExit(
				SvnCommand,
				string.Format(cmd, objects),
				buf
			);

			buf.Seek(0, SeekOrigin.Begin);
			return buf;
		}
	}
}
