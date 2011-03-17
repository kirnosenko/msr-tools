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

namespace MSR.Models.Prediction
{
	public class Prediction
	{
		private List<Func<PredictorContext,double>> predictors = new List<Func<PredictorContext,double>>();
		
		protected IRepositoryResolver repositories;
		protected string[] releaseRevisions;
		protected PredictorContext context;
		
		public string Title
		{
			get; set;
		}
		public void AddPredictor(Func<PredictorContext,double> predictor)
		{
			predictors.Add(predictor);
		}
		public double[] GetPredictorValuesFor(PredictorContext c)
		{
			return predictors.Select(p => p(c)).ToArray();
		}
		public virtual void Init(IRepositoryResolver repositories, string[] releaseRevisions)
		{
			this.repositories = repositories;
			this.releaseRevisions = releaseRevisions;
			context = new PredictorContext(repositories);
		}
		protected string LastReleaseRevision
		{
			get { return releaseRevisions.Last(); }
		}
		protected string NextToLastReleaseRevision
		{
			get { return PreReleaseRevisions.Last(); }
		}
		protected IEnumerable<string> PreReleaseRevisions
		{
			get { return releaseRevisions.Take(releaseRevisions.Count() - 1); }
		}
	}
}
