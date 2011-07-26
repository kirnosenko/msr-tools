/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Models.Prediction
{
	public class ROCEvaluationResult
	{
		public ROCEvaluationResult(EvaluationResult[] results)
		{
			var se = results.Select(x => x.Sensitivity).ToArray();
			var sp = results.Select(x => x.Specificity).ToArray();
			var pf = results.Select(x => x.Pf).ToArray();
			
			AUC = 0;
			MaxPoint = 0;
			BalancePoint = 0;
			
			double max = 0;
			double min = 1;
			
			for (int i = 0; i < results.Length - 1; i++)
			{
				AUC += ((se[i + 1] + se[i]) / 2) * (pf[i] - pf[i + 1]);
			}
			for (int i = 0; i < results.Length; i++)
			{
				double tmax = se[i] + sp[i];
				if (tmax > max)
				{
					max = tmax;
					MaxPoint = (double)i * 0.01;
				}
				double tmin = Math.Abs(se[i] - sp[i]);
				if (tmin < min)
				{
					min = tmin;
					BalancePoint = (double)i * 0.01;
				}
			}
		}
		public double AUC
		{
			get; private set;
		}
		public double MaxPoint
		{
			get; private set;
		}
		public double BalancePoint
		{
			get; private set;
		}
		public override string ToString()
		{
			return string.Format("AUC = {0:0.00}, MaxPoint = {1:0.00}, BalancePoint = {2:0.00}",
				AUC, MaxPoint, BalancePoint
			);
		}
	}
}
