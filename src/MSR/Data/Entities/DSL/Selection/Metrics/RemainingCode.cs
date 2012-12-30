/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2011  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	public static class RemainingCode
	{
		public static IDictionary<int,double> CalculateRemainingCodeSize(this CodeBlockSelectionExpression code, string revision)
		{
			Dictionary<int,double> remainingCode = new Dictionary<int,double>();
			
			foreach (var codeByAddedCode in (
				from acb in code
				join dcb in
					(
						from cb in code.Queryable<CodeBlock>()
						join m in code.Queryable<Modification>() on cb.ModificationID equals m.ID
						join c in code.Queryable<Commit>() on m.CommitID equals c.ID
						let revisionOrderedNumber = code.Queryable<Commit>()
							.Single(x => x.Revision == revision)
							.OrderedNumber
						where
							c.OrderedNumber <= revisionOrderedNumber
						select cb
					) on acb.ID equals dcb.TargetCodeBlockID into dcbj
				from dcb in dcbj.DefaultIfEmpty()
				group dcb by acb into g
				select new
				{
					AddedCodeID = g.Key.ID,
					CodeSize = g.Key.Size + g.Sum(x => x == null ? 0 : x.Size)
				}
			).Where(x => x.CodeSize > 0))
			{
				remainingCode.Add(codeByAddedCode.AddedCodeID, codeByAddedCode.CodeSize);
			}
			
			return remainingCode;
		}
	}
}
