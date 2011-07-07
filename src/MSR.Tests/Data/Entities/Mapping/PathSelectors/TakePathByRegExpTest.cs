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
	public class TakePathByRegExpTest
	{
		private TakePathByRegExp selector;

		[SetUp]
		public void SetUp()
		{
			selector = new TakePathByRegExp();
		}
		[Test]
		public void Should_select_file()
		{
			selector.FilePath = "/dir/file1";

			selector.InSelection("/dir/file1")
				.Should().Be.True();
			selector.InSelection("/dir/file2")
				.Should().Be.False();
		}
		[Test]
		public void Should_select_files_in_directory()
		{
			selector.DirPath = "/dir1";

			selector.InSelection("/dir1/file1")
				.Should().Be.True();
			selector.InSelection("/dir1/file2")
				.Should().Be.True();
			selector.InSelection("/dir2/file2")
				.Should().Be.False();
			selector.InSelection("/dir/1")
				.Should().Be.False();
		}
	}
}
