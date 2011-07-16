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
using Rhino.Mocks;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	[TestFixture]
	public class SkipPathByExtensionTest
	{
		private SkipPathByExtension selector;
		private string[] extensions = new string[] { ".h", ".cpp" };
		
		[SetUp]
		public void SetUp()
		{
			selector = new SkipPathByExtension()
			{
				Extensions = extensions
			};
		}
		[Test]
		public void Should_ignore_matched_paths()
		{
			foreach (var ext in extensions)
			{
				selector.InSelection(ext)
					.Should().Be.False();
			}

			selector.InSelection("file.cs")
				.Should().Be.True();
		}
		[Test]
		public void Should_be_case_insensitive()
		{
			selector.InSelection("file.CPP")
				.Should().Be.False();
		}
		[Test]
		public void Can_be_case_sensitive()
		{
			selector.IgnoreCase = false;
			
			selector.InSelection("file.CPP")
				.Should().Be.True();
		}
	}
}
