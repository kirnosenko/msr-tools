/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class CommitMapperTest : BaseMapperTest
	{
		private CommitMapper mapper;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			mapper = new CommitMapper(scmData);
		}
		[Test]
		public void Should_add_commit()
		{
			logStub.Stub(x => x.Revision)
				.Return("1");
			logStub.Stub(x => x.Author)
				.Return("alan");
			logStub.Stub(x => x.Date)
				.Return(DateTime.Today);
			logStub.Stub(x => x.Message)
				.Return("log");
			scmData.Stub(x => x.Log("1"))
				.Return(logStub);
			
			mappingDSL.Revision = "1";
			mapper.Map(mappingDSL);
			SubmitChanges();

			Queryable<Commit>().Count()
				.Should().Be(1);
			Queryable<Commit>().Single()
				.Satisfy(x =>
					x.Revision == logStub.Revision &&
					x.Author == logStub.Author &&
					x.Date == logStub.Date &&
					x.Message == logStub.Message
				);
		}
	}
}
