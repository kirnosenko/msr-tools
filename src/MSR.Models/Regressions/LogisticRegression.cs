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
			regression = new LogisticRegressionAnalysis(
				predictorList.ToArray(),
				resultList.ToArray()
			);
			regression.Compute();
		}
		public override double Predict(double[] predictor)
		{
			return regression.Regression.Compute(predictor);
		}
	}
}
