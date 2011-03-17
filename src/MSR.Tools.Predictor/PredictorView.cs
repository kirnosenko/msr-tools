using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MSR.Tools.Predictor
{
	public interface IPredictorView
	{
		event Action<string> OnOpenConfigFile;
		event Action<int> OnPredict;
		event Action<int> OnPredictAndEvaluate;
		
		void Show();
		void SetReleaseList(IEnumerable<string> releases);
		void SetModelList(IEnumerable<string> models);
		IEnumerable<string> SelectedReleases { get; }
		bool CommandMenuAvailable { get; set; }
	}
	
	public partial class PredictorView : Form, IPredictorView
	{
		public event Action<string> OnOpenConfigFile;
		public event Action<int> OnPredict;
		public event Action<int> OnPredictAndEvaluate;		
		
		public PredictorView()
		{
			InitializeComponent();
		}
		public new void Show()
		{
			Application.Run(this);
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
		}
		public void AddModel(string name)
		{
			modelList.Items.Add(name);
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
		public bool CommandMenuAvailable
		{
			get { return commandToolStripMenuItem.Visible; }
			set { commandToolStripMenuItem.Visible = value; }
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
			OnPredict(modelList.SelectedIndex);
		}
		private void evaluateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OnPredictAndEvaluate(modelList.SelectedIndex);
		}
	}
}
