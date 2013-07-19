/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data
{
	public interface IRepository
	{
		void Add<T>(T entity) where T : class;
		void AddRange<T>(IEnumerable<T> entities) where T : class;
		void Delete<T>(T entity) where T : class;
		IQueryable<T> Queryable<T>() where T : class;
	}
}
