/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class SkipPathByExtension : SelectPathByExtension
	{
		public SkipPathByExtension(string[] extensionsToSkip)
			: base(extensionsToSkip)
		{
		}
		protected override bool SelectMatchedPath()
		{
			return false;
		}
	}
}
