/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data
{
	public interface IDataStore
	{
		ISession OpenSession();
	}
}
