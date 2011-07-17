/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSR.Data.Entities.Mapping
{
	public class BugFixDetectorBasedOnLogMessage : IBugFixDetector
	{
		public BugFixDetectorBasedOnLogMessage()
		{
			KeyWords = new string[]
			{
				"fix",
				"fixes",
				"bug",
				"bugs",
				"bugfix",
				"bugfixes",
				"fixed"
			};
			StopWords = new string[]
			{
				"warning",
				"typo",
				"grammar"
			};
		}
		public bool IsBugFix(Commit commit)
		{
			if (! Regex.IsMatch(commit.Message, MessageRegExp, RegexOptions.IgnoreCase))
			{
				return false;
			}
			string messageToLower = commit.Message.ToLower();
			if (StopWords.Any(x => messageToLower.IndexOf(x) > 0))
			{
				return false;
			}
			return true;
		}
		public string MessageRegExp
		{
			get; set;
		}
		public string[] KeyWords
		{
			set
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < value.Length-1; i++)
				{
					sb.Append(value[i]);
					sb.Append("|");
				}
				sb.Append(value[value.Length-1]);
				
				MessageRegExp = @"(\P{L}|^)(" + sb.ToString() + @")(\P{L}|$)";
			}
		}
		public string[] StopWords
		{
			get; set;
		}
	}
}
