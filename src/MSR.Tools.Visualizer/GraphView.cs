/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
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
		string Title { get; set; }
		string XAxisTitle { get; set; }
		string YAxisTitle { get; set; }

		void ShowPoints(PointPairList points);
		void ShowPoints(IEnumerable<PointPairList> pointsList);
	}

	public partial class GraphView : Form, IGraphView
	{
		private const int WindowHeight = 500;
		private const int WindowWidth = 800;
		
		private List<Color> differentColors = new List<Color>()
		{
			Color.Black,
			Color.Blue,
			Color.Red,
			Color.Green,
			Color.Brown,
			Color.Gray,
			Color.Lime,
			Color.Orange,
			Color.Pink,
			Color.Purple,
			Color.Silver
		};
		
		private MenuStrip menu;
		private ZedGraphControl graph;

		public GraphView()
		{
			InitializeComponent();
			Height = WindowHeight;
			Width = WindowWidth;
			
			menu = new MenuStrip();
			menu.Parent = this;
			ToolStripMenuItem m1 = new ToolStripMenuItem("Scale");
			ToolStripMenuItem m2 = new ToolStripMenuItem("log x");
			m2.Click += (s,e) =>
			{
				(s as ToolStripMenuItem).Checked = ! (s as ToolStripMenuItem).Checked;
				XAxisLogScale = (s as ToolStripMenuItem).Checked;
			};
			ToolStripMenuItem m3 = new ToolStripMenuItem("log y");
			m3.Click += (s,e) =>
			{
				(s as ToolStripMenuItem).Checked = ! (s as ToolStripMenuItem).Checked;
				YAxisLogScale = (s as ToolStripMenuItem).Checked;
			};
			menu.Items.Add(m1);
			m1.DropDownItems.Add(m2);
			m1.DropDownItems.Add(m3);
			
			graph = new ZedGraphControl();
			graph.Parent = this;
			graph.Dock = DockStyle.Fill;
		}
		public string Title
		{
			get { return Pane.Title.Text; }
			set { Pane.Title.Text = value; }
		}
		public string XAxisTitle
		{
			get { return Pane.XAxis.Title.Text; }
			set { Pane.XAxis.Title.Text = value; }
		}
		public string YAxisTitle
		{
			get { return Pane.YAxis.Title.Text; }
			set { Pane.YAxis.Title.Text = value; }
		}
		public void ShowPoints(PointPairList points)
		{
			LineItem myCurve = Pane.AddCurve("My Curve", points, Color.Blue, SymbolType.Diamond);
			myCurve.Symbol.Fill = new Fill(Color.White);
			myCurve.Line.IsVisible = false;
			graph.AxisChange();
			
			Application.Run(this);
		}
		public void ShowPoints(IEnumerable<PointPairList> pointsList)
		{
			int color = 0;
			
			foreach (var points in pointsList)
			{
				LineItem myCurve = Pane.AddCurve("Curve", points, differentColors[color], SymbolType.Diamond);
				myCurve.Symbol.Fill = new Fill(Color.White);
				myCurve.Line.IsVisible = false;
				graph.AxisChange();
				color++;
			}	
			
			Application.Run(this);
		}

		private GraphPane Pane
		{
			get { return graph.GraphPane; }
		}
		private bool XAxisLogScale
		{
			get { return IsLogScale(Pane.XAxis); }
			set { SetLogScale(Pane.XAxis, value); }
		}
		private bool YAxisLogScale
		{
			get { return IsLogScale(Pane.YAxis); }
			set { SetLogScale(Pane.YAxis, value); }
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
			graph.AxisChange();
			graph.Invalidate();
		}
	}
}
