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
		void PrepairPointsForDateScale(double[] points, DateTime startDate);
		void ShowPoints(string legend, double[] x, double[] y);
		void ShowLine(string legend, double[] x, double[] y);
		void ShowLineWithPoints(string legend, double[] x, double[] y);
		void ShowHistogram(string legend, double[] x, double[] y);
		void CleanUp();

		string Title { get; set; }
		string XAxisTitle { get; set; }
		string YAxisTitle { get; set; }
		bool XAxisLogScale { get; set; }
		bool YAxisLogScale { get; set; }
		bool XAxisDayScale { get; set; }
		bool YAxisDayScale { get; set; }
		float XAxisFontAngle { get; set; }
		float YAxisFontAngle { get; set; }
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
		public void PrepairPointsForDateScale(double[] points, DateTime startDate)
		{
			for (int i = 0; i < points.Length; i++)
			{
				points[i] = new XDate(startDate.AddDays(points[i]));
			}
		}
		public void ShowPoints(string legend, double[] x, double[] y)
		{
			points.Add(x, y);
			ShowCurve(legend, x, y, NextColor(), false, true);
		}
		public void ShowLine(string legend, double[] x, double[] y)
		{
			ShowCurve(legend, x, y, NextColor(), true, false);
		}
		public void ShowLineWithPoints(string legend, double[] x, double[] y)
		{
			ShowCurve(legend, x, y, NextColor(), true, true);
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
			get { return IsScale(GraphPane.XAxis, AxisType.Log); }
			set { SetScale(GraphPane.XAxis, value ? AxisType.Log : AxisType.Linear); }
		}
		public bool YAxisLogScale
		{
			get { return IsScale(GraphPane.YAxis, AxisType.Log); }
			set { SetScale(GraphPane.YAxis, value ? AxisType.Log : AxisType.Linear); }
		}
		public bool XAxisDayScale
		{
			get { return IsScale(GraphPane.XAxis, AxisType.Date); }
			set { SetScale(GraphPane.XAxis, value ? AxisType.Date : AxisType.Linear); }
		}
		public bool YAxisDayScale
		{
			get { return IsScale(GraphPane.YAxis, AxisType.Date); }
			set { SetScale(GraphPane.YAxis, value ? AxisType.Date : AxisType.Linear); }
		}
		public float XAxisFontAngle
		{
			get { return GraphPane.XAxis.Scale.FontSpec.Angle; }
			set { GraphPane.XAxis.Scale.FontSpec.Angle = value; }
		}
		public float YAxisFontAngle
		{
			get { return GraphPane.YAxis.Scale.FontSpec.Angle; }
			set { GraphPane.YAxis.Scale.FontSpec.Angle = value; }
		}
		public IDictionary<double[],double[]> Points
		{
			get { return points; }
		}
		private void ShowCurve(string legend, double[] x, double[] y, Color color, bool line, bool points)
		{
			LineItem myCurve = GraphPane.AddCurve(legend, x, y, color, points ? SymbolType.Circle : SymbolType.None);
			myCurve.Symbol.Fill = new Fill(line ? color : Color.White);
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
		private bool IsScale(Axis axis, AxisType scale)
		{
			return axis.Type == scale;
		}
		private void SetScale(Axis axis, AxisType scale)
		{
			axis.Type = scale;
			AxisChange();
			Invalidate();
		}
	}

}
