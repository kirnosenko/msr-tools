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

namespace MSR.Data.Entities.DSL.Mapping
{
	[TestFixture]
	public class RepositoryMappingExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_not_keep_expression_chain_after_submit()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.Should().Not.Be(mappingDSL);
		}
		[Test]
		public void Can_give_last_entity_by_type()
		{
			var exp = mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
					.AddFile("file2").Modified();
			
			exp.CurrentEntity<Commit>().Revision
				.Should().Be("1");
			exp.CurrentEntity<ProjectFile>().Path
				.Should().Be("file2");
			exp.CurrentEntity<CodeBlock>()
				.Should().Be.Null();
		}
	}
}
