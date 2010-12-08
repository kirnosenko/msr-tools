/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

namespace System
{
	using System.IO;
	using System.Text;
	
	public static class StringExtension
	{
		public static string[] ToLines(this string text)
		{
			return text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
		public static Stream ToStream(this string text)
		{
			return new MemoryStream(new UTF8Encoding().GetBytes(text));
		}
		public static string Repeat(this string text, int times)
		{
			StringBuilder str = new StringBuilder(text);
			while (times > 1)
			{
				str.Append(text);
				times--;
			}
			return str.ToString();
		}
	}
}
