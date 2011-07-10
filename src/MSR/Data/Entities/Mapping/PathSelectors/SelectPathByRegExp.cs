/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public abstract class SelectPathByRegExp : IPathSelector
	{
		private Regex regExp;
		
		public SelectPathByRegExp()
		{
		}
		public SelectPathByRegExp(string regExpText)
		{
			RegExp = regExpText;
		}
		public bool InSelection(string path)
		{
			Match m = regExp.Match(path);
			return
				(m.Success && SelectMatchedPath())
				||
				(!m.Success && !SelectMatchedPath());
		}
		public string RegExp
		{
			set
			{
				regExp = new Regex(value, RegexOptions.Compiled);
			}
		}
		protected abstract bool SelectMatchedPath();
	}
}
