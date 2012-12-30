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
	[TestFixture]
	public class ModificationMapperTest : BaseMapperTest
	{
		private ModificationMapper mapper;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			mapper = new ModificationMapper(scmData);
		}
		[Test]
		public void Should_map_modifacation_for_modified_file()
		{
			mappingDSL
				.AddCommit("9")
					.AddFile("file1").Modified()
					.AddFile("file2").Modified()
			.Submit();
			
			CommitMappingExpression commitExp = mappingDSL.AddCommit("10");
			mapper.Map(
				commitExp.File("file2")
			);
			mapper.Map(
				commitExp.AddFile("file3")
			);
			SubmitChanges();

			Queryable<Modification>()
				.Where(m => m.Commit.Revision == "10")
				.Select(m => m.File.Path)
				.ToArray()
					.Should().Have.SameSequenceAs(new string[]
					{
						"file2", "file3"
					});
		}
	}
}
