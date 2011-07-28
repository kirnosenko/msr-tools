/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerView
	{
		event Action<string> OnOpenConfigFile;
		event Action<int> OnVisualizationActivate;
		event Action<bool> OnChangeCleanUpOption;
		
		void Show();
		void ShowError(string text);
		void SetVisualizationList(IEnumerable<string> visualizations);
		bool ConfigureVisualization(IVisualization visualization);

		string Title { get; set; }
		string Status { get; set; }
		IGraphView Graph { get; }
		bool AutomaticallyCleanUp { get; set; }
		bool BlackAndWhite { get; set; }
	}
	
	public partial class VisualizerView : Form, IVisualizerView
	{
		public event Action<string> OnOpenConfigFile;
		public event Action<int> OnVisualizationActivate;
		public event Action<bool> OnChangeCleanUpOption;
		
		public VisualizerView()
		{
			InitializeComponent();

			Graph = new GraphView(panel1);
			
			BlackAndWhite = false;
		}
		public new void Show()
		{
			Application.Run(this);
		}
		public void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public void SetVisualizationList(IEnumerable<string> visualizations)
		{
			visualizationsMenu.DropDownItems.Clear();
			int i = 0;
			foreach (var v in visualizations)
			{
				var menuItem = visualizationsMenu.DropDownItems.Add(v);
				menuItem.Tag = i;
				menuItem.Click += (s,e) =>
				{
					OnVisualizationActivate((int)(s as ToolStripMenuItem).Tag);
				};
				i++;
			}
		}
		public bool ConfigureVisualization(IVisualization visualization)
		{
			var config = new VisualizationConfigView();
			return config.ShowDialog(visualization);
		}
		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}
		public string Status
		{
			get { return statusText.Text; }
			set { statusText.Text = value; }
		}
		public IGraphView Graph
		{
			get; private set;
		}
		public bool AutomaticallyCleanUp
		{
			get { return automaticallyMenu.Checked; }
			set { automaticallyMenu.Checked = value; }
		}
		public bool BlackAndWhite
		{
			get { return Graph.BlackAndWhite; }
			set
			{
				Graph.BlackAndWhite = value;
				blackAndWhiteMenu.Checked = value;
			}
		}

		private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				OnOpenConfigFile(dialog.FileName);
			}
		}

		private void logXToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			Graph.XAxisLogScale = item.Checked;
		}
		private void logYToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			Graph.YAxisLogScale = item.Checked;
		}
		private void automaticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			item.Checked = ! item.Checked;
			OnChangeCleanUpOption(item.Checked);
		}
		private void cleanUpNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Graph.CleanUp();
		}
		private void blackAndWhiteMenu_Click(object sender, EventArgs e)
		{
			var item = (sender as ToolStripMenuItem);
			BlackAndWhite = ! item.Checked;
		}
	}
}
