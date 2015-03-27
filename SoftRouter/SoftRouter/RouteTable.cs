using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SoftRouter
{
	class RouteTable
	{
		private IPAddress _net;
		private IPAddress _mask;
		private IPAddress _nextHop;

		public RouteTable(IPAddress net, IPAddress mask, IPAddress nextHop)
		{
			_net = net;
			_mask = mask;
			_nextHop = nextHop;
		}
	}
}
