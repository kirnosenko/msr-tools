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
		public event Action<string,string> OnRevisionMapping;
		
		private IScmData scmData;
		
		private List<object> availableExpressions;
		private Dictionary<Type,Action> mappers = new Dictionary<Type,Action>();
		
		public MappingController(IScmData scmData, IMapper[] mappers)
		{
			this.scmData = scmData;
			CreateDataBase = false;
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
			int nextRevisionNumber = MappingStartRevision(data);
			string nextRevision = scmData.RevisionByNumber(nextRevisionNumber);
			
			do
			{
				if (OnRevisionMapping != null)
				{
					OnRevisionMapping(nextRevision, nextRevisionNumber.ToString());
				}
				Map(data, nextRevision);
				nextRevision = nextRevision == StopRevision ?
					null
					:
					scmData.NextRevision(nextRevision);
				nextRevisionNumber++;
			} while (nextRevision != null);
		}
		public void Map(IDataStore data, string revision)
		{
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
					mapper.Value();
				}
				
				s.SubmitChanges();
			}
		}
		public void RegisterMapper<T,IME,OME>(EntityMapper<T,IME,OME> mapper)
		{
			if (mappers.ContainsKey(typeof(T)))
			{
				mappers.Remove(typeof(T));
			}
			mappers.Add(typeof(T), () =>
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
		public void KeepOnlyMappers(Type[] mapperTypesToKeep)
		{
			var mappersToRemove = mappers.Where(x => !mapperTypesToKeep.Contains(x.Key)).ToList();
			foreach (var mapper in mappersToRemove)
			{
				mappers.Remove(mapper.Key);
			}
		}
		public bool CreateDataBase
		{
			get; set;
		}
		public string StopRevision
		{
			get; set;
		}
		private void CreateSchema(IDataStore data)
		{
			data.CreateSchema(mappers.Keys.ToArray());
		}
		private bool RevisionExists(IDataStore data, string revision)
		{
			using (var s = data.OpenSession())
			{
				return s.Repository<Commit>().SingleOrDefault(c => c.Revision == revision) != null;
			}
		}
		private int MappingStartRevision(IDataStore data)
		{
			using (var s = data.OpenSession())
			{
				return s.Repository<Commit>().Count() + 1;
			}
		}
	}
}
