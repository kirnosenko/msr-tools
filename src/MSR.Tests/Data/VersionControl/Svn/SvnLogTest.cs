/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using SharpTestsEx;
using Rhino.Mocks;

namespace MSR.Data.VersionControl.Svn
{
	[TestFixture]
	public class SvnLogTest
	{
		private string logXml_1 = 
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='11'>
		<author>hp</author>
		<date>2002-01-16T06:06:17.000000Z</date>
		<paths>
		<path
		   kind=''
		   action='A'>/trunk/po</path>
		<path
		   kind=''
		   action='M'>/trunk/ChangeLog</path>
		<path
		   kind=''
		   action='A'>/trunk/po/POTFILES.in</path>
		<path
		   kind=''
		   action='M'>/trunk/autogen.sh</path>
		<path
		   kind=''
		   action='M'>/trunk/src/profile-editor.c</path>
		</paths>
		<msg>2002-01-16  Havoc Pennington  &lt;hp@pobox.com&gt;

			* configure.in: fixes

			* autogen.sh: get rid of horrible autogen subdirs feature and
			fix to use glib-gettextize
		</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_1 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/ChangeLog</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/src/profile-editor.c</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/autogen.sh</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/trunk/po/POTFILES.in</path>
		<path
		   props='none'
		   kind='dir'
		   item='added'>file:///E:/repo/gnome-terminal/svn/trunk/po</path>
		</paths>
		</diff>
		";
		
		private string logXml_2 =
		@"<log>
		<logentry
		   revision='1242'>
		<author>mariano</author>
		<date>2004-06-07T16:24:23.000000Z</date>
		<paths>
		<path
		   kind=''
		   action='M'>/trunk/src/terminal-screen.c</path>
		<path
		   kind=''
		   action='M'>/trunk/src/terminal-widget-vte.c</path>
		<path
		   kind=''
		   action='D'>/trunk/src/terminal-widget-zvt.c</path>
		<path
		   kind=''
		   action='M'>/trunk/ChangeLog</path>
		<path
		   kind=''
		   action='M'>/trunk/src/terminal-widget.h</path>
		</paths>
		<msg>2004-06-07 Mariano SuA?rez-Alvarez &lt;msuarezalvarez@arnet.com.ar&gt;
		</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_2 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='file'
		   item='deleted'>file:///E:/repo/gnome-terminal/svn/trunk/src/terminal-widget-zvt.c</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/src/terminal-screen.c</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/src/terminal-widget.h</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/src/terminal-widget-vte.c</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-terminal/svn/trunk/ChangeLog</path>
		</paths>
		</diff>
		";
		
		private string logXml_3 =
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='1217'>
		<date>2004-04-18T06:02:38.000000Z</date>
		<paths>
		<path
		   kind=''
		   copyfrom-path='/branches/gnome-2-6/NEWS'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/NEWS</path>
		<path
		   kind=''
		   copyfrom-path='/branches/gnome-2-6/po'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/po</path>
		<path
		   kind=''
		   copyfrom-path='/trunk'
		   copyfrom-rev='1189'
		   action='A'>/tags/GNOME_TERMINAL_2_6_1</path>
		<path
		   kind=''
		   copyfrom-path='/branches/gnome-2-6/gnome-terminal.desktop.in.in'
		   copyfrom-rev='1216'
		   action='A'>/tags/GNOME_TERMINAL_2_6_1/gnome-terminal.desktop.in.in</path>
		<path
		   kind=''
		   copyfrom-path='/branches/gnome-2-6/configure.in'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/configure.in</path>
		<path
		   kind=''
		   copyfrom-path='/branches/gnome-2-6/ChangeLog'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/ChangeLog</path>
		<path
		   kind=''
		   copyfrom-path='/trunk/po/POTFILES.in'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/po/POTFILES.in</path>
		<path
		   kind=''
		   copyfrom-path='/trunk/po/.cvsignore'
		   copyfrom-rev='1216'
		   action='R'>/tags/GNOME_TERMINAL_2_6_1/po/.cvsignore</path>
		<path
		   kind=''
		   action='D'>/tags/GNOME_TERMINAL_2_6_1/gnome-terminal.desktop.in</path>
		</paths>
		<msg>This commit was manufactured by cvs2svn to create tag
		'GNOME_TERMINAL_2_6_1'.</msg>
		</logentry>
		</log>
		";

