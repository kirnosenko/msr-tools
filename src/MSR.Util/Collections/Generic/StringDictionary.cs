/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

namespace System.Collections.Generic
{
	public class StringDictionary : Dictionary<string,string>
	{
		private static readonly char[] separators = new char[] { '\t', ' ' };
		
		public StringDictionary(string[] keysAndValues)
		{
			foreach (var kv in keysAndValues)
			{
				string[] keyAndValue = kv.Split(separators);
				Add(keyAndValue[0], keyAndValue[1]);
			}
		}
	}
}
