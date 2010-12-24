/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Text.RegularExpressions;

namespace MSR.Data.Entities.Mapping
{
	public class BugFixDetectorBasedOnLogMessage : IBugFixDetector
	{
		public BugFixDetectorBasedOnLogMessage()
		{
			MessageRegExp = @"(\P{L}|^)(fix|fixes|bug|bugs|bugfix|bugfixes|fixed)(\P{L}|$)";
		}
		public bool IsBugFix(Commit commit)
		{
			if (commit.Message.ToLower().IndexOf("warning") > 0)
			{
				return false;
			}
			return Regex.IsMatch(commit.Message, MessageRegExp, RegexOptions.IgnoreCase);
		}
		public string MessageRegExp
		{
			get; set;
		}
	}
}
