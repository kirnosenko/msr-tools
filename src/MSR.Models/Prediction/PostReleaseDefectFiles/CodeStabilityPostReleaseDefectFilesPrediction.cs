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
		public override IEnumerable<string> Predict(string[] previousReleaseRevisions, string releaseRevision)
		{
			var bugLifetimes = repositories.SelectionDSL()
				.Commits().TillRevision(previousReleaseRevisions.Last())
				.BugFixes().InCommits().CalculateAvarageBugLifetime();
			
			double defectLineProbability = repositories.SelectionDSL()
				.Commits().TillRevision(previousReleaseRevisions.Last())
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensity();
			
			var files = FilesInRevision(releaseRevision);
			int filesInRelease = files.Count();
			Dictionary<string,double> faultProneFiles = new Dictionary<string, double>();
			
			foreach (var file in files)
			{
				var codeBlocks = repositories.SelectionDSL()
					.Commits().TillRevision(releaseRevision)
					.Files().IdIs(file.ID)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateRemainingCodeSize(releaseRevision);
				var codeByAge = (
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
				
				//if (q.Count() == 0)
				//{
				//	Console.WriteLine(file.Path)
				//}
				
				double currentLoc = codeByAge.Sum(x => x.CodeSize);
				
				double codeStability = codeByAge.Sum(x =>
					Math.Pow(1 - defectLineProbability, currentLoc) * defectLineProbability
					*
					(double)bugLifetimes.Where(t => t <= x.Age).Count() / bugLifetimes.Count()
				);
				faultProneFiles.Add(file.Path, codeStability);
			}
			
			return faultProneFiles
				.OrderBy(x => x.Value)
				.Select(x => x.Key)
				.TakeNoMoreThan((int)(0.2d * faultProneFiles.Count));
		}
	}
}