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
		public override void Predict()
		{
			var bugLifetimes = repositories.SelectionDSL()
				.Commits().TillRevision(TrainReleases.Last())
				.BugFixes().InCommits().CalculateAvarageBugLifetime();
			
			double defectLineProbability = repositories.SelectionDSL()
				.Commits().TillRevision(TrainReleases.Last())
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensityAtRevision(TrainReleases.Last());
			
			var files = GetFilesInRevision(PredictionRelease);
			int filesInRelease = files.Count();
			Dictionary<string,double> fileStability = new Dictionary<string,double>();
			
			var defectCodeSizeByRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease)
			);
			var addedCodeSizeByRevision = new SmartDictionary<string, double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateLOC()
			);
			
			foreach (var file in files)
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
					let releaseDate = repositories.Repository<Commit>()
						.Single(x => x.Revision == PredictionRelease)
						.Date
					let age = (releaseDate - c.Date).TotalDays
					let codeSize = cb.Value
					select new
					{
						Revision = revision,
						Age = age,
						CodeSize = codeSize,
						AddedCodeSize = addedCodeSizeByRevision[revision],
						DefectCodeSize = defectCodeSizeByRevision[revision],
						// Code from revision has errors probability
						// (code size predictor)
						EP = 1 - Math.Pow(1 - defectLineProbability, codeSize),
						// Code from revision has errors that should be detected probability
						// (code age predictor)
						DEP = (double)bugLifetimes.Where(t => t <= age).Count() / bugLifetimes.Count(),
						// Code from revision has errors that were not fixed probability
						// (fixed code predictor)
						NFEP = defectCodeSizeByRevision[revision] == 0 ?
							1
							:
							1 - LaplaceIntegralTheorem(
								defectLineProbability,
								addedCodeSizeByRevision[revision],
								defectCodeSizeByRevision[revision] + 1,
								defectCodeSizeByRevision[revision] + codeSize
							)
					}).ToList();
				
				double fileHasDefectsProbability = 0;
				foreach (var codeFromRevision in codeByRevision)
				{
					double codeFromRevisionHasDefectsProbability = 
						(codeFromRevision.EP * codeFromRevision.DEP * codeFromRevision.NFEP);
					fileHasDefectsProbability += 
						codeFromRevisionHasDefectsProbability
						-
						fileHasDefectsProbability * codeFromRevisionHasDefectsProbability;
				}
				
				fileStability.Add(file.Path, fileHasDefectsProbability);
			}

			PredictedDefectFiles = fileStability
				//.Where(x => x.Value <= 0.01)
				.OrderByDescending(x => x.Value)
				.TakeNoMoreThan((int)(0.2 * fileStability.Count))
				.Select(x => x.Key)
				.ToList();
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