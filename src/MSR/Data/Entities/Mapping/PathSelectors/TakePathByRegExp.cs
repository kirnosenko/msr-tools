/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class TakePathByRegExp : SelectPathByRegExp
	{
		protected override bool SelectMatchedPath()
		{
			return true;
		}
	}
}
