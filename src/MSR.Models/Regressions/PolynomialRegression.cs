/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using AccordPolynomialRegression = Accord.Statistics.Models.Regression.Linear.PolynomialRegression;

namespace MSR.Models.Regressions
{
	public class PolynomialRegression : Regression<double>
	{
		private AccordPolynomialRegression regression;
		
		public PolynomialRegression()
		{
			Degree = 3;
		}
		public override void Train()
		{
			regression = new AccordPolynomialRegression(Degree);
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
				return regression.CoefficientOfDetermination(
					predictorList.ToArray(), resultList.ToArray()
				);
			}
		}
		public int Degree
		{
			get; set;
		}
	}
}
