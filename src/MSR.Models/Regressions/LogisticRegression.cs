/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Analysis;

namespace MSR.Models.Regressions
{
	public class LogisticRegression
	{
		private List<double[]> predictorsList = new List<double[]>();
		private List<double> resultList = new List<double>();
		private LogisticRegressionAnalysis lr;
		
		public void AddTrainingData(double[] predictors, double result)
		{
			predictorsList.Add(predictors);
			resultList.Add(result);
		}
		public void Train()
		{
			lr = new LogisticRegressionAnalysis(
				predictorsList.ToArray(),
				resultList.ToArray()
			);
			lr.Compute();
		}
		public double Predict(double[] predictors)
		{
			return lr.Regression.Compute(predictors);
		}
	}
}
