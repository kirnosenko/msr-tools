/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MSR.Tools.Visualizer
{
	public interface IGraphView
	{
		void ShowPoints(double[] x, double[] y);
		
		string Title { get; set; }
		string XAxisTitle { get; set; }
		string YAxisTitle { get; set; }
		bool XAxisLogScale { get; set; }
		bool YAxisLogScale { get; set; }
	}
	
	public class GraphView : ZedGraphControl, IGraphView
	{
		public GraphView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Fill;
		}
		public void ShowPoints(double[] x, double[] y)
		{
			LineItem myCurve = GraphPane.AddCurve("My Curve", x, y, Color.Blue, SymbolType.Diamond);
			myCurve.Symbol.Fill = new Fill(Color.White);
			myCurve.Line.IsVisible = false;
			AxisChange();
			Invalidate();
		}
		public string Title
		{
			get { return GraphPane.Title.Text; }
			set { GraphPane.Title.Text = value; }
		}
		public string XAxisTitle
		{
			get { return GraphPane.XAxis.Title.Text; }
			set { GraphPane.XAxis.Title.Text = value; }
		}
		public string YAxisTitle
		{
			get { return GraphPane.YAxis.Title.Text; }
			set { GraphPane.YAxis.Title.Text = value; }
		}
		public bool XAxisLogScale
		{
			get { return IsLogScale(GraphPane.XAxis); }
			set { SetLogScale(GraphPane.XAxis, value); }
		}
		public bool YAxisLogScale
		{
			get { return IsLogScale(GraphPane.YAxis); }
			set { SetLogScale(GraphPane.YAxis, value); }
		}
		private bool IsLogScale(Axis axis)
		{
			return axis.Type == AxisType.Log;
		}
		private void SetLogScale(Axis axis, bool logScale)
		{
			if (logScale)
			{
				axis.Type = AxisType.Log;
			}
			else
			{
				axis.Type = AxisType.Linear;
			}
			AxisChange();
			Invalidate();
		}
	}

}
