using System;

namespace MapDrive.WNet
{
	/// <summary>
	/// Summary description for WNetException.
	/// </summary>
	public class WNetException:
		Exception
	{
		public WNetException(int error, string message):
			base(message)
		{
			this._errorCode = error;
		}
		int _errorCode;
		public int Win32ErrorCode
		{
			get
			{
				return _errorCode;
			}
		}
	}
}
