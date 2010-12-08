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
			mapper = new MappingController(data, scmDataStub, new IMapper[] {});
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
			
			mapper.Map();
			
			commitMapperStub.VerifyAllExpectations();
		}
		[Test]
		public void Should_set_revision_being_mapped()
		{
			scmDataStub.Stub(x => x.RevisionByNumber(10))
				.Return("revision id");
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.Matching(
					(RepositoryMappingExpression e) => e.Revision == "revision id"
				))
				.Return(Enumerable.Empty<CommitMappingExpression>());

			mapper.RegisterMapper<RepositoryMappingExpression, CommitMappingExpression>(commitMapperStub);
			mapper.RevisionNumber = 10;
			mapper.Map();

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

			mapper.Map();
			
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

			mapper.Map();

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
			
			mapper.Map();
			mapper.Map();
		}
	}
}
