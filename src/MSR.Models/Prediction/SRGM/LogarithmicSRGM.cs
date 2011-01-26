/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction.SRGM
{
	public class LogarithmicSRGM
	{
		private double p1, p2;

		public LogarithmicSRGM(double p1, double p2)
		{
			this.p1 = p1;
			this.p2 = p2;
		}
		public double Predict(double t)
		{
			return p1 * Math.Log(1 + p2 * t);
		}
	}
}
