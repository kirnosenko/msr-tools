/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
		bool BlackAndWhite { get; set; }
		IDictionary<double[],double[]> Points { get; }
	}
	
	public class GraphView : ZedGraphControl, IGraphView
	{
		private Dictionary<double[],double[]> points = new Dictionary<double[],double[]>();
		private Queue<Color> differentColors;
		private Queue<SymbolType> differentSymbols;
		private bool blackAndWhite;
		
		public GraphView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Fill;
			
			Title = "";
			XAxisTitle = "";
			YAxisTitle = "";
			
			InitColorsAndSymbols();
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
			ShowCurve(legend, x, y, false, true);
		}
		public void ShowLine(string legend, double[] x, double[] y)
		{
			ShowCurve(legend, x, y, true, false);
		}
		public void ShowLineWithPoints(string legend, double[] x, double[] y)
		{
			ShowCurve(legend, x, y, true, true);
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
			InitColorsAndSymbols();
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
		public bool BlackAndWhite
		{
			get { return blackAndWhite; }
			set
			{
				if (blackAndWhite != value)
				{
					blackAndWhite = value;
					InitColorsAndSymbols();
					
					var curves = GraphPane.CurveList.Where(x => x.Tag != null).ToArray();
					foreach (var curve in curves)
					{
						GraphPane.CurveList.Remove(curve);
						
						Color color = BlackAndWhite ? Color.Black : NextColor();
						SymbolType symbol = BlackAndWhite ? NextSymbol() : SymbolType.Circle;
						
						LineItem myCurve = GraphPane.AddCurve(curve.Label.Text, curve.Points, color, symbol);
						myCurve.Symbol.Fill = new Fill(color);
						myCurve.Line.IsVisible = true;
						myCurve.Tag = this;
					}
					Invalidate();
				}
			}
		}
		public IDictionary<double[],double[]> Points
		{
			get { return points; }
		}
		private void ShowCurve(string legend, double[] x, double[] y, bool line, bool points)
		{
			Color color = BlackAndWhite ? Color.Black : NextColor();
			SymbolType symbol = BlackAndWhite ? NextSymbol() : SymbolType.Circle;
			if (! points)
			{
				symbol = SymbolType.None;
			}
			
			LineItem myCurve = GraphPane.AddCurve(legend, x, y, color, symbol);
			myCurve.Symbol.Fill = new Fill(line ? color : Color.White);
			myCurve.Line.IsVisible = line;
			myCurve.Tag = (line && points) ? this : null;
			AxisChange();
			Invalidate();
		}
		private void InitColorsAndSymbols()
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
			differentSymbols = new Queue<SymbolType>(new SymbolType[]
			{
				SymbolType.None,
				SymbolType.Diamond,
				SymbolType.Triangle,
				SymbolType.TriangleDown,
				SymbolType.Square,
				SymbolType.Circle
			});
		}
		private Color NextColor()
		{
			Color result = differentColors.Dequeue();
			differentColors.Enqueue(result);
			return result;
		}
		private SymbolType NextSymbol()
		{
			SymbolType result = differentSymbols.Dequeue();
			differentSymbols.Enqueue(result);
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
