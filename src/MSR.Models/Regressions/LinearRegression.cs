/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using Accord.Statistics.Models.Regression.Linear;

namespace MSR.Models.Regressions
{
	public class LinearRegression : MultipleRegression
	{
		private MultipleLinearRegression regression;

		public override void Train()
		{
			regression = new MultipleLinearRegression(predictorsList[0].Length, true);
			regression.Regress(
				predictorsList.ToArray(),
				resultList.ToArray()
			);
		}
		public override double Predict(double[] predictors)
		{
			return regression.Compute(predictors);
		}
	}
}
