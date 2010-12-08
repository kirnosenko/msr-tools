/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data.VersionControl
{
	public interface IBlame : IDictionary<int,string>
	{
	}
}
