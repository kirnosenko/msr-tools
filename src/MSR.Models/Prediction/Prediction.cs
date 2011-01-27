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
		protected IRepositoryResolver repositories;
		private List<Func<PredictorContext,double>> predictors = new List<Func<PredictorContext,double>>();
		protected PredictorContext context;
		
		public Prediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
			context = new PredictorContext(repositories);
		}
		public void AddPredictor(Func<PredictorContext,double> predictor)
		{
			predictors.Add(predictor);
		}
		public double[] GetPredictorValuesFor(PredictorContext c)
		{
			return predictors.Select(p => p(c)).ToArray();
		}
	}
}
