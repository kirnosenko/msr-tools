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
		private DataContext context;
		
		public PersistentRepository(DataContext context)
		{
			this.context = context;
		}
		public void Add(T entity)
		{
			Table.InsertOnSubmit(entity);
		}
		public void AddRange(IEnumerable<T> entities)
		{
			Table.InsertAllOnSubmit(entities);
		}
		public void Delete(T entity)
		{
			Table.DeleteOnSubmit(entity);
		}

		#region IQueryable Members

		public Type ElementType
		{
			get { return ((IQueryable)Table).ElementType; }
		}

		public System.Linq.Expressions.Expression Expression
		{
			get { return ((IQueryable)Table).Expression; }
		}

		public IQueryProvider Provider
		{
			get { return ((IQueryable)Table).Provider; }
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return Table.GetEnumerator();
		}

		#endregion
		
		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Table).GetEnumerator();
		}

		#endregion
		
		private Table<T> Table
		{
			get { return context.GetTable<T>(); }
		}
	}
}
