/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Data.Entities.Mapping
{
	public interface IMappingHost
	{
		void RegisterMapper<T,IME,OME>(EntityMapper<T,IME,OME> mapper);
	}
}
