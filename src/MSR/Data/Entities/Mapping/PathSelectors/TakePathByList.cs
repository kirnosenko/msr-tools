using System;

namespace MSR.Data.Entities.Mapping.PathSelectors
{
	public class TakePathByList : SelectPathByList
	{
		protected override bool SelectMatchedPath()
		{
			return true;
		}
	}
}
