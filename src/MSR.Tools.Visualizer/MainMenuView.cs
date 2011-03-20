/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public class MainMenuView : MenuStrip, IMainMenuView
	{
		private Dictionary<string,ToolStripMenuItem> items = new Dictionary<string,ToolStripMenuItem>();
		
		public MainMenuView(Control parent)
		{
			Parent = parent;
			Dock = DockStyle.Top;
		}
		public IMenuItemView AddCommand(string name)
		{
			return AddCommand(name, null);
		}
		public IMenuItemView AddCommand(string name, string parentName)
		{
			ToolStripMenuItem newItem = new ToolStripMenuItem();
			items.Add(name, newItem);
			if (parentName == null)
			{
				Items.Add(newItem);
			}
			else
			{
				items[parentName].DropDownItems.Add(newItem);
			}

			return new MenuItemView(newItem, name);
		}
		public void DeleteCommand(string name)
		{
			if (items.ContainsKey(name))
			{
				foreach (var child in items.Where(x => items[name].DropDownItems.Contains(x.Value)).ToList())
				{
					DeleteCommand(child.Key);
				}
			}
			if (items.ContainsKey(name))
			{
				items[name].Owner.Items.Remove(items[name]);
				items.Remove(name);
			}
		}
	}
}
