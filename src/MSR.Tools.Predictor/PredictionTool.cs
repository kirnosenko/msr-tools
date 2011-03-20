/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data.Entities;
using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public class PredictionTool : Tool
	{
		public PredictionTool(string configFile)
			: base(configFile, "predictiontool")
		{
			Models = GetConfiguredType<PredictionModelPool>();
			using (var s = data.OpenSession())
			{
				Releases = s.Releases();
			}
		}
		public IDictionary<string,string> Releases
		{
			get; private set;
		}
		public PredictionModelPool Models
		{
			get; private set;
		}
	}
}
