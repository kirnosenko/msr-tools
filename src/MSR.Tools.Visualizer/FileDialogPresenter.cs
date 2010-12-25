/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IFileDialogPresenter
	{
		string OpenFile(string fileName);
		string SaveFile(string fileName);
	}
	
	public class FileDialogPresenter : IFileDialogPresenter
	{
		private IFileDialogView view;

		public FileDialogPresenter(IFileDialogView view)
		{
			this.view = view;
		}
		public string OpenFile(string fileName)
		{
			return ShowFileDialog(fileName, view.Open);
		}
		public string SaveFile(string fileName)
		{
			return ShowFileDialog(fileName, view.Save);
		}
		private string ShowFileDialog(string fileName, Func<bool> showDialog)
		{
			view.FileName = fileName;
			if (showDialog())
			{
				return view.FileName;
			}
			return null;
		}
	}
}
