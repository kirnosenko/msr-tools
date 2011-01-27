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
	public interface IRepositoryMappingExpression : IRepositoryResolver
	{
		IRepositoryMappingExpression Submit();
		T CurrentEntity<T>() where T : class;
		string Revision { get; }
	}

	public class RepositoryMappingExpression : IRepositoryMappingExpression
	{
		private ISession session;

		public RepositoryMappingExpression(ISession session)
		{
			this.session = session;
		}
		public IRepository<T> Repository<T>() where T : class
		{
			return session.Repository<T>();
		}
		public IRepositoryMappingExpression Submit()
		{
			session.SubmitChanges();
			return session.MappingDSL();
		}
		public T CurrentEntity<T>() where T : class
		{
			return default(T);
		}
		public string Revision
		{
			get; set;
		}
	}
}
