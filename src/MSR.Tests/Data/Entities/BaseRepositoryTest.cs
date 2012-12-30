/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities
{
	public class BaseRepositoryTest : ISession
	{
		protected RepositoryMappingExpression mappingDSL;
		protected RepositorySelectionExpression selectionDSL;
		
		protected InMemoryDataStore data;
		protected ISession session;
		
		[SetUp]
		public virtual void SetUp()
		{
			data = new InMemoryDataStore();
			session = data.OpenSession();
			mappingDSL = session.MappingDSL();
			selectionDSL = session.SelectionDSL();
		}
		public void Dispose()
		{
		}
		public void Add<T>(T entity) where T : class
		{
			session.Add(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			session.AddRange(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			session.Delete(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return session.Queryable<T>();
		}
		public void SubmitChanges()
		{
			session.SubmitChanges();
		}
	}
}
