/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Models.Prediction.Predictors
{
	[TestFixture]
	public class ProjectFilePredictorsTest : BaseRepositoryTest
	{
		private Prediction p;
		private PredictorContext context;
		
		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			context = new PredictorContext(this);
			p = new Prediction().AddFilesTouchCountInCommitsPredictor();
			p.Init(this, new string[] {});
		}
		[Test]
		public void Should_count_number_of_touches_for_file()
		{
			mappingDSL
				.AddCommit("1")
					.AddFile("file1").Modified()
						.Code(100)
			.Submit()
				.AddCommit("2")
					.File("file1").Modified()
						.Code(-5).ForCodeAddedInitiallyInRevision("1")
						.Code(10)
					.AddFile("file2").Modified()
						.Code(50)
			.Submit();
			
			context
				.SetValue("files", (Func<ProjectFileSelectionExpression,ProjectFileSelectionExpression>)(e =>
					e.IdIs(selectionDSL.Files().PathIs("file1").Single().ID)
				))
				.SetValue("commits", (Func<CommitSelectionExpression,CommitSelectionExpression>)(e =>
					e.TillRevision("2")
				));
				
			PredictorValue()
				.Should().Be(2);

			context
				.SetValue("commits", (Func<CommitSelectionExpression,CommitSelectionExpression>)(e =>
					e.AfterRevision("1").TillRevision("2")
				));

			PredictorValue()
				.Should().Be(1);
		}
		private double PredictorValue()
		{
			return p.GetPredictorValuesFor(context).Single();
		}
	}
}
