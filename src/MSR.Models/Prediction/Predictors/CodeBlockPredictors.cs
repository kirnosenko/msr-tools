/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

using MSR.Data.Entities.DSL.Selection;
using MSR.Data.Entities.DSL.Selection.Metrics;

namespace MSR.Models.Prediction.Predictors
{
	public static class CodeBlockPredictors
	{
		public static Prediction AddLocPredictor(this Prediction p)
		{
			p.AddPredictor((Func<CodeBlockSelectionExpression,double>)(code =>
			{
				return code.CalculateLOC();
			}));
			return p;
		}
		public static Prediction AddNumberOfDefectsPredictor(this Prediction p)
		{
			p.AddPredictor((Func<CodeBlockSelectionExpression,double>)(code =>
			{
				return code.CalculateNumberOfDefects();
			}));
			return p;
		}
	}
}
