/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IVisualizerView
	{
		void ShowView();
		void ShowError(string text);

		string Title { get; set; }
		IMainMenuView MainMenu { get; }
		IGraphView Graph { get; }

		string Status { get; set; }
	}
	public partial class VisualizerView : Form, IVisualizerView
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
		
		public VisualizerView()
		{
			InitializeComponent();
			Height = WindowHeight;
			Width = WindowWidth;

			Graph = new GraphView(this);
			MainMenu = new MainMenuView(this);
		}
		public void ShowView()
		{
			Application.Run(this);
		}
		public void ShowError(string text)
		{
			MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public string Title
		{
			get { return Text; }
			set { Text = value; }
		}
		public IMainMenuView MainMenu
		{
			get; private set;
		}
		public IGraphView Graph
		{
			get; private set;
		}
		public string Status
		{
			get { return statusText.Text; }
			set { statusText.Text = value; }
		}
	}
}
