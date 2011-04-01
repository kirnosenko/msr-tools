/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using Accord.Statistics.Analysis;

namespace MSR.Models.Regressions
{
	public class LogisticRegression : Regression<double[]>
	{
		private LogisticRegressionAnalysis regression;
		
		public override void Train()
		{
			Train(predictorList.ToArray(), resultList.ToArray());
		}
		public override void Train(double[][] predictors, double[] results)
		{
			regression = new LogisticRegressionAnalysis(predictors, results);
			regression.Compute();
		}
		public override double Predict(double[] predictor)
		{
			return regression.Regression.Compute(predictor);
		}
	}
}
