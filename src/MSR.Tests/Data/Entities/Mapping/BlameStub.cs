/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

using MSR.Data.VersionControl;

namespace MSR.Data.Entities.Mapping
{
	public class BlameStub : SmartDictionary<int,string>, IBlame
	{
		public BlameStub()
			: base()
		{
		}
		public BlameStub(Func<int,string> defaultValueBuilder)
			: base(defaultValueBuilder)
		{
		}
	}
}
