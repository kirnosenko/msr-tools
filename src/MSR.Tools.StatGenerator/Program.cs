/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.StatGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			string configFile;
			string cmd;
			string targetDir = null;
			string outputDir = null;
			string templateDir = null;
			string targetPath = null;
			
			try
			{
				configFile = args[0];
				cmd = args[1];
				int i = 2;
				while (i < args.Length)
				{
					switch (args[i])
					{
						case "-d":
							targetDir = args[++i];
							break;
						case "-o":
							outputDir = args[++i];
							break;
						case "-t":
							templateDir = args[++i];
							break;
						case "-p":
							targetPath = args[++i];
							break;
						default:
							break;
					}
					i++;
				}
			}
			catch
			{
				Console.WriteLine("usage: MSR.Tools.StatGenerator CONFIG_FILE_NAME COMMAND [ARGS]");
				Console.WriteLine("Commands:");
				Console.WriteLine("  stat		create stat using templates");
				Console.WriteLine("    -d DIR	directory to generate statistics for");
				Console.WriteLine("    -o DIR	path to output directory");
				Console.WriteLine("    -t DIR	relative path to templates directory");
				Console.WriteLine("  blame		show blame for a path");
				Console.WriteLine("    -d DIR	show blame for the directory");
				Console.WriteLine("    -p PATH	show blame for the file");
				
				return;
			}

			GeneratingTool generator = null;
			try
			{
				generator = new GeneratingTool(configFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			switch (cmd)
			{
				case "stat":
					generator.GenerateStat(targetDir, outputDir, templateDir);
					break;
				case "blame":
					generator.ShowBlame(targetDir, targetPath);
					break;
				default:
					Console.WriteLine("Unknown command {0}", cmd);
					break;
			}
		}
	}
}
