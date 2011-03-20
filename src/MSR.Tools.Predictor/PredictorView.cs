/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSR.Tools.Predictor
{
	public interface IPredictorView
	{
		event Action<string> OnOpenConfigFile;
		event Action OnPredict;
		event Action OnPredictAndEvaluate;
		
		void Show();
		void ShowError(string text);
		void SetReleaseList(IEnumerable<string> releases);
		void SetModelList(IEnumerable<string> models);
		void SetReport(string text);
		
		IEnumerable<string> SelectedReleases { get; }
		int SelectedModel { get; }
		bool CommandMenuAvailable { get; set; }
		bool ShowFiles { get; set; }
	}
	
	public partial class PredictorView : Form, IPredictorView
	{
		public event Action<string> OnOpenConfigFile;
		public event Action OnPredict;
		public event Action OnPredictAndEvaluate;		
		
		public PredictorView()
		{
			InitializeComponent();
			CommandMenuAvailable = false;
			ShowFiles = true;
		}
		public new void Show()
		{
			Application.Run(this);
		}
		public void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public void SetReleaseList(IEnumerable<string> releases)
		{
			foreach (var r in releases)
			{
				releaseList.Items.Add(r);
			}
		}
		public void SetModelList(IEnumerable<string> models)
		{
			foreach (var m in models)
			{
				modelList.Items.Add(m);
			}
			if (modelList.Items.Count > 0)
			{
				modelList.SelectedIndex = 0;
			}
		}
		public void SetReport(string text)
		{
			outputText.Text = text;
		}
		
		public IEnumerable<string> SelectedReleases
		{
			get
			{
				foreach (var item in releaseList.CheckedItems)
				{
					yield return item.ToString();
				}
			}
		}
		public int SelectedModel
		{
			get { return modelList.SelectedIndex; }
		}
		public bool CommandMenuAvailable
		{
			get { return commandToolStripMenuItem.Visible; }
			set { commandToolStripMenuItem.Visible = value; }
		}
		public bool ShowFiles
		{
			get { return showFilesToolStripMenuItem.Checked; }
			set { showFilesToolStripMenuItem.Checked = value; }
		}

		private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				OnOpenConfigFile(dialog.FileName);
			}
		}
		private void predictToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnPredict();
		}
		private void predictAndEvaluateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnPredictAndEvaluate();
		}

		private void showFilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showFilesToolStripMenuItem.Checked = ! showFilesToolStripMenuItem.Checked;
		}
	}
}
