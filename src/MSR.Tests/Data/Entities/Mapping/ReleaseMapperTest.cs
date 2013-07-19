/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
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
	public class ReleaseMapperTest : BaseMapperTest
	{
		private ReleaseMapper mapper;
		private IReleaseDetector detector;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			detector = MockRepository.GenerateStub<IReleaseDetector>();
			mapper = new ReleaseMapper(scmData, detector);
		}
		[TestCase(null)]
		[TestCase("1.0.0")]
		public void Should_add_release_for_commit_with_tag(string tag)
		{
			detector.Stub(x => x.TagForCommit(null))
				.IgnoreArguments()
				.Return(tag);

			mapper.Map(
				mappingDSL.AddCommit("1")
			);
			SubmitChanges();

			if (tag == null)
			{
				Queryable<Release>().Count()
					.Should().Be(0);
			}
			else
			{
				Queryable<Release>().Count()
					.Should().Be(1);
				Queryable<Release>().Single()
					.Satisfy(x =>
						x.Tag == tag
						&&
						x.Commit.Revision == "1"
					);
			}
		}
	}
}
