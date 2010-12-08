/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace MSR.Data.BugTracking.BugZilla
{
	public class BugZillaData
	{
		private XDocument xml;
		private string uri;
		
		public BugZillaData(string uri)
		{
			this.uri = uri;
			xml = XDocument.Load(uri);
		}
		public int Count
		{
			get { return xml.Descendants("bug").Count(); }
		}
		public IEnumerable<string> Bugs
		{
			get { return xml.Descendants("bug").Elements("bug_id").Select(x => x.Value); }
		}
		public bool IsFixed(string bug)
		{
			return BugElement(bug)
				.Element("resolution").Value == "FIXED";
		}
		public bool IsResolved(string bug)
		{
			return BugElement(bug)
				.Element("bug_status").Value == "RESOLVED";
		}
		public string Priority(string bug)
		{
			return BugElement(bug)
				.Element("priority").Value;
		}
		public DateTime Created(string bug)
		{
			return DateTime.Parse(
				BugElement(bug).Element("creation_ts").Value,
				CultureInfo.InvariantCulture
			);
		}
		public DateTime Fixed(string bug)
		{
			return DateTime.Parse(
				BugElement(bug).Element("delta_ts").Value,
				CultureInfo.InvariantCulture
			);
		}
		private XElement BugElement(string bug)
		{
			return xml.Descendants("bug")
				.Single(x => x.Element("bug_id").Value == bug);
		}
	}
}
