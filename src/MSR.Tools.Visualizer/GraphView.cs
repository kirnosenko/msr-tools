using System;
using System.Windows.Forms;
using ZedGraph;

namespace MSR.Tools.Visualizer
{
	public interface IGraphView
	{
	
	}
	
	public class GraphView : ZedGraphControl, IGraphView
	{
		public GraphView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Fill;
		}
	}

}
