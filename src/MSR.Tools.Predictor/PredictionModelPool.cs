/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public class PredictionModelPool
	{
		private PostReleaseDefectFilesPrediction[] models;
		
		public PredictionModelPool(PostReleaseDefectFilesPrediction[] models)
		{
			this.models = models;
		}
		public PostReleaseDefectFilesPrediction[] Models()
		{
			return models;
		}
		public string TargetDir
		{
			set
			{
				foreach (var m in models)
				{
					m.FileSelector = e => e.InDirectory(value);
				}
			}
		}
		public int PostReleasePeriodInDays
		{
			set
			{
				foreach (var m in models)
				{
					m.PostReleasePeriodInDays = value;
				}
			}
		}
	}
}
