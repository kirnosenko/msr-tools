/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping
{
	public interface IMapper
	{
		void RegisterHost(IMappingHost host);
		Type Type { get; }
	}
}
