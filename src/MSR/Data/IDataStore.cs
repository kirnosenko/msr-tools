/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data
{
	public interface IDataStore
	{
		void CreateSchema(params Type[] tables);
		ISession OpenSession();
	}
}
