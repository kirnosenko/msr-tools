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
	public class LinearRegression : Regression<double>
	{
		private SimpleLinearRegression regression;
		
		public override void Train()
		{
			regression = new SimpleLinearRegression();
			regression.Regress(predictorList.ToArray(), resultList.ToArray());
		}
		public override double Predict(double predictor)
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
					predictorList.ToArray(), resultList.ToArray(), true
				);
			}
		}
	}
}
