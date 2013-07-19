/*
 * MSR Tools - tools for mining software repositories
 * 
 * Copyright (C) 2010  Semyon Kirnosenko
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MSR.Data.Entities.DSL.Selection.Metrics
{
	/// <summary>
	/// Calculate bug lifetime metrics.
	/// </summary>
	public static class BugLifetime
	{
		/// <summary>
		/// Calculate for each fix the time between the fix date
		/// and the date when the oldest buggy code were added.
		/// </summary>
		/// <param name="bugFixes">Fixes to be processed.</param>
		/// <returns>Time in days.</returns>
		public static IEnumerable<double> CalculateMaxBugLifetime(this BugFixSelectionExpression bugFixes)
		{
			return
				(
					from bf in bugFixes
					join c in bugFixes.Queryable<Commit>() on bf.CommitID equals c.ID
					join m in bugFixes.Queryable<Modification>() on c.ID equals m.CommitID
					join dcb in bugFixes.Queryable<CodeBlock>() on m.ID equals dcb.ModificationID
					join acb in bugFixes.Queryable<CodeBlock>() on dcb.TargetCodeBlockID equals acb.ID
					where
						dcb.Size < 0
					let codeDate = bugFixes.Queryable<Commit>().Single(x => x.ID == acb.AddedInitiallyInCommitID).Date
					group codeDate by c.Date into g
					select (g.Key - g.Min()).TotalDays
				).ToArray();
		}
		/// <summary>
		/// Calculate for each fix the time between the fix date
		/// and the date when the newest buggy code were added.
		/// </summary>
		/// <param name="bugFixes">Fixes to be processed.</param>
		/// <returns>Time in days.</returns>
		public static IEnumerable<double> CalculateMinBugLifetime(this BugFixSelectionExpression bugFixes)
		{
			return
				(
					from bf in bugFixes
					join c in bugFixes.Queryable<Commit>() on bf.CommitID equals c.ID
					join m in bugFixes.Queryable<Modification>() on c.ID equals m.CommitID
					join dcb in bugFixes.Queryable<CodeBlock>() on m.ID equals dcb.ModificationID
					join acb in bugFixes.Queryable<CodeBlock>() on dcb.TargetCodeBlockID equals acb.ID
					where
						dcb.Size < 0
					let codeDate = bugFixes.Queryable<Commit>().Single(x => x.ID == acb.AddedInitiallyInCommitID).Date
					group codeDate by c.Date into g
					select (g.Key - g.Max()).TotalDays
				).ToArray();
		}
		/// <summary>
		/// Calculate for each fix the time between the fix date
		/// and the avarage date of adding for the newest and 
		/// the oldest code.
		/// </summary>
		/// <param name="bugFixes">Fixes to be processed.</param>
		/// <returns>Time in days.</returns>
		public static IEnumerable<double> CalculateAvarageBugLifetime(this BugFixSelectionExpression bugFixes)
		{
			return
				(
					from bf in bugFixes
					join c in bugFixes.Queryable<Commit>() on bf.CommitID equals c.ID
					join m in bugFixes.Queryable<Modification>() on c.ID equals m.CommitID
					join dcb in bugFixes.Queryable<CodeBlock>() on m.ID equals dcb.ModificationID
					join acb in bugFixes.Queryable<CodeBlock>() on dcb.TargetCodeBlockID equals acb.ID
					where
						dcb.Size < 0
					let codeDate = bugFixes.Queryable<Commit>().Single(x => x.ID == acb.AddedInitiallyInCommitID).Date
					group codeDate by c.Date into g
					select ((g.Key - g.Min()).TotalDays + (g.Key - g.Max()).TotalDays) / 2
				).ToArray();
		}
		/// <summary>
		/// Calculate for each fix the time between dates when
		/// the oldest and the newest buggy code were added.
		/// </summary>
		/// <param name="bugFixes">Fixes to be processed.</param>
		/// <returns>Time in days.</returns>
		public static IEnumerable<double> CalculateBugLifetimeSpread(this BugFixSelectionExpression bugFixes)
		{
			return
				(
					from bf in bugFixes
					join c in bugFixes.Queryable<Commit>() on bf.CommitID equals c.ID
					join m in bugFixes.Queryable<Modification>() on c.ID equals m.CommitID
					join dcb in bugFixes.Queryable<CodeBlock>() on m.ID equals dcb.ModificationID
					join acb in bugFixes.Queryable<CodeBlock>() on dcb.TargetCodeBlockID equals acb.ID
					where
						dcb.Size < 0
					let codeDate = bugFixes.Queryable<Commit>().Single(x => x.ID == acb.AddedInitiallyInCommitID).Date
					group codeDate by c.Date into g
					select (g.Max() - g.Min()).TotalDays
				).ToArray();
		}
	}
}
