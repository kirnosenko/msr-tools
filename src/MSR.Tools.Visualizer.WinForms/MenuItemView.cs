/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer.WinForms
{
	public class MenuItemView : IMenuItemView
	{
		private ToolStripMenuItem menuItem;

		public MenuItemView(ToolStripMenuItem menuItem, string name)
		{
			this.menuItem = menuItem;
			this.Name = name;
			this.Text = name;
			menuItem.Click += (s,e) =>
			{
				if (OnClick != null)
				{
					OnClick(this);
				}
			};
		}
		public event Action<IMenuItemView> OnClick;
		public string Name
		{
			get; private set;
		}
		public string Text
		{
			get { return menuItem.Text; }
			set { menuItem.Text = value; }
		}
		public bool Enabled
		{
			get { return menuItem.Enabled; }
			set { menuItem.Enabled = value; }
		}
		public bool Checked
		{
			get { return menuItem.Checked; }
			set { menuItem.Checked = value; }
		}
	}
}
