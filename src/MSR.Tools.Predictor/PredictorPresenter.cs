using System;
using System.Windows.Forms;

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
				//view.SetModelList(
				view.CommandMenuAvailable = true;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
