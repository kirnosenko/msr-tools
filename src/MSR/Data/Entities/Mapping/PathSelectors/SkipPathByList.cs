using System;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class SkipPathByList : SelectPathByList
	{
		protected override bool SelectMatchedPath()
		{
			return false;
		}
	}
}
