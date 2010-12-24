using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer
{
	public interface IMenuItemView
	{
		event Action<IMenuItemView> OnClick;
		string Name { get; }
		string Text { get; set; }
		bool Checked { get; set; }
	}
	
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
		public bool Checked
		{
			get { return menuItem.Checked; }
			set { menuItem.Checked = value; }
		}
	}
}
