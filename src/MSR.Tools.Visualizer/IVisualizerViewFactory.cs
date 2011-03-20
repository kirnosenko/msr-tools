using System;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerViewFactory
	{
		IFileDialogView FileDialog();
		
		IConfigView ConfigDialog();
		IVisualizerView Visualizer();
	}
}
