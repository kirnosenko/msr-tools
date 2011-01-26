/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction.SRGM
{
	/// <summary>
	/// Exponential SRGM with two parameters.
	/// p0 - the initial estimate of the total failure
	/// recovered at the end of the testing process.
	/// p1 - the ratio between the initial failure
	/// intensity and total failure.
	/// </summary>
	public class ExponentialSRGM : ISRGM
	{
		private double p1, p2;
		
		public ExponentialSRGM(double p1, double p2)
		{
			this.p1 = p1;
			this.p2 = p2;
		}
		public double Predict(double t)
		{
			return p1 * (1 - Math.Exp(-p2 * t));
		}
	}
}
