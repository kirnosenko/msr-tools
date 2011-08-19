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
		private double addedCodeSize;
		private double defectCodeSize;
		private double remainCodeSizeFromRevision;
		private double addedCodeSizeFromRevision;
		private double defectCodeSizeFromRevision;
		
		public string Revision { get; set; }
		public int PathID { get; set; }
		public double RemainCodeSize { get; set; }
		public double AgeInDays { get; set; }

		public double AddedCodeSize
		{
			get
			{
				if (AddedCodeSizeResolver != null)
				{
					addedCodeSize = AddedCodeSizeResolver(Revision, PathID);
					AddedCodeSizeResolver = null;
				}
				return addedCodeSize;
			}
		}
		public double DefectCodeSize
		{
			get
			{
				if (DefectCodeSizeResolver != null)
				{
					defectCodeSize = DefectCodeSizeResolver(Revision, PathID);
					DefectCodeSizeResolver = null;
				}
				return defectCodeSize;
			}
		}
		public double RemainCodeSizeFromRevision
		{
			get
			{
				if (RemainCodeSizeFromRevisionResolver != null)
				{
					remainCodeSizeFromRevision = RemainCodeSizeFromRevisionResolver(Revision);
					RemainCodeSizeFromRevisionResolver = null;
				}
				return remainCodeSizeFromRevision;
			}
		}
		public double AddedCodeSizeFromRevision
		{
			get
			{
				if (AddedCodeSizeFromRevisionResolver != null)
				{
					addedCodeSizeFromRevision = AddedCodeSizeFromRevisionResolver(Revision);
					AddedCodeSizeFromRevisionResolver = null;
				}
				return addedCodeSizeFromRevision;
			}
		}
		public double DefectCodeSizeFromRevision
		{
			get
			{
				if (DefectCodeSizeFromRevisionResolver != null)
				{
					defectCodeSizeFromRevision = DefectCodeSizeFromRevisionResolver(Revision);
					DefectCodeSizeFromRevisionResolver = null;
				}
				return defectCodeSizeFromRevision;
			}
		}
		
		public Func<string,int,double> AddedCodeSizeResolver { private get; set; }
		public Func<string,int,double> DefectCodeSizeResolver { private get; set; }
		public Func<string,double> RemainCodeSizeFromRevisionResolver { private get; set; }
		public Func<string,double> AddedCodeSizeFromRevisionResolver { private get; set; }
		public Func<string,double> DefectCodeSizeFromRevisionResolver { private get; set; }
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
		public static double EP_REVISION(this CodeSetData codeSet, double defectLineProbability)
		{
			return 1 - Math.Pow(1 - defectLineProbability, codeSet.AddedCodeSizeFromRevision);
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
		/// Probability that code from revision has errors were not removed
		/// (remain code predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <returns></returns>
		public static double EWNRFP(this CodeSetData codeSet)
		{
			return codeSet.RemainCodeSize / codeSet.AddedCodeSize;
		}
		public static double EWNRFP_REVISION(this CodeSetData codeSet)
		{
			return codeSet.RemainCodeSizeFromRevision / codeSet.AddedCodeSizeFromRevision;
		}
		public static double EWNRFP_MIXED(this CodeSetData codeSet)
		{
			return codeSet.RemainCodeSize / codeSet.AddedCodeSizeFromRevision;
		}
		/// <summary>
		/// Probability that code from revision has errors were not removed during refactoring
		/// (code refactoring predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <returns></returns>
		public static double EWNRP(this CodeSetData codeSet)
		{
			return (codeSet.RemainCodeSize + codeSet.DefectCodeSize) / codeSet.AddedCodeSize;
		}
		public static double EWNRP_REVISION(this CodeSetData codeSet)
		{
			return (codeSet.RemainCodeSizeFromRevision + codeSet.DefectCodeSizeFromRevision) / codeSet.AddedCodeSizeFromRevision;
		}
		public static double EWNRP_MIXED(this CodeSetData codeSet)
		{
			return (codeSet.RemainCodeSize + codeSet.DefectCodeSizeFromRevision) / codeSet.AddedCodeSizeFromRevision;
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
				codeSet.DefectCodeSize + codeSet.RemainCodeSize
			);
		}
		public static double EWNFP_REVISION(this CodeSetData codeSet, double defectLineProbability)
		{
			return LaplaceIntegralTheorem(
				defectLineProbability,
				codeSet.AddedCodeSizeFromRevision,
				codeSet.DefectCodeSizeFromRevision + 1,
				codeSet.DefectCodeSizeFromRevision + codeSet.RemainCodeSizeFromRevision
			);
		}
		public static double EWNFP_MIXED(this CodeSetData codeSet, double defectLineProbability)
		{
			return LaplaceIntegralTheorem(
				defectLineProbability,
				codeSet.AddedCodeSizeFromRevision,
				codeSet.DefectCodeSizeFromRevision + 1,
				codeSet.DefectCodeSizeFromRevision + codeSet.RemainCodeSize
			);
		}
		/// <summary>
		/// Probability that errors in revision are located in specified code set.
		/// (code set size predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="defectLineProbability"></param>
		/// <returns></returns>
		public static double ELP(this CodeSetData codeSet)
		{
			return codeSet.RemainCodeSize / codeSet.RemainCodeSizeFromRevision;
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
		public static readonly CodeSetEstimationStrategy G1M0 = new G1M0();
		public static readonly CodeSetEstimationStrategy G1M1 = new G1M1();
		public static readonly CodeSetEstimationStrategy G1M2 = new G1M2();
		public static readonly CodeSetEstimationStrategy G1M3 = new G1M3();
		public static readonly CodeSetEstimationStrategy G2M0 = new G2M0();
		public static readonly CodeSetEstimationStrategy G2M1 = new G2M1();
		public static readonly CodeSetEstimationStrategy G2M2 = new G2M2();
		public static readonly CodeSetEstimationStrategy G2M3 = new G2M3();
		public static readonly CodeSetEstimationStrategy G3M1 = new G3M1();
		public static readonly CodeSetEstimationStrategy G3M2 = new G3M2();
		public static readonly CodeSetEstimationStrategy G3M3 = new G3M3();
		
		public abstract double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet);
	}

	class G1M0 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EWNRFP_MIXED();
		}
	}
	class G1M1 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EWNRP_MIXED()
				*
				codeSet.EWNFP_MIXED(model.DefectLineProbability);
		}
	}
	class G1M2 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution);
		}
	}
	class G1M3 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				(
					(
						codeSet.EFDP(model.BugLifetimeDistribution)
						+
						(
							codeSet.EWNRP_MIXED()
							*
							codeSet.EWNFP_MIXED(model.DefectLineProbability)
						)
					)
					/
					2
				);
		}
	}
	class G2M0 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EWNRFP();
		}
	}
	class G2M1 : CodeSetEstimationStrategy
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
	class G2M2 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution);
		}
	}
	class G2M3 : CodeSetEstimationStrategy
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
	class G3M1 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EWNRP_REVISION()
				*
				codeSet.EWNFP_REVISION(model.DefectLineProbability)
				*
				codeSet.ELP();
		}
	}
	class G3M2 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution)
				*
				codeSet.ELP();
		}
	}
	class G3M3 : CodeSetEstimationStrategy
	{
		public override double Estimate(CodeStabilityPostReleaseDefectFilesPrediction model, CodeSetData codeSet)
		{
			return
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				(
					(
						codeSet.EFDP(model.BugLifetimeDistribution)
						+
						(
							codeSet.EWNRP_REVISION()
							*
							codeSet.EWNFP_REVISION(model.DefectLineProbability)
						)
					)
					/
					2
				)
				*
				codeSet.ELP();
		}
	}
	
	public class CodeStabilityPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{	
		public CodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model";
			CodeSetEstimation = CodeSetEstimationStrategy.G3M3;
		}
		public override void Init(IRepositoryResolver repositories, IEnumerable<string> releases)
		{
			base.Init(repositories, releases);

			ReleaseDate = repositories.Repository<Commit>()
				.Single(x => x.Revision == PredictionRelease)
				.Date;

			RemainCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateRemainingCodeSize(PredictionRelease).Sum(x => x.Value)
			);
			AddedCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateLOC()
			);
			DefectCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease)
			);
			
			AddedCodeSizeResolver = (revision,pathid) =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(pathid)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Added().CalculateLOC();
			DefectCodeSizeResolver = (revision,pathid) =>
				repositories.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(pathid)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease);
			RemainCodeSizeFromRevisionResolver = (revision) =>
				RemainCodeSizeFromRevision[revision];
			AddedCodeSizeFromRevisionResolver = (revision) =>
				AddedCodeSizeFromRevision[revision];
			DefectCodeSizeFromRevisionResolver = (revision) =>
				DefectCodeSizeFromRevision[revision];
			
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
					PathID = file.ID,
					RemainCodeSize = rcb.Value,
					AgeInDays = (ReleaseDate - ic.Date).TotalDays,
					
					AddedCodeSizeResolver = AddedCodeSizeResolver,
					DefectCodeSizeResolver = DefectCodeSizeResolver,
					RemainCodeSizeFromRevisionResolver = RemainCodeSizeFromRevisionResolver,
					AddedCodeSizeFromRevisionResolver = AddedCodeSizeFromRevisionResolver,
					DefectCodeSizeFromRevisionResolver = DefectCodeSizeFromRevisionResolver
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
		protected IDictionary<string,double> RemainCodeSizeFromRevision
		{
			get; set;
		}
		protected IDictionary<string,double> AddedCodeSizeFromRevision
		{
			get; set;
		}
		protected IDictionary<string,double> DefectCodeSizeFromRevision
		{
			get; set;
		}
		protected Func<string,int,double> AddedCodeSizeResolver
		{
			get; set;
		}
		protected Func<string,int,double> DefectCodeSizeResolver
		{
			get; set;
		}
		protected Func<string,double> RemainCodeSizeFromRevisionResolver
		{
			get; set;
		}
		protected Func<string,double> AddedCodeSizeFromRevisionResolver
		{
			get; set;
		}
		protected Func<string,double> DefectCodeSizeFromRevisionResolver
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