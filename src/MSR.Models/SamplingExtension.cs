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
		List<T> selection = new List<T>();
		T item;

		for (int i = 0; i < selectionSize; i++)
		{
			do
			{
				item = sampledPopulation.Skip(Rng.GetInt(sampledPopulation.Count())).First();
			} while (selection.Contains(item));

			selection.Add(item);
		}
		
		return selection;
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
