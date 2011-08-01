/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

using MSR.Models.Prediction.SRGM;
using MSR.Models.Regressions;

namespace MSR.Models.Prediction.PostReleaseDefectFiles
{
	public class CodeStabilityPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public CodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model";
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);

			List<double> bugLifetimes = new List<double>(
				repositories.SelectionDSL()
					.Commits().TillRevision(PredictionRelease)
					.BugFixes().InCommits().CalculateAvarageBugLifetime()
			);
			bugLifetimes.Add(1000000);
			BugLifetimes = bugLifetimes;

			DefectLineProbability = repositories.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensityAtRevision(PredictionRelease);

			AddedCodeSizeByRevision = new SmartDictionary<string, double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateLOC()
			);
			DefectCodeSizeByRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease)
			);
			ReleaseDate = repositories.Repository<Commit>()
				.Single(x => x.Revision == PredictionRelease)
				.Date;
		}
		protected override double FileFaultProneProbability(ProjectFile file)
		{
			var codeBlocks = repositories.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().IdIs(file.ID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateRemainingCodeSize(PredictionRelease);

			var codeByRevision = (
				from cb in codeBlocks
				let CommitID = repositories.Repository<CodeBlock>()
					.Single(x => x.ID == cb.Key)
					.AddedInitiallyInCommitID
				from c in repositories.Repository<Commit>()
				where
					c.ID == CommitID
				let revision = c.Revision
				let age = (ReleaseDate - c.Date).TotalDays
				let codeSize = cb.Value
				let defectCodeSize = DefectCodeSizeByRevision[revision]
				let addedCodeSize = AddedCodeSizeByRevision[revision]
				select new
				{
					// Probability that code from revision has errors
					// (code size predictor)
					EP = 1 - Math.Pow(1 - DefectLineProbability, addedCodeSize),
					// Probability that code from revision has errors will be detected in future
					// (code age predictor)
					EFDP = (double)BugLifetimes.Where(t => t > age).Count() / BugLifetimes.Count(),
					// Probability that code from revision has errors were not removed
					// (code removing predictor)
					//EWNRFP = codeSize / AddedCodeSizeByRevision[revision],
					// Probability that code from revision has errors were not removed during refactoring
					// (code refactoring predictor)
					EWNRP = (codeSize + defectCodeSize) / addedCodeSize,
					// Probability that code from revision has errors were not fixed before
					// (fixed code predictor)
					EWNFP = LaplaceIntegralTheorem(
						DefectLineProbability,
						addedCodeSize,
						defectCodeSize + 1,
						defectCodeSize + codeSize
					)

				}).ToArray();

			double fileHasDefectsProbability = 0;
			foreach (var codeFromRevision in codeByRevision)
			{
				double codeFromRevisionHasDefectsProbability =
				(
					codeFromRevision.EP
					*
					((codeFromRevision.EFDP + (codeFromRevision.EWNRP * codeFromRevision.EWNFP)) / 2)
				);
				
				fileHasDefectsProbability =
					(fileHasDefectsProbability + codeFromRevisionHasDefectsProbability)
					-
					(fileHasDefectsProbability * codeFromRevisionHasDefectsProbability);
			}
			
			return fileHasDefectsProbability;
		}
		protected IEnumerable<double> BugLifetimes
		{
			get; set;
		}
		protected double DefectLineProbability
		{
			get; set;
		}
		protected IDictionary<string,double> AddedCodeSizeByRevision
		{
			get; set;
		}
		protected IDictionary<string,double> DefectCodeSizeByRevision
		{
			get; set;
		}
		protected DateTime ReleaseDate
		{
			get; set;
		}
		
		private double LaplaceIntegralTheorem(double p, double n, double k1, double k2)
		{
			double q = 1 - p;
			double from = (k1 - n * p)/Math.Sqrt(n * p * q);
			double to = (k2 - n * p)/Math.Sqrt(n * p * q);
			
			return (1 / Math.Sqrt(2 * Math.PI)) * Integral(x => Math.Exp(-Math.Pow(x,2)/2), from, to);
		}
		private double Integral(Func<double,double> func, double from, double to)
		{
			double delta = 0.001;
			double sum = 0;
			
			for (double x = from + delta/2; x < to; x += delta)
			{
				sum += func(x) * delta;
			}
			
			return sum;
		}
	}
}