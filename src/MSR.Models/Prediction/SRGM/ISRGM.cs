/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Models.Prediction.SRGM
{
	public interface ISRGM
	{
		double Predict(double t);
	}
}
