/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class BaseMapperTest : BaseRepositoryTest
	{
		protected IScmData scmData;
		protected ILog logStub;
		protected IDiff diffStub;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			scmData = MockRepository.GenerateStub<IScmData>();
			logStub = MockRepository.GenerateStub<ILog>();
			diffStub = MockRepository.GenerateStub<IDiff>();
		}
	}
}
