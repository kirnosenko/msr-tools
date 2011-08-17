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
	struct CodeSetData
	{
		public string Revision { get; set; }
		public double CodeSize { get; set; }
		public double AddedCodeSize { get; set; }
		public double DefectCodeSize { get; set; }
		public double AgeInDays { get; set; }
	}

	static class CodeSetDataMetrics
	{
		/// <summary>
		/// Probability that code from revision has errors
		/// (code size predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="DefectLineProbability"></param>
		/// <returns></returns>
		public static double EP(this CodeSetData codeSet, double defectLineProbability)
		{
			return 1 - Math.Pow(1 - defectLineProbability, codeSet.AddedCodeSize);
		}
		/// <summary>
		/// Probability that code from revision has errors will be detected in future
		/// (code age predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="bugLifetimeDistribution"></param>
		/// <returns></returns>
		public static double EFDP(this CodeSetData codeSet, Func<double,double> bugLifetimeDistribution)
		{
			return 1 - bugLifetimeDistribution(codeSet.AgeInDays);
		}
		/// <summary>
		/// Probability that code from revision has errors were not removed during refactoring
		/// (code refactoring predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <returns></returns>
		public static double EWNRP(this CodeSetData codeSet)
		{
			return (codeSet.CodeSize + codeSet.DefectCodeSize) / codeSet.AddedCodeSize;
		}
		/// <summary>
		/// Probability that code from revision has errors were not fixed before
		/// (fixed code predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="defectLineProbability"></param>
		/// <returns></returns>
		public static double EWNFP(this CodeSetData codeSet, double defectLineProbability)
		{
			return LaplaceIntegralTheorem(
				defectLineProbability,
				codeSet.AddedCodeSize,
				codeSet.DefectCodeSize + 1,
				codeSet.DefectCodeSize + codeSet.CodeSize
			);
		}
		
		static double LaplaceIntegralTheorem(double p, double n, double k1, double k2)
		{
			double q = 1 - p;
			double from = (k1 - n * p) / Math.Sqrt(n * p * q);
			double to = (k2 - n * p) / Math.Sqrt(n * p * q);

			return (1 / Math.Sqrt(2 * Math.PI)) * Integral(x => Math.Exp(-Math.Pow(x, 2) / 2), from, to);
		}
		static double Integral(Func<double, double> func, double from, double to)
		{
			double delta = 0.001;
			double sum = 0;

			for (double x = from + delta / 2; x < to; x += delta)
			{
				sum += func(x) * delta;
			}

			return sum;
		}
	}

	abstract class CodeSetEstimationStrategy
	{
		public static readonly CodeSetEstimationStrategy M0 = new M0();
		public static readonly CodeSetEstimationStrategy M1 = new M1();
		public static readonly CodeSetEstimationStrategy M2 = new M2();

		public abstract double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet);
	}

	class M0 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EWNRP()
				*
				codeSet.EWNFP(model.DefectLineProbability);
		}
	}
	class M1 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution);
		}
	}
	class M2 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP(model.DefectLineProbability)
				*
				(
					(
						codeSet.EFDP(model.BugLifetimeDistribution)
						+
						(
							codeSet.EWNRP()
							*
							codeSet.EWNFP(model.DefectLineProbability)
						)
					)
					/
					2
				);
		}
	}
	
	public class CodeStabilityPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{	
		public CodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model";
			CodeSetEstimation = CodeSetEstimationStrategy.M2;
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);

			ReleaseDate = repositories.Repository<Commit>()
				.Single(x => x.Revision == PredictionRelease)
				.Date;

			RemainCodeSizeByRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateRemainingCodeSize(PredictionRelease).Sum(x => x.Value)
			);
			AddedCodeSizeByRevision = new SmartDictionary<string,double>(r =>
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
			
			AddedCodeSize = (revision,pathid) =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(pathid)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Added().CalculateLOC();
			DefectCodeSize = (revision,pathid) =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(pathid)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease);
			
			EstimateDefectLineProbability(repositories);
			EstimateBugLifetimeDistribution(repositories);
		}
		public double DefectLineProbability
		{
			get; protected set;
		}
		public Func<double, double> BugLifetimeDistribution
		{
			get; protected set;
		}
		
		protected override double GetFileEstimation(ProjectFile file)
		{
			var codeBlocks = repositories.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().IdIs(file.ID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateRemainingCodeSize(PredictionRelease);

			var codeByRevision = (
				from rcb in codeBlocks
				join cb in repositories.Repository<CodeBlock>() on rcb.Key equals cb.ID
				join m in repositories.Repository<Modification>() on cb.ModificationID equals m.ID
				join c in repositories.Repository<Commit>() on m.CommitID equals c.ID
				join ic in repositories.Repository<Commit>() on cb.AddedInitiallyInCommitID equals ic.ID
				select new CodeSetData()
				{
					Revision = c.Revision,
					CodeSize = rcb.Value,
					AddedCodeSize = AddedCodeSize(c.Revision,file.ID),
					DefectCodeSize = DefectCodeSize(c.Revision,file.ID),
					AgeInDays = (ReleaseDate - ic.Date).TotalDays
				}).ToArray();

			double fileHasDefectsProbability = 0;
			foreach (var codeFromRevision in codeByRevision)
			{
				double codeFromRevisionHasDefectsProbability = CodeSetEstimation.Estimate(this, codeFromRevision);
				
				fileHasDefectsProbability =
					(fileHasDefectsProbability + codeFromRevisionHasDefectsProbability)
					-
					(fileHasDefectsProbability * codeFromRevisionHasDefectsProbability);
			}
			
			return fileHasDefectsProbability;
		}
		protected virtual void EstimateDefectLineProbability(IRepositoryResolver repositories)
		{
			DefectLineProbability = repositories.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensity(PredictionRelease);
			/*
			double stabilizationPeriod = repositories.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.BugFixes().InCommits().CalculateStabilizationPeriod(0.9);

			var stableCode = repositories.SelectionDSL()
				.Commits().DateIsLesserOrEquelThan(ReleaseDate.AddDays(- stabilizationPeriod))
				.Files().Reselect(FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();

			DefectLineProbability = stableCode.CalculateDefectCodeDensity(PredictionRelease);*/
		}
		protected virtual void EstimateBugLifetimeDistribution(IRepositoryResolver repositories)
		{
			List<double> bugLifetimes = new List<double>(
				repositories.SelectionDSL()
					.Commits().TillRevision(PredictionRelease)
					.BugFixes().InCommits().CalculateAvarageBugLifetime()
			);
			bugLifetimes.Add(1000000);
			
			BugLifetimeDistribution = time =>
			{
				return (double)bugLifetimes.Where(t => t <= time).Count() / bugLifetimes.Count();
			};
		}
		protected IDictionary<string,double> RemainCodeSizeByRevision
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
		protected Func<string,int,double> AddedCodeSize
		{
			get; set;
		}
		protected Func<string,int,double> DefectCodeSize
		{
			get; set;
		}
		protected DateTime ReleaseDate
		{
			get; set;
		}
		
		private CodeSetEstimationStrategy CodeSetEstimation
		{
			get; set;
		}
	}
}