using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MapDrive
{
    class Program
    {
		/// <summary>
		/// The drive letter to connect.
		/// </summary>
		private static string _drive;

		/// <summary>
		/// The share path to map.
		/// </summary>
		private static string _share;

		private static int _timeoutSec = 0;
        private static string _username;
        private static string _password;

        static void Main(string[] args)
        {
			if (!ParseArgs(args))
			{
				ShowHelp();
				return;
			}

			// If the drive is online, we are done.
			if (WNet.NetworkDrive.IsNetworkDriveOnline(_drive, 1000))
			{
				return;
			}

			// If the drive is mapped but offline, disconnect it before attempting to reconnect.
			if (WNet.NetworkDrive.IsNetworkDriveMapped(_drive))
			{
				WNet.NetworkDrive.DisconnectDrive(_drive, true);
			}

			bool mapped = false;
			while (!mapped && _timeoutSec >= 0)
			{
				try
				{
                    if (String.IsNullOrEmpty(_username))
                    {
                        WNet.NetworkDrive.ConnectDrive(_drive, _share, false);
                    }
                    else
                    {
                        WNet.NetworkDrive.ConnectDrive(_drive, _share, false, _username, _password, true);
                    }
					mapped = true;
				}
				catch
				{
					if (_timeoutSec > 0)
					{
						Thread.Sleep(1000);
					}

					_timeoutSec--;
				}
			}

			// This would be the ultimate one line solution to solve disconnected mapped network drives.
			// Unfortunately, Microsoft removed this in Vista.  What a PITA!!!
			//WNet.NetworkDrive.RestoreAllConnections();
        }

		/// <summary>
		/// Shows the help.
		/// </summary>
		static void ShowHelp()
		{
			string helpText = 

			"MapDrive 1.20 (c) 2013 Zorn Software\nhttp://zornsoftware.codenature.info\n\n" +
			"Makes mapping network drives on Windows 7+ startup reliable.  If the drive mapping " +
			"fails, the program will keep attempting to create the mapping until the specified timeout is reached.  " +
			"This is a solution for the poor reliablity of mapping network drives on startup in Windows 7+.\n\n" +
			"Syntax: MapDrive.exe <driveLettter:> <serverShare> [timeoutSeconds] [username] [password]\n\n" +
			"Example: MapDrive s: \\\\server\\share 20\n" +
			"This will keep attempting to map s: to the specified path for up to 20 seconds before giving up.\n\n" +
			"You can run this program via a shortcut in your Startup folder to map drives for non-elevated processes and/or " +
			"via a group-policy logon script configured via gpedit.msc for elevated processes.  See the above website for more information.";

			MessageBox.Show(helpText, "MapDrive", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		/// <summary>
		/// Parses the args.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns>True if successful, otherwise false.</returns>
		static bool ParseArgs(string[] args)
		{
			if (args.Length < 2 || args.Length > 5 || args.Length == 4)
			{
				return false;
			}

			_drive = args[0];
			_share = args[1];
			if (args.Length == 3 ||args.Length == 5)
			{
				Int32.TryParse(args[2], out _timeoutSec);
				if (_timeoutSec < 0)
				{
					return false;
				}
			}

            if (args.Length == 5)
            {
                _username = args[3];
                _password = args[4];
            }

			if (_drive.Length == 1)
			{
				_drive = _drive + ":";
			}

			if (_drive.Length != 2)
			{
				return false;
			}

			if (!_share.StartsWith(@"\\"))
			{
				return false;
			}

			return true;
		}
    }
}
