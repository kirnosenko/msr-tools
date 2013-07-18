/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
 */

using System;
using System.IO;

namespace MSR.Data.VersionControl.Hg
{
    public class CommandLineHgClient : IHgClient
    {
        public CommandLineHgClient(string repositoryPath)
        {
            RepositoryPath = repositoryPath;
            HgCommand = "hg";
            Branch = "default";
        }
        public Stream RevList()                                                                         // [+]
        { return RunCommand("log --template \"{node}\\n\" -r 0:tip"); }
        public Stream Log(string revision)                                                              // [+]
        { return RunCommand("log --template \"{node}\\n{author|person}\\n{date|isodatesec}\\n{desc}\\n{file_mods % 'M\\t{file}\\n'}\\n{file_adds % 'A\\t{file}\\n'}\\n{file_dels % 'D\\t{file}\\n'}\\nC\\t{join(file_copies, '\\nC\\t')}\\nEOF\" -r " + revision); }
        public Stream Diff(string revision, string path)                                                // [+-]
        { return RunCommand("diff -gpc " + revision + " " + RepositoryPath + path + " -R"); }
        public Stream Diff(string newPath, string newRevision, string oldPath, string oldRevision)      // [+-]
        { return RunCommand("diff -gp -r " + oldRevision + " -r " + newRevision + " " + RepositoryPath + newPath + " -R"); }
        public Stream Blame(string revision, string path)                                               // [+-]
        { return RunCommand("blame -r " + revision + " -c --debug " + RepositoryPath + path + " -R"); }
        
        public string RepositoryPath
        { get; private set; }
        public string HgCommand
        { get; set; }
        public string Branch
        { get; set; }
        private Stream RunCommand(string cmd)
        {
            MemoryStream buf = new MemoryStream();

            Shell.RunAndWaitForExit(HgCommand, cmd + " " + RepositoryPath, buf);
            buf.Seek(0, SeekOrigin.Begin);
            return buf;
        }
        /// <summary>
        /// Remove leading slash to get hg path.
        /// </summary>
        /// <param name="path">Internal path with leading slash.</param>
        /// <returns>Hg path.</returns>
        private string ToHgPath(string Path) { return Path.Remove(0, 1); }
    }
}
