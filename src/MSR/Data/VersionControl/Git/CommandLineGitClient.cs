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
			Command = "git";
			RepositoryPath = repositoryPath;
		}
		public string Command
		{
			get; set;
		}
		public Stream RevList()
		{
			return RunCommand("rev-list master --topo-order --reverse");
		}
		public Stream Log(string revision)
		{
			return RunCommand("log -n 1 --format=format:%H%n%cn%n%ci%n%s --name-status -C {0}", revision);
		}
		public Stream Diff(string revision, string path)
		{
			return RunCommand("diff-tree -p --root {0} -- {1}", revision, path);
		}
		public Stream Blame(string revision, string path)
		{
			return RunCommand("blame -l -s --root {0} -- {1}", revision, path);
		}
		
		public string RepositoryPath
		{
			get; private set;
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
