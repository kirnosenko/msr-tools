/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
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
	[TestFixture]
	public class MappingControllerTest : BaseRepositoryTest
	{
		private MappingController mapper;
		private IScmData scmDataStub;
		private CommitMapper commitMapperStub;
		private BugFixMapper bugFixMapperStub;
		private ProjectFileMapper fileMapperStub;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			scmDataStub = MockRepository.GenerateStub<IScmData>();
			mapper = new MappingController(scmDataStub, new IMapper[] {});
			commitMapperStub = MockRepository.GenerateStub<CommitMapper>(null as IScmData);
			bugFixMapperStub = MockRepository.GenerateStub<BugFixMapper>(null, null);
			fileMapperStub = MockRepository.GenerateStub<ProjectFileMapper>(null as IScmData);
		}
		[Test]
		public void Should_use_registered_mapper()
		{
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.NotNull())
				.Return(Enumerable.Empty<CommitMappingExpression>());
			
			mapper.RegisterMapper<RepositoryMappingExpression,CommitMappingExpression>(commitMapperStub);
			
			mapper.Map(data, "1");
			
			commitMapperStub.VerifyAllExpectations();
		}
		[Test]
		public void Should_set_revision_being_mapped()
		{
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.Matching(
					(RepositoryMappingExpression e) => e.Revision == "10"
				))
				.Return(Enumerable.Empty<CommitMappingExpression>());

			mapper.RegisterMapper<RepositoryMappingExpression, CommitMappingExpression>(commitMapperStub);
			mapper.Map(data, "10");

			commitMapperStub.VerifyAllExpectations();
		}
		[Test]
		public void Should_use_output_expression_for_registered_mapper()
		{
			CommitMappingExpression commitExp = mappingDSL.AddCommit("1");
			
			commitMapperStub.Stub(x => x.Map(null))
				.IgnoreArguments()
				.Return(new CommitMappingExpression[] { commitExp });
			bugFixMapperStub.Stub(x => x.Map(null))
				.IgnoreArguments()
				.Return(Enumerable.Empty<BugFixMappingExpression>());
			
			mapper.RegisterMapper<RepositoryMappingExpression, CommitMappingExpression>(commitMapperStub);
			mapper.RegisterMapper<CommitMappingExpression, BugFixMappingExpression>(bugFixMapperStub);

			mapper.Map(data, "1");
			
			bugFixMapperStub.AssertWasCalled(x => x.Map(commitExp));
		}
		[Test]
		public void Can_use_the_same_expression_for_several_mappers()
		{
			CommitMappingExpression commitExp = mappingDSL.AddCommit("1");

			commitMapperStub.Stub(x => x.Map(null))
				.IgnoreArguments()
				.Return(new CommitMappingExpression[] { commitExp });
			bugFixMapperStub.Stub(x => x.Map(null))
				.IgnoreArguments()
				.Return(Enumerable.Empty<BugFixMappingExpression>());
			fileMapperStub.Stub(x => x.Map(null))
				.IgnoreArguments()
				.Return(Enumerable.Empty<ProjectFileMappingExpression>());

			mapper.RegisterMapper<RepositoryMappingExpression, CommitMappingExpression>(commitMapperStub);
			mapper.RegisterMapper<CommitMappingExpression, BugFixMappingExpression>(bugFixMapperStub);
			mapper.RegisterMapper<CommitMappingExpression, ProjectFileMappingExpression>(fileMapperStub);

			mapper.Map(data, "1");

			fileMapperStub.AssertWasCalled(x => x.Map(commitExp));
		}
		[Test]
		public void Should_not_keep_expressions_between_sessions()
		{
			CommitMappingExpression commitExp = mappingDSL.AddCommit("1");

			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Return(new CommitMappingExpression[] { commitExp })
				.Repeat.Twice();
			mapper.RegisterMapper<RepositoryMappingExpression, CommitMappingExpression>(commitMapperStub);
			
			mapper.Map(data, "1");
			mapper.Map(data, "1");
		}
		[Test]
		public void Should_map_until_last_revision()
		{
			List<string> revisions = new List<string>();
			
			scmDataStub.Stub(x => x.NextRevision("8"))
				.Return("9");
			scmDataStub.Stub(x => x.NextRevision("9"))
				.Return("10");
			scmDataStub.Stub(x => x.NextRevision("10"))
				.Return("11");
			
			mapper.NextRevision = "8";
			mapper.StopRevision = "10";
			mapper.OnRevisionMapping += r => revisions.Add(r);
			
			mapper.Map(data);
			
			revisions.ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"8", "9", "10"
				});
		}
		[Test]
		public void Should_stop_if_no_more_revisions()
		{
			List<string> revisions = new List<string>();

			scmDataStub.Stub(x => x.NextRevision("8"))
				.Return("9");
			scmDataStub.Stub(x => x.NextRevision("9"))
				.Return(null);

			mapper.NextRevision = "8";
			mapper.StopRevision = null;
			mapper.OnRevisionMapping += r => revisions.Add(r);

			mapper.Map(data);

			revisions.ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"8", "9"
				});
		}
	}
}
