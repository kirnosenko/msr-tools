/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MSR.Data.VersionControl;
using MSR.Data.Entities.DSL.Mapping;

namespace MSR.Data.Entities.Mapping
{
	public class ReleaseMapper : EntityMapper<Release,CommitMappingExpression,ReleaseMappingExpression>
	{
		private IReleaseDetector releaseDetector;
		
		public ReleaseMapper(IScmData scmData, IReleaseDetector releaseDetector)
			: base(scmData)
		{
			this.releaseDetector = releaseDetector;
		}
		public override IEnumerable<ReleaseMappingExpression> Map(CommitMappingExpression expression)
		{
			string tag = releaseDetector.TagForCommit(expression.CurrentEntity<Commit>());
			if (tag != null)
			{
				return new ReleaseMappingExpression[]
				{
					expression.IsRelease(tag)
				};
			}
			return Enumerable.Empty<ReleaseMappingExpression>();
		}
	}
}
