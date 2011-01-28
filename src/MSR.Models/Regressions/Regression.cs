/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;

namespace MSR.Models.Regressions
{
	public abstract class Regression<INPUT>
	{
		protected List<INPUT> predictorList = new List<INPUT>();
		protected List<double> resultList = new List<double>();

		public void AddTrainingData(INPUT predictor, double result)
		{
			predictorList.Add(predictor);
			resultList.Add(result);
		}
		public abstract void Train();
		public abstract double Predict(INPUT predictor);
	}
}
