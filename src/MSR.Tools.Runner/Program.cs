/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MSR.Tools.Calculator;
using MSR.Tools.Mapper;

namespace MSR.Tools.Runner
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
			//configFile = @"E:\repo\jquery\jquery.config";
			//configFile = @"E:\repo\django\django.config";
			configFile = @"E:\repo\postgresql\postgresql.config";
			//configFile = @"E:\repo\msr\msr.config"; // 184 revisions
			
			Mapping();
			//Predict();
			//Stat();
			
			Console.ReadKey();
		}
		static void Mapping()
		{
			MappingTool mapper = new MappingTool(configFile);
			
			//mapper.Map(false, "1000");
			//mapper.Truncate("414");
			mapper.Check(1000);
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
			
			CalculatingTool calc = new CalculatingTool(configFile);
			string previousReleaseRevision = gnome_terminal_r_2_6_0;
			string releaseRevision = gnome_terminal_r_2_8_0;
			
			calc.Predict(previousReleaseRevision, releaseRevision);
		}
		static void Stat()
		{
			CalculatingTool calc = new CalculatingTool(configFile);
			
			calc.LocStat();
		}
	}
}
