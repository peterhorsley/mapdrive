using System;
using System.Text;
using System.Runtime.InteropServices;

namespace MapDrive.WNet
{
	public class WNet_Api
	{
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetAddConnection(string remoteName,
			string password, string localName);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetAddConnection2(NetResource netResource,
			string password, string userName, ConnectionFlags flags);
					
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetAddConnection3(IntPtr owner, NetResource netResource,
			string password, string userName, ConnectionFlags flags);
			
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetCancelConnection(string name,
			bool isForce);
			
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetCancelConnection2(string name, ConnectionFlags flags, bool isForce);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetUseConnection(IntPtr owner, NetResource netResource, string password,
			string userId, ConnectionFlags flags, StringBuilder accessName, ref int bufferSize, ref int result);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetConnectionDialog(IntPtr hwnd, ResourceType type);
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetDisconnectDialog(IntPtr hwnd, ResourceType type);
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetConnectionDialog1(ConnectDialogInfo connDlgStruct);
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetDisconnectDialog1(DisconnectDialogInfo disConnDlgStruct);
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetOpenEnum( ResourceScope scope, ResourceType type,
			ResourceUsage usage, NetResource netResource, out IntPtr handle );

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetEnumResource( IntPtr handle, ref int count,
			IntPtr buffer, ref int size );
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetCloseEnum( IntPtr handle );

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetUser(string deviceName, StringBuilder userName, ref int length);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetLastError(out int error, out StringBuilder errorBuf, int errorBufSize, out StringBuilder nameBuf, int nameBufSize);
		
		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int MultinetGetConnectionPerformance(NetResource netResource, ConnectInfo info);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetProviderName(NetworkType netType, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetNetworkInformation(string provider, NetInfo info);		

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetResourceInformation(NetResource netResource, IntPtr buffer, ref int length,
			out string path);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetResourceParent(NetResource netResource, IntPtr buffer, ref int length);

		[DllImport("mpr.dll", CharSet=CharSet.Auto)]
		public static extern int WNetGetUniversalName(string localPath, NameInfoLevel level, IntPtr buffer, ref int length);

		public enum NameInfoLevel:uint
		{
			Universal = 0x00000001,
			Remote = 0x00000002
		}
	}
	
	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class ConnectDialogInfo
	{
		public int StructureSize;
		public IntPtr Owner;
		public NetResource ConnectResource;
		public ConnectDialogFlags Flags;
		public int DeviceNumber;
	}


	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class DisconnectDialogInfo
	{
		public int StructureSize;
		public IntPtr Owner;
		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string LocalName;
		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string RemoteName;
		public DisconnectDialogFlags Flags;
	}
	

	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class ConnectInfo
	{
		public int StructureSize;
		public int Flags;
		public int Speed;
		public int Delay;
		public int OptDataSize;
	}
	

	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class NetInfo
	{
		public int StructureSize;
		public int ProviderVersion;
		public int Status;
		public int Characteristics;
		public IntPtr Handle;
		public short NetType;
		public int Printers;
		public int Drives;
	}
	

	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class NetResource
	{
		public ResourceScope  Scope;
		public ResourceType  ResourceType;
		public ResourceDisplayType  DisplayType;
		public ResourceUsage  Usage;

		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string   LocalName;
		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string   RemoteName;
		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string   Comment;
		[MarshalAs(UnmanagedType.LPTStr, SizeConst=200)]
		public string   Provider;

		public static NetResource Root = new NetResource();

		public NetResource WithResourceType(ResourceType resourceType)
		{
			this.ResourceType = resourceType;
			return this;
		}
		public NetResource WithLocalName(string localName)
		{
			this.LocalName = localName;
			return this;
		}
		public NetResource WithRemoteName(string remoteName)
		{
			this.RemoteName = remoteName;
			return this;
		}
		public NetResource WithProvider(string provider)
		{
			this.Provider = provider;
			return this;
		}
	}


	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class RemoteNameInfo
	{
		[MarshalAs(UnmanagedType.LPTStr)]
		public string   UniversalName;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string   ConnectionName;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string   RemainingPath;
	}


