/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
		private IDataStore dataStore;
		private ISession session;
		private IScmData scmData;
		
		private List<object> availableExpressions;
		private List<Action> mappers = new List<Action>();
		
		public MappingController(IDataStore dataStore, IScmData scmData, IMapper[] mappers)
		{
			this.dataStore = dataStore;
			this.scmData = scmData;
			foreach (var mapper in mappers)
			{
				mapper.RegisterHost(this);
			}
		}
		public void Map()
		{
			using (session = dataStore.OpenSession())
			{
				availableExpressions = new List<object>()
				{ 
					new RepositoryMappingExpression(session)
					{
						Revision = scmData.RevisionByNumber(RevisionNumber)
					}
				};
				
				foreach (var mapper in mappers)
				{
					mapper();
				}
				
				session.SubmitChanges();
			}
		}
		public void RegisterMapper<IME,OME>(EntityMapper<IME,OME> mapper)
		{
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
		public int RevisionNumber
		{
			get; set;
		}
	}
}
