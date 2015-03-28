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
		#region 获取IP地址的MAC信息,通过广播ARP请求包获取
		static public void GetMacAddress(IPAddress ip)
		{
			//每个网卡均发送arp request包，并进入监听模式等待收取response包
			foreach (Device dev in Device.GetDeviceList())
			{
				PhysicalAddress targetMac = PhysicalAddress.Parse("00-00-00-00-00-00");
				IPAddress targetIp = ip;
				PhysicalAddress senderMac = dev.MacAddress;
				IPAddress senderIp = ((WinPcapDevice)dev.Interface).Addresses[0].Addr.ipAddress;
				PhysicalAddress destMac = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF");

				PacketDotNet.ARPPacket arp = new ARPPacket(ARPOperation.Request, targetMac, targetIp, senderMac, senderIp);

				EthernetPacket eth = new EthernetPacket(senderMac, destMac, EthernetPacketType.Arp);
				eth.PayloadPacket = arp;
				dev.Interface.SendPacket(eth);
			}
		}
		#endregion
	}
}
