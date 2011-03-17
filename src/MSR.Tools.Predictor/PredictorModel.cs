using System;
using System.Collections.Generic;

namespace MSR.Tools.Predictor
{
	public interface IPredictorModel
	{
		void OpenConfig(string fileName);
		void Predict();
		void PredictAndEvaluate();
		IEnumerable<string> Releases { get; }
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
			get { return tool.Releases; }
		}
		public IEnumerable<string> Models
		{
			get { return tool.Releases; }
		}
		public void Predict()
		{
			
		}
		public void PredictAndEvaluate()
		{

		}
	}
}
