/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;

namespace MSR.Tools.StatGenerator
{
	public interface IStatPageBuilder
	{
		IDictionary<string,object> BuildData(IRepository repository);
		string PageName { get; }
		string PageTemplate { get; }
		string TargetDir { get; set; }
	}
}
