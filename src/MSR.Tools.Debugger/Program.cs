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
			//configFile = @"E:\repo\linux-2.6\linux-2.6.config";
			//configFile = @"E:\repo\jquery\jquery.config";
			//configFile = @"E:\repo\django\django.config";
			//configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\nhibernate\nhibernate.config";
			//configFile = @"E:\repo\msr\msr.config";

			//Debug();
			//Mapping();
			//Predict();
			//LocStat();
			GenerateStat();

			Console.ReadKey();
		}
		static void Debug()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);

			
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);
			
			//mapper.Info();
			//mapper.Map(false, 756);
			//mapper.Truncate(755);
			mapper.Check(756);
		}
		static void Predict()
		{
			// gnome-terminal

			string gnome_terminal_r_2_0_0 = "386";
			string gnome_terminal_r_2_2_0 = "689";
			string gnome_terminal_r_2_4_0 = "951";
			string gnome_terminal_r_2_6_0 = "1198";
			string gnome_terminal_r_2_8_0 = "1365";

			// dia

			string dia_r_0_90 = "1617";
			string dia_r_0_91 = "2154";
			string dia_r_0_92 = "2437";
			string dia_r_0_94 = "2859";
			string dia_r_0_95 = "3413";
			string dia_r_0_96 = "3652";

			// gnome-vfs

			string gnome_vfs_r_2_0_0 = "2440";
			string gnome_vfs_r_2_2_0 = "2782";
			string gnome_vfs_r_2_4_0 = "3129";
			string gnome_vfs_r_2_6_0 = "3633";
			string gnome_vfs_r_2_8_0 = "4095";
			string gnome_vfs_r_2_10_0 = "4366";
			string gnome_vfs_r_2_12_0 = "4593";

			DebuggingTool debugger = new DebuggingTool(configFile);
			string previousReleaseRevision = gnome_terminal_r_2_6_0;
			string releaseRevision = gnome_terminal_r_2_8_0;

			debugger.Predict(previousReleaseRevision, releaseRevision);
		}
		static void LocStat()
		{
			DebuggingTool debugger = new DebuggingTool(configFile);
			debugger.LocStat();
		}
		static void GenerateStat()
		{
			using (ConsoleTimeLogger.Start("stat generating"))
			{
				GeneratingTool generator = new GeneratingTool(configFile);
				generator.GenerateStat(null, "d:/temp", null);
			}
		}
	}
}
