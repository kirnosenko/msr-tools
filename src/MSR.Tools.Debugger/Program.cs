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
			configFile = @"E:\repo\git\git.config";
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
			//configFile = @"E:\repo\pgadmin3\pgadmin3.config";
			//configFile = @"E:\repo\gnuplot\gnuplot.config";

			Debug();
			//Mapping();
			//PartialMapping();
			//MapReleases();
			//MapSyntheticReleases(10, 0.8);
			//GenerateStat();

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

			//mapper.Info();
			mapper.Map(true, 10);
			//mapper.Map(false, 800);
			//mapper.Truncate(10);
			//mapper.Check(6651);
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
					{ "f29b607b20b61fddf93885132ed0708308770cf8", "1.12.0 BETA 3" },
					{ "7fb632e2d838f1e75aec512e060841977c6cb5a5", "1.10.0" },
					{ "72fbb5a89ac45a8795cda0cd1537e7bbe666602f", "1.8.0" },
					{ "d8b0a5b7da22c4621b3064fca08d55830430bddb", "1.6.0" },
					{ "71edf25b4a6a1f882e3a463292b3828f40c894c9", "1.4.0" },
					{ "3f83858cea5aa8bca9647d8363adc35c1f1d5801", "1.2.0" },
					{ "d4bfe61f1dc71363c0d237b1add257a4e50488d5", "1.0.0" },
					{ "ccbd6c1cd30acc12df9e91aba0230c848d69b6cc", "0.9.0" },
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
