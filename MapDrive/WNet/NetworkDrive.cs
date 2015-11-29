/*
 * NetworkDrive Class
 * ------------------
 * Provides access to Win32 network drive functions. Connect, Disconnect, Reconnect and connection dialogs. 
 * Originally inspired by Adam ej Woods's contribution to CodeProject.
 * http://www.aejw.com/ Adam's site
 * http://www.thecodeproject.com/csharp/mapnetdrive.asp Adam's article on CodeProject
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;
using System.Management;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace MapDrive.WNet
{
    /// <summary>
    /// Provides functions for managing networkdrives
    /// </summary>
    public class NetworkDrive
    {
        #region Native Enums and Structs
        /// <summary>
        /// Flags for use in connecting network drives.
        /// </summary>
        [Flags]
        public enum ShareFlags : int
        {
            /// <summary>
            /// Whether or not to make a persistant connection that reconnects on logon. (CONNECT_UPDATE_PROFILE)
            /// </summary>
            RestoreOnLogon = 0x00000001,
            /// <summary>
            /// Saves login credentials.
            /// </summary>
            SaveCredentials = 0x00001000,
            /// <summary>
            /// No flags.
            /// </summary>
            None = 0x00000000
        }

        /// <summary>
        /// Describes a network resource type
        /// </summary>
        public enum RESOURCETYPE : int
        {
            /// <summary>
            /// Any type.
            /// </summary>
            ANY = 0,
            /// <summary>
            /// Resource is a disk
            /// </summary>
            DISK = 1,
            /// <summary>
            /// Resource is a printer
            /// </summary>
            PRINT = 2
        }
        /// <summary>
        /// Scope of the enumeration. This member can be one of the following values.
        /// </summary>
        public enum RESOURCESCOPE : int
        {
            /// <summary>
            /// Enumerate currently connected resources. The dwUsage member cannot be specified.
            /// </summary>
            CONNECTED = 0x1,
            /// <summary>
            /// Enumerate all resources on the network. The dwUsage member is specified.
            /// </summary>
            GLOBALNET = 0x2,
            /// <summary>
            /// Enumerate remembered (persistent) connections. The dwUsage member cannot be specified.
            /// </summary>
            REMEMBERED = 0x3
        }

        /// <summary>
        /// Scope of the enumeration. This member can be one of the following values.
        /// </summary>
        public enum RESOURCEDISPLAYTYPE : int
        {
            /// <summary>
            /// The method used to display the object does not matter.
            /// </summary>
            GENERIC = 0x0,
            /// <summary>
            /// The object should be displayed as a domain.
            /// </summary>
            DOMAIN = 0x1,
            /// <summary>
            /// The object should be displayed as a server.
            /// </summary>
            SERVER = 0x2,
            /// <summary>
            /// The object should be displayed as a share.
            /// </summary>
            SHARE = 0x3
        }

        /// <summary>
        /// Set of bit flags describing how the resource can be used. 
        /// </summary>
        [Flags]
        public enum RESORUCEUSAGE : int
        {
            /// <summary>
            /// e resource is a connectable resource; the name pointed to by the lpRemoteName member can be passed to the WNetAddConnection function to make a network connection
            /// </summary>
            CONNECTABLE = 0x1,
            /// <summary>
            /// The resource is a container resource; the name pointed to by the lpRemoteName member can be passed to the WNetOpenEnum function to enumerate the resources in the container.
            /// </summary>
            CONTAINER = 0x2

        }

        /// <summary>
        /// Native struct for use in Network Drive creation and enumeration
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            /// <summary>
            /// Scope of the enumeration.
            /// </summary>

            public RESOURCESCOPE Scope;
            /// <summary>
            /// Set of bit flags identifying the type of resource. 
            /// </summary>
            public RESOURCETYPE Type;
            /// <summary>
            /// Display options for the network object in a network browsing user interface.
            /// </summary>
            public RESOURCEDISPLAYTYPE DisplayType;
            /// <summary>
            /// Set of bit flags describing how the resource can be used. Note that this member can be specified only if the dwScope member is equal to RESOURCE_GLOBALNET. 
            /// </summary>
            public RESORUCEUSAGE Usage;
            /// <summary>
            /// If the dwScope member is equal to RESOURCE_CONNECTED or RESOURCE_REMEMBERED, this member is a pointer to a null-terminated character string that specifies the name of a local device. This member is NULL if the connection does not use a device.
            /// </summary>
            public string LocalName;
            /// <summary>
            /// If the entry is a network resource, this member is a pointer to a null-terminated character string that specifies the remote network name.
            /// If the entry is a current or persistent connection, lpRemoteName points to the network name associated with the name pointed to by the lpLocalName member.
            /// The string can be MAX_PATH characters in length, and it must follow the network provider's naming conventions.
            /// </summary>
            public string RemoteName;
            /// <summary>
            /// Pointer to a null-terminated string that contains a comment supplied by the network provider.
            /// </summary>
            public string Comment;
            /// <summary>
            /// Pointer to a null-terminated string that contains the name of the provider that owns the resource. This member can be NULL if the provider name is unknown. To retrieve the provider name, you can call the WNetGetProviderName function.
            /// </summary>
            public string Provider;
        }
        
        #endregion

        #region Native API

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2A(string psName, ShareFlags piFlags, bool pfForce);

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2A(ref NETRESOURCE pstNetRes, string psPassword, string psUsername, ShareFlags piFlags);
        
        [DllImport("mpr.dll")]
        private static extern int WNetConnectionDialog(IntPtr phWnd, RESOURCETYPE piType);
        
        [DllImport("mpr.dll")]
        private static extern int WNetDisconnectDialog(IntPtr phWnd, RESOURCETYPE piType);
        
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        private static extern int WNetRestoreConnectionW(IntPtr phWnd, string psLocalDrive);
        #endregion

        /// <summary>
        /// Disconnects a Network drive. Throws a Win32Exception on error!
        /// </summary>
        /// <param name="name">The drivename of the network drive eg. "z:" or the path to the drive's share eg. "\\server\share"</param>
        /// <param name="force">Force disconnection even if files are open.</param>
        public static void DisconnectDrive(string name, bool force)
        {
            int i = WNetCancelConnection2A(name, ShareFlags.RestoreOnLogon, force);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }

        /// <summary>
        /// Maps a network share to the specfied drive. Throws Win32Exception on error.
        /// </summary>
        /// <param name="drive">Drive letter to map networkshare to. </param>
        /// <param name="sharePath">Path to the network share.</param>
        /// <param name="persistant">Whether or not to reconnect the drive on logon</param>
        public static void ConnectDrive(string drive, string sharePath, bool persistant)
        {
            NETRESOURCE nr = new NETRESOURCE();
            nr.Type = RESOURCETYPE.DISK;
            nr.LocalName = drive;
            nr.RemoteName = sharePath;
            int i = WNetAddConnection2A(ref nr, null, null, persistant ? ShareFlags.RestoreOnLogon : ShareFlags.None);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }
        
        /// <summary>
        /// Maps a network share to the specfied drive. Using credentials. Throws Win32Exception on error.
        /// </summary>
        /// <param name="drive">Drive letter to map networkshare to. </param>
        /// <param name="sharePath">Path to the network share.</param>
        /// <param name="persistant">Whether or not to reconnect the drive on logon</param>
        /// <param name="username">Username to use</param>
        /// <param name="password">Password that matches username</param>
        /// <param name="saveLogin">Whether or not to save the username and password</param>
        public static void ConnectDrive(string drive, string sharePath, bool persistant, string username, string password, bool saveLogin)
        {
            NETRESOURCE nr = new NETRESOURCE();
            nr.Type = RESOURCETYPE.DISK;
            nr.LocalName = drive;
            nr.RemoteName = sharePath;
            ShareFlags flags = ShareFlags.None;
            if (persistant) flags |= ShareFlags.RestoreOnLogon;
            if (saveLogin) flags |= ShareFlags.SaveCredentials;

            int i = WNetAddConnection2A(ref nr, username, password, flags);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }	
        }

        /// <summary>
        /// Displays the standard Network drive Connection dialog
        /// </summary>
        public static void ConnectDialog()
        {
            ConnectDialog(null);
        }

        /// <summary>
        /// Displays the standard Network drive Connection dialog. Modal.
        /// </summary>
        /// <param name="owner">The dialog's owner</param>
        public static void ConnectDialog(Form owner)
        {
            int i = WNetConnectionDialog(owner != null ? owner.Handle : IntPtr.Zero, RESOURCETYPE.DISK);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }

        /// <summary>
        /// Displays the standard network drive disconnect dialog.
        /// </summary>
        public static void DisconnectDialog()
        {
            DisconnectDialog(null);
        }

        /// <summary>
        /// Displays the standard network drive disconnect dialog. Modal.
        /// </summary>
        /// <param name="owner">The dialog's owner</param>
        public static void DisconnectDialog(Form owner)
        {
            int i = WNetDisconnectDialog(owner != null ? owner.Handle : IntPtr.Zero, RESOURCETYPE.DISK);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }

        /// <summary>
        /// Restores connection to a network drive.
        /// </summary>
        /// <param name="drive">The drive to restore connection to. If NULL, the function will attempt to reconnect all Persistant drives.</param>
        
        public static void RestoreConnection(string drive)
        {
            int i = WNetRestoreConnectionW(IntPtr.Zero, drive);
            if (i > 0) { throw new System.ComponentModel.Win32Exception(i); }
        }

        /// <summary>
        /// Restores the connection to all Network Drives
        /// </summary>
        public static void RestoreAllConnections() {
            using (ManagementObjectSearcher ShareDiskSearch = new ManagementObjectSearcher(new SelectQuery("Select * from Win32_MappedLogicalDisk")))
            {
                using (ManagementObjectCollection moSharedDiskCollection = ShareDiskSearch.Get())
                {
                    foreach (ManagementObject mo in moSharedDiskCollection)
                    {
                        try
                        {
                            RestoreConnection(mo["Name"].ToString());
                        }
                        finally
                        {
                            mo.Dispose();
                        }
                    }
                }
            }
            
        }
        /// <summary>
        /// Attempts to connect to port 445 and 139 on the server denoted by the uncPath provided
        /// </summary>
        /// <param name="uncPath">A path to a network share</param>
        /// <param name="timeout">Amount of milliseconds to wait for connections (total amount of time to wait)</param>
        /// <returns>true if connection was succesful</returns>
        public static bool IsSMBAvailable(string uncPath, int timeout)
        {
            Match m = Regex.Match(uncPath, @"^\\\\([^\\]+)\\?");
            if (!m.Success)
                return false;
            string host = m.Groups[1].Value;
            if (IsPortAvailable(host, 445, timeout / 2))
                return true;

            if (IsPortAvailable(host, 139, timeout / 2))
                return true;
            return false;
        }
        /// <summary>
        /// Attempts to connect to a port on the host specified.
        /// </summary>
        /// <param name="host">Host to connect to</param>
        /// <param name="port">Port to connect to</param>
        /// <param name="timeOut">Amount of time in milliseconds to wait for connection</param>
        /// <returns>true if successful in creating connection.</returns>
        public static bool IsPortAvailable(string host, int port, int timeOut)
        {
            try
            {
                IPAddress addr = null;
                if (!IPAddress.TryParse(host, out addr))
                {
                    IPHostEntry entry = Dns.GetHostEntry(host);
                    addr = entry.AddressList[0];
                }
                TcpClient tcp = new TcpClient();

                ManualResetEvent are = new ManualResetEvent(false);
                object[] state = new object[] { tcp, are };

                tcp.BeginConnect(addr, port, new AsyncCallback(delegate(IAsyncResult result)
                {
                    TcpClient client = (TcpClient)((object[])result.AsyncState)[0];
                    ManualResetEvent resetEvent = (ManualResetEvent)((object[])result.AsyncState)[1];
                    resetEvent.Set();
                }), state);

                are.WaitOne(timeOut, false);
                bool found = tcp.Client != null && tcp.Connected;
                if (tcp.Client != null)
                {
                    tcp.Client.Close();
                    tcp.Client = null;
                }
                return found;
            }
            catch
            {
                return false;
            }
        }

		public static bool IsNetworkDriveMapped(string driveRoot)
		{
			try
			{
				string uncPath = WNet.GetRemoteUniversalName(driveRoot).UniversalName;
				return !String.IsNullOrEmpty(uncPath);
			}
			catch
			{
				return false;
			}
		}

        public static bool IsNetworkDriveOnline(string driveRoot, int timeout)
        {
            try
            {
                string uncPath = WNet.GetRemoteUniversalName(driveRoot).UniversalName;
                return IsSMBAvailable(uncPath, timeout);
            }
            catch
            {
                return false;
            }
        }

    }
}