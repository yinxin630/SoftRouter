using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using PacketDotNet;
using SharpPcap.WinPcap;
using System.Net;
using System.Threading;

namespace SoftRouter
{
	class SoftRouter
	{
		#region 存储可用设备列表
		static public List<ICaptureDevice> deviceList;
		#endregion

		#region 存储已处理IP包列表
		static private List<ushort> hadHandledIpList = new List<ushort>();
		#endregion

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
			if (e.Packet.LinkLayerType == LinkLayers.Ethernet)
			{
				try
				{
					EthernetPacket eth = (EthernetPacket)EthernetPacket.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

					if (eth.PayloadPacket is ARPPacket)
					{
						ARPPacket arp = (ARPPacket)eth.PayloadPacket;

						if (arp.Operation == ARPOperation.Response || arp.Operation == ARPOperation.Request)
						{
							MacAddress.macAddress.Add(arp.SenderProtocolAddress, arp.SenderHardwareAddress);
						}
					}
					else if (eth.PayloadPacket is IPv4Packet)
					{
						IPv4Packet ip = (IPv4Packet)eth.PayloadPacket;

						if (hadHandledIpList.Contains(ip.Id))
						{
							return;
						}
						hadHandledIpList.Add(ip.Id);

						if (!(ip.PayloadPacket is ICMPv4Packet))
							return;

						Thread thread = new Thread(() => {
							Console.WriteLine(string.Format("{0}:{1}:{2}/{5} = {3} -> {4}", DateTime.Now.Hour, DateTime.Now.Minute,
								DateTime.Now.Second, ip.SourceAddress, ip.DestinationAddress, DateTime.Now.Millisecond));
						});
						thread.Start();

						foreach (ICaptureDevice dev in deviceList)
						{
							if (dev != (ICaptureDevice)sender)
							{
								eth.SourceHwAddress = dev.MacAddress;
								eth.DestinationHwAddress = MacAddress.GetMacAddress(ip.DestinationAddress);
								if (MacAddress.macAddress[ip.DestinationAddress] == null)
								{
									return;
								}
								dev.SendPacket(eth);
							}
						}
					}
				}
				catch
				{
					//Protocol of 49185 is not implemented
					//不支持此协议,对此类协议包进行忽略
					return;
				}
			}
		}
		#endregion
	}
}
