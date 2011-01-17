/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.VersionControl;

namespace MSR.Data.Entities.Mapping
{
	public class MappingController : IMappingHost
	{
		public event Action<string> OnRevisionMapping;
		
		private IScmData scmData;
		
		private List<object> availableExpressions;
		private List<Action> mappers = new List<Action>();
		private List<Type> mapperTypes = new List<Type>();
		
		public MappingController(IScmData scmData, IMapper[] mappers)
		{
			this.scmData = scmData;
			foreach (var mapper in mappers)
			{
				mapper.RegisterHost(this);
			}
		}
		public void Map(IDataStore data)
		{
			if (CreateDataBase)
			{
				CreateSchema(data);
			}
			else if (RevisionExists(data, StopRevision))
			{
				return;
			}
			
			do
			{
				Map(data, NextRevision);
				NextRevision = NextRevision == StopRevision ?
					null
					:
					scmData.NextRevision(NextRevision);
			} while (NextRevision != null);
		}
		public void Map(IDataStore data, string revision)
		{
			if (OnRevisionMapping != null)
			{
				OnRevisionMapping(revision);
			}
			using (var s = data.OpenSession())
			{
				availableExpressions = new List<object>()
				{ 
					new RepositoryMappingExpression(s)
					{
						Revision = revision
					}
				};
				
				foreach (var mapper in mappers)
				{
					mapper();
				}
				
				s.SubmitChanges();
			}
		}
		public void RegisterMapper<T,IME,OME>(EntityMapper<T,IME,OME> mapper)
		{
			mapperTypes.Add(typeof(T));
			mappers.Add(() =>
			{
				List<object> newExpressions = new List<object>();
				
				foreach (var iExp in availableExpressions.Where(x => x.GetType() == typeof(IME)))
				{
					foreach (var oExp in mapper.Map((IME)iExp))
					{
						newExpressions.Add(oExp);
					}
				}
				
				foreach (var exp in newExpressions)
				{
					availableExpressions.Add(exp);
				}
			});
		}
		public bool CreateDataBase
		{
			get; set;
		}
		public string NextRevision
		{
			get; set;
		}
		public string StopRevision
		{
			get; set;
		}
		private void CreateSchema(IDataStore data)
		{
			data.CreateSchema(mapperTypes.ToArray());
		}
		private bool RevisionExists(IDataStore data, string revision)
		{
			using (var s = data.OpenSession())
			{
				return s.Repository<Commit>().SingleOrDefault(c => c.Revision == revision) != null;
			}
		}
	}
}
