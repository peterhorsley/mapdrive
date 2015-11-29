using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

namespace MapDrive.WNet
{

	/// <summary>
	/// Summary description for WNet.
	/// </summary>
	public class WNet
	{
		public static void AddConnection(string remoteName, string password, string localName)
		{
			GenerateExceptionIfError(WNet_Api.WNetAddConnection(remoteName, password, localName));
		}
		public static void AddConnection(NetResource netResource, string password, string userName, 
			ConnectionFlags options)
		{
			GenerateExceptionIfError(WNet_Api.WNetAddConnection2(netResource, password, userName, options));
		}
		public static void AddConnection(Control ownerWindow, NetResource netResource, string password, 
			string userName, ConnectionFlags options)
		{
			GenerateExceptionIfError(WNet_Api.WNetAddConnection3(Common.ControlToHwnd(ownerWindow), 
				netResource, password, userName, options));
		}

		public static void CancelConnection(string name, bool isForce)
		{
			GenerateExceptionIfError(WNet_Api.WNetCancelConnection(name, isForce));
		}
		public static void CancelConnection(string name, ConnectionFlags options, bool isForce)
		{
			GenerateExceptionIfError(WNet_Api.WNetCancelConnection2(name, options, isForce));
		}

		public static DialogResult ConnectionDialog(Control ownerWindow, ResourceType type)
		{
			return MakeDialogResultOrGenerateException(WNet_Api.WNetConnectionDialog(
				Common.ControlToHwnd(ownerWindow), 
				type));
		}
		public static DialogResult ConnectionDialog(ConnectDialogInfo info)
		{
			info.StructureSize = Marshal.SizeOf(typeof(ConnectDialogInfo));
			return MakeDialogResultOrGenerateException(WNet_Api.WNetConnectionDialog1(info));
		}
		public static DialogResult DisconnectWithDialog(Control ownerWindow, ResourceType type)
		{
			return MakeDialogResultOrGenerateException(WNet_Api.WNetDisconnectDialog(
				Common.ControlToHwnd(ownerWindow), type));
		}
//		public static DialogResult DisconnectWithDialog(DisconnectDialogInfo info)
//		{
//			info.Size = Marshal.SizeOf(typeof(DisconnectDialogInfo));
//			return MakeDialogResultOrGenerateException(WNet_Api.WNetDisconnectDialog1(info));
//		}

