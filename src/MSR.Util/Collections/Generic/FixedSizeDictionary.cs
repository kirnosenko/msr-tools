/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System.Collections.Generic
{
	public class FixedSizeDictionary<TKey, TValue> : SmartDictionary<TKey, TValue>
	{
		private int maxSize;
		private Queue<TKey> orderedKeys;
		
		public FixedSizeDictionary(int maxSize)
			: base()
		{
			this.maxSize = maxSize;
			orderedKeys = new Queue<TKey>();
		}
		public FixedSizeDictionary(int maxSize, Func<TKey, TValue> defaultValueBuilder)
			: base(defaultValueBuilder)
		{
			this.maxSize = maxSize;
			orderedKeys = new Queue<TKey>();
		}
		public override void Add(TKey key, TValue value)
		{
			base.Add(key, value);
			orderedKeys.Enqueue(key);
			if (Count > maxSize)
			{
				Remove(orderedKeys.Dequeue());
			}
		}
	}
}
