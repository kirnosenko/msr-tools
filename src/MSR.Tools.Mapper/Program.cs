/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Tools.Mapper
{
	class Program
	{
		static void Main(string[] args)
		{
			string configFile = args[0];
			string cmd = args[1];
			bool createSchema = false;
			int numberOfRevisions = 0;
			
			try
			{
				int i = 2;
				while (i < args.Count())
				{
					switch (args[i])
					{
						case "-c":
							createSchema = true;
							break;
						case "-n":
							i++;
							numberOfRevisions = Convert.ToInt32(args[i]);
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
				Console.WriteLine("  map		map data from software repositories to database");
				Console.WriteLine("    -c		create data base");
				Console.WriteLine("    -n N		map commits from first to N incrementally");
				Console.WriteLine("  check		check validity of mapped data");
				Console.WriteLine("    -n N		check data till revision N");
				
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
			
			switch (cmd)
			{
				case "map":
					mapper.Map(createSchema, numberOfRevisions);
					break;
				case "check":
					mapper.Check(numberOfRevisions);
					break;
				default:
					Console.WriteLine("Unknown command {0}", cmd);
					break;
			}
		}
	}
}
