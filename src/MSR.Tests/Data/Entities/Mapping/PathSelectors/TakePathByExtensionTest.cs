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
	public class TakePathByExtensionTest
	{
		private TakePathByExtension selector;
		private string[] extensions = new string[] { ".h", ".cpp" };
		
		[SetUp]
		public void SetUp()
		{
			selector = new TakePathByExtension()
			{
				Extensions = extensions
			};
		}
		[Test]
		public void Should_select_matched_paths()
		{
			foreach (var ext in extensions)
			{
				selector.InSelection(ext)
					.Should().Be.True();
			}
			
			selector.InSelection(".cs")
				.Should().Be.False();
		}
	}
}
