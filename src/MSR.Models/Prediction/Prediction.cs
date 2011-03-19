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
		public virtual void Init(IRepositoryResolver repositories, IEnumerable<string> releaseRevisions)
		{
			this.repositories = repositories;
			PreReleaseRevisions = releaseRevisions.Take(releaseRevisions.Count() - 1);
			LastReleaseRevision = releaseRevisions.Last();
			NextToLastReleaseRevision = PreReleaseRevisions.Last();
			context = new PredictorContext(repositories);
		}
		public string LastReleaseRevision
		{
			get; private set;
		}
		public string NextToLastReleaseRevision
		{
			get; private set;
		}
		public IEnumerable<string> PreReleaseRevisions
		{
			get; private set;
		}
	}
}
