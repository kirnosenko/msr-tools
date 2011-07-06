/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.Entities.Mapping
{
	[TestFixture]
	public class BugFixDetectorBasedOnLogMessageTest
	{
		private BugFixDetectorBasedOnLogMessage detector;
		private Commit commit;

		[SetUp]
		public void SetUp()
		{
			detector = new BugFixDetectorBasedOnLogMessage();
			commit = new Commit();
		}
		[TestCase("simple log message", false)]
		[TestCase("some fixtures were added", false)]
		[TestCase("new string prefix", false)]
		[TestCase("fix for some feature", true)]
		[TestCase("this is fix too", true)]
		[TestCase("it is bugfix", true)]
		[TestCase("bug was fixed", true)]
		[TestCase("One More Fix.", true)]
		public void Can_detect_commit_with_word_fix_in_message_as_bugfix(string message, bool isBugFix)
		{
			commit.Message = message;
			detector.IsBugFix(commit)
				.Should().Be(isBugFix);
		}
		[TestCase("Fixed warnings", false)]
		[TestCase("PATCH: [ 715553 ] Add commandline options to not add paths to MRU", false)]
		[TestCase("Apply icon change (2004-03-13 Seier) to languages", false)]
		[TestCase("Text modifications", false)]
		[TestCase("Fix 'cannot select CDROM directory' problem", true)]
		[TestCase("bug fix scroll to first diff.", true)]
		[TestCase("fix some bugs with undo and merge", true)]
		[TestCase("bugfix to use system colours for unselected lines in directory.", true)]
		[TestCase("Bug #125572: state of recurse checkbox is now saved", true)]
		[TestCase("[ 686699 ] Check &amp; return file saving success - fix", true)]
		[TestCase("BUG: [ 683753 ] Rescan is not prompting to save dirty current file", true)]
		[TestCase("PATCH: [ 709502 ] Fix missing/existing EOL at end of file", true)]
		public void Can_detect_bugfix_for_winmerge(string message, bool isBugFix)
		{
			commit.Message = message;
			detector.IsBugFix(commit)
				.Should().Be(isBugFix);
		}
		[TestCase("this is error", true)]
		[TestCase("this is not error", false)]
		public void Can_be_configured_with_keywords_and_stopwords(string message, bool isBugFix)
		{
			commit.Message = message;
			
			detector.KeyWords = "bug error";
			detector.StopWords = "not no";
			
			detector.IsBugFix(commit)
				.Should().Be(isBugFix);
		}
	}
}
