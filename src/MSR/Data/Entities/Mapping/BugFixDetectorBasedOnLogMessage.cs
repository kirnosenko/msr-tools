/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSR.Data.Entities.Mapping
{
	public class BugFixDetectorBasedOnLogMessage : IBugFixDetector
	{
		private static char[] WordsSeparator = new char[] { ' ' };
		
		private IEnumerable<string> stopWords;
		
		public BugFixDetectorBasedOnLogMessage()
		{
			MessageRegExp = @"(\P{L}|^)(fix|fixes|bug|bugs|bugfix|bugfixes|fixed)(\P{L}|$)";
			stopWords = new string[] { "warning" };
		}
		public bool IsBugFix(Commit commit)
		{
			if (! Regex.IsMatch(commit.Message, MessageRegExp, RegexOptions.IgnoreCase))
			{
				return false;
			}
			string messageToLower = commit.Message.ToLower();
			if (stopWords.Any(x => messageToLower.IndexOf(x) > 0))
			{
				return false;
			}
			return true;
		}
		public string MessageRegExp
		{
			get; set;
		}
		public string KeyWords
		{
			set
			{
				string keywords = value.Replace(' ', '|');

				MessageRegExp = @"(\P{L}|^)(" + keywords + @")(\P{L}|$)";
			}
		}
		public string StopWords
		{
			set
			{
				stopWords = value.Split(WordsSeparator);
			}
		}
	}
}
