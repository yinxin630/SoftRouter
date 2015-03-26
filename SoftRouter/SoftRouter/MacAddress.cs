using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using SharpPcap;
using PacketDotNet;
using SharpPcap.WinPcap;
using System.Threading;

namespace SoftRouter
{
	class MacAddress
	{
		#region 存储IP -> MAC 映射关系
		static public Dictionary<IPAddress, PhysicalAddress> macAddress = new Dictionary<IPAddress, PhysicalAddress>();
		#endregion

		#region 获取IP所对应的MAC地址,超时返回null
		static public PhysicalAddress GetMacAddress(IPAddress ip)
		{
			if (macAddress.ContainsKey(ip))
			{
				return macAddress[ip];
			}

			//每个网卡均发送arp request包，并进入监听模式等待收取response包
			foreach (ICaptureDevice dev in SoftRouter.deviceList)
			{
				PhysicalAddress targetMac = PhysicalAddress.Parse("00-00-00-00-00-00");
				IPAddress targetIp = ip;
				PhysicalAddress senderMac = dev.MacAddress;
				IPAddress senderIp = ((WinPcapDevice)dev).Addresses[0].Addr.ipAddress;
				PhysicalAddress destMac = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

				PacketDotNet.ARPPacket arp = new ARPPacket(ARPOperation.Request, targetMac, targetIp, senderMac, senderIp);

				EthernetPacket eth = new EthernetPacket(senderMac, destMac, EthernetPacketType.Arp);
				eth.PayloadPacket = arp;
				dev.SendPacket(eth);
			}
			for (int i = 0; i < 10 && !macAddress.ContainsKey(ip); i++)
			{
				Thread.Sleep(100);
			}

			if (!macAddress.ContainsKey(ip))
			{
				macAddress.Add(ip, null);
			}
			return macAddress[ip];
		}
		#endregion
	}
}
