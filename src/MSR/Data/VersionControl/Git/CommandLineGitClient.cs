/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Data.VersionControl.Git
{
	public class CommandLineGitClient : IGitClient
	{
		public CommandLineGitClient(string repositoryPath)
		{
			RepositoryPath = repositoryPath;
			Command = "git";
		}
		public Stream RevList()
		{
			return RunCommand(
				"rev-list master --topo-order --reverse"
			);
		}
		public Stream Log(string revision)
		{
			return RunCommand(
				"log -n 1 -C --format=format:%H%n%cn%n%ci%n%s --name-status {0}",
				revision
			);
		}
		public Stream Diff(string revision, string path)
		{
			return RunCommand(
				"diff-tree -p --root {0} -- {1}",
				revision, path
			);
		}
		public Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			return RunCommand(
				"diff-tree -p -C --root {0} -- {1} {2} -- {3}",
				newRevision, newPath, oldRevision, oldPath
			);
		}
		public Stream Blame(string revision, string path)
		{
			return RunCommand(
				"blame -l -s --root --incremental {0} -- {1}",
				revision, path
			);
		}
		
		public string RepositoryPath
		{
			get; private set;
		}
		public string Command
		{
			get; set;
		}
		private Stream RunCommand(string cmd, params object[] objects)
		{
			MemoryStream buf = new MemoryStream();

			Shell.RunAndWaitForExit(
				Command,
				string.Format("--git-dir={0} ", RepositoryPath) + string.Format(cmd, objects),
				buf
			);

			buf.Seek(0, SeekOrigin.Begin);
			return buf;
		}
	}
}
