/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MSR.Data.Persistent
{
	public interface IDataContext : IDisposable
	{
		void CreateSchema(params Type[] tables);
		
		void Add<T>(T entity) where T : class;
		void AddRange<T>(IEnumerable<T> entities) where T : class;
		void Delete<T>(T entity) where T : class;
		IQueryable<T> Queryable<T>() where T : class;
		IEnumerable<T> Enumerable<T>() where T : class;
		
		void SubmitChanges();
		
		int CommandTimeout { get; set; }
		TextWriter Log { get; set; }
	}
}
