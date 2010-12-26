/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Diagnostics;
using System.IO;

namespace MSR.Data.Persistent
{
	public class PersistentDataStoreProfiler
	{
		private PersistentDataStore data;
		private Stream log;
		private StreamWriter logger;
		private Stopwatch timer;

		public PersistentDataStoreProfiler(PersistentDataStore data)
		{
			this.data = data;
			log = new MemoryStream();
			logger = new StreamWriter(log);
			Start();
		}
		public void Start()
		{
			data.Logger = logger;
			timer = Stopwatch.StartNew();
		}
		public void Stop()
		{
			timer.Stop();
			data.Logger = null;
			logger.Flush();
		}
		public int NumberOfQueries
		{
			get
			{
				int numberOfQueries = 0;
				
				log.Seek(0, SeekOrigin.Begin);
				TextReader reader = new StreamReader(log);
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("SELECT"))
					{
						numberOfQueries++;
					}
				}
				
				return numberOfQueries;
			}
		}
		public TimeSpan ElapsedTime
		{
			get
			{
				return timer.Elapsed;
			}
		}
	}
}