		private string diffSumXml_3 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/configure.in</path>
		<path
		   props='modified'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/src/terminal.c</path>
		<path
		   props='modified'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/src/terminal.h</path>
		<path
		   props='modified'
		   kind='dir'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/src</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/ChangeLog</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/README</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/gnome-terminal.desktop.in.in</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/NEWS</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/po/POTFILES.in</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/po/.cvsignore</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/po/ar.po</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/po/gl.po</path>
		<path
		   props='modified'
		   kind='dir'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1/po</path>
		<path
		   props='modified'
		   kind='dir'
		   item='added'>file:///E:/repo/gnome-terminal/svn/tags/GNOME_TERMINAL_2_6_1</path>
		</paths>
		</diff>
		";
		
		private string logXml_4 =
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='4711'>
		<author>gicmo</author>
		<date>2005-11-26T10:46:56.000000Z</date>
		<paths>
		<path
		   kind=''
		   copyfrom-path='/branches/NEON_DIST/imported/neon/ne_207.c'
		   copyfrom-rev='4710'
		   action='R'>/trunk/imported/neon/ne_207.c</path>
		<path
		   kind=''
		   copyfrom-path='/branches/NEON_DIST/imported/neon/ne_207.h'
		   copyfrom-rev='4710'
		   action='R'>/trunk/imported/neon/ne_207.h</path>
		</paths>
		<msg>This commit was generated by cvs2svn to compensate for changes in r4710,
		which included commits to RCS files with non-trunk default branches.
		</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_4 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-vfs/svn/trunk/imported/neon/ne_207.h</path>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/gnome-vfs/svn/trunk/imported/neon/ne_207.c</path>
		</paths>
		</diff>
		";
		
		private string logXml_5 = 
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='1404'>
		<author>mariano</author>
		<date>2004-11-05T04:54:49.000000Z</date>
		<paths>
		<path
		   kind=''
		   action='D'>/trunk/src/Attic</path>
		</paths>
		<msg>Remove this which is now useless
		</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_5 = 
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='dir'
		   item='deleted'>file:///E:/repo/gnome-terminal/svn/trunk/src/Attic</path>
		</paths>
		</diff>
		";

private string list_5_1 =
@".cvsignore
eel/
";
		
private string list_5_2 =
@".cvsignore
";
		
		private string logXml_6 =
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='4575'>
		<author>steverstrong</author>
		<date>2009-07-04T23:39:49.051864Z</date>
		<paths>
		<path
		   kind='file'
		   action='R'>/trunk/nhibernate/src/NHibernate.Test/NHSpecificTest/NH1849/Fixture.cs</path>
		</paths>
		<msg>Fix for NH1849, plus some improvement to AST error reporting</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_6 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='none'
		   kind='file'
		   item='modified'>file:///E:/repo/nhibernate/svn/trunk/nhibernate/src/NHibernate.Test/NHSpecificTest/NH1849/Fixture.cs</path>
		</paths>
		</diff>
		";
		
		private string logXml_7 =
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='3868'>
		<author>fabiomaulo</author>
		<date>2008-10-20T15:43:09.135722Z</date>
		<paths>
		<path
		   kind='file'
		   action='A'>/trunk/nhibernate/src/NHibernate.Test/HQL/HqlFixture.cs</path>
		<path
		   kind='dir'
		   copyfrom-path='/trunk/nhibernate/src/NHibernate.Test/HQLFunctionTest'
		   copyfrom-rev='3865'
		   action='A'>/trunk/nhibernate/src/NHibernate.Test/HQL</path>
		<path
		   kind='file'
		   action='M'>/trunk/nhibernate/src/NHibernate.Test/HQL/Animal.cs</path>
		</paths>
		<msg>- Fix NH-1538, NH-1537 (new feature comments)
		- Refactoring in tests (to have only one namespace for various HQL tests)</msg>
		</logentry>
		</log>
		";
		
		private string diffSumXml_7 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		<path
		   props='modified'
		   kind='file'
		   item='added'>file:///E:/repo/nhibernate/svn/trunk/nhibernate/src/NHibernate.Test/HQL/HqlFixture.cs</path>
		<path
		   props='none'
		   kind='file'
		   item='added'>file:///E:/repo/nhibernate/svn/trunk/nhibernate/src/NHibernate.Test/HQL/Animal.cs</path>
		<path
		   props='modified'
		   kind='dir'
		   item='added'>file:///E:/repo/nhibernate/svn/trunk/nhibernate/src/NHibernate.Test/HQL</path>
		</paths>
		</diff>
		";
		
		private string logXml_8 =
		@"<?xml version='1.0'?>
		<log>
		<logentry
		   revision='4166'>
		<author>darioquintana</author>
		<date>2009-03-29T15:36:13.952775Z</date>
		<paths>
		<path
		   kind='file'
		   copyfrom-path='/trunk/nhibernate/src/NHibernate.Test/TypesTest/TimeSpanInt64Class.cs'
		   copyfrom-rev='4165'
		   action='R'>/trunk/nhibernate/src/NHibernate.Test/TypesTest/TimeSpanClass.cs</path>
		</paths>
		<msg>- the actual TimeSpan type moved to TimeAsTimeSpan.
		- TimeSpanInt64 type moved back to TimeSpan (related to NH-1617)
		</msg>
		</logentry>
		</log>";
		
