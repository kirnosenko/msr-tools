/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.VersionControl.Svn
{
	[TestFixture]
	public class SvnBlameTest
	{

private string blame1 =
@"   357    michael       if (terminal_invoke_factory (argc_copy, argv_copy))
  1000    mariano         {
  1000    mariano           g_strfreev (argv_copy);
  2331     behdad           option_parsing_results_free (parsing_results);
  1000    mariano           return 0;
  1000    mariano         }
  2843       chpe       /* FIXME: else return 1; ? */
";

		private SvnBlame blame;
		
		[Test]
		public void Should_keep_revision_for_each_line()
		{
			blame = SvnBlame.Parse(blame1.ToStream());
			
			blame[1]
				.Should().Be("357");
			blame[2]
				.Should().Be("1000");
			blame[7]
				.Should().Be("2843");
		}
	}
}
