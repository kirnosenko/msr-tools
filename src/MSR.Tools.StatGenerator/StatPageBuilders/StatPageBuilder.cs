/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;

namespace MSR.Tools.StatGenerator.StatPageBuilders
{
	public abstract class StatPageBuilder : IStatPageBuilder
	{
		public abstract IDictionary<string,object> BuildData(IRepository repository);
		public string PageName
		{
			get; protected set;
		}
		public string PageTemplate
		{
			get; protected set;
		}
		public string TargetDir
		{
			get; set;
		}
	}
}
