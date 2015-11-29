using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MapDrive.WNet
{
	/// <summary>
	/// Summary description for Common.
	/// </summary>
	class Common
	{
		public static string GetSystemErrorString(int error)
		{
			StringBuilder message = new StringBuilder(10000);
			Common_Api.FormatMessage(FormatMessageFlags.FromSystem | FormatMessageFlags.IgnoreInserts, IntPtr.Zero, error, 0, message, message.Capacity, IntPtr.Zero);
			if (message.Length >= 2 && message[message.Length - 1] == '\n' && message[message.Length - 2] == '\r')
				message.Remove(message.Length - 2, 2);
			return message.ToString();
		}
		public static IntPtr ControlToHwnd(Control control)
		{
			if (control == null)
				return IntPtr.Zero;
			return control.Handle;
		}
	}

	class Common_Api
	{
		[DllImport("kernel32.dll", EntryPoint="FormatMessage",  
			 CharSet=CharSet.Auto)]
		public static extern int FormatMessage(FormatMessageFlags flags, IntPtr source, int messageId, int languageId,
			StringBuilder buffer, int size, IntPtr arguments);

	}

	[Flags]
	enum FormatMessageFlags:
		uint
	{
		AllocateBuffer = 0x00000100,
		IgnoreInserts  = 0x00000200,
		FromString     = 0x00000400,
		FromHModule    = 0x00000800,
		FromSystem     = 0x00001000,
		ArgumentArray  = 0x00002000,
		MaxWidthMask  = 0x000000FF
	}

}
