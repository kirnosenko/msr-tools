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
	public struct CodeSetData
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

	public abstract class FileEstimationStrategy
	{
		protected CodeStabilityPostReleaseDefectFilesPrediction model;
		protected List<double> codeSetEstimations;
		
		public FileEstimationStrategy(CodeStabilityPostReleaseDefectFilesPrediction model)
		{
			this.model = model;
			model.OnInit += Init;
			model.OnNewFile += NewFile;
			model.OnNewCodeSet += NewCodeSet;
		}
		public virtual void Init(IRepository repository)
		{
		}
		public virtual void NewFile(ProjectFile file)
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
		public G2M0(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G2M1(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G2M2(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G2M3(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
	class G2M4 : FileEstimationStrategy
	{
		public G2M4(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EWNRP_REVISION()
				*
				codeSet.EWNFP_REVISION(model.DefectLineProbability)
			);
		}
	}
	class G2M6 : FileEstimationStrategy
	{
		public G2M6(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
							codeSet.EWNRP_REVISION()
							*
							codeSet.EWNFP_REVISION(model.DefectLineProbability)
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
		public G3M1(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G3M2(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G3M3(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G3M4(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public G3M5(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
	
	abstract class G4 : FileEstimationStrategy
	{
		protected double cutOffValue;
		
		public G4(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
			EstimateCutOffValue = false;
		}
		public override void Init(IRepository repository)
		{
			base.Init(repository);
			
			if (! EstimateCutOffValue)
			{
				cutOffValue = 1;
			}
			else
			{
				var fixCommits = repository.SelectionDSL()
					.Commits().TillRevision(model.PredictionRelease).AreBugFixes();
				var defectCount = fixCommits.Count();
				var deletedLoc = - fixCommits
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Deleted().CalculateLOC();
				var deletedLocPerDefect = deletedLoc / defectCount;
				
				var touchedFiles = 
					(
						from c in fixCommits
						join m in repository.Queryable<Modification>() on c.ID equals m.CommitID
						join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
						select f
					).Count();
				
				var touchedFilesPerDefect = (float)touchedFiles / defectCount;
				
				cutOffValue = deletedLocPerDefect / touchedFilesPerDefect;
			}
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
			get { return cutOffValue; }
		}
		public override double RocEvaluationDelta
		{
			get
			{
				double maxFileEstimation = model.FileEstimations.Max();
				return (maxFileEstimation + maxFileEstimation / 100) / 100;
			}
		}
		public bool EstimateCutOffValue
		{
			get; set;
		}
	}
	class G4M1 : G4
	{
		public G4M1(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
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
	}
	class G4M2 : G4
	{
		public G4M2(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
		}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.DLN_REVISION(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution)
				*
				codeSet.ESP()
			);
		}
	}
	class G4M3 : G4
	{
		public G4M3(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
		}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				((
					(
						codeSet.DLN_REVISION(model.DefectLineProbability)
						*
						codeSet.EWNRP_REVISION()
						*
						codeSet.EWNFP_REVISION(model.DefectLineProbability)
					)
					+
					(
						codeSet.DLN_REVISION(model.DefectLineProbability)
						*
						codeSet.EFDP(model.BugLifetimeDistribution)
					)
				) / 2)
				*
				codeSet.ESP()
			);
		}
	}

	class G5M1 : FileEstimationStrategy
	{
		public G5M1(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
	class G5M2 : FileEstimationStrategy
	{
		public G5M2(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				codeSet.EP(model.DefectLineProbability)
				*
				codeSet.EFDP(model.BugLifetimeDistribution)
			);
		}
	}
	class G5M3 : FileEstimationStrategy
	{
		public G5M3(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		public override void NewCodeSet(CodeSetData codeSet)
		{
			codeSetEstimations.Add(
				(
					(
						codeSet.EP_REVISION(model.DefectLineProbability)
						*
						codeSet.EWNRP_REVISION()
						*
						codeSet.EWNFP_REVISION(model.DefectLineProbability)
						*
						codeSet.ESP()
					)
					+
					(
						codeSet.EP(model.DefectLineProbability)
						*
						codeSet.EFDP(model.BugLifetimeDistribution)
					)
				) / 2
			);
		}
	}
	
	public abstract class DefectLineProbabilityEstimationStrategy
	{
		protected CodeStabilityPostReleaseDefectFilesPrediction model;

		public DefectLineProbabilityEstimationStrategy(CodeStabilityPostReleaseDefectFilesPrediction model)
		{
			this.model = model;
			model.OnInit += Init;
		}
		public abstract void Init(IRepository repository);
		public double Estimation
		{
			get; protected set;
		}
	}
	class DefectLineProbabilityForTheWholeCode : DefectLineProbabilityEstimationStrategy
	{
		public DefectLineProbabilityForTheWholeCode(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		public override void Init(IRepository repository)
		{
			Estimation = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.Files().Reselect(model.FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease);
		}
	}
	class DefectLineProbabilityForTheStableCode : DefectLineProbabilityEstimationStrategy
	{
		public DefectLineProbabilityForTheStableCode(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		public override void Init(IRepository repository)
		{
			double stabilizationPeriod = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.BugFixes().InCommits().CalculateStabilizationPeriod(0.9);

			var stableCode = repository.SelectionDSL()
				.Commits().DateIsLesserOrEquelThan(model.ReleaseDate.AddDays(-stabilizationPeriod))
				.Files().Reselect(model.FileSelector)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications();

			Estimation = stableCode.CalculateDefectCodeDensity(model.PredictionRelease);
		}
	}
	class DefectLineProbabilityForTheCodeOfAuthor : DefectLineProbabilityEstimationStrategy
	{
		private IDictionary<string,string> authorByRevision;
		private SmartDictionary<string,double> defectLineProbabilityOfAuthor;
		
		public DefectLineProbabilityForTheCodeOfAuthor(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
			model.OnNewCodeSet += NewCodeSet;
		}
		public override void Init(IRepository repository)
		{
			authorByRevision = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.ToDictionary(x => x.Revision, x => x.Author);
			
			defectLineProbabilityOfAuthor = new SmartDictionary<string,double>(a =>
				repository.SelectionDSL()
					.Commits().AuthorIs(a).BeforeRevision(model.PredictionRelease)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease)
			);
		}
		protected void NewCodeSet(CodeSetData codeSet)
		{
			Estimation = defectLineProbabilityOfAuthor[authorByRevision[codeSet.Revision]];
		}
	}
	class DefectLineProbabilityForTheCodeInFile : DefectLineProbabilityEstimationStrategy
	{
		protected IRepository repository;

		public DefectLineProbabilityForTheCodeInFile(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
			model.OnNewFile += NewFile;
		}
		public override void Init(IRepository repository)
		{
			this.repository = repository;
		}
		protected void NewFile(ProjectFile file)
		{
			Estimation = repository.SelectionDSL()
				.Commits().BeforeRevision(model.PredictionRelease)
				.Files().IdIs(file.ID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease);
		}
	}
	class DefectLineProbabilityForTheCodeOfAuthorInFileAverage : DefectLineProbabilityEstimationStrategy
	{
		protected DefectLineProbabilityForTheCodeOfAuthor codeOfAuthor;
		protected DefectLineProbabilityForTheCodeInFile codeInFile;
		
		public DefectLineProbabilityForTheCodeOfAuthorInFileAverage(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
			codeOfAuthor = new DefectLineProbabilityForTheCodeOfAuthor(model);
			codeInFile = new DefectLineProbabilityForTheCodeInFile(model);
			model.OnNewCodeSet += NewCodeSet;
		}
		public override void Init(IRepository repository)
		{
		}
		protected void NewCodeSet(CodeSetData codeSet)
		{
			Estimation = (codeOfAuthor.Estimation + codeInFile.Estimation) - (codeOfAuthor.Estimation * codeInFile.Estimation);
		}
	}
	class DefectLineProbabilityForTheCodeOfAuthorInFileRegression : DefectLineProbabilityEstimationStrategy
	{
		private Dictionary<int,double> DCDF;
		private Dictionary<string,double> DCDA;
		private Dictionary<string,string> authorByRevision;
		
		private double fileEstimation;
		
		public DefectLineProbabilityForTheCodeOfAuthorInFileRegression(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{
			model.OnNewFile += NewFile;
			model.OnNewCodeSet += NewCodeSet;
		}
		public override void Init(IRepository repository)
		{
			authorByRevision = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.ToDictionary(x => x.Revision, x => x.Author);
			
			int releaseRevisionOrderedNumber = repository.Queryable<Commit>()
				.Single(x => x.Revision == model.PredictionRelease).OrderedNumber;
			var codeByAuthorAndFile =
				(
					from cb in repository.Queryable<CodeBlock>()
					join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
					join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
					join f in repository.Queryable<ProjectFile>() on m.FileID equals f.ID
					where
						cb.Size > 0
						&&
						c.OrderedNumber <= releaseRevisionOrderedNumber
					group cb by new { Author = c.Author, FileID = f.ID } into g
					select new
					{
						Author = g.Key.Author,
						FileID = g.Key.FileID,
						CodeSize = g.Sum(x => x.Size)
					}
				).ToArray();

			var allFiles = model.AllFiles.Select(x => x.ID).ToArray();
			var allAuthors = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.Select(x => x.Author).Distinct().ToArray();
			
			int numberOfFiles = allFiles.Count();
			int numberOfAuthors = allAuthors.Count();
			int numberOfEquations = numberOfFiles + numberOfAuthors + 1;
			double[,] equations = new double[numberOfEquations,numberOfEquations];
			double[] results = new double[numberOfEquations];

			int equation = 0;
			double locAdded = codeByAuthorAndFile.Sum(x => x.CodeSize);
			
			for (int i = 0; i < allFiles.Length; i++)
			{
				double locAddedInFile = codeByAuthorAndFile
					.Where(x => x.FileID == allFiles[i])
					.Sum(x => x.CodeSize);
				equations[numberOfEquations-1, i] = locAddedInFile / locAdded;
				equations[equation, i] = 1;
				
				if (locAddedInFile > 0)
				{
					double dcd = repository.SelectionDSL()
						.Commits()
							.TillRevision(model.PredictionRelease)
						.Files()
							.IdIs(allFiles[i])
						.Modifications().InCommits().InFiles()
						.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease);
					
					for (int j = 0; j < allAuthors.Length; j++)
					{
						double locAddedInFileByAuthor = codeByAuthorAndFile
							.Where(x => x.FileID == allFiles[i] && x.Author == allAuthors[j])
							.Sum(x => x.CodeSize);
						equations[equation, numberOfFiles + j] = (locAddedInFileByAuthor / locAddedInFile);
					}
					results[equation] = dcd;
				}
				equation++;
			}
			
			for (int i = 0; i < allAuthors.Length; i++)
			{
				double locAddedByAuthor = codeByAuthorAndFile
					.Where(x => x.Author == allAuthors[i])
					.Sum(x => x.CodeSize);
				equations[numberOfEquations-1, numberOfFiles + i] = locAddedByAuthor / locAdded;
				equations[equation, numberOfFiles + i] = 1;
				
				if (locAddedByAuthor > 0)
				{
					double dcd = repository.SelectionDSL()
						.Commits()
							.AuthorIs(allAuthors[i])
							.TillRevision(model.PredictionRelease)
						.Modifications().InCommits()
						.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease);
					for (int j = 0; j < allFiles.Length; j++)
					{
						double locAddedByAuthorInFile = codeByAuthorAndFile
							.Where(x => x.Author == allAuthors[i] && x.FileID == allFiles[j])
							.Sum(x => x.CodeSize);
						equations[equation, j] = (locAddedByAuthorInFile / locAddedByAuthor);
					}
					results[equation] = dcd;
				}
				equation++;
			}
			
			results[numberOfEquations-1] = repository.SelectionDSL()
				.Commits()
					.TillRevision(model.PredictionRelease)
				.Modifications().InCommits()
				.CodeBlocks().InModifications().CalculateDefectCodeDensity(model.PredictionRelease);
			
			int varCount = equations.GetUpperBound(1)+1;
			double[,] normalEquations = new double[varCount, varCount];
			double[] normalResults = new double[varCount];
			Func<double[,],double[],int,int,double> nc = (e, r, n1, n2) =>
			{
				double sum = 0;
				for (int i = 0; i < numberOfEquations; i++)
				{
					if (n2 < numberOfEquations)
					{
						sum += e[i,n1] * e[i,n2];
					}
					else
					{
						sum += e[i,n1] * r[i];
					}
				}
				return sum;
			};
			
			for (int i = 0; i < varCount; i++)
			{
				for (int j = 0; j < varCount; j++)
				{
					normalEquations[i,j] = nc(equations,results,i,j);
				}
				normalResults[i] = nc(equations,results,i,numberOfEquations);
			}
			var DCD = new Accord.Math.Decompositions.SingularValueDecomposition(normalEquations).Solve(normalResults);
			
			DCDF = new Dictionary<int,double>();
			DCDA = new Dictionary<string,double>();
			double DCD_min = 0.001;
			for (int i = 0; i < allFiles.Length; i++)
			{
				DCDF.Add(allFiles[i], DCD[i] > 0 ? DCD[i] : DCD_min);
			}
			for (int i = 0; i < allAuthors.Length; i++)
			{
				DCDA.Add(allAuthors[i], DCD[numberOfFiles + i] > 0 ? DCD[numberOfFiles + i] : DCD_min);
			}
		}
		protected void NewFile(ProjectFile file)
		{
			fileEstimation = DCDF[file.ID];
		}
		protected void NewCodeSet(CodeSetData codeSet)
		{
			Estimation = fileEstimation + DCDA[authorByRevision[codeSet.Revision]];
		}
	}
	
	public abstract class BugLifetimeDistributionEstimationStrategy
	{
		protected CodeStabilityPostReleaseDefectFilesPrediction model;
		
		public BugLifetimeDistributionEstimationStrategy(CodeStabilityPostReleaseDefectFilesPrediction model)
		{
			this.model = model;
			model.OnInit += Init;
		}
		public virtual void Init(IRepository repository)
		{
			Estimation = Estimate(repository);
		}
		public Func<double,double> Estimation
		{
			get; protected set;
		}
		protected virtual Func<double,double> Estimate(IRepository repository)
		{
			List<double> bugLifetimes = new List<double>(
				BugLifetimes(
					BugFixes(repository)
				)
			);
			bugLifetimes.Add(1000000);

			return Distribution(bugLifetimes);
		}
		protected virtual BugFixSelectionExpression BugFixes(IRepository repository)
		{
			return repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.BugFixes().InCommits();
		}
		protected virtual IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateAvarageBugLifetime();
		}
		protected abstract Func<double,double> Distribution(IEnumerable<double> bugLifetimes);
	}
	class BugLifetimeDistributionExperimental : BugLifetimeDistributionEstimationStrategy
	{
		public BugLifetimeDistributionExperimental(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public BugLifetimeDistributionExperimentalMin(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMinBugLifetime();
		}
	}
	class BugLifetimeDistributionExperimentalMax : BugLifetimeDistributionExperimental
	{
		public BugLifetimeDistributionExperimentalMax(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMaxBugLifetime();
		}
	}
	class BugLifetimeDistributionExperimentalForStableCode : BugLifetimeDistributionExperimental
	{
		public BugLifetimeDistributionExperimentalForStableCode(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		protected override BugFixSelectionExpression BugFixes(IRepository repository)
		{
			double stabilizationPeriod = repository.SelectionDSL()
				.Commits().TillRevision(model.PredictionRelease)
				.BugFixes().InCommits().CalculateStabilizationPeriod(0.9);

			var stableCommits = repository.SelectionDSL()
				.Commits().DateIsLesserOrEquelThan(model.ReleaseDate.AddDays(-stabilizationPeriod));
				
			return stableCommits.BugFixes().InCommits();
		}
	}
	class BugLifetimeDistributionExponential : BugLifetimeDistributionEstimationStrategy
	{
		public BugLifetimeDistributionExponential(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
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
		public BugLifetimeDistributionExponentialMin(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMinBugLifetime();
		}
	}
	class BugLifetimeDistributionExponentialMax : BugLifetimeDistributionExponential
	{
		public BugLifetimeDistributionExponentialMax(CodeStabilityPostReleaseDefectFilesPrediction model)
			: base(model)
		{}
		protected override IEnumerable<double> BugLifetimes(BugFixSelectionExpression bugFixes)
		{
			return bugFixes.CalculateMaxBugLifetime();
		}
	}
	
	public abstract class CodeStabilityPostReleaseDefectFilesPrediction : PostReleaseDefectFilesPrediction
	{
		public event Action<IRepository> OnInit;
		public event Action<ProjectFile> OnNewFile;
		public event Action<CodeSetData> OnNewCodeSet;
		
		public override void Init(IRepository repository, IEnumerable<string> releases)
		{
			base.Init(repository, releases);

			ReleaseDate = repository.Queryable<Commit>()
				.Single(x => x.Revision == PredictionRelease)
				.Date;

			RemainCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repository.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateRemainingCodeSize(PredictionRelease).Sum(x => x.Value)
			);
			AddedCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repository.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().Added().CalculateLOC()
			);
			DefectCodeSizeFromRevision = new SmartDictionary<string,double>(r =>
				repository.SelectionDSL()
					.Commits().RevisionIs(r)
					.Modifications().InCommits()
					.CodeBlocks().InModifications().CalculateDefectCodeSize(PredictionRelease)
			);
			
			AddedCodeSizeResolver = (revision,pathid) =>
				repository.SelectionDSL()
					.Commits().RevisionIs(revision)
					.Files().IdIs(pathid)
					.Modifications().InCommits().InFiles()
					.CodeBlocks().InModifications().Added().CalculateLOC();
			DefectCodeSizeResolver = (revision,pathid) =>
				repository.SelectionDSL()
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

			OnInit(repository);
		}
		public double DefectLineProbability
		{
			get { return DefectLineProbabilityEstimation.Estimation; }
		}
		public Func<double,double> BugLifetimeDistribution
		{
			get { return BugLifetimeDistributionEstimation.Estimation; }
		}
		public override ROCEvaluationResult EvaluateUsingROC()
		{
			rocEvaluationDelta = FileEstimation.RocEvaluationDelta;
			return base.EvaluateUsingROC();
		}
		public DateTime ReleaseDate
		{
			get; protected set;
		}

		protected override double DefaultCutOffValue
		{
			get { return FileEstimation.DefaultCutOffValue; }
		}
		protected override double GetFileEstimation(ProjectFile file)
		{
			var codeBlocks = repository.SelectionDSL()
				.Commits().TillRevision(PredictionRelease)
				.Files().IdIs(file.ID)
				.Modifications().InCommits().InFiles()
				.CodeBlocks().InModifications().CalculateRemainingCodeSize(PredictionRelease);

			var codeByRevision = (
				from rcb in codeBlocks
				join cb in repository.Queryable<CodeBlock>() on rcb.Key equals cb.ID
				join m in repository.Queryable<Modification>() on cb.ModificationID equals m.ID
				join c in repository.Queryable<Commit>() on m.CommitID equals c.ID
				join ic in repository.Queryable<Commit>() on cb.AddedInitiallyInCommitID equals ic.ID
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
			
			OnNewFile(file);
			foreach (var codeFromRevision in codeByRevision)
			{
				OnNewCodeSet(codeFromRevision);
			}
			
			return FileEstimation.FileEstimation;
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
		protected FileEstimationStrategy FileEstimation
		{
			get; set;
		}
		protected BugLifetimeDistributionEstimationStrategy BugLifetimeDistributionEstimation
		{
			get; set;
		}
		protected DefectLineProbabilityEstimationStrategy DefectLineProbabilityEstimation
		{
			get; set;
		}
	}
}