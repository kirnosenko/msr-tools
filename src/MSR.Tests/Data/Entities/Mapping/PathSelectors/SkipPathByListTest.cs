/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	[TestFixture]
	public class SkipPathByListTest
	{
		private SkipPathByList selector;

		[SetUp]
		public void SetUp()
		{
			selector = new SkipPathByList();

			selector.Dirs = new string[] { "/dir1" };
			selector.Paths = new string[] { "/dir2/file2", "/dir2/file3" };
		}
		[Test]
		public void Should_ignore_path_or_directory()
		{
			selector.InSelection("/dir1/file1")
				.Should().Be.False();
			selector.InSelection("/dir2/file2")
				.Should().Be.False();
			selector.InSelection("/dir2/file3")
				.Should().Be.False();
			selector.InSelection("/dir2/file4")
				.Should().Be.True();
		}
		[Test]
		public void Should_be_case_insensitive()
		{
			selector.InSelection("/DIR1/file1")
				.Should().Be.False();
			selector.InSelection("/dir2/File2")
				.Should().Be.False();
		}
		[Test]
		public void Can_be_case_sensitive()
		{
			selector.IgnoreCase = false;

			selector.InSelection("/DIR1/file1")
				.Should().Be.True();
			selector.InSelection("/dir2/File2")
				.Should().Be.True();
		}
	}
}
