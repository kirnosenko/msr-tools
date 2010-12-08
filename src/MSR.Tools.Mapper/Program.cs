/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Linq;

namespace MSR.Tools.Mapper
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Count() < 2)
			{
				Console.WriteLine("usage: MSR.Tools.Mapper CONFIG_FILE_NAME COMMAND [ARGS]");
				Console.WriteLine("Commands:");
				Console.WriteLine("  map		map data from software repositories to database");
				Console.WriteLine("  check		check validity of mapped data");
				return;
			}
			string configFile = args[0];
			string cmd = args[1].ToLower();

			MappingTool mapper = new MappingTool(configFile);
			switch (cmd)
			{
				case "map":
				break;
				case "check":
				break;
				default:
				break;
			}
		}
	}
}
