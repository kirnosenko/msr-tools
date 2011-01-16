/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

namespace MSR.Tools.Mapper
{
	class Program
	{
		static void Main(string[] args)
		{
			string configFile;
			string cmd;
			bool createSchema = false;
			int stopRevisionNumber = 0;
			string stopRevision = null;
			bool automaticallyFixDiffErrors = false;
			string path = null;
			
			try
			{
				configFile = args[0];
				cmd = args[1];
				int i = 2;
				while (i < args.Length)
				{
					switch (args[i])
					{
						case "-c":
							createSchema = true;
							break;
						case "-f":
							automaticallyFixDiffErrors = true;
							break;
						case "-n":
							stopRevisionNumber = Convert.ToInt32(args[++i]);
							break;
						case "-r":
							stopRevision = args[++i];
							break;
						case "-p":
							path = args[++i];
							break;
						default:
							break;
					}
					i++;
				}
			}
			catch
			{
				Console.WriteLine("usage: MSR.Tools.Mapper CONFIG_FILE_NAME COMMAND [ARGS]");
				Console.WriteLine("Commands:");
				Console.WriteLine("  info		print general information about database");
				Console.WriteLine("  map		map data incrementally from software repositories to database");
				Console.WriteLine("    -c		create database");
				Console.WriteLine("    -n N		map data till commit number N");
				Console.WriteLine("    -r R		map data till revision R");
				Console.WriteLine("  check		check validity of mapped data");
				Console.WriteLine("    -n N		check data till revision N");
				Console.WriteLine("    -p PATH	check the file on path");
				Console.WriteLine("    -f		automatically fix diff errors");
				
				return;
			}
			
			MappingTool mapper = null;
			try
			{
				mapper = new MappingTool(configFile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			
			try
			{
				switch (cmd)
				{
					case "info":
						mapper.Info();
						break;
					case "map":
						if (stopRevisionNumber != 0)
						{
							mapper.Map(createSchema, stopRevisionNumber);
						}
						else
						{
							mapper.Map(createSchema, stopRevision);
						}
						break;
					case "check":
						if (stopRevisionNumber != 0)
						{
							mapper.Check(stopRevisionNumber, path, automaticallyFixDiffErrors);
						}
						else
						{
							mapper.Check(stopRevision, path, automaticallyFixDiffErrors);
						}
						break;
					default:
						Console.WriteLine("Unknown command {0}", cmd);
						break;
				}
			}
			catch (MsrException e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
