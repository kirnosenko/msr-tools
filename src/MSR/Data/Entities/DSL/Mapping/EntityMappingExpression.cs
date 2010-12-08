/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
		public IRepository<T> Repository<T>() where T : class
		{
			return parentExp.Repository<T>();
		}
		public IRepositoryMappingExpression Submit()
		{
			return parentExp.Submit();
		}
		public void AddEntity()
		{
			Repository<E>().Add(entity);
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