	[StructLayoutAttribute(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class UniversalNameInfo
	{
		[MarshalAs(UnmanagedType.LPTStr)]
		public string   UniversalName;
	}

	public enum NetworkType
	{
		MsNet						= 0x00010000,
		LanMan						= 0x00020000,
		NetWare					= 0x00030000,
		Vines						= 0x00040000,
		_10Net						= 0x00050000,
		Locus						= 0x00060000,
		SunPcNfs					= 0x00070000,
		LanStep					= 0x00080000,
		_9Tiles						= 0x00090000,
		Lantastic					= 0x000A0000,
		As400						= 0x000B0000,
		FtpNfs					= 0x000C0000,
		PathWorks					= 0x000D0000,
		LifeNet					= 0x000E0000,
		PowerLan					= 0x000F0000,
		Bwnfs						= 0x00100000,
		Cogent						= 0x00110000,
		Farallon					= 0x00120000,
		AppleTalk					= 0x00130000,
		InterGraph					= 0x00140000,
		SymfoNet					= 0x00150000,
		ClearCase					= 0x00160000,
		Frontier					= 0x00170000,
		Bmc						= 0x00180000,
		Dce						= 0x00190000,
		Decorb						= 0x00200000,
		Protstor					= 0x00210000,
		Fj_Redir					= 0x00220000,
		Distinct					= 0x00230000,
		Twins						= 0x00240000,
		Rdr2Sample					= 0x00250000
	}
	[Flags]
	public enum ResourceScope: int
	{
		Connected					= 0x00000001,
		GlobalNet					= 0x00000002,
		Remembered					= 0x00000003,
		Recent						= 0x00000004,
		Context					= 0x00000005,
	}

	public enum ResourceType: uint
	{
		Any					= 0x00000000,
		Disk					= 0x00000001,
		Print					= 0x00000002,
		Reserved				= 0x00000008,
		Unknown				= 0xFFFFFFFFu
	}

	[Flags]
	public enum ResourceUsage: uint
	{
		Connectable			= 0x00000001,
		Container				= 0x00000002,
		NoLocalDevice			= 0x00000004,
		Sibling				= 0x00000008,
		Attached				= 0x00000010,
		All					= 0x00000013,
		Reserved				= 0x80000000u
	}

	public enum ResourceDisplayType:
		int
	{
		Generic         = 0x00000000,
		Domain          = 0x00000001,
		Server          = 0x00000002,
		Share           = 0x00000003,
		File            = 0x00000004,
		Group           = 0x00000005,
		Network         = 0x00000006,
		Root            = 0x00000007,
		ShareAdmin      = 0x00000008,
		Directory		= 0x00000009,
		Tree			= 0x0000000A,
		NdsContainer	= 0x0000000B,
	}

	public class WNetErrors
	{
		public const int NoError = 0;
		public const int NoMoreItems = 259;
		public const int NoMoreData = 234;
	}

	[Flags]
	public enum ConnectionFlags:
		uint
	{
		UpdateProfile				= 0x00000001,
		UpdateRecent				= 0x00000002,
		Temporary					= 0x00000004,
		Interactive					= 0x00000008,
		Prompt						= 0x00000010,
		NeedDrive					= 0x00000020,
		RefCount					= 0x00000040,
		Redirect					= 0x00000080,
		LocalDrive					= 0x00000100,
		CurrentMedia				= 0x00000200,
		Deferred					= 0x00000400,
		Reserved					= 0xFF000000u
	}

	[Flags]
	public enum DisconnectDialogFlags:
		int
	{
		UpdateProfile					= 0x00000001,
		NoForce						= 0x00000040,
	}

	[Flags]
	public enum ConnectDialogFlags:
		int
	{
		ReadOnlyPath						= 0x00000001, 
//		Conn_point					= 0x00000002, 
		UseMru						= 0x00000004, 
		HideBox					= 0x00000008, 
		Persist						= 0x00000010, 
		NotPersist					= 0x00000020, 
	}
}
