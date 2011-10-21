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
		public ROCEvaluationResult(EvaluationResult[] results, double rocEvaluationDelta)
		{
			Count = results.Length;
			P = results.Select(x => x.Precision).ToArray();
			Se = results.Select(x => x.Sensitivity).ToArray();
			Sp = results.Select(x => x.Specificity).ToArray();
			Pf = results.Select(x => x.Pf).ToArray();
			
			AUC = 0;
			OptimalPoint = 0;
			MaxPoint = 0;
			BalancePoint = 0;
			
			double max = 0;
			double min = 1;
			double euclideanDistance = Math.Sqrt(2);
			
			for (int i = 0; i < results.Length - 1; i++)
			{
				AUC += (Pf[i] - Pf[i + 1]) * (Se[i] + Se[i + 1]) / 2;
			}
			for (int i = 0; i < results.Length; i++)
			{
				double tempMax = Se[i] + Sp[i];
				if (tempMax > max)
				{
					max = tempMax;
					MaxPoint = (double)i * rocEvaluationDelta;
				}
				double tempMin = Math.Abs(Se[i] - Sp[i]);
				if (tempMin < min)
				{
					min = tempMin;
					BalancePoint = (double)i * rocEvaluationDelta;
				}
				double tempEuclideanDistance = Math.Sqrt(Math.Pow(0 - Pf[i], 2) + Math.Pow(1 - Se[i], 2));
				if (tempEuclideanDistance < euclideanDistance)
				{
					euclideanDistance = tempEuclideanDistance;
					OptimalPoint = (double)i * rocEvaluationDelta;
				}
			}
		}
		public int Count
		{
			get; private set;
		}
		public double[] P
		{
			get; private set;
		}
		public double[] Se
		{
			get; private set;
		}
		public double[] Sp
		{
			get; private set;
		}
		public double[] Pf
		{
			get; private set;
		}
		public double AUC
		{
			get; private set;
		}
		public double OptimalPoint
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
	}
}
