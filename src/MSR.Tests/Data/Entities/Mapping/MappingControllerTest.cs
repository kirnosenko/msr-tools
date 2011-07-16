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
			mapper = new MappingController(scmDataStub);
			commitMapperStub = MockRepository.GenerateStub<CommitMapper>(null as IScmData);
			bugFixMapperStub = MockRepository.GenerateStub<BugFixMapper>(null, null);
			fileMapperStub = MockRepository.GenerateStub<ProjectFileMapper>(null as IScmData);
		}
		[Test]
		public void Should_use_registered_mapper()
		{
			commitMapperStub = MockRepository.GenerateMock<CommitMapper>(null as IScmData);
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.NotNull())
				.Return(Enumerable.Empty<CommitMappingExpression>());
			
			mapper.RegisterMapper(commitMapperStub);
			
			mapper.Map(data, "1");
			
			commitMapperStub.VerifyAllExpectations();
		}
		[Test]
		public void Should_set_revision_being_mapped()
		{
			commitMapperStub = MockRepository.GenerateMock<CommitMapper>(null as IScmData);
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.Matching(
					(RepositoryMappingExpression e) => e.Revision == "10"
				))
				.Return(Enumerable.Empty<CommitMappingExpression>());

			mapper.RegisterMapper(commitMapperStub);
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
			
			mapper.RegisterMapper(commitMapperStub);
			mapper.RegisterMapper(bugFixMapperStub);

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

			mapper.RegisterMapper(commitMapperStub);
			mapper.RegisterMapper(bugFixMapperStub);
			mapper.RegisterMapper(fileMapperStub);

			mapper.Map(data, "1");

			fileMapperStub.AssertWasCalled(x => x.Map(commitExp));
		}
		[Test]
		public void Should_not_keep_expressions_between_sessions()
		{
			CommitMappingExpression commitExp = mappingDSL.AddCommit("1");

			commitMapperStub = MockRepository.GenerateMock<CommitMapper>(null as IScmData);
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Return(new CommitMappingExpression[] { commitExp })
				.Repeat.Twice();
			mapper.RegisterMapper(commitMapperStub);
			
			mapper.Map(data, "1");
			mapper.Map(data, "1");
		}
		[Test]
		public void Should_map_until_last_revision()
		{
			List<string> revisions = new List<string>();
			
			scmDataStub.Stub(x => x.RevisionByNumber(0))
				.IgnoreArguments()
				.Return("8");
			scmDataStub.Stub(x => x.NextRevision("8"))
				.Return("9");
			scmDataStub.Stub(x => x.NextRevision("9"))
				.Return("10");
			scmDataStub.Stub(x => x.NextRevision("10"))
				.Return("11");
			
			mapper.StopRevision = "10";
			mapper.OnRevisionMapping += (r,n) => revisions.Add(r);
			
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

			scmDataStub.Stub(x => x.RevisionByNumber(0))
				.IgnoreArguments()
				.Do((Func<int, string>)(n =>
				{
					return n.ToString();
				}));
			scmDataStub.Stub(x => x.NextRevision(null))
				.IgnoreArguments()
				.Do((Func<string, string>)(r =>
				{
					if (r == "5")
					{
						return null;
					}
					return (Convert.ToInt32(r) + 1).ToString();
				}));
			
			mapper.StopRevision = null;
			mapper.OnRevisionMapping += (r,n) => revisions.Add(r);

			mapper.Map(data);

			revisions.ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"1", "2", "3", "4", "5"
				});
		}

		[Test]
		public void Should_execute_partial_mapping_for_all_mapped_revisions_from_specified()
		{
			List<string> revisions = new List<string>();
			
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
			.Submit()
				.AddCommit("2")
					.AddFile("file2").Modified()
			.Submit()
				.AddCommit("3")
					.AddFile("file3").Modified()
			.Submit();
			scmDataStub.Stub(x => x.NextRevision(null))
				.IgnoreArguments()
				.Do((Func<string,string>)(r =>
				{
					return (Convert.ToInt32(r) + 1).ToString();
				}));
			
			mapper.StartRevision = "2";
			mapper.OnRevisionMapping += (r, n) => revisions.Add(r);
			
			mapper.Map(data);

			revisions.ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"2", "3"
				});
		}
		
		[Test]
		public void Should_create_schema_for_registered_expressions()
		{
			IDataStore dataStub = MockRepository.GenerateStub<IDataStore>();
			dataStub.Stub(x => x.CreateSchema(null))
				.IgnoreArguments()
				.Callback(new Func<Type[],bool>((t) =>
				{
					t.Should().Have.SameValuesAs(new Type[]
					{
						typeof(Commit), typeof(BugFix), typeof(ProjectFile)
					});
					return true;
				}));

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
			mapper.RegisterMapper(commitMapperStub);
			mapper.RegisterMapper(bugFixMapperStub);
			mapper.RegisterMapper(fileMapperStub);
			mapper.CreateDataBase = true;
			
			mapper.Map(data, "1");
		}
		[Test]
		public void Can_replace_mappers()
		{
			commitMapperStub = MockRepository.GenerateMock<CommitMapper>(null as IScmData);
			commitMapperStub.Expect(x => x.Map(null))
				.IgnoreArguments()
				.Constraints(Rhino.Mocks.Constraints.Is.NotNull())
				.Return(Enumerable.Empty<CommitMappingExpression>());

			CommitMapper commitMapperStub2 = MockRepository.GenerateMock<CommitMapper>(null as IScmData);
			
			mapper.RegisterMapper(commitMapperStub2);
			mapper.RegisterMapper(commitMapperStub);

			mapper.Map(data, "1");
			
			commitMapperStub.VerifyAllExpectations();
			commitMapperStub2.VerifyAllExpectations();
		}
	}
}
