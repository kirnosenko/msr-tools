/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Predictor
{
	class Program
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
