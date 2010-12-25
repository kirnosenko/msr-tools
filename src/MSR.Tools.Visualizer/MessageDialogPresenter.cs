/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Visualizer
{
	public interface IMessageDialogPresenter
	{
		bool YesNo(string text);
		void Error(string text);
	}
	
	public class MessageDialogPresenter : IMessageDialogPresenter
	{
		private IMessageDialogView view;
		
		public MessageDialogPresenter(IMessageDialogView view)
		{
			this.view = view;
		}
		public bool YesNo(string text)
		{
			return view.ShowYesNo("Question", text);
		}
		public void Error(string text)
		{
			view.ShowError("Error", text);
		}
	}
}
