/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MSR.Data.Entities;
using MSR.Data.Entities.Mapping;
using MSR.Data.Entities.DSL.Mapping;
using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;
using MSR.Data.BugTracking;
using MSR.Data.Persistent;
using MSR.Data.VersionControl;
using MSR.Models;

namespace MSR.Tools.Calculator
{
	public class CalculatingTool : Tool
	{
		public CalculatingTool(string configFile)
			: base(configFile)
		{
		}

		public void Predict(string previousReleaseRevision, string releaseRevision)
		{
			using (ConsoleTimeLogger.Start("prediction"))
			using (var s = data.OpenSession())
			{
				PostReleaseDefectFilePredictionEvaluation evaluator = new PostReleaseDefectFilePredictionEvaluation(s);
				Dictionary<string,IPostReleaseDefectFilePrediction> predictors = new Dictionary<string,IPostReleaseDefectFilePrediction>()
				{
					{ "random", new RandomPostReleaseDefectFilePrediction(s) },
					{ "max loc", new MaxLocPostReleaseDefectFilePrediction(s) },
					{ "max added loc", new MaxAddedLocPostReleaseDefectFilePrediction(s) },
					{ "max touch", new MaxTouchCountPostReleaseDefectFilePrediction(s) },
					{ "dd based", new DefectDensityBasedDefectFilePrediction(s) },
					{ "dcd based", new DefectCodeDensityBasedDefectFilePrediction(s) }
				};
				
				evaluator.PostReleasePeriod = 30 * 6;
				evaluator.PreviousReleaseRevision = previousReleaseRevision;
				evaluator.ReleaseRevision = releaseRevision;
				evaluator.FileSelector = fe => fe.InDirectory("/trunk");
				
				foreach (var predictor in predictors)
				{
					Console.WriteLine(predictor.Key + ":");
					EvaluationResult result = evaluator.Evaluate(predictor.Value);
					Console.WriteLine(result);
				}
			}
		}
		public void QueryUnderProfiler()
		{
			PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(data);
			data.Logger = Console.Out;
			
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				
				Console.WriteLine(
					selectionDSL
						.BugFixes().CalculateBugLifetimeSpread()
				);
			}

			Console.WriteLine("{0}", prof.NumberOfQueries);
		}
		public void LocStat()
		{
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				
				var code = selectionDSL
					.Files()
						.InDirectory("/trunk")
						.Exist()
					.Modifications()
						.InFiles()
					.CodeBlocks()
						.InModifications().Fixed();
				
				Console.WriteLine(
					"LOC+ {0}", 
					code.Added().CalculateLOC()
				);
				Console.WriteLine(
					"LOC- {0}",
					code.Deleted().CalculateLOC()
				);
				Console.WriteLine(
					"LOCc {0}",
					code.Added().CalculateLOC() + code.Added().ModifiedBy().CalculateLOC()
				);
				Console.WriteLine(
					"LOCm- {0}",
					code.Added().ModifiedBy().CalculateLOC()
				);
				Console.WriteLine(
					"LOCmf- {0}",
					code.Added().ModifiedBy().InBugFixes().CalculateLOC()
				);
				Console.WriteLine("DD {0}", code.CalculateTraditionalDefectDensity());
				Console.WriteLine("DCD {0}", code.CalculateDefectCodeDensity());
			}
		}
		
		#region Author Stat

		public void AuthorStat()
		{
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);

				var authors = selectionDSL.Commits()
					.Select(x => x.Author)
					.Distinct();

				Console.WriteLine("{0,15} | {1,6} | {2,7} | {3,7} | {4,7} | {5,5} | {6,5}",
					"author",
					"dd",
					"LOCa",
					"LOCd",
					"LOCc",
					"fc+",
					"fc-"
				);

				Dictionary<string, CodeBlockSelectionExpression> codeByAuthor = new Dictionary<string, CodeBlockSelectionExpression>();
				foreach (var author in authors)
				{
					codeByAuthor.Add(
						author,
						selectionDSL
							.Commits().AuthorIs(author)
							.Modifications().InCommits()
							.CodeBlocks().InModifications()
							.Fixed()
					);
				}

				var stat =
					from a in codeByAuthor
					let author = a.Key
					let code = a.Value
					select new
					{
						author = author,
						dcd = code.CalculateDefectCodeDensity().ToString("F04"),
						added = code.Added().CalculateLOC(),
						deleted = code.Deleted().CalculateLOC(),
						current = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC(),
						addedInFixes = code.Added().InBugFixes().CalculateLOC(),
						deletedInFixes = code.Deleted().InBugFixes().CalculateLOC()
					};

				foreach (var authorStat in stat.OrderByDescending(x => x.dcd))
				{
					Console.WriteLine("{0,15} | {1,6} | {2,7} | {3,7} | {4,7} | {5,5} | {6,5}",
						authorStat.author,
						authorStat.dcd,
						authorStat.added,
						authorStat.deleted,
						authorStat.current,
						authorStat.addedInFixes,
						authorStat.deletedInFixes
					);
				}
			}
		}

		#endregion
	}
}
