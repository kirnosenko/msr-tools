using System;

namespace MSR
{
	public class MsrException : ApplicationException
	{
		public MsrException(string message)
			: base(message)
		{}
	}
}
