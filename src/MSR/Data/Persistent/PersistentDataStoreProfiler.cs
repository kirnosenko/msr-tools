/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.IO;

namespace MSR.Data.Persistent
{
	public class PersistentDataStoreProfiler
	{
		private PersistentDataStore data;
		private Stream log;
		private StreamWriter logger;

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
		}
		public void Stop()
		{
			data.Logger = null;
			logger.Flush();
		}
		public int NumberOfQueries
		{
			get
			{
				Stop();
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
				
				Start();
				return numberOfQueries;
			}
		}
	}
}
