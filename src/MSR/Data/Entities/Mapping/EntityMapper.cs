/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Selection;

namespace MSR.Data.Entities.Mapping
{
	public abstract class EntityMapper<T,IME,OME> : IMapper
	{
		protected IScmData scmData;
		
		public EntityMapper(IScmData scmData)
		{
			this.scmData = scmData;
		}
		public abstract IEnumerable<OME> Map(IME expression);
		public void RegisterHost(IMappingHost host)
		{
			host.RegisterMapper(this);
		}
		public Type Type
		{
			get { return typeof(T); }
		}
	}
}
