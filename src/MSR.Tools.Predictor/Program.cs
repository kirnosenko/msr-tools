using System;
using System.Windows.Forms;

namespace MSR.Tools.Predictor
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			PredictorPresenter predictor = new PredictorPresenter(
				new PredictorModel(),
				new PredictorView()
			);
			
			predictor.Run();
		}
	}
}
