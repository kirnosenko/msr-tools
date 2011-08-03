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
	public class PredictorPresenter
	{
		private IPredictorModel model;
		private IPredictorView view;
		
		public PredictorPresenter(IPredictorModel model, IPredictorView view)
		{
			this.model = model;
			this.view = view;
			model.OnTitleUpdated += x => view.Title = x;
			model.OnClearReport += () => view.ClearReport();
			model.OnAddReport += x => view.AddReport(x);
			model.OnClearRocList += () => view.ClearROCs();
			model.OnRocAdded += view.AddRoc;
			model.OnReadyStateChanged += x => view.Ready = x;
			model.OnProgressStateChanged += x => view.Status = x;
			model.OnError += x => view.ShowError(x);
			view.OnOpenConfigFile += OpenConfigFile;
			view.OnPredict += Predict;
			view.OnShowROC += model.ShowROC;
			view.ShowFiles = model.ShowFiles;
			view.Evaluate = model.Evaluate;
			view.EvaluateUsingROC = model.EvaluateUsingROC;
			if (model.ReleaseSetGetting == ReleaseSetGettingAlgo.All)
			{
				view.ReleaseSetGettingAll = true;
			}
			else if (model.ReleaseSetGetting == ReleaseSetGettingAlgo.IncrementalGrowth)
			{
				view.ReleaseSetGettingIncrementalGrowth = true;
			}
			else
			{
				view.ReleaseSetGettingFixed = true;
			}
			view.ReleaseSetSize = model.ReleaseSetSize;
		}
		public void Run()
		{
			view.Show();
		}
		private void ReadOptions()
		{
			view.SetReleaseList(model.Releases.Values);
			view.SetModelList(model.Models.Select(x => x.Title));
			view.ClearReport();
			view.CommandMenuAvailable = true;
		}
		private void UpdateOptions()
		{
			List<PostReleaseDefectFilesPrediction> models = new List<PostReleaseDefectFilesPrediction>();
			foreach (var n in view.SelectedModels)
			{
				models.Add(model.Models[n]);
			}
			model.SelectedModels = models;
			
			model.SelectedReleases = model.Releases
				.Where(x => view.SelectedReleases.Contains(x.Value))
				.ToDictionary(x => x.Key, x => x.Value);
			
			model.ShowFiles = view.ShowFiles;
			model.Evaluate = view.Evaluate;
			model.EvaluateUsingROC = view.EvaluateUsingROC;
			if (view.ReleaseSetGettingAll)
			{
				model.ReleaseSetGetting = ReleaseSetGettingAlgo.All;
			}
			else if (view.ReleaseSetGettingIncrementalGrowth)
			{
				model.ReleaseSetGetting = ReleaseSetGettingAlgo.IncrementalGrowth;
			}
			else
			{
				model.ReleaseSetGetting = ReleaseSetGettingAlgo.Fixed;
			}
			model.ReleaseSetSize = view.ReleaseSetSize;
		}
		private void OpenConfigFile(string fileName)
		{
			try
			{
				model.OpenConfig(fileName);
				ReadOptions();
			}
			catch (Exception e)
			{
				view.ShowError(e.Message);
			}
		}
		private void Predict()
		{
			UpdateOptions();
			model.Predict();
		}
	}
}
