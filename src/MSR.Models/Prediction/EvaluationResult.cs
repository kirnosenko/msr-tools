/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Models.Prediction
{
	public class EvaluationResult
	{
		private double TP;
		private double TN;
		private double FP;
		private double FN;

		public EvaluationResult(string[] defectFiles, string[] nonDefectFiles, string[] predictedDefectFiles, string[] predictedNonDefectFiles)
		{
			TP = predictedDefectFiles.Intersect(defectFiles).Count();
			TN = predictedNonDefectFiles.Intersect(nonDefectFiles).Count();
			FP = predictedDefectFiles.Intersect(nonDefectFiles).Count();
			FN = predictedNonDefectFiles.Intersect(defectFiles).Count();
		}
		public double Precision
		{
			get
			{
				if (TP == 0) return 0;
				return TP / (TP + FP);
			}
		}
		public double Recall
		{
			get
			{
				if (TP == 0) return 0;
				return TP / (TP + FN);
			}
		}
		public double Pf
		{
			get
			{
				if (FP == 0) return 0;
				return FP / (TN + FP);
			}
		}
		public double Fmeasure
		{
			get
			{
				double P = Precision;
				double R = Recall;
				
				if (P + R == 0)
				{
					return 0;
				}
				return 2 * P * R / (P + R);
			}
		}
		public double Accuracy
		{
			get { return (TP + TN) / (TP + TN + FP + FN); }
		}
		public double Selectivity
		{
			get { return (TP + FP) / (TP + TN + FP + FN); }
		}
		public double Sensitivity
		{
			get
			{
				if (TP == 0) return 0;
				return TP / (TP + FN);
			}
		}
		public double Specificity
		{
			get
			{
				if (TN == 0) return 0;
				return TN / (TN + FP);
			}
		}
		public double NegPos
		{
			get { return (TN + FP) / (FN + TP); }
		}
	}
}
