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
		
		protected IRepository repository;
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
		public virtual void Init(IRepository repository, IEnumerable<string> releases)
		{
			this.repository = repository;
			PredictionRelease = releases.Last();
			if (releases.Count() > 1)
			{
				TrainReleases = releases.Take(releases.Count() - 1);
			}
			else
			{
				TrainReleases = releases;
			}
			context = new PredictorContext(repository);
		}
		public string PredictionRelease
		{
			get; private set;
		}
		public IEnumerable<string> TrainReleases
		{
			get; private set;
		}
	}
}
