/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;

using MSR.Data.Entities.Mapping;
using MSR.Data.Entities.Mapping.PathSelectors;

namespace MSR.Tools.Mapper
{
	class Program
	{
		static void Main(string[] args)
		{
			string configFile;
			string cmd;
			bool createDataBase = false;
			int revisionNumber = 0;
			string revision = null;
			bool automaticallyFixDiffErrors = false;
			string path = null;
			string dir = null;
			
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
							createDataBase = true;
							break;
						case "-f":
							automaticallyFixDiffErrors = true;
							break;
						case "-n":
							revisionNumber = Convert.ToInt32(args[++i]);
							break;
						case "-r":
							revision = args[++i];
							break;
						case "-p":
							path = args[++i];
							break;
						case "-d":
							dir = args[++i];
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
				Console.WriteLine("  info              print general information about database");
				Console.WriteLine("  map               map data incrementally from software repositories to database");
				Console.WriteLine("    -c              create database");
				Console.WriteLine("    -n N            map data till revision number N");
				Console.WriteLine("    -r R            map data till revision R");
				Console.WriteLine("  pmap              map data partially from software repositories to database");
				Console.WriteLine("    -n N            map data from a revision number N to the last one in database");
				Console.WriteLine("    -r R            map data from a revision R to the last one in database");
				Console.WriteLine("    -p PATH         map file in the path");
				Console.WriteLine("    -d DIR          map files in the directory");
				Console.WriteLine("  emap              map a single data entity to database");
				Console.WriteLine("    release R TAG   map a release entity with TAG for revision R to database");
				Console.WriteLine("  truncate          remove mapped data from database");
				Console.WriteLine("    -n N            keep the first N revisions");
				Console.WriteLine("    -r R            keep all revisions until revision R");
				Console.WriteLine("  check             check validity of mapped data");
				Console.WriteLine("    -n N            check data till revision number N");
				Console.WriteLine("    -p PATH         check the file on path");
				Console.WriteLine("    -f              automatically fix diff errors");
				
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
						if (revisionNumber != 0)
						{
							mapper.Map(createDataBase, revisionNumber);
						}
						else
						{
							mapper.Map(createDataBase, revision);
						}
						break;
					case "pmap":
						IPathSelector[] pathSelectors = new IPathSelector[]
						{
							new TakePathByList()
							{
								Paths = path != null ? new string[] { path } : null,
								Dirs = dir != null ? new string[] { dir } : null
							}
						};
						if (revisionNumber != 0)
						{
							mapper.PartialMap(revisionNumber, pathSelectors);
						}
						else
						{
							mapper.PartialMap(revision, pathSelectors);
						}
						break;
					case "emap":
					{
						switch (args[2])
						{
							case "release":
							{
								mapper.MapReleaseEntity(args[3], args[4]);
								break;
							}
						}
						break;
					}
					case "truncate":
						if (revisionNumber != 0)
						{
							mapper.Truncate(revisionNumber);
						}
						else
						{
							mapper.Truncate(revision);
						}
						break;
					case "check":
						if (revisionNumber != 0)
						{
							mapper.Check(revisionNumber, path, automaticallyFixDiffErrors);
						}
						else
						{
							mapper.Check(revision, path, automaticallyFixDiffErrors);
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
