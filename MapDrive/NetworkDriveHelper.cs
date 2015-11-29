﻿using System;
using System.IO;

//==============================================================================================================
//
//  cNetworkDrive -  Map Network Drive API Class
//  -----------------------------------------------
//  Copyright (c)2006-2007 aejw.com
//  http://www.aejw.com/
//
// Build:         0028 - March 2007
// Thanks To:     "jsantos98", "FeLiZk" from CodeProject.com for there comments and help
//                "MartinPreis" for reporting two bugs
// EULA:          Creative Commons - Attribution-ShareAlike 3.0
//                http://creativecommons.org/licenses/by-sa/3.0/
//
//==============================================================================================================

namespace MapDrives
{
    public class NetworkDriveHelper
    {
        #region Public variables and properties
        //---------------------------------------------------------------------------------------------------------------------------------------

        private bool _saveCredentials = false;
        private bool _persistent = false;
        private bool _force = false;
        private bool _promptForCredentials = false;
        private bool _findNextFreeDrive = false;
        private string _localDrive = null;
        private string _shareName = "";

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Option to save credentials on reconnection...
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool SaveCredentials
        {
            get
            {
                return _saveCredentials;
            }
            set
            {
                _saveCredentials = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Option to reconnect drive after log off / reboot...
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool Persistent
        {
            get
            {
                return _persistent;
            }
            set
            {
                _persistent = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Option to force connection if drive is already mapped...
        // or force disconnection if network path is not responding...
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool Force
        {
            get
            {
                return _force;
            }
            set
            {
                _force = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Option to prompt for user credintals when mapping a drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool PromptForCredentials
        {
            get
            {
                return _promptForCredentials;
            }
            set
            {
                _promptForCredentials = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Option to auto select the 'LocalDrive' property to next free driver letter when mapping a network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public bool FindNextFreeDrive
        {
            get
            {
                return _findNextFreeDrive;
            }
            set
            {
                _findNextFreeDrive = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Drive to be used in mapping / unmapping (eg. 's:')
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string LocalDrive
        {
            get
            {
                return _localDrive;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _localDrive = null;
                }
                else
                {

                    _localDrive = value.Substring(0, 1) + ":";
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Share address to map drive to. (eg. '\\Computer\C$')
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string ShareName
        {
            get
            {
                return _shareName;
            }
            set
            {
                _shareName = value;
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Returns a string array of currently mapped network drives
        //---------------------------------------------------------------------------------------------------------------------------------------
        public string[] MappedDrives
        {
            get
            {
                System.Collections.ArrayList driveArray = new System.Collections.ArrayList();
                foreach (string driveLetter in System.IO.Directory.GetLogicalDrives())
                {
                    if (PathIsNetworkPath(driveLetter))
                    {
                        driveArray.Add(driveLetter);
                    }
                }
                return ((string[])driveArray.ToArray(typeof(string)));
            }
        }

        #endregion

        #region Public functions

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Map network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void MapDrive()
        {
            mapDrive(null, null);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Map network drive (using supplied Username and Password)
        //
        // <param name="username">Username passed for permissions / credintals ('Username' may be passed as null, to map using only a password)</param>
        // <param name="password">Password passed for permissions / credintals</param>
        //---------------------------------------------------------------------------------------------------------------------------------------

        public void MapDrive(string username, string password)
        {
            mapDrive(username, password);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Set common propertys, then map the network drive
        //
        // <param name="localDrive">LocalDrive to use for connection</param>
        // <param name="shareName">Share name for the connection (eg. '\\Computer\Share')</param>
        // <param name="force">Option to force dis/connection</param>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void MapDrive(string localDrive, string shareName, bool force)
        {
            _localDrive = localDrive;
            _shareName = shareName;
            _force = force;
            mapDrive(null, null);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Set common propertys, then map the network drive
        //
        // <param name="localDrive">Password passed for permissions / credintals</param>
        // <param name="force">Option to force dis/connection</param>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void MapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            _force = force;
            mapDrive(null, null);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Unmap network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void UnMapDrive()
        {
            unMapDrive();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Unmap network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void UnMapDrive(string localDrive)
        {
            _localDrive = localDrive;
            unMapDrive();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Unmap network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void UnMapDrive(string localDrive, bool force)
        {
            _localDrive = localDrive;
            _force = force;
            unMapDrive();
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Check / restore persistent network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void RestoreDrives()
        {
            restoreDrive(null);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Check / restore persistent network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void RestoreDrive(string localDrive)
        {
            restoreDrive(localDrive);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Display windows dialog for mapping a network drive (using Desktop as parent form)
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void ShowConnectDialog()
        {
            displayDialog(System.IntPtr.Zero, 1);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Display windows dialog for mapping a network drive
        //
        // <param name="parentFormHandle">Form used as a parent for the dialog</param>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void ShowConnectDialog(System.IntPtr parentFormHandle)
        {
            displayDialog(parentFormHandle, 1);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Display windows dialog for disconnecting a network drive (using Desktop as parent form)
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void ShowDisconnectDialog()
        {
            displayDialog(System.IntPtr.Zero, 2);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Display windows dialog for disconnecting a network drive
        //
        // <param name="parentFormHandle">Form used as a parent for the dialog</param>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public void ShowDisconnectDialog(System.IntPtr parentFormHandle)
        {
            displayDialog(parentFormHandle, 2);
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Returns the share name of a connected network drive
        //
        // <param name="localDrive">Drive name (eg. 'X:')</param>
        // <returns>Share name (eg. \\computer\share)</returns>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static string GetMappedShareName(string localDrive)
        {

            // collect and clean the passed LocalDrive param
            if (localDrive == null || localDrive.Length == 0)
                throw new System.Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            localDrive = localDrive.Substring(0, 1);

            // call api to collect LocalDrive's share name
            int i = 255;
            byte[] bSharename = new byte[i];
            int iCallStatus = WNetGetConnection(localDrive + ":", bSharename, ref i);
            switch (iCallStatus)
            {
                case 1201:
                    throw new System.Exception("Cannot collect 'ShareName', Passed 'DriveName' is valid but currently not connected (API: ERROR_CONNECTION_UNAVAIL)");
                case 1208:
                    throw new System.Exception("API function 'WNetGetConnection' failed (API: ERROR_EXTENDED_ERROR:" + iCallStatus.ToString() + ")");
                case 1203:
                case 1222:
                    throw new System.Exception("Cannot collect 'ShareName', No network connection found (API: ERROR_NO_NETWORK / ERROR_NO_NET_OR_BAD_PATH)");
                case 2250:
                    throw new System.Exception("Invalid 'DriveName' passed, Drive is not a network drive (API: ERROR_NOT_CONNECTED)");
                case 1200:
                    throw new System.Exception("Invalid / Malfored 'Drive Name' passed to 'GetShareName' function (API: ERROR_BAD_DEVICE)");
                case 234:
                    throw new System.Exception("Invalid 'Buffer' length, buffer is too small (API: ERROR_MORE_DATA)");
            }

            // return collected share name
            return System.Text.Encoding.GetEncoding(1252).GetString(bSharename, 0, i).TrimEnd((char)0);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // Returns true if passed drive is a network drive
        //
        // <param name="localDrive">Drive name (eg. 'X:')</param>
        // <returns>'True' if the passed drive is a mapped network drive</returns>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNetworkDrive(string localDrive)
        {

            // collect and clean the passed LocalDrive param
            if (localDrive == null || localDrive.Trim().Length == 0)
                throw new System.Exception("Invalid 'localDrive' passed, 'localDrive' parameter cannot be 'empty'");
            localDrive = localDrive.Substring(0, 1);

            // return status of drive type
            return PathIsNetworkPath(localDrive + ":");
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        // Returns the full path with the network drive letter replaced by the URL
        //
        // <param name="ThePath">The Path that is to be modified(eg. 'h:\Documents')</param>
        // <returns>full path (eg. \\myserver\users\hans\Documents)</returns>
        //---------------------------------------------------------------------------------------------------------------------------------------
        public static string ReplaceDriveLetterWithURL(string ThePath)
        {
            string TheRoot = "";
            string MappedDrive = "";
            string TheFinalString = ThePath;

            TheRoot = Path.GetPathRoot(ThePath);
            if ((TheRoot.Length == 3) & (TheRoot.EndsWith(":\\")))
            {
                if (IsNetworkDrive(TheRoot))
                {
                    MappedDrive =
                        GetMappedShareName(TheRoot) + "\\";
                    TheFinalString = ThePath.Replace(TheRoot, MappedDrive);
                }
            }

            return TheFinalString;
        }

        public static double PathLatency(string ThePath)
        {
            DateTime Before;
            DateTime After;
            TimeSpan Elapsed;
            bool Check;
            double mylatency;

            Before = DateTime.Now;
            Check = Directory.Exists(ThePath);
            After = DateTime.Now;
            if (Check)
            {
                Elapsed = After - Before;
                mylatency = Elapsed.TotalMilliseconds;
            }
            else
            {
                mylatency = -1;
            }

            return mylatency;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Private functions

        //---------------------------------------------------------------------------------------------------------------------------------------
        // map network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        private void mapDrive(string username, string password)
        {

            // if drive property is set to auto select, collect next free drive
            if (_findNextFreeDrive)
            {
                _localDrive = nextFreeDrive();
                if (_localDrive == null || _localDrive.Length == 0)
                    throw new System.Exception("Could not find valid free drive name");
            }

            // create struct data to pass to the api function
            structNetResource stNetRes = new structNetResource();
            stNetRes.Scope = 2;
            stNetRes.Type = RESOURCETYPE_DISK;
            stNetRes.DisplayType = 3;
            stNetRes.Usage = 1;
            stNetRes.RemoteName = _shareName;
            stNetRes.LocalDrive = _localDrive;

            // prepare flags for drive mapping options
            int iFlags = 0;
            if (_saveCredentials)
                iFlags += CONNECT_CMD_SAVECRED;
            if (_persistent)
                iFlags += CONNECT_UPDATE_PROFILE;
            if (_promptForCredentials)
                iFlags += CONNECT_INTERACTIVE + CONNECT_PROMPT;

            // prepare username / password params
            if (username != null && username.Length == 0)
                username = null;
            if (password != null && password.Length == 0)
                password = null;

            // if force, unmap ready for new connection
            if (_force)
            {
                try
                {
                    this.unMapDrive();
                }
                catch
                {
                }
            }

            // call and return
            int i = WNetAddConnection(ref stNetRes, password, username, iFlags);
            if (i > 0)
                throw new System.ComponentModel.Win32Exception(i);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // unmap network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        private void unMapDrive()
        {

            // prep vars and call unmap
            int iFlags = 0;
            int iRet = 0;

            // if persistent, set flag
            if (_persistent)
            {
                iFlags += CONNECT_UPDATE_PROFILE;
            }

            // if local drive is null, unmap with use connection
            if (_localDrive == null)
            {
                // unmap use connection, passing the share name, as local drive
                iRet = WNetCancelConnection(_shareName, iFlags, System.Convert.ToInt32(_force));
            }
            else
            {
                // unmap drive
                iRet = WNetCancelConnection(_localDrive, iFlags, System.Convert.ToInt32(_force));
            }

            // if errors, throw exception
            if (iRet > 0)
                throw new System.ComponentModel.Win32Exception(iRet);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // check / restore a network drive
        //---------------------------------------------------------------------------------------------------------------------------------------
        private void restoreDrive(string driveName)
        {

            // call restore and return
            int i = WNetRestoreConnection(0, driveName);

            // if error returned, throw
            if (i > 0)
                throw new System.ComponentModel.Win32Exception(i);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // display windows dialog
        //---------------------------------------------------------------------------------------------------------------------------------------
        private void displayDialog(System.IntPtr wndHandle, int dialogToShow)
        {

            // prep variables
            int i = -1;
            int iHandle = 0;

            // get parent handle
            if (wndHandle != System.IntPtr.Zero)
                iHandle = wndHandle.ToInt32();

            // choose dialog to show bassed on
            if (dialogToShow == 1)
                i = WNetConnectionDialog(iHandle, RESOURCETYPE_DISK);
            else if (dialogToShow == 2)
                i = WNetDisconnectDialog(iHandle, RESOURCETYPE_DISK);

            // if error returned, throw
            if (i > 0)
                throw new System.ComponentModel.Win32Exception(i);

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // returns the next viable drive name to use for mapping
        //---------------------------------------------------------------------------------------------------------------------------------------
        private string nextFreeDrive()
        {

            // loop from c to z and check that drive is free
            string retValue = null;
            for (int i = 67; i <= 90; i++)
            {
                if (GetDriveType(((char)i).ToString() + ":") == 1)
                {
                    retValue = ((char)i).ToString() + ":";
                    break;
                }
            }

            // return selected drive
            return retValue;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region API functions / calls
        //---------------------------------------------------------------------------------------------------------------------------------------

        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetAddConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetAddConnection(ref structNetResource netResStruct, string password, string username, int flags);
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2A", CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int WNetCancelConnection(string name, int flags, int force);
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetConnectionDialog", SetLastError = true)]
        private static extern int WNetConnectionDialog(int hWnd, int type);
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetDisconnectDialog", SetLastError = true)]
        private static extern int WNetDisconnectDialog(int hWnd, int type);
        [System.Runtime.InteropServices.DllImport("mpr.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern int WNetRestoreConnection(int hWnd, string localDrive);
        [System.Runtime.InteropServices.DllImport("mpr.dll", EntryPoint = "WNetGetConnection", SetLastError = true)]
        private static extern int WNetGetConnection(string localDrive, byte[] remoteName, ref int bufferLength);
        [System.Runtime.InteropServices.DllImport("shlwapi.dll", EntryPoint = "PathIsNetworkPath", SetLastError = true)]
        private static extern bool PathIsNetworkPath(string localDrive);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", EntryPoint = "GetDriveType", SetLastError = true)]
        private static extern int GetDriveType(string localDrive);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct structNetResource
        {
            public int Scope;
            public int Type;
            public int DisplayType;
            public int Usage;
            public string LocalDrive;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        // standard
        private const int RESOURCETYPE_DISK = 0x1;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_UPDATE_PROFILE = 0x00000001;

        // ie4+
        private const int CONNECT_REDIRECT = 0x00000080;

        // nt5+
        private const int CONNECT_COMMANDLINE = 0x00000800;
        private const int CONNECT_CMD_SAVECRED = 0x00001000;

        //---------------------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}
