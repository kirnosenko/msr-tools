/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSR.Data.VersionControl.Hg
{
    public class HgLog : Log
    {
        public HgLog(Stream log)
        {
            TextReader reader = new StreamReader(log);
            touchedFiles = new List<TouchedFile>();

            Revision = reader.ReadLine();
            Author = reader.ReadLine();
            Date = DateTime.Parse(reader.ReadLine()).ToUniversalTime();
            Message = reader.ReadLine();

            string line;
            string[] blocks;
            TouchedFileHgAction action;
            bool fl = false;
            while (!fl)
            {
                line = reader.ReadLine();
                if (line != "" && line != "EOF")
                {
                    blocks = line.Split('\t');
                    if (blocks[0].Equals("M") || blocks[0].Equals("A") || blocks[0].Equals("D") || blocks[0].Equals("C"))
                    {
                        action = ParsePathAction(blocks[0]);

                        switch (action)
                        {
                            case TouchedFileHgAction.MODIFIED:
                                TouchFile(TouchedFile.TouchedFileAction.MODIFIED, blocks[1]);
                                break;
                            case TouchedFileHgAction.ADDED:
                                TouchFile(TouchedFile.TouchedFileAction.ADDED, blocks[1]);
                                break;
                            case TouchedFileHgAction.DELETED:
                                TouchFile(TouchedFile.TouchedFileAction.DELETED, blocks[1]);
                                break;
                            /*case TouchedFileHgAction.RENAMED:
                                TouchFile(TouchedFile.TouchedFileAction.DELETED, blocks[1]);
                                TouchFile(TouchedFile.TouchedFileAction.ADDED, blocks[2], blocks[1]);
                                break;
                             */
                            case TouchedFileHgAction.COPIED:
                                if (blocks[1] != "")
                                {
                                    string[] copiedFiles = blocks[1].Split(' ');
                                    TouchFile(TouchedFile.TouchedFileAction.ADDED, copiedFiles[0], copiedFiles[1].Remove(0, 1).Remove(copiedFiles[1].Length - 2, 1));
                                }
                                break;
                            default:
                                break;
                        }
                        touchedFiles.Sort((x, y) => string.CompareOrdinal(x.Path.ToLower(), y.Path.ToLower()));
                    }
                }
                else if (line == "EOF")
                    fl = true;
            }
        }
        private void TouchFile(TouchedFile.TouchedFileAction action, string path)
        {
            TouchFile(action, path, null);
        }
        private void TouchFile(TouchedFile.TouchedFileAction action, string path, string sourcePath)
        {
            path = path.Replace("\"", "");
            if (sourcePath != null)
            {
                sourcePath = sourcePath.Replace("\"", "");
            }

            path = "/" + path;
            if (sourcePath != null)
            {
                sourcePath = "/" + sourcePath;
            }
            touchedFiles.Add(new TouchedFile()
            {
                Path = path,
                Action = action,
                SourcePath = sourcePath
            });
        }
        private TouchedFileHgAction ParsePathAction(string action)
        {
            switch (action.Substring(0, 1).ToUpper())
            {
                case "M": return TouchedFileHgAction.MODIFIED;
                case "A": return TouchedFileHgAction.ADDED;
                case "D": return TouchedFileHgAction.DELETED;
                //case "R": return TouchedFileHgAction.RENAMED;
                case "C": return TouchedFileHgAction.COPIED;
            }
            throw new ApplicationException(string.Format("{0} - is invalid path action", action));
        }
    }
}
