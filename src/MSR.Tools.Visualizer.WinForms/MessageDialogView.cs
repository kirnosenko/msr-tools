/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Windows.Forms;

namespace MSR.Tools.Visualizer.WinForms
{
	public class MessageDialogView : IMessageDialogView
	{
		public bool ShowYesNo(string title, string text)
		{
			return MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		public void ShowError(string title, string text)
		{
			MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
