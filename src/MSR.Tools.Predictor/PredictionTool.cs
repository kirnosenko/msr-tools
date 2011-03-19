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
	public class PredictionTool : Tool
	{
		public PredictionTool(string configFile)
			: base(configFile, "predictiontool")
		{
			Models = GetConfiguredType<PredictionModelPool>();
		}
		public PredictionModelPool Models
		{
			get; private set;
		}
	}
}
