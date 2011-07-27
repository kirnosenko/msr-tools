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
		event Action OnShowLastROC;
		
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
		bool Evaluate { get; set; }
		bool EvaluateUsingROC { get; set; }
		bool ReleaseSetGettingAll { get; set; }
		bool ReleaseSetGettingIncrementalGrowth { get; set; }
		bool ReleaseSetGettingFixed { get; set; }
		int ReleaseSetSize { get; set; }
	}
	
	public partial class PredictorView : Form, IPredictorView
	{
		public event Action<string> OnOpenConfigFile;
		public event Action OnPredict;
		public event Action OnShowLastROC;
		
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
			{
				outputText.Text += text;
				outputText.SelectionStart = outputText.TextLength;
				outputText.ScrollToCaret();
			});
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
					rbAll.Enabled = value;
					rbIncrementalGrowth.Enabled = value;
					rbFixed.Enabled = value;
					releaseSetSize.Enabled = value && ReleaseSetGettingFixed;
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
			get { return commandMenu.Visible; }
			set { commandMenu.Visible = value; }
		}
		public bool ShowFiles
		{
			get { return showFilesMenu.Checked; }
			set { showFilesMenu.Checked = value; }
		}
		public bool Evaluate
		{
			get { return evaluateMenu.Checked; }
			set { evaluateMenu.Checked = value; }
		}
		public bool EvaluateUsingROC
		{
			get { return evaluateUsingROCMenu.Checked; }
			set { evaluateUsingROCMenu.Checked = value; }
		}
		public bool ReleaseSetGettingAll
		{
			get { return rbAll.Checked; }
			set
			{
				rbAll.Checked = true;
				releaseSetSize.Enabled = false;
			}
		}
		public bool ReleaseSetGettingIncrementalGrowth
		{
			get { return rbIncrementalGrowth.Checked; }
			set
			{
				rbIncrementalGrowth.Checked = true;
				releaseSetSize.Enabled = false;
			}
		}
		public bool ReleaseSetGettingFixed
		{
			get { return rbFixed.Checked; }
			set
			{
				rbFixed.Checked = true;
				releaseSetSize.Enabled = true;
			}
		}
		public int ReleaseSetSize
		{
			get { return (int)releaseSetSize.Value; }
			set { releaseSetSize.Value = value; }
		}
		private void DoWork()
		{
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
			if (base.IsHandleCreated)
			{
				base.Invoke((Action)(DoWork));
			}
		}
		private void openConfigMenuClick(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				OnOpenConfigFile(dialog.FileName);
			}
		}
		private void predictMenuClick(object sender, EventArgs e)
		{
			OnPredict();
		}
		private void showLastROCMenuClick(object sender, EventArgs e)
		{
			OnShowLastROC();
		}
		private void SwitchMenuOptionClick(object sender, EventArgs e)
		{
			(sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
		}
		private void rbAll_CheckedChanged(object sender, EventArgs e)
		{
			releaseSetSize.Enabled = rbFixed.Checked;
		}
	}
}
