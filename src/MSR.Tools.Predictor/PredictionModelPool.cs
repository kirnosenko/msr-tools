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
		public PredictionModelPool()
		{
			Models = new PostReleaseDefectFilesPrediction[] {};
		}
		public PostReleaseDefectFilesPrediction[] Models
		{
			get; set;
		}
		public string TargetDir
		{
			set
			{
				foreach (var m in Models)
				{
					m.FileSelector = e => e.InDirectory(value);
				}
			}
		}
		public int PostReleasePeriodInDays
		{
			set
			{
				foreach (var m in Models)
				{
					m.PostReleasePeriodInDays = value;
				}
			}
		}
	}
}
