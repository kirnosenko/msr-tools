/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

namespace MSR.Data.VersionControl.Git
{
	[TestFixture]
	public class GitDataTest
	{
		private GitData gitData;
		private IGitClient gitStub;
		private string nl = Environment.NewLine;
		
		[SetUp]
		public void SetUp()
		{
			gitStub = MockRepository.GenerateStub<IGitClient>();
			gitData = new GitData(gitStub);
		}
		[Test]
		public void Should_return_revision_by_number()
		{
			gitStub.Stub(x => x.RevList())
				.Return(("a" + nl + "b" + nl + "c").ToStream());

			gitData.RevisionByNumber(1)
				.Should().Be("a");
			gitData.RevisionByNumber(2)
				.Should().Be("b");
			gitData.RevisionByNumber(3)
				.Should().Be("c");
			gitData.RevisionByNumber(4)
				.Should().Be.Null();
		}
		[Test]
		public void Should_return_next_revision()
		{
			gitStub.Stub(x => x.RevList())
				.Return(("a" + nl + "b" + nl + "c").ToStream());
			
			gitData.NextRevision("a")
				.Should().Be("b");
			gitData.NextRevision("b")
				.Should().Be("c");
			gitData.NextRevision("c")
				.Should().Be.Null();
		}
	}
}
