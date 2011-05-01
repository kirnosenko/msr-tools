/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MSR.Tools.Visualizer
{
	public interface IGraphView
	{
		void ShowPoints(string legend, double[] x, double[] y);
		void ShowLine(string legend, double[] x, double[] y);
		void ShowHistogram(string legend, double[] x, double[] y);
		void CleanUp();

		string Title { get; set; }
		string XAxisTitle { get; set; }
		string YAxisTitle { get; set; }
		bool XAxisLogScale { get; set; }
		bool YAxisLogScale { get; set; }
		IDictionary<double[],double[]> Points { get; }
	}
	
	public class GraphView : ZedGraphControl, IGraphView
	{
		private Dictionary<double[],double[]> points = new Dictionary<double[],double[]>();
		private Queue<Color> differentColors;
		
		public GraphView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Fill;
			
			Title = "";
			XAxisTitle = "";
			YAxisTitle = "";
			
			InitColorsQueue();
		}
		public void ShowPoints(string legend, double[] x, double[] y)
		{
			points.Add(x, y);
			ShowCurve(legend, x, y, NextColor(), false);
		}
		public void ShowLine(string legend, double[] x, double[] y)
		{
			ShowCurve(legend, x, y, NextColor(), true);
		}
		public void ShowHistogram(string legend, double[] x, double[] y)
		{
			BarItem bar = GraphPane.AddBar(legend, x, y, NextColor());
			AxisChange();
			Invalidate();
		}
		public void CleanUp()
		{
			GraphPane.CurveList.Clear();
			Invalidate();
			points.Clear();
			InitColorsQueue();
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
		public IDictionary<double[],double[]> Points
		{
			get { return points; }
		}
		private void ShowCurve(string legend, double[] x, double[] y, Color color, bool line)
		{
			LineItem myCurve = GraphPane.AddCurve(legend, x, y, color, line ? SymbolType.None : SymbolType.Diamond);
			myCurve.Symbol.Fill = new Fill(Color.White);
			myCurve.Line.IsVisible = line;
			AxisChange();
			Invalidate();
		}
		private void InitColorsQueue()
		{
			differentColors = new Queue<Color>(new Color[]
			{
				Color.Black,
				Color.Blue,
				Color.Red,
				Color.Green,
				Color.Brown,
				Color.Gray,
				Color.Orange,
				Color.Pink,
				Color.Purple,
				Color.Silver
			});
		}
		private Color NextColor()
		{
			Color result = differentColors.Dequeue();
			differentColors.Enqueue(result);
			return result;
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
