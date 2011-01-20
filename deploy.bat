/*
@echo off & cls
set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST "%WinDirNet%\v2.0.50727\csc.exe" set csc="%WinDirNet%\v2.0.50727\csc.exe"
IF EXIST "%WinDirNet%\v3.5\csc.exe" set csc="%WinDirNet%\v3.5\csc.exe"
IF EXIST "%WinDirNet%\v4.0.30319\csc.exe" set csc="%WinDirNet%\v4.0.30319\csc.exe"
%csc% /nologo /out:"%~0.exe" %0
"%~0.exe"
del "%~0.exe"
exit
*/

using System;
using System.Collections.Generic;
using System.IO;
 
class Deploy
{
	static void Main()
	{
		List<string> files = new List<string>()
		{
			"./lib/Microsoft.Practices.Unity.dll",
			"./lib/Microsoft.Practices.Unity.Configuration.dll",
			"./lib/NVelocity.dll",
			"./lib/ZedGraph.dll",
			
			"./src/MSR/bin/Release/MSR.dll",
			"./src/MSR.Util/bin/Release/MSR.Util.dll",
			"./src/MSR.Tools/bin/Release/MSR.Tools.dll",
			"./src/MSR.Tools.Mapper/bin/Release/MSR.Tools.Mapper.exe",
			"./src/MSR.Tools.StatGenerator/bin/Release/MSR.Tools.StatGenerator.exe",
			"./src/MSR.Tools.Visualizer/bin/Release/MSR.Tools.Visualizer.dll",
			"./src/MSR.Tools.Visualizer.WinForms/bin/Release/MSR.Tools.Visualizer.WinForms.exe"
		};
		foreach (var file in files)
		{
			if (File.Exists(file))
			{
				File.Copy(file, "./deploy/" + Path.GetFileName(file), true);
			}
			else
			{
				Console.WriteLine("Could not find file {0}", file);
			}
		}
	}
}
