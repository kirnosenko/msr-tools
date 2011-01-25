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
	public class PolynomialRegression
	{
		private AccordPolynomialRegression regression;
		
		public PolynomialRegression()
		{
			Degree = 3;
		}
		public void Train(double[] inputs, double[] outputs)
		{
			regression = new AccordPolynomialRegression(Degree);
			regression.Regress(inputs, outputs);
		}
		public double Predict(double predictor)
		{
			return regression.Compute(predictor);
		}
		public int Degree
		{
			get; set;
		}
	}
}
