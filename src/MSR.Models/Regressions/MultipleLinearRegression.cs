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
	public class MultipleLinearRegression : Regression<double[]>
	{
		private AccordMultipleLinearRegression regression;

		public override void Train()
		{
			regression = new AccordMultipleLinearRegression(predictorList[0].Length, true);
			regression.Regress(
				predictorList.ToArray(),
				resultList.ToArray()
			);
		}
		public override double Predict(double[] predictor)
		{
			return regression.Compute(predictor);
		}
		public double R2
		{
			get
			{
				if (regression == null)
				{
					Train();
				}
				return regression.CoefficientOfDetermination(
					predictorList.ToArray(), resultList.ToArray()
				);
			}
		}
	}
}
