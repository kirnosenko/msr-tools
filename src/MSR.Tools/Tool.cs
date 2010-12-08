/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

using MSR.Data.Persistent;
using MSR.Data.Entities;

namespace MSR.Tools
{
	public class Tool
	{
		private IUnityContainer container;
		
		protected PersistentDataStore data;
		
		public Tool(string configFileName)
		{
			container = GetContainer(configFileName);
			
			data = GetConfiguredType<PersistentDataStore>();
		}
		protected T GetConfiguredType<T>()
		{
			return container.Resolve<T>();
		}
		protected T GetConfiguredType<T>(string name)
		{
			return container.Resolve<T>(name);
		}
		private static IUnityContainer GetContainer(string configFile)
		{
			var map = new ExeConfigurationFileMap();
			map.ExeConfigFilename = configFile;
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			var section = (UnityConfigurationSection)config.GetSection("unity");
			var container = new UnityContainer();
			section.Configure(container, "container");

			return container;
		}
	}
}
