using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SoftRouter
{
	class RouteTable
	{
		#region 字段:网络地址,子网掩码,下一跳地址
		private IPAddress _net;
		private IPAddress _mask;
		private IPAddress _nextHop;
		#endregion

		public RouteTable(IPAddress net, IPAddress mask, IPAddress nextHop)
		{
			_net = net;
			_mask = mask;
			_nextHop = nextHop;
		}
	}
}