		public static string GetConnectionRemoteName(string localName)
		{
			int length = 0;
			GenerateExceptionIfError(WNet_Api.WNetGetConnection(localName, null, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			StringBuilder remoteName = new StringBuilder(length);
			GenerateExceptionIfError(WNet_Api.WNetGetConnection(localName, remoteName, ref length));
			return remoteName.ToString();
		}
		public static string GetConnection(string localName)
		{
			return GetConnectionRemoteName(localName);
		}
		public static void GetLastError(out int errorCode, out string errorText, out string providerName)
		{
			const int maxLength = 1000;
			StringBuilder text = new StringBuilder(maxLength);
			StringBuilder provider = new StringBuilder(maxLength);
			GenerateExceptionIfError(WNet_Api.WNetGetLastError(out errorCode, out text, text.Capacity,
				out provider, provider.Capacity));
			errorText = text.ToString();
			providerName = provider.ToString();
		}

		public static NetInfo GetNetworkInformation(string provider)
		{
			NetInfo netInfo = new NetInfo();
			netInfo.StructureSize = Marshal.SizeOf(typeof(NetInfo));
			GenerateExceptionIfError(WNet_Api.WNetGetNetworkInformation(provider, netInfo));
			return netInfo;
		}
		public static string GetProviderName(NetworkType netType)
		{
			int length = 0;
			GenerateExceptionIfError(WNet_Api.WNetGetProviderName(netType, null, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			StringBuilder providerName = new StringBuilder(length);
			GenerateExceptionIfError(WNet_Api.WNetGetProviderName(netType, providerName, ref length));
			return providerName.ToString();
		}
		public static string GetUser(string name)
		{
			int length = 0;
			GenerateExceptionIfError(WNet_Api.WNetGetUser(name, null, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			StringBuilder user = new StringBuilder(length);
			GenerateExceptionIfError(WNet_Api.WNetGetUser(name, user, ref length));
			return user.ToString();
		}

		public static ConnectInfo GetConnectionPerformance(NetResource netResource)
		{
			ConnectInfo info = new ConnectInfo();
			info.StructureSize = Marshal.SizeOf(typeof(ConnectInfo));
			GenerateExceptionIfError(WNet_Api.MultinetGetConnectionPerformance(netResource, info));
			return info;
		}
		public static ConnectInfo MultinetGetConnectionPerformance(NetResource netResource)
		{
			return GetConnectionPerformance(netResource);
		}
		public static NetResource GetResourceInformation(NetResource netResource, out string path)
		{
			int length = 0;
			GenerateExceptionIfError(WNet_Api.WNetGetResourceInformation(netResource, IntPtr.Zero, ref length, out path), WNetErrors.NoError, WNetErrors.NoMoreData);
			IntPtr buffer = Marshal.AllocCoTaskMem(length);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetResourceInformation(netResource, buffer, ref length, out path));
				return (NetResource)Marshal.PtrToStructure(buffer, typeof(NetResource));
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}
		public static NetResource GetResourceInformation(NetResource netResource)
		{
			string path;
			return GetResourceInformation(netResource, out path);
		}

		public static NetResource GetResourceParent(NetResource netResource)
		{
			int length = 0;
			GenerateExceptionIfError(WNet_Api.WNetGetResourceParent(netResource, IntPtr.Zero, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			IntPtr buffer = Marshal.AllocCoTaskMem(length);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetResourceParent(netResource, buffer, ref length));
				return (NetResource)Marshal.PtrToStructure(buffer, typeof(NetResource));
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}
		public static string GetUniversalName(string localPath)
		{
			int length = 0;
			//WNetGetUniversalName don't allow buffer is null
			IntPtr dummyBuffer = Marshal.AllocCoTaskMem(5);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetUniversalName(localPath, WNet_Api.NameInfoLevel.Universal, dummyBuffer, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			}
			finally
			{
				Marshal.FreeCoTaskMem(dummyBuffer);
			}
			IntPtr buffer = Marshal.AllocCoTaskMem(length);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetUniversalName(localPath, WNet_Api.NameInfoLevel.Universal, buffer, ref length));
				UniversalNameInfo nameInfo = (UniversalNameInfo)Marshal.PtrToStructure(buffer, typeof(UniversalNameInfo));
				return nameInfo.UniversalName;
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		public static RemoteNameInfo GetRemoteUniversalName(string localPath)
		{
			int length = 0;
			//WNetGetUniversalName don't allow buffer is null
			IntPtr dummyBuffer = Marshal.AllocCoTaskMem(5);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetUniversalName(localPath, WNet_Api.NameInfoLevel.Remote, dummyBuffer, ref length), WNetErrors.NoError, WNetErrors.NoMoreData);
			}
			finally
			{
				Marshal.FreeCoTaskMem(dummyBuffer);
			}
			IntPtr buffer = Marshal.AllocCoTaskMem(length);
			try
			{
				GenerateExceptionIfError(WNet_Api.WNetGetUniversalName(localPath, WNet_Api.NameInfoLevel.Remote, buffer, ref length));
				return (RemoteNameInfo)Marshal.PtrToStructure(buffer, typeof(RemoteNameInfo));
			}
			finally
			{
				Marshal.FreeCoTaskMem(buffer);
			}
		}

		public static string UseConnection(Control ownerWindow, NetResource netResource, 
			string password, string userId, ConnectionFlags flags)
		{
			StringBuilder accessName = new StringBuilder(0);
			int length = 0;
			int result = 0;
			GenerateExceptionIfError(WNet_Api.WNetUseConnection(Common.ControlToHwnd(ownerWindow), netResource, 
				password, userId, flags, accessName, ref length, ref result), WNetErrors.NoError, WNetErrors.NoMoreData);
			accessName = new StringBuilder(length);
			GenerateExceptionIfError(WNet_Api.WNetUseConnection(Common.ControlToHwnd(ownerWindow), netResource, 
				password, userId, flags, accessName, ref length, ref result));
			return accessName.ToString();
		}


		public static NetResource[] EnumResources(ResourceScope scope, ResourceType type,
			ResourceUsage usage, NetResource target)
		{
			IntPtr enumHandle;
			GenerateExceptionIfError(WNet_Api.WNetOpenEnum(scope, type, usage, target, out enumHandle));
			try
			{
				const int maxBufferSize = 20000;
				IntPtr buffer = Marshal.AllocCoTaskMem(maxBufferSize);
				try
				{
					NetResource[] results = new NetResource[]{};
					for (;;)
					{
						int bufferSize = maxBufferSize;
						int count = -1;
						int error = WNet_Api.WNetEnumResource(enumHandle, ref count, buffer, ref bufferSize);			
						if (error == WNetErrors.NoMoreItems)
							break;
						if (error != WNetErrors.NoError)
							GenerateException(error);
						if (count < 0)
							continue;
						int index = results.Length;
						results = Realloc(results, index + count);
						for (int i = 0; i < count; ++i)
						{
							results[index + i] = (NetResource)Marshal.PtrToStructure(new IntPtr(buffer.ToInt64() + i * Marshal.SizeOf(typeof(NetResource))), typeof(NetResource));
						}
					}
					return results;
				}
				finally
				{
					Marshal.FreeCoTaskMem(buffer);
				}
			}
			finally
			{
				WNet_Api.WNetCloseEnum(enumHandle);
			}
		}
	
		static NetResource[] Realloc(NetResource[] resources, int newSize)
		{
			NetResource[] results = new NetResource[newSize];
			Array.Copy(resources, results, 0);
			return results;
		}

		static void GenerateExceptionIfError(int error)
		{	
			if (error == WNetErrors.NoError)
				return;
			GenerateException(error);
		}
		static void GenerateExceptionIfError(int error, params int[] successErrorCodes)
		{	
			foreach (int successCode in successErrorCodes)
				if (error == successCode)
					return;
			GenerateException(error);
		}
		static DialogResult MakeDialogResultOrGenerateException(int error)
		{	
			if (error == -1)
				return DialogResult.Cancel;
			if (error != WNetErrors.NoError)
				GenerateException(error);
			return DialogResult.OK;
		}
		static void GenerateException(int error)
		{
			throw new WNetException(error, Common.GetSystemErrorString(error));
		}

	}

}
