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
	public abstract class Prediction
	{
		protected IRepositoryResolver repositories;
		private Dictionary<object,Type> predictors = new Dictionary<object,Type>();
		
		public Prediction(IRepositoryResolver repositories)
		{
			this.repositories = repositories;
		}
		public void AddPredictor<Exp>(Func<Exp,double> predictor)
		{
			predictors.Add(predictor, typeof(Exp));
		}
		protected IEnumerable<double> GetPredictorValuesFor<Exp>(Exp exp)
		{
			return predictors
				.Where(p => exp.GetType() == p.Value)
				.Select(p => (p.Key as Func<Exp,double>)(exp));
		}
	}
}
