/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer.WinForms
{
	public class VisualizationConfigView : Form, IVisualizationConfigView
	{
		private Dictionary<string,Func<object>> options = new Dictionary<string,Func<object>>();
		
		public void AddTextOption(string name, string value)
		{
			TextBox text = new TextBox();
			text.Parent = this;
			text.Text = value;
			options.Add(name, () => text.Text);
		}
		public void AddSelectionOption(string name, object[] values, object selected)
		{
			ComboBox combo = new ComboBox();
			combo.Parent = this;
			foreach (var value in values)
			{
				combo.Items.Add(value);
			}
			combo.SelectedItem = selected;
			options.Add(name, () => combo.SelectedItem);
		}
		public object GetOption(string name)
		{
			return options[name]();
		}
		public new bool ShowDialog()
		{
			base.ShowDialog();
			return true;
		}
	}
}
