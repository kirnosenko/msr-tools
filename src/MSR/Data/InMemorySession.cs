/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Data
{
	public class InMemorySession : ISession
	{
		private IRepositoryResolver repositories;
		private event Action OnSubmit;
		
		public InMemorySession(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public void Dispose()
		{
		}
		public IRepository<T> Repository<T>() where T : class
		{
			InMemoryRepository<T> repository = repositories.Repository<T>() as InMemoryRepository<T>;
			OnSubmit += repository.SubmitChanges;
			return repository;
		}
		public void SubmitChanges()
		{
			if (OnSubmit != null)
			{
				OnSubmit();
			}
		}
	}
}
