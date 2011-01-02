/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using NVelocity;
using NVelocity.App;

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
		
		public void CreateStat(string outputDir, string templateDir)
		{
			using (var s = data.OpenSession())
			{
				int commits = s.Repository<Commit>().Count();
				var authors = s.Repository<Commit>()
					.Select(x => x.Author)
					.Distinct();

				Dictionary<string, CodeBlockSelectionExpression> codeByAuthor = new Dictionary<string, CodeBlockSelectionExpression>();
				foreach (var author in authors)
				{
					codeByAuthor.Add(
						author,
						s.SelectionDSL()
							.Commits().AuthorIs(author)
							.Modifications().InCommits()
							.CodeBlocks().InModifications()
							.Fixed()
					);
				}

				var statByAuthor =
					from a in codeByAuthor
					let author = a.Key
					let code = a.Value
					let authorCommits = code.Commits().Again().Count()
					select new
					{
						name = author,
						commits = string.Format("{0} ({1}%)", authorCommits, (((double)authorCommits / commits) * 100).ToString("F02")),
						dd = code.CalculateTraditionalDefectDensity().ToString("F02"),
						added = code.Added().CalculateLOC(),
						deleted = code.Deleted().CalculateLOC(),
						current = code.Added().CalculateLOC() + code.ModifiedBy().CalculateLOC(),
						addedInFixes = code.Added().InBugFixes().CalculateLOC(),
						deletedInFixes = code.Deleted().InBugFixes().CalculateLOC()
					};

				VelocityContext context = new VelocityContext();
				context.Put("authors", statByAuthor.OrderBy(x => x.name));

				File.Copy(templateDir + "/stats.css", outputDir + "/stats.css", true);
				File.Copy(templateDir + "/sortable.js", outputDir + "/sortable.js", true);

				using (TextWriter writer = new StreamWriter(outputDir + "/authors.html"))
				{
					Velocity.Init();
					Velocity.MergeTemplate(
						templateDir + "/authors.html",
						Encoding.UTF8.WebName,
						context,
						writer
					);
				}
			}
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
			//PersistentDataStoreProfiler prof = new PersistentDataStoreProfiler(data);
			
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);

				foreach (var f in selectionDSL.Files().PathIs("src/interfaces/libpq++/libpq++.h"))
				{
					Console.WriteLine(f.Path);
				}
			}

			//Console.WriteLine("{0}", prof.NumberOfQueries);
		}
		public void LocStat()
		{
			using (var s = data.OpenSession())
			{
				RepositorySelectionExpression selectionDSL = new RepositorySelectionExpression(s);
				
				var code = selectionDSL
					.Files()
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
	}
}
