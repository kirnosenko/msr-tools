/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZedGraph;

namespace MSR.Tools.Visualizer
{
	public class VisualizerPresenter
	{
		private VisualizerModel model;
		private IVisualizerView view;
		private IPresenterFactory presenters;

		public VisualizerPresenter(VisualizerModel model, IVisualizerView view, IPresenterFactory presenters)
		{
			this.model = model;
			this.view = view;
			this.presenters = presenters;
			
			Title = string.Empty;
			CreateMainMenu();
		}
		public void Show()
		{
			view.ShowView();
		}
		public string Title
		{
			get { return view.Title; }
			set
			{
				view.Title = "Visualizer";
				if ((value != null) && (value != string.Empty))
				{
					view.Title += " - " + value;
				}
			}
		}
		private void CreateMainMenu()
		{
			view.MainMenu.AddCommand("Visualizer");
			view.MainMenu.AddCommand("Open config...", "Visualizer").OnClick += i =>
			{
				string fileName = presenters.FileDialog().OpenFile(null);
				if (fileName != null)
				{
					try
					{
						model.OpenConfig(fileName);
						Title = fileName;
					}
					catch (Exception e)
					{
						presenters.MessageDialog().Error(e.Message);
					}
				}
			};
			view.MainMenu.AddCommand("Scale");
			view.MainMenu.AddCommand("log x", "Scale").OnClick += i =>
			{
				i.Checked = ! i.Checked;
				view.Graph.XAxisLogScale = i.Checked;
			};
			view.MainMenu.AddCommand("log y", "Scale").OnClick += i =>
			{
				i.Checked = ! i.Checked;
				view.Graph.YAxisLogScale = i.Checked;
			};
			view.MainMenu.AddCommand("Visualizations");
			foreach (var visualization in model.Visualizations)
			{
				view.MainMenu.AddCommand(visualization, "Visualizations").OnClick += i =>
				{
					model.Visualize(i.Name, view.Graph);
					view.StatusBar.Status = model.LastVisualizationProfiling;
				};
			}
		}
	}
}
