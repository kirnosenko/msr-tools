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
			Dictionary<string,double> faultProneFiles = new Dictionary<string,double>();
			
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
				
				double addedLoc = codeByRevision.Where(x => x.CodeSize > 0).Sum(x => x.CodeSize);
				double currentLoc = codeByRevision.Sum(x => x.CodeSize);
				var fileCodeInRelease = repositories.SelectionDSL()
					.Files().IdIs(file.ID)
					.Commits().TillRevision(releaseRevision)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Fixed();
				double defectLoc = fileCodeInRelease.CalculateDefectCodeSize(releaseRevision);
				double dcdForFileAtReleaseTime = fileCodeInRelease.CalculateDefectCodeDensityAtRevision(releaseRevision);
				/*
				double fileHasErrorsProbability =
					Math.Pow(1 - defectLineProbability, currentLoc) * defectLineProbability - dcdForFileAtReleaseTime;
				if (fileHasErrorsProbability < 0)
				{
					fileHasErrorsProbability = 0;
				}
				/*
				fileHasErrorsProbability = defectLoc == 0 ?
					1 - Math.Pow(1 - defectLineProbability, currentLoc)
					:
					LaplaceIntegralTheorem(defectLineProbability, defectLoc + currentLoc, defectLoc + 1, defectLoc + currentLoc);
				*/
				
				double errorProneProbability = 0;
				
				foreach (var codeFromRevision in codeByRevision)
				{
					double codeFromRevisionHasErrorsProbability = 
						1 - Math.Pow(1 - defectLineProbability, codeFromRevision.CodeSize);
						
					errorProneProbability += 
						codeFromRevisionHasErrorsProbability
						*
						(double)bugLifetimes.Where(t => t <= codeFromRevision.Age).Count() / bugLifetimes.Count();
				}
				faultProneFiles.Add(file.Path, errorProneProbability);
			}
			
			var q = faultProneFiles
				.OrderBy(x => x.Value);
			
			return faultProneFiles
				//.Where(x => x.Value < 0.95)
				.OrderByDescending(x => x.Value)
				.Select(x => x.Key)
				.TakeNoMoreThan((int)(0.2d * faultProneFiles.Count));
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