/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

using MSR.Data;
using MSR.Data.Entities;
using MSR.Models.Prediction.PostReleaseDefectFiles;

namespace MSR.Tools.Predictor
{
	public class PredictionTool : Tool
	{
		private Dictionary<string, Func<IRepositoryResolver, IEnumerable<string>, IEnumerable<string>>> models =
			new Dictionary<string, Func<IRepositoryResolver, IEnumerable<string>, IEnumerable<string>>>();
		
		public PredictionTool(string configFile)
			: base(configFile, "predictiontool")
		{
			
		}
		public IEnumerable<string> Releases
		{
			get
			{
				using (var s = data.OpenSession())
				{
					return (from r in s.Repository<Release>() select r.Tag).ToList();
				}
			}
		}
		public IDictionary<string, Func<IRepositoryResolver, IEnumerable<string>, IEnumerable<string>>> Models
		{
			get { return models; }
		}
	}
}
