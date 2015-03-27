using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SoftRouter
{
	class RouteTableList
	{
		Dictionary<IPAddress, RouteTable> routeTable;

		public RouteTableList()
		{
 			routeTable = new Dictionary<IPAddress,RouteTable>();
		}
	}
}
