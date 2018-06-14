### Map network drives on startup of Windows 7+ reliably.

If the drive mapping fails, the program will keep attempting to create the mapping until the specified timeout is reached. 
This is a solution for the poor reliablity of mapping network drives on startup in Windows 7+.

*Syntax:*
`MapDrive.exe <driveLettter:> <serverShare> [timeoutSeconds] [username] [password]`

*Example:*
`MapDrive.exe s: \\server\share 20`

This will keep attempting to map s: to the specified path for up to 20 seconds before giving up.

You can run this program via a shortcut in your Startup folder to map drives for non-elevated processes and/or via a group-policy logon script configured via gpedit.msc for elevated processes.  

More info here: http://zornsoftware.codenature.info/blog/windows-7-disconnected-network-drives.html