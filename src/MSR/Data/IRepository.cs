/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data
{
	public interface IRepository<T> : IQueryable<T>, IEnumerable<T>
	{
		void Add(T entity);
		void AddRange(IEnumerable<T> entities);
		void Delete(T entity);
	}
}
