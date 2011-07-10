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
	public class TakePathByListTest
	{
		private TakePathByList selector;

		[SetUp]
		public void SetUp()
		{
			selector = new TakePathByList();
		}
		[Test]
		public void Should_take_path_or_directory()
		{
			selector.DirList = new string[] { "/dir1", "/dir2" };
			selector.PathList = new string[] { "/dir3/file3" };

			selector.InSelection("/dir1/file1")
				.Should().Be.True();
			selector.InSelection("/dir2/file2")
				.Should().Be.True();
			selector.InSelection("/dir3/file3")
				.Should().Be.True();
			selector.InSelection("/dir3/file4")
				.Should().Be.False();
		}
	}
}
