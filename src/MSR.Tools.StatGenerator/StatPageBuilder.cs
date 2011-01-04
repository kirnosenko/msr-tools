/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using NVelocity;

using MSR.Data;

namespace MSR.Tools.StatGenerator
{
	public abstract class StatPageBuilder : IStatPageBuilder
	{
		public abstract void AddData(IDataStore data, VelocityContext context);
		public string PageName
		{
			get; set;
		}
		public string PageTemplate
		{
			get; set;
		}
	}
}
