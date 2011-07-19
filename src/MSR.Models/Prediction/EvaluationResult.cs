/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction
{
	public class EvaluationResult
	{
		private double TP;
		private double TN;
		private double FP;
		private double FN;
		
		public EvaluationResult(int TP, int TN, int FP, int FN)
		{
			this.TP = TP;
			this.TN = TN;
			this.FP = FP;
			this.FN = FN;
		}
		public double Precision
		{
			get { return TP / (TP + FP); }
		}
		public double Recall
		{
			get { return TP / (TP + FN); }
		}
		public double Pf
		{
			get { return FP / (TN + FP); }
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
			get { return TP / (TP + FN); }
		}
		public double Specificity
		{
			get { return TN / (TN + FP); }
		}
		public double NegPos
		{
			get { return (TN + FP) / (FN + TP); }
		}
		public override string ToString()
		{
			return string.Format("Precision = {0:0.00}, Recall = {1:0.00}, NegPos = {2:0.00}",
				Precision, Recall, NegPos
			);
		}
	}
}
