/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Tools.Visualizer
{
	public class VisualizerPresenter
	{
		private IVisualizerModel model;
		private IVisualizerView view;

		public VisualizerPresenter(IVisualizerModel model, IVisualizerView view)
		{
			this.model = model;
			this.view = view;
			view.OnOpenConfigFile += OpenConfigFile;
			view.OnVisualizationActivate += UseVisualization;
			
			Title = string.Empty;
			//CreateMainMenu();
		}
		public void Run()
		{
			view.Show();
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
		private void ReadOptions()
		{
			//view.CommandMenuAvailable = true;
			view.SetVisualizationList(model.Visualizations.Select(x => x.Title));
		}
		private void UpdateOptions()
		{
		
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
		private void UseVisualization(int number)
		{
			IVisualization visualization = model.Visualizations[number];

			if (! visualization.Configurable || view.ConfigureVisualization(visualization))
			{
				if (model.AutomaticallyCleanUp)
				{
					view.Graph.CleanUp();
				}

				model.CalcVisualization(visualization);
				visualization.Draw(view.Graph);
				view.Status = model.LastVisualizationProfiling;
			}
		}
		/*
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
						CreateVisualizationsMenu();
					}
					catch (Exception e)
					{
						view.ShowError(e.Message);
					}
				}
			};
			view.MainMenu.AddCommand("View");
			view.MainMenu.AddCommand("Scale", "View");
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
			view.MainMenu.AddCommand("Clean up", "View");
			var acu = view.MainMenu.AddCommand("Automatically", "Clean up");
			acu.OnClick += i =>
			{
				i.Checked = ! i.Checked;
				model.AutomaticallyCleanUp = i.Checked;
			};
			acu.Checked = model.AutomaticallyCleanUp;
			view.MainMenu.AddCommand("Clean up now", "Clean up").OnClick += i =>
			{
				view.Graph.CleanUp();
			};
		}
		private void CreateVisualizationsMenu()
		{
			view.MainMenu.DeleteCommand("Visualizations");
			view.MainMenu.AddCommand("Visualizations");
			
			foreach (var visualizationName in model.Visualizations)
			{
				view.MainMenu.AddCommand(visualizationName, "Visualizations").OnClick += i =>
				{
					IVisualization visualization = model.Visualization(i.Name);
					if (! visualization.Configurable || presenters.ConfigDialog().Config(visualization))
					{
						if (model.AutomaticallyCleanUp)
						{
							view.Graph.CleanUp();
						}
						
						model.CalcVisualization(visualization);
						visualization.Draw(view.Graph);
						view.Status = model.LastVisualizationProfiling;
					}
				};
			}
		}
		*/
	}
}
