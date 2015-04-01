using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SharpPcap;

namespace SoftRouter
{
	public class RouteTable
	{
		#region 字段:网络地址,子网掩码,下一跳地址
		private IPAddress _net;
		private IPAddress _mask;
		private IPAddress _nextHop;
		private int _level;
		private ICaptureDevice _outInterface;
		#endregion

		public RouteTable(IPAddress net, IPAddress mask, IPAddress nextHop, int level, ICaptureDevice ic)
		{
			_net = net;
			_mask = mask;
			_nextHop = nextHop;
			_level = level;
			_outInterface = ic;
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

		public int Level
		{
			get
			{
				return _level;
			}
		}

		public ICaptureDevice OutInterface
		{
			get
			{
				return _outInterface;
			}
		}

		public IPAddress NextHop
		{
			get
			{
				return _nextHop;
			}
		}
	}
}
