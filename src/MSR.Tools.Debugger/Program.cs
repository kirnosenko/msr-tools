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
			//configFile = @"E:\repo\django\django.config";
			//configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\nhibernate\nhibernate.config";
			//configFile = @"E:\repo\msr\msr.config";
			configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions

			//Debug();
			Mapping();
			//Predict();
			//LocStat();
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
			//mapper.Map(false, 13998);
			//mapper.Truncate(600);
			mapper.Check(13998);
		}
		static void Predict()
		{
			string[] gnome_terminal_releases =
			{
				"386", // 2.0
				"534", // 2.1
				"689", // 2.2
				"951", // 2.4
				"1004", // 2.5
				"1198", // 2.6
				"1365", // 2.8
				"1513", // 2.10
				"1628", // 2.12
				"1943", // 2.14
			};
			
			string[] django_releases =
			{
				"1e3c3934bda6625d381599d2d373087733235c91", // 1.0
				"55876d49e89f74365a9c737370e58567bd1eabff", // 1.1
				"e5f9122545127e539b50fa57d8ec8520dc9123ac", // 1.2
			};
			
			string[] nhibernate_releases =
			{
				"1750", // 1.0
				"2657", // 1.2
				"3728", // 2.0
				"4655", // 2.1
			};
			
			DebuggingTool debugger = new DebuggingTool(configFile);
			
			for (int i = 3; i <= 4; i++)
			{
				debugger.Predict(
					nhibernate_releases.Take(i).ToArray()
				);
			}
		}
		static void LocStat()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.LocStat();
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp/1", null);
		}
	}
}
