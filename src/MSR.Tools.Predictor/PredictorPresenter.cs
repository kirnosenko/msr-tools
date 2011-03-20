/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Predictor
{
	public class PredictorPresenter
	{
		private IPredictorModel model;
		private IPredictorView view;
		
		public PredictorPresenter(IPredictorModel model, IPredictorView view)
		{
			this.model = model;
			this.view = view;
			view.OnOpenConfigFile += OpenConfigFile;
			view.OnPredict += () => Predict(false);
			view.OnPredictAndEvaluate += () => Predict(true);
		}
		public void Run()
		{
			view.CommandMenuAvailable = false;
			view.Show();
		}
		
		private void OpenConfigFile(string fileName)
		{
			try
			{
				model.OpenConfig(fileName);
				view.SetReleaseList(model.Releases);
				view.SetModelList(model.Models);
				view.CommandMenuAvailable = true;
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
		private void Predict(bool evaluate)
		{
			try
			{
				model.Predict(view.SelectedReleases, view.SelectedModel, evaluate, view.ShowFiles);
				view.SetReport(model.Report);
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
	}
}
