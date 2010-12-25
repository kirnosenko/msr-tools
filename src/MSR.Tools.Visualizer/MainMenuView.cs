/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IMainMenuView
	{
		IMenuItemView AddCommand(string name);
		IMenuItemView AddCommand(string name, string parentName);
	}
	
	public class MainMenuView : MenuStrip, IMainMenuView
	{
		private Dictionary<string,ToolStripMenuItem> items = new Dictionary<string,ToolStripMenuItem>();
		
		public MainMenuView(Control parent)
		{
			Parent = parent;
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
	}
}
