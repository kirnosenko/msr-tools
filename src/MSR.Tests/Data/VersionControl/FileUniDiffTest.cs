/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.VersionControl
{
	[TestFixture]
	public class FileUniDiffTest
	{

private string diff1 =
@"Index: test/Program.cs
===================================================================
--- test/Program.cs	(revision 2)
+++ test/Program.cs	(revision 3)
@@ -9,8 +9,7 @@
 	{
 		static void Main(string[] args)
 		{
-			DoThis();
-			DoThat();
+			DoAll();
 			Console.ReadKey();
 		}
 		static void DoThis()
@@ -21,5 +20,10 @@
 		{
 			Console.WriteLine(""that"");
 		}
+		static void DoAll()
+		{
+			DoThis();
+			DoThat();
+		}
 	}
 }
";

private string diff2 =
@"19b2860cba5742ab31fd682b80fefefac19be141
diff --git a/read-cache.c b/read-cache.c
index b151981..60a0b5b 100644
--- a/read-cache.c
+++ b/read-cache.c
@@ -89,7 +89,7 @@ void * read_sha1_file(unsigned char *sha1, char *type, unsigned long *size)
 	z_stream stream;
 	char buffer[8192];
 	struct stat st;
-	int i, fd, ret, bytes;
+	int fd, ret, bytes;
 	void *map, *buf;
 	char *filename = sha1_file_name(sha1);
 
@@ -173,7 +173,7 @@ int write_sha1_file(char *buf, unsigned len)
 int write_sha1_buffer(unsigned char *sha1, void *buf, unsigned int size)
 {
 	char *filename = sha1_file_name(sha1);
-	int i, fd;
+	int fd;
 
 	fd = open(filename, O_WRONLY | O_CREAT | O_EXCL, 0666);
 	if (fd < 0)
@@ -228,6 +228,7 @@ int read_cache(void)
 	if (fd < 0)
 		return (errno == ENOENT) ? 0 : error('open failed');
 
+	size = 0; // avoid gcc warning
 	map = (void *)-1;
 	if (!fstat(fd, &st)) {
 		map = NULL;
";

private string diff4 =
@"Index: trunk/AUTHORS
===================================================================
--- trunk/AUTHORS	(revision 123)
+++ trunk/AUTHORS	(revision 124)
@@ -1 +1,16 @@
-Ettore Perazzoli <ettore@comm2000.it>
\ No newline at end of file
+Main author:
+	Ettore Perazzoli <ettore@gnu.org>
+
+FTP method:
+	Miguel de Icaza <miguel@gnu.org>, based on the GNU Midnight
+	Commander code by:
+		Norbert Warmuth
+		Ching Hui
+		Jakup Jelinek
+		Pavel Macheck
+
+BZIP2 method:
+	Cody Russell <bratsche@dfw.net>
+
+GConf method:
+	Dave Camp <campd@oit.edu>
";

private string diff5 =
@"Index: gnome-terminal.xml
===================================================================
--- gnome-terminal.xml	(revision 306)
+++ gnome-terminal.xml	(revision 307)
@@ -1,196 +1,1411 @@
-<?xml version='1.0'?>
-  Remember that this is a guide, rather than a perfect model to follow
-  slavishly. Make your manual logical and readable.  And don't forget
-  to remove these comments in your final documentation!  ;-)
--->
";

		private FileUniDiff diff;

		[Test]
		public void Should_be_empty_for_empty_stream()
		{
			diff = new FileUniDiff("".ToStream());

			diff.AddedLines.Count()
				.Should().Be(0);
			diff.RemovedLines.Count()
				.Should().Be(0);
			diff.IsEmpty
				.Should().Be.True();
		}
		[Test]
		public void Can_identify_added_lines()
		{
			diff = new FileUniDiff(diff1.ToStream());

			diff.AddedLines.ToArray()
				.Should().Have.SameSequenceAs(new int[] { 12, 23, 24, 25, 26, 27 });

			diff = new FileUniDiff(diff2.ToStream());

			diff.AddedLines.ToArray()
				.Should().Have.SameSequenceAs(new int[] { 92, 176, 231 });
		}
		[Test]
		public void Can_identify_removed_lines()
		{
			diff = new FileUniDiff(diff1.ToStream());

			diff.RemovedLines.ToArray()
				.Should().Have.SameSequenceAs(new int[] { 12, 13 });

			diff = new FileUniDiff(diff2.ToStream());

			diff.RemovedLines.ToArray()
				.Should().Have.SameSequenceAs(new int[] { 92, 176 });
		}
		[Test]
		public void Can_parse_incomplete_hunk_header()
		{
			diff = new FileUniDiff(diff4.ToStream());

			diff.AddedLines.Count()
				.Should().Be(16);
			diff.RemovedLines.Count()
				.Should().Be(1);
		}
		[Test]
		public void Should_parse_incomplete_lines_correctly()
		{
			diff = new FileUniDiff(diff4.ToStream());

			diff.AddedLines.Max()
				.Should().Be(16);
		}
		[Test]
		public void Should_parse_text_like_header_correctly()
		{
			diff = new FileUniDiff(diff5.ToStream());

			diff.RemovedLines.Count()
				.Should().Be(5);
		}
	}
}
