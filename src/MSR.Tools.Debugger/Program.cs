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
			//configFile = @"E:\repo\linux-2.6\linux-2.6.config";
			//configFile = @"E:\repo\django\django.config";
			//configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\nhibernate\nhibernate.config";
			//configFile = @"E:\repo\msr\msr.config";
			//configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions
			//configFile = @"E:\repo\frund\frund.config";
			//configFile = @"E:\repo\httpd\httpd.config";
			//configFile = @"E:\repo\subtle\subtle.config";
			//configFile = @"E:\repo\hc\hc.config";
			configFile = @"E:\repo\pgadmin3\pgadmin3.config";

			//Debug();
			Mapping();
			//PartialMapping();
			//MapReleases();
			//MapSyntheticReleases(5);
			//GenerateStat();

			Console.ReadKey();
		}
		static void Debug()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.FindDiffError(256);
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			//mapper.Info();
			mapper.Map(false, 1000);
			//mapper.Map(false, 800);
			//mapper.Truncate(10);
			//mapper.Check(100);
		}
		static void PartialMapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			mapper.PartialMap(30,
				new IPathSelector[]
				{
					new TakePathByList()
					{
						PathList = new string[] { "/src/utils/sysLogger.h" }
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
					{ "7e92915b1012773fc4f699951d13da1e5780b3ae", "0.10" },
					{ "ca2f4b0e0f61675d9085faf3cef57b4a7bc03496", "0.9" },
					{ "6bd33739504d20cb20570df9b28b9c18eab896cd", "0.8" },
					{ "dfb16fdae6f7f966c2adad76b867f96eae164e7a", "0.7" },
				}
			);
		}
		static void MapSyntheticReleases(int count)
		{
			MappingTool mapper = new MappingTool(configFile);
			mapper.MapSyntheticReleases(count, 0.8);
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp/1", null);
		}
	}
}
