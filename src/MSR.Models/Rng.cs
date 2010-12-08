/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;

namespace MSR.Models
{

public static class Rng
{
	private static Random rng;
	
	static Rng()
	{
		rng = new Random();
	}
	public static int GetInt(int max)
	{
		return rng.Next(max);
	}
}

}
