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
		public CodeStabilityPostReleaseDefectFilesPrediction(IRepositoryResolver repositories)
			: base(repositories)
		{
		}
		public override IEnumerable<string> Predict(string[] revisions)
		{
			IEnumerable<string> previousReleaseRevisions = revisions.Take(revisions.Count() - 1);
			string releaseRevision = revisions.Last();
			
			var bugLifetimes = repositories.SelectionDSL()
				.Commits().TillRevision(previousReleaseRevisions.Last())
				.BugFixes().InCommits().CalculateAvarageBugLifetime();
			
			double defectLineProbability = repositories.SelectionDSL()
				.Commits().TillRevision(previousReleaseRevisions.Last())
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensityAtRevision(previousReleaseRevisions.Last());
			
			var files = FilesInRevision(releaseRevision);
			int filesInRelease = files.Count();
			Dictionary<string,double> fileStability = new Dictionary<string,double>();
			
			var defectCodeSizeByRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(releaseRevision)
			);
			
			foreach (var file in files)
			{
				var codeBlocks = repositories.SelectionDSL()
					.Commits().TillRevision(releaseRevision)
					.Files().IdIs(file.ID)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateRemainingCodeSize(releaseRevision);
				
				var codeByRevision = (
					from cb in codeBlocks
					let CommitID = repositories.Repository<CodeBlock>()
						.Single(x => x.ID == cb.Key)
						.AddedInitiallyInCommitID
					from c in repositories.Repository<Commit>()
					where
						c.ID == CommitID
					let releaseDate = repositories.Repository<Commit>()
						.Single(x => x.Revision == releaseRevision)
						.Date
					select new
					{
						Revision = c.Revision,
						Age = (releaseDate - c.Date).TotalDays,
						CodeSize = cb.Value
					}).ToList();
				
				double fileHasNoErrorsProbability = 1;
				
				foreach (var codeFromRevision in codeByRevision)
				{
					// Code from revision has errors probability
					// (code size predictor)
					double ep = 1 - Math.Pow(1 - defectLineProbability, codeFromRevision.CodeSize);
					// Code from revision has errors that should be detected probability
					// (code age predictor)
					double dep = (double)bugLifetimes.Where(t => t <= codeFromRevision.Age).Count() / bugLifetimes.Count();
					
					double fdcs = defectCodeSizeByRevision[codeFromRevision.Revision];
					// Code from revision has errors that were not fixed probability
					// (fixed code predictor)
					double nfep = fdcs == 0 ?
						1
						:
						1 - LaplaceIntegralTheorem(
							defectLineProbability,
							repositories.SelectionDSL()
								.Commits().RevisionIs(codeFromRevision.Revision)
								.Modifications().InCommits()
								.CodeBlocks().InModifications().Added().CalculateLOC(),
							fdcs + 1,
							fdcs + codeFromRevision.CodeSize
						);
					
					fileHasNoErrorsProbability *= 1 - (ep * dep * nfep);
				}
				fileStability.Add(file.Path, fileHasNoErrorsProbability);
			}
			
			return fileStability
				//.Where(x => x.Value <= 0.01)
				.OrderBy(x => x.Value)
				.TakeNoMoreThan((int)(0.2 * fileStability.Count))
				.Select(x => x.Key);
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
			double delta = 0.0001;
			double sum = 0;
			
			for (double x = from; x < to; x += delta)
			{
				sum += func(x + delta/2) * delta;
			}
			
			return sum;
		}
	}
}