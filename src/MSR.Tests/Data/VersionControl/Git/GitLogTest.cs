/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.VersionControl.Git
{
	[TestFixture]
	public class GitLogTest
	{

private string log_1 =
@"4bb04f2190d526f8917663f0be62d8026e1ed100
Linus Torvalds
2005-04-11 15:47:57 -0700
Rename '.dircache' directory to '.git'
M	README
M	cache.h
M	init-db.c
M	read-cache.c
M	read-tree.c
M	update-cache.c";

private string log_2 =
@"a3df180138b85a603656582bde6df757095618cf
Linus Torvalds
2005-04-29 14:09:11 -0700
Rename git core commands to be 'git-xxxx' to avoid name clashes.
M	Makefile
R100	show-diff.c	diff-files.c
R100	git-export.c	export.c
R100	git-mktag.c	mktag.c";
		
		private GitLog log;
		
		[Test]
		public void Should_parse_general_data_about_revision()
		{
			log = new GitLog(log_1.ToStream());
			
			log.Revision
				.Should().Be("4bb04f2190d526f8917663f0be62d8026e1ed100");
			log.Author
				.Should().Be("Linus Torvalds");
			log.Date
				.Should().Be(DateTime.Parse("12.04.2005 02:47:57"));
			log.Message
				.Should().Be("Rename '.dircache' directory to '.git'");
		}
		[Test]
		public void Should_keep_alphabetically_sorted_list_of_touched_paths()
		{
			log = new GitLog(log_1.ToStream());
			
			log.TouchedPaths.Select(x => x.Path).ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"cache.h",
					"init-db.c",
					"read-cache.c",
					"read-tree.c",
					"README",
					"update-cache.c"
				});
		}
		[Test]
		public void Should_interpret_renamed_file_as_deleted_and_added()
		{
			log = new GitLog(log_2.ToStream());
			
			log.TouchedPaths
				.Where(x => x.Action == TouchedPath.TouchedPathAction.DELETED)
				.Select(x => x.Path).ToArray()
					.Should().Have.SameValuesAs(new string[]
					{
						"show-diff.c",
						"git-export.c",
						"git-mktag.c"
					});
			log.TouchedPaths
				.Where(x => x.Action == TouchedPath.TouchedPathAction.ADDED)
				.Select(x => x.Path).ToArray()
					.Should().Have.SameValuesAs(new string[]
					{
						"diff-files.c",
						"export.c",
						"mktag.c"
					});
		}
		[Test]
		public void Should_keep_source_path_for_renamed_path()
		{
			log = new GitLog(log_2.ToStream());
			
			log.TouchedPaths
				.Single(x => x.Path == "export.c")
				.Satisfy(x =>
					x.SourcePath == "git-export.c"
					&&
					x.SourceRevision == null
				);
		}
	}
}
