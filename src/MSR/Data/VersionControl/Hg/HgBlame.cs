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
    /// <summary>
    /// Keeps the revision last modified each line.
    /// </summary>
    public class HgBlame : Dictionary<int, string>, IBlame
    {
        /// <summary>
        /// Parse blame stream.
        /// </summary>
        /// <param name="blameData">Hg annotate in incremental format.</param>
        /// <returns>Dictionary of line numbers (from 1) with revisions.</returns>
        public static HgBlame Parse(Stream blameData)
        {
            TextReader reader = new StreamReader(blameData);
            HgBlame blame = new HgBlame();
            string line;
            int index = 1;
            while ((line = reader.ReadLine()) != null)
            {
                string[] subs = line.Split(':');
                if (subs[0].Length == 40)
                {
                    blame.Add(index, subs[0]);
                    index++;
                }
            }

            return blame;
        }
    }
}
