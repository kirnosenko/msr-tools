/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010-2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

using MSR.Data.Persistent;
using MSR.Data.Entities;
using MSR.Data.VersionControl;

namespace MSR.Tools
{
	public class Tool
	{
		private IUnityContainer container;
		
		protected PersistentDataStore data;
		
		public Tool(string configFileName, params string[] additionalConfigSections)
		{
			container = GetContainer(configFileName, additionalConfigSections);
			
			data = GetConfiguredType<PersistentDataStore>();
		}
		protected T GetConfiguredType<T>()
		{
			return container.Resolve<T>();
		}
		protected IEnumerable<T> GetConfiguredTypes<T>()
		{
			return container.ResolveAll<T>();
		}
		protected T GetConfiguredType<T>(string name)
		{
			return container.Resolve<T>(name);
		}
		protected IScmData GetScmData()
		{
			return GetConfiguredType<IScmData>();
		}
		protected IScmData GetScmDataWithoutCache()
		{
			return GetConfiguredType<IScmData>("nocache");
		}
		private static IUnityContainer GetContainer(string configFile, params string[] additionalConfigSections)
		{
			var map = new ExeConfigurationFileMap();
			map.ExeConfigFilename = configFile;
			var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
			var section = (UnityConfigurationSection)config.GetSection("unity");
			var container = new UnityContainer();
			section.Configure(container, "tool");
			foreach (var configSection in additionalConfigSections)
			{
				section.Configure(container, configSection);
			}

			return container;
		}
	}
}
