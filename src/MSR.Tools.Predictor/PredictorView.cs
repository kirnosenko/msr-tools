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
		void ClearReport();
		void AddReport(string text);
		
		string Title { get; set; }
		string Status { set; }
		bool Ready { set; }
		IEnumerable<string> SelectedReleases { get; }
		IEnumerable<int> SelectedModels { get; }
		bool CommandMenuAvailable { get; set; }
		bool ShowFiles { get; set; }
		bool LimitReleaseSetSize { get; set; }
		int ReleaseSetSize { get; set; }
	}
	
	public partial class PredictorView : Form, IPredictorView
	{
		public event Action<string> OnOpenConfigFile;
		public event Action OnPredict;
		public event Action OnPredictAndEvaluate;		
		
		private Queue<Action> workToDo = new Queue<Action>();
		
		public PredictorView()
		{
			InitializeComponent();
			Ready = true;
			CommandMenuAvailable = false;
		}
		public new void Show()
		{
			Application.Run(this);
		}
		public void ShowError(string text)
		{
			AddWorkToDo(() =>
				MessageBox.Show(this, text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
			);
		}
		public void SetReleaseList(IEnumerable<string> releases)
		{
			releaseList.Items.Clear();
			foreach (var r in releases)
			{
				releaseList.Items.Add(r);
			}
		}
		public void SetModelList(IEnumerable<string> models)
		{
			modelList.Items.Clear();
			foreach (var m in models)
			{
				modelList.Items.Add(m);
			}
		}
		public void ClearReport()
		{
			AddWorkToDo(() =>
				outputText.Text = string.Empty
			);
		}
		public void AddReport(string text)
		{
			AddWorkToDo(() =>
				outputText.Text += text
			);
		}

		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}
		public string Status
		{
			set
			{
				AddWorkToDo(() =>
					statusText.Text = value
				);
			}
		}
		public bool Ready
		{
			set
			{
				AddWorkToDo(() =>
				{
					mainMenu.Enabled = value;
					releaseList.Enabled = value;
					modelList.Enabled = value;
					cbLimitReleaseSetSize.Enabled = value;
					releaseSetSize.Enabled = value && LimitReleaseSetSize;
				});
				Status = value ? "Ready" : "";
			}
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
		public IEnumerable<int> SelectedModels
		{
			get
			{
				int index = 0;
				foreach (var item in modelList.Items)
				{
					if (modelList.CheckedItems.Contains(item))
					{
						yield return index;
					}
					index++;
				}
			}
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
		public bool LimitReleaseSetSize
		{
			get { return cbLimitReleaseSetSize.Checked; }
			set
			{
				cbLimitReleaseSetSize.Checked = value;
				releaseSetSize.Enabled = value;
			}
		}
		public int ReleaseSetSize
		{
			get { return (int)releaseSetSize.Value; }
			set { releaseSetSize.Value = value; }
		}

		protected override void DefWndProc(ref Message m)
		{
			base.DefWndProc(ref m);
			lock (workToDo)
			{
				while (workToDo.Count > 0)
				{
					workToDo.Dequeue()();
				}
			}
		}
		private void AddWorkToDo(Action action)
		{
			lock (workToDo)
			{
				workToDo.Enqueue(action);
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
		private void cbLimitReleaseSetSize_CheckedChanged(object sender, EventArgs e)
		{
			LimitReleaseSetSize = cbLimitReleaseSetSize.Checked;
		}
	}
}
