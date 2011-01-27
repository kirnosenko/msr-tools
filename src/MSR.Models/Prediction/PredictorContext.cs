/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data;

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
	}
}