		private string diffSumXml_8 =
		@"<?xml version='1.0'?>
		<diff>
		<paths>
		</paths>
		</diff>
		";
		
		private string repositoryPath = "file:///E:/repo/gnome-terminal/svn";
		
		private SvnLog log;
		private ISvnClient svnStub;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			logXml_1 = logXml_1.Replace('\'', '"');
			logXml_2 = logXml_2.Replace('\'', '"');
			logXml_3 = logXml_3.Replace('\'', '"');
			logXml_4 = logXml_4.Replace('\'', '"');
			logXml_5 = logXml_5.Replace('\'', '"');
			logXml_6 = logXml_6.Replace('\'', '"');
			logXml_7 = logXml_7.Replace('\'', '"');
			logXml_8 = logXml_8.Replace('\'', '"');
			diffSumXml_1 = diffSumXml_1.Replace('\'', '"');
			diffSumXml_2 = diffSumXml_2.Replace('\'', '"');
			diffSumXml_3 = diffSumXml_3.Replace('\'', '"');
			diffSumXml_4 = diffSumXml_4.Replace('\'', '"');
			diffSumXml_5 = diffSumXml_5.Replace('\'', '"');
			diffSumXml_6 = diffSumXml_6.Replace('\'', '"');
			diffSumXml_7 = diffSumXml_7.Replace('\'', '"');
			diffSumXml_8 = diffSumXml_8.Replace('\'', '"');
		}
		[SetUp]
		public void SetUp()
		{
			svnStub = MockRepository.GenerateStub<ISvnClient>();
		}
		[Test]
		public void Should_parse_general_data_about_revision()
		{
			log = CreateLog("11", logXml_1.ToStream(), null, "");
			
			log.Revision
				.Should().Be("11");
			log.Author
				.Should().Be("hp");
			log.Date
				.Should().Be(new DateTime(2002, 1, 16, 6, 6, 17));
			log.Message
				.Should()
					.Contain("configure.in: fixes")
					.And
					.Contain("autogen.sh: get rid of horrible autogen subdirs feature");
		}
		[Test]
		public void Should_keep_alphabetically_sorted_list_of_touched_paths()
		{
			log = CreateLog("11", logXml_1.ToStream(), diffSumXml_1.ToStream(), repositoryPath);
			
			log.TouchedFiles.Select(x => x.Path).ToArray()
				.Should().Have.SameSequenceAs(new string[]
				{
					"/trunk/autogen.sh",
					"/trunk/ChangeLog",
					"/trunk/po/POTFILES.in",
					"/trunk/src/profile-editor.c"
				});
		}
		[Test]
		public void Should_keep_action_for_each_touched_path()
		{
			log = CreateLog("11", logXml_1.ToStream(), diffSumXml_1.ToStream(), repositoryPath);
			
			log.TouchedFiles.Select(x => x.Action).ToArray()
				.Should().Have.SameSequenceAs(new TouchedFile.TouchedFileAction[]
				{
					TouchedFile.TouchedFileAction.MODIFIED,
					TouchedFile.TouchedFileAction.MODIFIED,
					TouchedFile.TouchedFileAction.ADDED,
					TouchedFile.TouchedFileAction.MODIFIED,
				});
		}
		[Test]
		public void Should_determine_action_for_added_and_replaced_files_as_added()
		{
			log = CreateLog("1217", logXml_3.ToStream(), diffSumXml_3.ToStream(), repositoryPath);

			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.ADDED).Count()
				.Should().Be(11);
			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.MODIFIED).Count()
				.Should().Be(0);
			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.DELETED).Count()
				.Should().Be(0);
		}
		[Test]
		public void Should_keep_replaced_files_as_deleted_and_added()
		{
			log = CreateLog("4711", logXml_4.ToStream(), diffSumXml_4.ToStream(), "file:///E:/repo/gnome-vfs/svn");

			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.DELETED).Count()
				.Should().Be(2);
			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.ADDED).Count()
				.Should().Be(2);
			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.MODIFIED).Count()
				.Should().Be(0);
		}
		[Test]
		public void Should_keep_files_added_in_copied_directory()
		{
			log = CreateLog("3868", logXml_7.ToStream(), diffSumXml_7.ToStream(), "file:///E:/repo/nhibernate/svn");
			
			log.TouchedFiles.Where(x => x.Action == TouchedFile.TouchedFileAction.ADDED).Count()
				.Should().Be(2);
			log.TouchedFiles.Single(x => x.Path == "/trunk/nhibernate/src/NHibernate.Test/HQL/Animal.cs")
				.Satisfy(x =>
					x.SourcePath == "/trunk/nhibernate/src/NHibernate.Test/HQLFunctionTest/Animal.cs"
					&&
					x.SourceRevision == "3865"
				);
			log.TouchedFiles.Single(x => x.Path == "/trunk/nhibernate/src/NHibernate.Test/HQL/HqlFixture.cs")
				.Satisfy(x =>
					x.SourcePath == null
					&&
					x.SourceRevision == null
				);
		}
		[Test]
		public void Should_not_take_sumdiff_if_touched_paths_were_not_requested()
		{
			log = CreateLog("11", logXml_1.ToStream(), null, "");
			
			string info = log.Revision + log.Author + log.Message + log.Date.ToString();
			
			svnStub.AssertWasNotCalled(x => x.DiffSum("11"));
		}
		[Test]
		public void Should_set_source_path_and_revision_for_files_in_copied_dir()
		{
			log = CreateLog("1217", logXml_3.ToStream(), diffSumXml_3.ToStream(), repositoryPath);

			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/README")
				.Satisfy(x =>
					x.SourcePath == "/trunk/README" &&
					x.SourceRevision == "1189"
				);
			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/src/terminal.c")
				.Satisfy(x =>
					x.SourcePath == "/trunk/src/terminal.c" &&
					x.SourceRevision == "1189"
				);
			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/po/ar.po")
				.Satisfy(x =>
					x.SourcePath == "/branches/gnome-2-6/po/ar.po" &&
					x.SourceRevision == "1216"
				);
		}
		[Test]
		public void Should_set_source_path_and_revision_for_copied_files()
		{
			log = CreateLog("1217", logXml_3.ToStream(), diffSumXml_3.ToStream(), repositoryPath);

			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/NEWS")
				.Satisfy(x =>
					x.SourcePath == "/branches/gnome-2-6/NEWS" &&
					x.SourceRevision == "1216"
				);
			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/gnome-terminal.desktop.in.in")
				.Satisfy(x =>
					x.SourcePath == "/branches/gnome-2-6/gnome-terminal.desktop.in.in" &&
					x.SourceRevision == "1216"
				);
			log.TouchedFiles.Single(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/po/POTFILES.in")
				.Satisfy(x =>
					x.SourcePath == "/trunk/po/POTFILES.in" &&
					x.SourceRevision == "1216"
				);
		}
		[Test]
		public void Should_not_keep_file_that_was_added_and_deleted_in_the_same_commit()
		{
			log = CreateLog("1217", logXml_3.ToStream(), diffSumXml_3.ToStream(), repositoryPath);
			
			log.TouchedFiles.SingleOrDefault(x => x.Path == "/tags/GNOME_TERMINAL_2_6_1/gnome-terminal.desktop.in")
				.Should().Be.Null();
		}
		[Test]
		public void Should_keep_deletes_files_for_deleted_directory()
		{
			log = CreateLog("1404", logXml_5.ToStream(), diffSumXml_5.ToStream(), repositoryPath);
			svnStub.Stub(x => x.List("1403", "/trunk/src/Attic"))
				.Return(list_5_1.ToStream());
			svnStub.Stub(x => x.List("1403", "/trunk/src/Attic/eel"))
				.Return(list_5_2.ToStream());

			log.TouchedFiles.Select(x => x.Path).ToArray()
				.Should().Have.SameValuesAs(new string[]
				{
					"/trunk/src/Attic/.cvsignore",
					"/trunk/src/Attic/eel/.cvsignore"
				});
		}
		[Test]
		public void Should_keep_replacing_without_source_as_modification()
		{
			log = CreateLog("4575", logXml_6.ToStream(), diffSumXml_6.ToStream(), "file:///E:/repo/nhibernate/svn");
			
			log.TouchedFiles.Count()
				.Should().Be(1);
			log.TouchedFiles.Single().Action
				.Should().Be(TouchedFile.TouchedFileAction.MODIFIED);
		}
		[Test]
		public void Should_be_able_to_process_strange_situation_when_no_path_in_diff()
		{
			log = CreateLog("4166", logXml_8.ToStream(), diffSumXml_8.ToStream(), "file:///E:/repo/nhibernate/svn");
		}
		private SvnLog CreateLog(string revision, Stream log, Stream diffSum, string repositoryPath)
		{
			svnStub.Stub(x => x.Log(revision))
				.Return(log);
			svnStub.Stub(x => x.DiffSum(revision))
				.Return(diffSum);
			svnStub.Stub(x => x.RepositoryPath)
				.Return(repositoryPath);
			return new SvnLog(svnStub, revision);
		}
	}
}
