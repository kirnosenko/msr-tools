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
		IDictionary<string,object> BuildData(IRepositoryResolver repositories, string targetDir);
		string PageName { get; set; }
		string PageTemplate { get; set; }
	}
}
