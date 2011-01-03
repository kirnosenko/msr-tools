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
	public interface IStatBuilder
	{
		void AddData(IDataStore data, VelocityContext context);
	}
}
