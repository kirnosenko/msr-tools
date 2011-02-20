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
			//configFile = @"E:\repo\wordpress\wordpress.config"; // 13998 revisions

			//Debug();
			//Mapping();
			Predict();
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

			string[] wordpress_releases =
			{
				"80069b8aacb093d7c03ec9d822a29f0bbadc0166", // 1.0
				"ad2fa731f98a9225ccac7d72e281b43bd924ca78", // 1.2
				"a4320c095bcb95a41b03eccededa7fcecdcd868f", // 1.5
				"98c489aa325de994e5e0e6d4cb079c4e47cfb5ae", // 2.0
				"6a84e1edd354933daf7b36173652301ea01f7fa8", // 2.1
				"6c5275d6365b64e5e363d99f221d586b6420003b", // 2.3
				"7e443e150792a862fdb3028ccf327cd6a95e656e", // 2.5
				"c7b9966b872b43dd3a82ff272b312be5bc1dcda7", // 2.6
				"e4aef4c98c8d99807b089a71a2a8530878826d8b", // 2.7
				"8638b546f0fb863c54d7b7434c140677e555a52a", // 2.8
			};
			
			DebuggingTool debugger = new DebuggingTool(configFile);

			for (int i = 3; i <= gnome_terminal_releases.Length; i++)
			{
				debugger.Predict(
					gnome_terminal_releases.Take(i).ToArray()
				);
			}
		}
		static void GenerateStat()
		{
			GeneratingTool generator = new GeneratingTool(configFile);
			generator.GenerateStat(null, "d:/temp/1", null);
		}
	}
}
