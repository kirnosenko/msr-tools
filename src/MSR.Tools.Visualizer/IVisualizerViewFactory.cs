using System;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerViewFactory
	{
		IMessageDialogView MessageDialog();
		IFileDialogView FileDialog();
		
		IConfigView ConfigDialog();
		IVisualizerView Visualizer();
	}
}
