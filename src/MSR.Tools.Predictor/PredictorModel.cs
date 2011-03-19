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
	public interface IPredictorModel
	{
		void OpenConfig(string fileName);
		void Predict(IEnumerable<string> releases, int modelNumber, bool evaluate);
		IEnumerable<string> Releases { get; }
		IEnumerable<string> Models { get; }
		
		IEnumerable<string> PredictedDefectFiles { get; }
		IEnumerable<string> DefectFiles { get; }
		string EvaluationResult { get; }
	}
	
	public class PredictorModel : IPredictorModel
	{
		private PredictionTool tool;
		
		public void OpenConfig(string fileName)
		{
			tool = new PredictionTool(fileName);
		}
		public IEnumerable<string> Releases
		{
			get
			{
				using (var s = tool.Data.OpenSession())
				{
					return (from r in s.Repository<Release>() select r.Tag).ToList();
				}
			}
		}
		public IEnumerable<string> Models
		{
			get { return tool.Models.Models.Select(x => x.Title); }
		}
		public void Predict(IEnumerable<string> releases, int modelNumber, bool evaluate)
		{
			var model = tool.Models.Models[modelNumber];
			using (var s = tool.Data.OpenSession())
			{
				var releaseRevisions = 
					from r in s.Repository<Release>().Where(x => releases.Contains(x.Tag))
					join c in s.Repository<Commit>() on r.CommitID equals c.ID
					select c.Revision;
				model.Init(s, releaseRevisions);
				model.Predict();
				PredictedDefectFiles = model.DefectFiles;
				if (evaluate)
				{
					PostReleaseDefectFilesPredictionEvaluation evaluation = new PostReleaseDefectFilesPredictionEvaluation();
					EvaluationResult = evaluation.Evaluate(s, model).ToString();
					DefectFiles = evaluation.DefectFiles;
				}
			}
		}
		public IEnumerable<string> PredictedDefectFiles
		{
			get; private set;
		}
		public IEnumerable<string> DefectFiles
		{
			get; private set;
		}
		public string EvaluationResult
		{
			get; private set;
		}
	}
}
