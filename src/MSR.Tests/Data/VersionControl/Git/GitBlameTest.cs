/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.VersionControl.Git
{
	[TestFixture]
	public class GitBlameTest
	{
		private GitBlame blame;

private string blame1 =
@"7c92fe0eaa4fb89e27fa3617b9ae52f20b511573 (Jeff King           2006-09-08 04:03:18 -0400   1) #include 'cache.h'
85023577a8f4b540aa64aa37f6f44578c0c305a3 (Junio C Hamano      2006-12-19 14:34:12 -0800   2) #include 'color.h'
7c92fe0eaa4fb89e27fa3617b9ae52f20b511573 (Jeff King           2006-09-08 04:03:18 -0400   3) 
6b2f2d9805dd22c6f74957e0d76a1d2921b40c16 (Matthias Kestenholz 2008-02-18 08:26:03 +0100   4) int git_use_color_default = 0;
6b2f2d9805dd22c6f74957e0d76a1d2921b40c16 (Matthias Kestenholz 2008-02-18 08:26:03 +0100   5) 
7c92fe0eaa4fb89e27fa3617b9ae52f20b511573 (Jeff King           2006-09-08 04:03:18 -0400   6) static int parse_color(const char *name, int len)
7c92fe0eaa4fb89e27fa3617b9ae52f20b511573 (Jeff King           2006-09-08 04:03:18 -0400   7) {";

		[Test]
		public void Should_keep_revisions_for_each_line()
		{
			blame = GitBlame.Parse(blame1.ToStream());
			
			blame[1]
				.Should().Be("7c92fe0eaa4fb89e27fa3617b9ae52f20b511573");
			blame[5]
				.Should().Be("6b2f2d9805dd22c6f74957e0d76a1d2921b40c16");
			blame[7]
				.Should().Be("7c92fe0eaa4fb89e27fa3617b9ae52f20b511573");
		}
	}
}
