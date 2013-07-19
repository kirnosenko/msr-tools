/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities.DSL.Mapping
{
	public class EntityMappingExpression<E> : IRepositoryMappingExpression where E : class
	{
		protected E entity;
		private IRepositoryMappingExpression parentExp;
		
		public EntityMappingExpression(IRepositoryMappingExpression parentExp)
		{
			this.parentExp = parentExp;
		}
		public void Add<T>(T entity) where T : class
		{
			parentExp.Add(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			parentExp.AddRange(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			parentExp.Delete(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return parentExp.Queryable<T>();
		}
		public IRepositoryMappingExpression Submit()
		{
			return parentExp.Submit();
		}
		public void AddEntity()
		{
			Add(entity);
		}
		public virtual T CurrentEntity<T>() where T : class
		{
			if (typeof(T) == typeof(E))
			{
				return entity as T;
			}
			return parentExp.CurrentEntity<T>();
		}
		public string Revision
		{
			get { return parentExp.Revision; }
		}
	}
}
