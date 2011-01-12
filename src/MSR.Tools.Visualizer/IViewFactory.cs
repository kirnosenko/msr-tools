using System;

namespace MSR.Tools.Visualizer
{
	public interface IViewFactory
	{
		IMessageDialogView MessageDialog();
		IFileDialogView FileDialog();
		IVisualizationConfigView ConfigDialog();

		IVisualizerView Visualizer();
	}
}
