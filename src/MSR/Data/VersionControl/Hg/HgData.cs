/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.VersionControl.Hg
{
    public class HgData : IScmData
    {
        private IHgClient hg;

        public List<string> revisions;

        public HgData(IHgClient hg)                                                                        //[+-]
		{
			this.hg = hg;
		}
        public ILog Log(string revision)                                                                    //[+-]
        {
            using (var log = hg.Log(revision))
            {
                return new HgLog(log);
            }
        }
        public IDiff Diff(string revision, string filePath)                                                 //[+-]
        {
            using (var diff = hg.Diff(revision, filePath))
            {
                return new FileUniDiff(diff);
            }
        }
        public IDiff Diff(string newPath, string newRevision, string oldPath, string oldRevision)           //[+-]
        {
            using (var diff = hg.Diff(newPath, newRevision, oldPath, oldRevision))
            {
                return new FileUniDiff(diff);
            }
        }
        public IBlame Blame(string revision, string filePath)                                               //[+-]
        {
            using (var blame = hg.Blame(revision, filePath))
            {
                return HgBlame.Parse(blame);
            }
        }
        public string RevisionByNumber(int revisionNumber)                                                  //[+-]
        {
            if (revisions == null)
            {
                GetAllRevisions();
            }
            if (revisionNumber - 1 < revisions.Count)
            {
                return revisions[revisionNumber - 1];
            }
            else
            {
                return null;
            }
        }
        public string NextRevision(string revision)                                                         //[+-]
        {
            if (revisions == null)
            {
                GetAllRevisions();
            }
            int revisionNumber = revisions.IndexOf(revision) + 1;
            return RevisionByNumber(revisionNumber + 1);
        }
        public string PreviousRevision(string revision)                                                     //[+-]
        {
            if (revisions == null)
            {
                GetAllRevisions();
            }
            int revisionNumber = revisions.IndexOf(revision) + 1;
            return RevisionByNumber(revisionNumber - 1);
        }
        private void GetAllRevisions()                                                                      //[+-]
        {
            revisions = new List<string>();

            using (var revlist = hg.RevList())
            {
                TextReader reader = new StreamReader(revlist);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    revisions.Add(line);
                }
            }
        }
    }
}
