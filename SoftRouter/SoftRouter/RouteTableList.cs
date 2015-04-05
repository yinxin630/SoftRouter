using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SoftRouter
{
	public class RouteTableList
	{
		List<RouteTable> routeTable;

		public RouteTableList()
		{
 			routeTable = new List<RouteTable>();
		}

		public RouteTable this[IPAddress ip]
		{
			get
			{
				RouteTable t = null;
				foreach (RouteTable route in routeTable)
				{
					var net = SoftRouter.GetNetIpAddress(ip, route.MaskAddress);
					if (net.ToString() == route.NetAddress.ToString() || route.NetAddress.ToString() == IPAddress.Any.ToString())
					{
						if (t == null)
						{
							t = route;
						}
						else
						{
							if (route.Level < t.Level)
								t = route;
						}
					}
				}
				return t;
			}
		}

		public List<RouteTable> RouteTable
		{
			get
			{
				return routeTable;
			}
		}
	}
}
