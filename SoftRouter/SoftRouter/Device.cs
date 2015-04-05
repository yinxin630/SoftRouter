using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpPcap;
using System.Net;
using System.Net.NetworkInformation;
using SharpPcap.WinPcap;

namespace SoftRouter
{
	public class Device
	{
		#region 字段：设备接口,MAC地址,IP地址,子网掩码,网络地址
		private ICaptureDevice _interface;
		private PhysicalAddress _mac;
		private IPAddress _ip;
		private IPAddress _mask;
		private IPAddress _net;
		private string _name;
		#endregion

		#region Device类构造函数
		public Device(ICaptureDevice icd)
		{
			WinPcapDevice win = (WinPcapDevice)icd;

			this._interface = icd;
			this._mac = win.MacAddress;
			this._ip = win.Addresses[0].Addr.ipAddress;
			this._mask = win.Addresses[0].Netmask.ipAddress;
			this._net = SoftRouter.GetNetIpAddress(_ip, _mask);
			this._name = win.Interface.FriendlyName + " " + icd.Description.Split('\'')[1];
		}
		#endregion

		#region 获取机器可用IPv4设备列表
		static public List<Device> GetDeviceList()
		{
			List<Device> deviceList = new List<Device>();
			CaptureDeviceList devices = CaptureDeviceList.Instance;

			foreach (ICaptureDevice dev in devices)
			{
				WinPcapDevice winDev = (WinPcapDevice)dev;
				IPAddress devIp = winDev.Addresses[0].Addr.ipAddress;
				if (devIp != null && devIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					dev.Open(DeviceMode.Promiscuous);
					deviceList.Add(new Device(dev));
				}
			}
			return deviceList;
		}
		#endregion

		public ICaptureDevice Interface
		{
			get
			{
				return _interface;
			}
		}

		public PhysicalAddress MacAddress
		{
			get
			{
				return _mac;
			}
		}

		public IPAddress MaskAddress
		{
			get
			{
				return _mask;
			}
		}

		public IPAddress NetAddress
		{
			get
			{
				return _net;
			}
		}
		public string Name
		{
			get
			{
				return _name;
			}
		}

		public override string ToString()
		{
			return string.Format("设备名:{5}\n标识名:{0}\nMAC地址:{1}\nIP地址:{2}\n子网掩码:{3}\n网络地址:{4}\n",
				_interface.Name, _mac, _ip, _mask, _net, _name);
		}
	}
}
