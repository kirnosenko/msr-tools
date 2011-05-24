/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
			Branch = "master";
		}
		public Stream RevList()
		{
			return RunCommand(
				"rev-list {0} --topo-order --reverse",
				Branch
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
				revision, ToGitPath(path)
			);
		}
		public Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision)
		{
			return RunCommand(
				"diff-tree -p -C --root {0} -- {1} {2} -- {3}",
				newRevision, ToGitPath(newPath), oldRevision, ToGitPath(oldPath)
			);
		}
		public Stream Blame(string revision, string path)
		{
			return RunCommand(
				"blame -l -s --root --incremental {0} -- {1}",
				revision, ToGitPath(path)
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
		public string Branch
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
		/// <summary>
		/// Remove leading slash to get git path.
		/// </summary>
		/// <param name="path">Internal path with leading slash.</param>
		/// <returns>Git path.</returns>
		private string ToGitPath(string path)
		{
			return path.Remove(0,1);
		}
	}
}
