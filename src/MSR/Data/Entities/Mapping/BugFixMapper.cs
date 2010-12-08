/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class BugFixMapper : EntityMapper<CommitMappingExpression, BugFixMappingExpression>
	{
		private IBugFixDetector bugFixDetector;
		
		public BugFixMapper(IScmData scmData, IBugFixDetector bugFixDetector)
			: base(scmData)
		{
			this.bugFixDetector = bugFixDetector;
		}
		public override IEnumerable<BugFixMappingExpression> Map(CommitMappingExpression expression)
		{
			if (bugFixDetector.IsBugFix(expression.CurrentEntity<Commit>()))
			{
				return new BugFixMappingExpression[]
				{
					expression.IsBugFix()
				};
			}
			return Enumerable.Empty<BugFixMappingExpression>();
		}
	}
}
