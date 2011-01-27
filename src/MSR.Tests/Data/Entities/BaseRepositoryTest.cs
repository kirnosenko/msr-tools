/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
	public class BaseRepositoryTest : IRepositoryResolver
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
		public IRepository<T> Repository<T>() where T : class
		{
			return session.Repository<T>();
		}
		public void Submit()
		{
			session.SubmitChanges();
		}
	}
}
