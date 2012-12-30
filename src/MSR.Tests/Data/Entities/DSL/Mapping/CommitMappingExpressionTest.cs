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
	public class CommitMappingExpressionTest : BaseRepositoryTest
	{
		[Test]
		public void Should_add_commit()
		{
			mappingDSL
				.AddCommit("1")
					.By("alan")
					.At(DateTime.Today)
					.WithMessage("log")
			.Submit();

			Queryable<Commit>().Count()
				.Should().Be(1);
			Queryable<Commit>().Single()
				.Satisfy(c =>
					c.Revision == "1" &&
					c.Author == "alan" &&
					c.Date == DateTime.Today &&
					c.Message == "log"
				);
		}
		[Test]
		public void Can_use_existent_commit()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.Commit("1").By("alan")
			.Submit();

			Queryable<Commit>().Count()
				.Should().Be(1);
			Queryable<Commit>().Single()
				.Satisfy(x =>
					x.Revision == "1"
					&&
					x.Author == "alan"
				);
		}
		[Test]
		public void Should_add_commit_with_incremental_order_number()
		{
			mappingDSL
				.AddCommit("1")
			.Submit()
				.AddCommit("2")
			.Submit()
				.AddCommit("3")
			.Submit();

			Queryable<Commit>().Select(c => c.OrderedNumber)
				.Should().Have.SameSequenceAs(new int[] { 1,2,3 });
		}
	}
}
