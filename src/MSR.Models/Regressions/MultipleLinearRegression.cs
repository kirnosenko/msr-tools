/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using AccordMultipleLinearRegression = Accord.Statistics.Models.Regression.Linear.MultipleLinearRegression;

using Accord.Statistics.Models.Regression.Linear;

namespace MSR.Models.Regressions
{
	public class MultipleLinearRegression : MultipleRegression
	{
		private AccordMultipleLinearRegression regression;

		public override void Train()
		{
			regression = new AccordMultipleLinearRegression(predictorsList[0].Length, true);
			regression.Regress(
				predictorsList.ToArray(),
				resultList.ToArray()
			);
		}
		public override double Predict(double[] predictors)
		{
			return regression.Compute(predictors);
		}
		public double R2()
		{
			return regression.CoefficientOfDetermination(
				predictorsList.ToArray(),
				resultList.ToArray()
			);
		}
	}
}
