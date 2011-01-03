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
			string outputDir = Environment.CurrentDirectory;
			string templateDir = Environment.CurrentDirectory + "/teplates/html";
			
			try
			{
				configFile = args[0];
				cmd = args[1];
				int i = 2;
				while (i < args.Length)
				{
					switch (args[i])
					{
						case "-o":
							i++;
							outputDir = args[i];
							break;
						case "-t":
							i++;
							templateDir = args[i];
							break;
						default:
							break;
					}
					i++;
				}
			}
			catch
			{
				Console.WriteLine("usage: MSR.Tools.Calculator CONFIG_FILE_NAME COMMAND [ARGS]");
				Console.WriteLine("Commands:");
				Console.WriteLine("  stat		create stat using templates");
				Console.WriteLine("    -o DIR	output directory");
				Console.WriteLine("    -t DIR	templates directory");
				
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
					generator.GenerateStat(outputDir, templateDir);
					break;
				default:
					Console.WriteLine("Unknown command {0}", cmd);
					break;
			}
		}
	}
}
