/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System.IO
{
	using System.Diagnostics;
	
	public class Shell
	{
		public static void RunAndWaitForExit(string command, string arguments, Stream output)
		{
			TextWriter writer = new StreamWriter(output);
			
			ProcessStartInfo psi = new ProcessStartInfo(command, arguments);
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;

			Process process = new Process();
			process.StartInfo = psi;
			process.OutputDataReceived += (s, e) =>
			{
				if (e.Data != null)
				{
					writer.WriteLine(e.Data);
				}
			};
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				throw new ApplicationException(
					string.Format(
						"Process {0} with arguments {1} exit code is {2}",
						command, arguments, process.ExitCode
					)
				);
			}
			
			writer.Flush();
		}
	}
}
