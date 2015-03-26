using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using PacketDotNet;
using SharpPcap.WinPcap;
using System.Net;

namespace SoftRouter
{
	class SoftRouter
	{
		static public List<ICaptureDevice> deviceList;
		static void Main(string[] args)
		{
			GetDeviceList();

			int index = 1;
			foreach (ICaptureDevice dev in deviceList)
			{
				Console.WriteLine(string.Format("ID:{0}\n{1}", index, dev));
				index++;
			}
		}

		#region 获取机器可用IPv4设备列表
		static private void GetDeviceList()
		{
			CaptureDeviceList devices = CaptureDeviceList.Instance;
			deviceList = new List<ICaptureDevice>();

			foreach (ICaptureDevice dev in devices)
			{
				WinPcapDevice winDev = (WinPcapDevice)dev;
				IPAddress devIp = winDev.Addresses[0].Addr.ipAddress;
				if (devIp != null && devIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					deviceList.Add(dev);
				}
			}
		}
		#endregion
	}
}
