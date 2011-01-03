/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using NVelocity;

using MSR.Data;

namespace MSR.Tools.StatGenerator
{
	public class CompositeStatBuilder : IStatBuilder
	{
		private IStatBuilder[] builders;
		
		public CompositeStatBuilder(IStatBuilder[] builders)
		{
			this.builders = builders;
		}
		public void AddData(IDataStore data, VelocityContext context)
		{
			foreach (var builder in builders)
			{
				builder.AddData(data, context);
			}
		}
	}
}
