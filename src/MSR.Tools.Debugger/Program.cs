/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MSR.Tools.Mapper;
using MSR.Tools.StatGenerator;

namespace MSR.Tools.Debugger
{
	class Program
	{
		private static string configFile;

		static void Main(string[] args)
		{
			Console.BufferHeight = 10000;

			//configFile = @"E:\repo\gnome-terminal\gnome-terminal.config"; // 3436 revisions
			//configFile = @"E:\repo\dia\dia.config"; // 4384 revisions
			//configFile = @"E:\repo\gnome-vfs\gnome-vfs.config"; // 5550 revisions
			//configFile = @"E:\repo\gedit\gedit.config"; // 6556 revisions
			//configFile = @"E:\repo\git\git.config";
			//configFile = @"E:\repo\gitstats\gitstats.config";
			//configFile = @"E:\repo\linux-2.6\linux-2.6.config";
			//configFile = @"E:\repo\jquery\jquery.config";
			configFile = @"E:\repo\django\django.config";
			//configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\nhibernate\nhibernate.config";
			//configFile = @"E:\repo\msr\msr.config";
			//configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions
			//configFile = @"E:\repo\frund\frund.config";
			//configFile = @"E:\repo\httpd\httpd.config";

			//Debug();
			Mapping();
			//PartialMapping();
			//MapReleases();
			//GenerateStat();

			Console.ReadKey();
		}
		static void Debug()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.QueryUnderProfiler();
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			//mapper.Info();
			mapper.Map(false, 10610);
			//mapper.Truncate(600);
			//mapper.Check(10132);
		}
		static void PartialMapping()
		{
			MappingTool mapper = new MappingTool(configFile);
			
			mapper.PartialMap(
				"0194aaa25eed767fe0f760eb05b7c5d016cd4511",
				"/django/conf/locale/es_MX/__init__py",
				null
			);
		}
		static void MapReleases()
		{
			MappingTool mapper = new MappingTool(configFile);
			mapper.MapReleases(
				new Dictionary<string,string>()
				{
					{ "1f59971150f055aa87332d6f013750f6fdbc5f44", "1.2.0" },
					{ "ac646f05f1c03579320e4aeab397e632920e37b0", "1.3.0" },
					{ "05b403f6c290c7205bd16db6800b4924cce5c073", "2.0.1" },
					{ "e598cd14c27e58c588c17a7d2349ed49129f7457", "2.1.1" },
					{ "b8b177e06a18a7c4119dc56de54f96a7d8b83876", "2.2.0" },
					{ "9063c0247480e06ac3913d9f788fd54a0621dd5a", "2.3.0" },
				}
			);
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp/1", null);
		}
	}
}
