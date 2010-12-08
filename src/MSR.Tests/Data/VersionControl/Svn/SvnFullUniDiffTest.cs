/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace MSR.Data.VersionControl.Svn
{
	[TestFixture]
	public class SvnFullUniDiffTest
	{

private string diff1 =
@"Index: test/Log.cs
===================================================================
--- test/Log.cs	(revision 0)
+++ test/Log.cs	(revision 2)
@@ -0,0 +1,14 @@
+using System;
+
+namespace test
+{
+
+static class Log
+{
+	public static void Write(string text)
+	{
+		Console.WriteLine(text);
+	}
+}
+
+}
Index: test/Program.cs
===================================================================
--- test/Program.cs	(revision 1)
+++ test/Program.cs	(revision 2)
@@ -15,11 +15,11 @@
 		}
 		static void DoThis()
 		{
-			Console.WriteLine(""this"");
+			Log.Write(""this"");
 		}
 		static void DoThat()
 		{
-			Console.WriteLine(""this"");
+			Log.Write(""that"");
 		}
 	}
 }
Index: test/test.csproj
===================================================================
--- test/test.csproj	(revision 1)
+++ test/test.csproj	(revision 2)
@@ -45,6 +45,7 @@
     <Reference Include=""System.Xml"" />
   </ItemGroup>
   <ItemGroup>
+    <Compile Include=""Log.cs"" />
     <Compile Include=""Program.cs"" />
     <Compile Include=""Properties\AssemblyInfo.cs"" />
   </ItemGroup>
";

		private SvnFullUniDiff diff;
		
		[Test]
		public void Should_parse_only_one_file()
		{
			diff = new SvnFullUniDiff(diff1.ToStream());
			
			diff.Count
				.Should().Be(3);

			diff["/test/Log.cs"].AddedLines.Count()
				.Should().Be(14);
			diff["/test/Log.cs"].RemovedLines.Count()
				.Should().Be(0);
			diff["/test/Program.cs"].AddedLines.Count()
				.Should().Be(2);
			diff["/test/Program.cs"].RemovedLines.Count()
				.Should().Be(2);
			diff["/test/test.csproj"].AddedLines.Count()
				.Should().Be(1);
			diff["/test/test.csproj"].RemovedLines.Count()
				.Should().Be(0);
		}
	}
}
