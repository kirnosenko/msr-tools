/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Models.Prediction
{
	public class PredictorContext : IRepositoryResolver
	{
		private IRepositoryResolver repositories;
		private Dictionary<string,object> data = new Dictionary<string,object>();
		
		public PredictorContext(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public IRepository<T> Repository<T>() where T : class
		{
			return repositories.Repository<T>();
		}
		public PredictorContext SetValue(string key, object value)
		{
			if (data.ContainsKey(key))
			{
				data[key] = value;
			}
			else
			{
				data.Add(key, value);
			}
			return this;
		}
		public T GetValue<T>(string key)
		{
			return (T)data[key];
		}
		public void Clear()
		{
			data.Clear();
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
