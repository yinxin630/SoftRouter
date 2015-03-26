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

			foreach (ICaptureDevice dev in deviceList)
			{
				dev.Open(DeviceMode.Promiscuous);
				dev.OnPacketArrival += OnPacketArrval;
				dev.StartCapture();
			}

			Console.ReadLine();

			foreach (ICaptureDevice dev in deviceList)
			{
				dev.StopCapture();
				dev.Close();
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

		#region 数据包捕获处理
		static public void OnPacketArrval(object sender, CaptureEventArgs e)
		{
			Console.WriteLine("Get a packet");
			Console.WriteLine(e.Packet);
		}
		#endregion
	}
}
