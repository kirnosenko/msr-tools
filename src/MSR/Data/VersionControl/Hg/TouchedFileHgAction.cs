/*
 * MSR Tools - tools for mining software repositories
 * 
 * Graduate work by Artem Makayda
 */

using System;

namespace MSR.Data.VersionControl.Hg
{
    public enum TouchedFileHgAction
    {
        ADDED,
        MODIFIED,
        DELETED,
        COPIED
    }
}
