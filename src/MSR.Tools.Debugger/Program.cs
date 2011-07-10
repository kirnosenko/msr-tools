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

using MSR.Data.Entities.Mapping;
using MSR.Data.Entities.Mapping.PathSelectors;
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
			//configFile = @"E:\repo\django\django.config";
			configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\nhibernate\nhibernate.config";
			//configFile = @"E:\repo\msr\msr.config";
			//configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions
			//configFile = @"E:\repo\frund\frund.config";
			//configFile = @"E:\repo\httpd\httpd.config";

			//Debug();
			//Mapping();
			PartialMapping();
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
			//mapper.Map(false, 28000);
			//mapper.Truncate(600);
			mapper.Check(28000);
		}
		static void PartialMapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			mapper.PartialMap("ac03efbb9cb9ed3eb9ede139dbdeb62782185128",
				new IPathSelector[]
				{
					new TakePathByList()
					{
						PathList = new string[] { "/src/interfaces/ecpg/test/expected/connect-test1.c.in" }
					}
				}
			);
		}
		static void MapReleases()
		{
			MappingTool mapper = new MappingTool(configFile);
			mapper.MapReleases(
				new Dictionary<string,string>()
				{
					{ "f44328b2b8cb175d05d424ab147d04ad0de744c5", "7.0" },
					{ "741604dd84dbbd58368a0206f73de259cb6718f4", "7.1" },
					{ "42e28d209c3dea644b6d111ae4c84b620fea0ded", "7.2" },
					{ "1941887d6e2055cae3a1532c9ae4d58504f93cc7", "7.3" },
					{ "cc3a149cb0b8a93bfee113c0e4a1ab35f49ad02c", "7.4 RC 1" },
					{ "c22b7eccd368754ea96865c046764382ab05db4b", "8.0" },
					{ "2a80c3c4dc56a44a26805c889ef71b614e769266", "8.1" },
					{ "2f52d7260ceac8e57613516d82a7404fbf3d137b", "8.2" },
					{ "9e647a13872a5f34dc0009566925c5c352463780", "8.3" },
					{ "4d53a2f9699547bdc12831d2860c9d44c465e805", "8.4" },
					{ "1084f317702e1a039696ab8a37caf900e55ec8f2", "9.0 BETA 3" },
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
