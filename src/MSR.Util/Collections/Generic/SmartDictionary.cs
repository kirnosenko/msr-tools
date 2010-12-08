/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System.Collections.Generic
{
	public class SmartDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
	{
		private Func<TKey, TValue> defaultValueBuilder;

		public SmartDictionary()
		{
			this.defaultValueBuilder = k => default(TValue);
		}
		public SmartDictionary(Func<TKey, TValue> defaultValueBuilder)
		{
			this.defaultValueBuilder = defaultValueBuilder;
		}
		public new virtual void Add(TKey key, TValue value)
		{
			base.Add(key, value);
		}
		public new TValue this[TKey key]
		{
			get
			{
				if (!ContainsKey(key))
				{
					Add(key, defaultValueBuilder(key));
				}
				return base[key];
			}
			set
			{
				base[key] = value;
			}
		}
	}
}
