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

			configFile = @"E:\repo\gnome-terminal\gnome-terminal.config"; // 3436 revisions
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

			//Debug();
			//Mapping();
			Predict();
			//LocStat();
			//GenerateStat();

			Console.ReadKey();
		}
		static void Debug()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.FindDiffError(272);
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);
			
			//mapper.Info();
			//mapper.Map(false, 217);
			//mapper.Truncate(755);
			mapper.Check(120);
		}
		static void Predict()
		{
			string[] gnome_terminal_releases =
			{
				"386", "689", "951", "1198", "1365"
			};
			
			string[] dia_releases =
			{
				"1617", "2154", "2437", "2859", "3413", "3652"
			};
			
			DebuggingTool debugger = new DebuggingTool(configFile);
			
			debugger.Predict(
				gnome_terminal_releases.Take(4).ToArray(),
				gnome_terminal_releases.Last()
			);
		}
		static void LocStat()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.LocStat();
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp", null);
		}
	}
}
