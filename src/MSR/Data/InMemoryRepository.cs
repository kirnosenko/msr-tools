/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace MSR.Data
{
	public class InMemoryRepository<T> : IQueryable<T>, IEnumerable<T> where T : class
	{
		private int nextID;
		private List<T> entities = new List<T>();
		private List<T> addedEntities = new List<T>();
		private List<T> deletedEntities = new List<T>();
		
		public InMemoryRepository()
		{
			this.nextID = 1;
		}
		public void Add(T entity)
		{
			addedEntities.Add(entity);
		}
		public void AddRange(IEnumerable<T> entities)
		{
			addedEntities.AddRange(entities);
		}
		public void Delete(T entity)
		{
			deletedEntities.Add(entity);
		}
		public void SubmitChanges()
		{
			Delete();
			Insert();
			Update();
		}

		#region IQueryable Members

		public Type ElementType
		{
			get { return (entities.AsQueryable()).ElementType; }
		}

		public System.Linq.Expressions.Expression Expression
		{
			get { return (entities.AsQueryable()).Expression; }
		}

		public IQueryProvider Provider
		{
			get { return (entities.AsQueryable()).Provider; }
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return entities.GetEnumerator();
		}

		#endregion
		
		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return entities.GetEnumerator();
		}

		#endregion
		
		private void Delete()
		{
			foreach (var entity in deletedEntities)
			{
				entities.Remove(entity);
			}
			deletedEntities.Clear();
		}
		private void Insert()
		{
			foreach (var entity in addedEntities)
			{
				entities.Add(entity);
				SetIDs(entity);
			}
			addedEntities.Clear();
		}
		private void Update()
		{
			foreach (var entity in entities)
			{
				SetIDs(entity);
			}
		}
		private void SetIDs(object entity)
		{
			if (GetPrimaryKeyValue(entity) == 0)
			{
				SetPrimaryKeyValue(entity, nextID++);
			}
			
			foreach (var pi in entity.GetType().GetProperties())
			{
				foreach (Attribute a in pi.GetCustomAttributes(true))
				{
					if (a.GetType() == typeof(AssociationAttribute))
					{
						string thisKey = (a as AssociationAttribute).ThisKey;
						PropertyInfo pi_id = entity.GetType().GetProperty(thisKey);
						
						object relationship = pi.GetValue(entity, null);
						if (relationship != null)
						{
							pi_id.SetValue(entity, GetPrimaryKeyValue(relationship), null);
						}
					}
				}
			}
		}
		private int GetPrimaryKeyValue(object entity)
		{
			return (int)GetPrimaryKey(entity).GetValue(entity, null);
		}
		private void SetPrimaryKeyValue(object entity, int id)
		{
			GetPrimaryKey(entity).SetValue(entity, id, null);
		}
		private PropertyInfo GetPrimaryKey(object entity)
		{
			foreach (var pi in entity.GetType().GetProperties())
			{
				foreach (Attribute a in pi.GetCustomAttributes(true))
				{
					if (a.GetType() == typeof(ColumnAttribute))
					{
						if ((a as ColumnAttribute).IsPrimaryKey)
						{
							return pi;
						}
					}
				}
			}
			
			return null;
		}
	}
}
