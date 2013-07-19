/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011-2012  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Models.Prediction
{
	public class PredictorContext : IRepository
	{
		private IRepository repository;
		private Dictionary<string,object> parameters = new Dictionary<string,object>();
		
		public PredictorContext(IRepository repository)
		{
			this.repository = repository;
		}
		public void Add<T>(T entity) where T : class
		{
			repository.Add(entity);
		}
		public void AddRange<T>(IEnumerable<T> entities) where T : class
		{
			repository.AddRange(entities);
		}
		public void Delete<T>(T entity) where T : class
		{
			repository.Delete(entity);
		}
		public IQueryable<T> Queryable<T>() where T : class
		{
			return repository.Queryable<T>();
		}
		public PredictorContext SetValue(string key, object value)
		{
			if (parameters.ContainsKey(key))
			{
				parameters[key] = value;
			}
			else
			{
				parameters.Add(key, value);
			}
			return this;
		}
		public T GetValue<T>(string key)
		{
			return (T)parameters[key];
		}
		public void Clear()
		{
			parameters.Clear();
		}

		public PredictorContext SetCommits(Func<CommitSelectionExpression,CommitSelectionExpression> selector)
		{
			SetValue("commits", (Func<CommitSelectionExpression,CommitSelectionExpression>)(e =>
				e.Reselect(selector)
			));
			return this;
		}
		public PredictorContext SetCommits(string afterRevision, string tillRevision)
		{
			SetCommits(e =>
				e.AfterRevision(afterRevision).TillRevision(tillRevision)
			);
			if (afterRevision != null)
			{
				SetValue("after_revision", afterRevision);
			}
			if (tillRevision != null)
			{
				SetValue("till_revision", tillRevision);
			}
			return this;
		}
		public PredictorContext SetFiles(Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression> selector)
		{
			SetValue("files", (Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression>)(e =>
				e.Reselect(selector)
			));
			return this;
		}
	}
}
