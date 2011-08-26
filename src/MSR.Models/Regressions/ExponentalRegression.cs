/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using MSR.Models.Prediction.SRGM;

namespace MSR.Models.Regressions
{
	public class ExponentalRegression : Regression<double>
	{
		private ExponentialSRGM srgm;
		
		public override void Train()
		{
			Train(predictorList.ToArray(), resultList.ToArray());
		}
		public override void Train(double[] predictors, double[] results)
		{			
			double xxy = 0;
			double xy = 0;
			double y = 0;
			double ylny = 0;
			double xylny = 0;
			
			for (int i = 0; i < predictors.Length; i++)
			{
				results[i] = 1.000001 - results[i];
				
				xxy += predictors[i] * predictors[i] * results[i];
				xy -= predictors[i] * results[i];
				y += results[i];
				ylny += results[i] * Math.Log(results[i]);
				xylny -= predictors[i] * results[i] * Math.Log(results[i]);
			}
			
			double p1 = 1;
			double p2 = (y * xylny - xy * ylny) / (y * xxy - xy * xy);
			srgm = new ExponentialSRGM(p1, p2);
		}
		public override double Predict(double predictor)
		{
			return srgm.Predict(predictor);
		}
	}
}
