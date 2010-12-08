/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Tests.Runner
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			NUnit.ConsoleRunner.Runner.Main(new string[] { "../../../MSR.nunit" });
		}
	}
}
