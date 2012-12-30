/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Tools.Visualizer.Visualizations
{
	[TestFixture]
	public class CodeSizeToDateTest : BaseRepositoryTest
	{
		private CodeSizeToDate visualization;
		private IGraphView graphStub;
		private double[] x,y;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			visualization = new CodeSizeToDate();
			visualization.DatePeriod = DatePeriod.DAY;
			graphStub = MockRepository.GenerateStub<IGraphView>();
			graphStub.Stub(x => x.ShowLineWithPoints(null, null, null))
				.IgnoreArguments()
				.Callback(new Func<string,double[],double[],bool>((l,x,y) =>
				{
					this.x = x;
					this.y = y;
					
					return true;
				}));
		}
		[Test]
		public void Should_calc_code_size_for_each_day()
		{
			mappingDSL
				.AddCommit("1").At(DateTime.Today.AddDays(-9))
					.AddFile("/file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2").At(DateTime.Today.AddDays(-8))
					.File("/file1").Modified()
						.Code(-20).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
					.AddFile("/file2").Modified()
						.Code(50)
			.Submit()
				.AddCommit("3").At(DateTime.Today.AddDays(-7))
					.File("/file1").Modified()
						.Code(40)
					.File("/file2").Delete().Modified()
						.DeleteCode()
			.Submit();
			
			using (var s = data.OpenSession())
			{
				visualization.Calc(s);
				visualization.Draw(graphStub);
			}
			
			x.Should().Have.SameSequenceAs(new double[]
			{
				0, 1, 2
			});
			y.Should().Have.SameSequenceAs(new double[]
			{
				100, 140, 130
			});
		}
		[Test]
		public void Should_use_number_of_points_to_keep_all_period()
		{
			visualization.DatePeriod = DatePeriod.WEEK;
			
			mappingDSL
				.AddCommit("1").At(DateTime.Today.AddDays(-15))
			.Submit()
				.AddCommit("2").At(DateTime.Today.AddDays(-10))
			.Submit()
				.AddCommit("3").At(DateTime.Today)
			.Submit();
			
			using (var s = data.OpenSession())
			{
				visualization.Calc(s);
				visualization.Draw(graphStub);
			}
			
			x.Length.Should().Be(3);
		}
	}
}
