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

using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	[TestFixture]
	public class LocTest : BaseRepositoryTest
	{
		[Test]
		public void Should_be_zero_for_empty_code()
		{
			selectionDSL
				.CodeBlocks().CalculateLOC()
					.Should().Be(0);
		}
		[Test]
		public void Loc_is_added_code_without_deleted_code()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-10).ForCodeAddedInitiallyInRevision("1")
						.Code(+20)
			.Submit();
			
			selectionDSL
				.CodeBlocks().CalculateLOC()
					.Should().Be(110);
		}
	}
}