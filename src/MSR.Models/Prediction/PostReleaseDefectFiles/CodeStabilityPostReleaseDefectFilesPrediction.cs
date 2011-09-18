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
		/// Number of residual defect lines in code from revision
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="defectLineProbability"></param>
		/// <returns></returns>
		public static double DLN_REVISION(this CodeSetData codeSet, double defectLineProbability)
		{
			return defectLineProbability * codeSet.AddedCodeSizeFromRevision;
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
		/// <summary>
		/// Probability that errors in revision are located in specified code set.
		/// (code set size predictor)
		/// </summary>
		/// <param name="codeSet"></param>
		/// <param name="defectLineProbability"></param>
		/// <returns></returns>
		public static double ESP(this CodeSetData codeSet)
		{
			return codeSet.RemainCodeSize / codeSet.RemainCodeSizeFromRevision;
		}
		public static double EISP(this CodeSetData codeSet)
		{
			return codeSet.AddedCodeSize / codeSet.AddedCodeSizeFromRevision;
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

	abstract class FileEstimationStrategy
	{
		protected CodeStabilityPostReleaseDefectFilesPrediction model;
		protected List<double> codeSetEstimations;

		public virtual void Init(IRepositoryResolver repositories, CodeStabilityPostReleaseDefectFilesPrediction model)
		{
			this.model = model;
		}
		public void NewFile()
		{
			codeSetEstimations = new List<double>();
		}
		public abstract void NewCodeSet(CodeSetData codeSet);
		public virtual double FileEstimation
		{
			get
			{
				double fileHasDefectsProbability = 0;
				
				foreach (var codeSetEstimation in codeSetEstimations)
				{
					fileHasDefectsProbability =
						(fileHasDefectsProbability + codeSetEstimation)
						-
						(fileHasDefectsProbability * codeSetEstimation);
				}
				
				return fileHasDefectsProbability;
			}
		}
		public virtual double DefaultCutOffValue
		{
			get { return 0.5; }
		}
		public virtual double RocEvaluationDelta
		{
			get { return 0.01; }
		}
	}
	
	class G2M0 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EWNRFP()
			);
		}
	}
	class G2M1 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EWNRP()
				*
				codeSet.EWNFP(model.DefectLineProbability)
			);
		}
	}
	class G2M2 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution)
			);
		}
	}
	class G2M3 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
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
				)
			);
		}
	}
	class G3M1 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EWNRP_REVISION()
				*
				codeSet.EWNFP_REVISION(model.DefectLineProbability)
				*
				codeSet.ESP()
			);
		}
	}
	class G3M2 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution)
				*
				codeSet.ESP()
			);
		}
	}
	class G3M3 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
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
				codeSet.ESP()
			);
		}
	}
	class G3M4 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				(
					Math.Min(
						codeSet.EFDP(model.BugLifetimeDistribution),
						codeSet.EWNRP_REVISION() * codeSet.EWNFP_REVISION(model.DefectLineProbability)
					)
				)
				*
				codeSet.ESP()
			);
		}
	}
	class G3M5 : FileEstimationStrategy
	{
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP_REVISION(model.DefectLineProbability)
				*
				(
					Math.Max(
						codeSet.EFDP(model.BugLifetimeDistribution),
						codeSet.EWNRP_REVISION() * codeSet.EWNFP_REVISION(model.DefectLineProbability)
					)
				)
				*
				codeSet.ESP()
			);
		}
	}
	class G4M1 : FileEstimationStrategy
	{
		protected double defectCodeSizePerDefect;

		public override void Init(IRepositoryResolver repositories, CodeStabilityPostReleaseDefectFilesPrediction model)
		{
			base.Init(repositories, model);
			
			defectCodeSizePerDefect = repositories.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.Files().Reselect(model.FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeSizePerDefect(model.PredictionRelease);
		}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.DLN_REVISION(model.DefectLineProbability)
				*
				codeSet.EWNRP_REVISION()
				*
				codeSet.EWNFP_REVISION(model.DefectLineProbability)
				*
				codeSet.ESP()
			);
		}
		public override double FileEstimation
		{
			get
			{
				double numberOfDefectLinesInFile = 0;

				foreach (var codeSetEstimation in codeSetEstimations)
				{
					numberOfDefectLinesInFile += codeSetEstimation;
				}

				return numberOfDefectLinesInFile;
			}
		}
		public override double DefaultCutOffValue
		{
			get { return defectCodeSizePerDefect; }
		}
		public override double RocEvaluationDelta
		{
			get
			{
				double maxFileEstimation = model.FileEstimations.Max();
				return (maxFileEstimation + maxFileEstimation / 100) / 100;
			}
		}
	}

	abstract class BugLifetimeDistributionEstimationStrategy
	{
		public Func<double,double> Estimate(IRepositoryResolver repositories, string predictionRelease)
		{
			List<double> bugLifetimes = new List<double>(BugLifetimes(
				repositories.SelectionDSL()
					.Commits().TillRevision(predictionRelease)
					.BugFixes().InCommits()
			));
			bugLifetimes.Add(1000000);

			return Distribution(bugLifetimes);
		}
		protected virtual IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateAvarageBugLifetime();
		}
		protected abstract Func<double,double> Distribution(IEnumerable<double> bugLifetimes);
	}
	class BugLifetimeDistributionExperimental : BugLifetimeDistributionEstimationStrategy
	{
		protected override Func<double,double> Distribution(IEnumerable<double> bugLifetimes)
		{
			return time =>
			{
				return (double)bugLifetimes.Where(t => t <= time).Count() / bugLifetimes.Count();
			};
		}
	}
	class BugLifetimeDistributionExperimentalMin : BugLifetimeDistributionExperimental
	{
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMinBugLifetime();
		}
	}
	class BugLifetimeDistributionExperimentalMax : BugLifetimeDistributionExperimental
	{
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMaxBugLifetime();
		}
	}
	class BugLifetimeDistributionExponential : BugLifetimeDistributionEstimationStrategy
	{
		protected override Func<double,double> Distribution(IEnumerable<double> bugLifetimes)
		{
			ExponentialRegression expRegression = new ExponentialRegression();
			foreach (var time in bugLifetimes)
			{
				expRegression.AddTrainingData(time, (double)bugLifetimes.Where(t => t <= time).Count() / bugLifetimes.Count());
			}
			expRegression.Train();
			return time =>
			{
				return expRegression.Predict(time);
			};
		}
	}
	class BugLifetimeDistributionExponentialMin : BugLifetimeDistributionExponential
	{
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMinBugLifetime();
		}
	}
	class BugLifetimeDistributionExponentialMax : BugLifetimeDistributionExponential
	{
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMaxBugLifetime();
		}
	}
	
	public class CodeStabilityPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public CodeStabilityPostReleaseDefectFilesPrediction()
		{
			Title = "Code stability model";
			FileEstimation = new G3M3();
			BugLifetimeDistributionEstimation = new BugLifetimeDistributionExperimental();
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
			
			FileEstimation.Init(repositories, this);
		}
		public double DefectLineProbability
		{
			get; protected set;
		}
		public Func<double,double> BugLifetimeDistribution
		{
			get; protected set;
		}
		public override string[] PredictedDefectFiles
		{
			get
			{
				defaultCutOffValue = FileEstimation.DefaultCutOffValue;
				return base.PredictedDefectFiles;
			}
		}
		public override ROCEvaluationResult EvaluateUsingROC()
		{
			rocEvaluationDelta = FileEstimation.RocEvaluationDelta;
			return base.EvaluateUsingROC();
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
				
			FileEstimation.NewFile();
			foreach (var codeFromRevision in codeByRevision)
			{
				FileEstimation.NewCodeSet(codeFromRevision);
			}
			
			return FileEstimation.FileEstimation;
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
			BugLifetimeDistribution = BugLifetimeDistributionEstimation.Estimate(
				repositories,
				PredictionRelease
			);
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
		
		private FileEstimationStrategy FileEstimation
		{
			get; set;
		}
		private BugLifetimeDistributionEstimationStrategy BugLifetimeDistributionEstimation
		{
			get; set;
		}
	}
}