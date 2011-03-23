using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public partial class VisualizationConfigView : Form
	{
		public VisualizationConfigView()
		{
			InitializeComponent();
		}
		public bool ShowDialog(object obj)
		{
			propertyList.SelectedObject = obj;
			base.ShowDialog();
			return DialogResult == DialogResult.OK;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
