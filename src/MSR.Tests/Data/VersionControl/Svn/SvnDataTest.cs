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

namespace MSR.Data.VersionControl.Svn
{
	[TestFixture]
	public class SvnDataTest
	{

private string SvnInfo =
@"Path: svn
URL: file:///E:/repo/gnome-terminal/svn
Repository Root: file:///E:/repo/gnome-terminal/svn
Repository UUID: e32f9464-e525-0410-8908-8a3b6990da27
Revision: 3436
Node Kind: directory
Last Changed Author: markkr
Last Changed Rev: 3436
Last Changed Date: 2009-04-16 14:01:42 +0400 (Чт, 16 апр 2009)";

		private SvnData svnData;
		private ISvnClient svnStub;
		private string nl = Environment.NewLine;

		[SetUp]
		public void SetUp()
		{
			svnStub = MockRepository.GenerateStub<ISvnClient>();
			svnStub.Stub(x => x.Info())
				.Return(SvnInfo.ToStream());
			svnData = new SvnData(svnStub);
		}
		[Test]
		public void Should_return_revision_by_number()
		{
			svnData.RevisionByNumber(1)
				.Should().Be("1");
			svnData.RevisionByNumber(3000)
				.Should().Be("3000");
			svnData.RevisionByNumber(4000)
				.Should().Be.Null();
		}
		[Test]
		public void Should_return_next_revision()
		{
			svnData.NextRevision("1")
				.Should().Be("2");
			svnData.NextRevision("3435")
				.Should().Be("3436");
			svnData.NextRevision("3436")
				.Should().Be.Null();
		}
	}
}
