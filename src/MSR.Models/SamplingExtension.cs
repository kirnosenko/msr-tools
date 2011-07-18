/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Models
{

public static class SamplingExtension
{
	public static IEnumerable<T> TakeRandomly<T>(this IEnumerable<T> sampledPopulation, int selectionSize)
	{
		var source = sampledPopulation.ToList();
		
		for (int i = 0; i < selectionSize; i++)
		{
			var item = source[Rng.GetInt(source.Count)];
			source.Remove(item);
			yield return item;
		}
	}
	public static IEnumerable<T> TakeNoMoreThan<T>(this IEnumerable<T> sampledPopulation, int maxSelectionSize)
	{
		if (sampledPopulation.Count() <= maxSelectionSize)
		{
			return sampledPopulation;
		}
		return sampledPopulation.Take(maxSelectionSize);
	}
}

}
