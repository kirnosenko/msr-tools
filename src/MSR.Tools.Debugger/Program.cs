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
			configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions
			//configFile = @"E:\repo\frund\frund.config";
			//configFile = @"E:\repo\httpd\httpd.config";
			//configFile = @"E:\repo\subtle\subtle.config";
			//configFile = @"E:\repo\hc\hc.config";
			//configFile = @"E:\repo\pgadmin3\pgadmin3.config";
			//configFile = @"E:\repo\gnuplot\gnuplot.config";
			//configFile = @"E:\repo\couchdb\couchdb.config";
			//configFile = @"E:\repo\dovecot\dovecot.config";

			//Debug();
			//Mapping();
			//PartialMapping();
			//MapReleases();
			//MapSyntheticReleases(1, 0.8);
			GenerateStat();

			Console.ReadKey();
		}
		static void Debug()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.FindDiffError(1, "/sha1_file.c");
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			mapper.Info();
			//mapper.Map(true, 1000);
			//mapper.Truncate(10);
			//mapper.Check(1992);
		}
		static void PartialMapping()
		{
			MappingTool mapper = new MappingTool(configFile);

			mapper.PartialMap(1901,
				new IPathSelector[]
				{
					new TakePathByList()
					{
						Paths = new string[] { "/src/gpexecute.inc" }
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
					{ "308aee2d8229cbdd26f29eb04954044592dffa9d", "0.1.0" },
					{ "4d00ea8121fd373525ecf6fa93243efc91666554", "0.1.1" },
					{ "f0c86737a276553e72a4ff1b070e8e30eaab50a0", "0.2.0" },
					{ "0e10ffa1fc83ae4d6b3657266865df4c4a3534da", "0.2.1" },
					{ "4f9d9cdbefa90a8d3673f94d078f196de2ec788b", "0.3.0" },
					{ "3bab948571635f5b2bf8c9f8ac7ce867c4e57414", "0.3.1" },
					{ "368d67c4f309ee9b3f437f9c36845f7a02837d41", "0.3.2" },
				}
			);
		}
		static void MapSyntheticReleases(int count, double stabilizationProbability)
		{
			MappingTool mapper = new MappingTool(configFile);
			mapper.MapSyntheticReleases(count, stabilizationProbability);
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp/1", null);
		}
	}
}
