/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace MSR.Data.Persistent
{
	public class PersistentRepository<T> : IRepository<T> where T : class
	{
		private IDataContext context;
		
		public PersistentRepository(IDataContext context)
		{
			this.context = context;
		}
		public void Add(T entity)
		{
			context.Add(entity);
		}
		public void AddRange(IEnumerable<T> entities)
		{
			context.AddRange(entities);
		}
		public void Delete(T entity)
		{
			context.Delete(entity);
		}

		#region IQueryable Members

		public Type ElementType
		{
			get { return context.Queryable<T>().ElementType; }
		}

		public System.Linq.Expressions.Expression Expression
		{
			get { return context.Queryable<T>().Expression; }
		}

		public IQueryProvider Provider
		{
			get { return context.Queryable<T>().Provider; }
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return context.Enumerable<T>().GetEnumerator();
		}

		#endregion
		
		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)context.Enumerable<T>()).GetEnumerator();
		}

		#endregion
	}
}
