using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpPcap.WinPcap;
using System.Net;

namespace SoftRouter
{
	public partial class AddRoute : Form
	{
		public AddRoute()
		{
			InitializeComponent();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			IPAddress net = IPAddress.Parse(ipAddressBox1.Text);
			IPAddress mask = IPAddress.Parse(ipAddressBox2.Text);
			IPAddress next = IPAddress.Parse(ipAddressBox3.Text);
			int level = Convert.ToInt32(numericUpDown1.Value);

			List<Device> list = MainForm.softRoute.deviceList;
			RouteTable route = new RouteTable(net, mask, next, level, list[comboBox1.SelectedIndex].Interface);
			MainForm.softRoute.StaticRouting.RouteTable.Add(route);

			ListViewItem item = new ListViewItem((listView1.Items.Count + 1).ToString());
			item.SubItems.AddRange(new string[] {ipAddressBox1.Text, ipAddressBox2.Text, ipAddressBox3.Text, level.ToString(), comboBox1.SelectedText});
			listView1.Items.Add(item);
		}

		private void AddRoute_Load(object sender, EventArgs e)
		{
			numericUpDown1.Value = 60;
			foreach (Device dev in Device.GetDeviceList())
			{
				comboBox1.Items.Add(dev.Name);
			}
			comboBox1.SelectedIndex = 0;

			RouteTableList list = MainForm.softRoute.StaticRouting;
			int index = 1;
			foreach (RouteTable route in list.RouteTable)
			{
				ListViewItem item = new ListViewItem(index.ToString());
				item.SubItems.AddRange(new string[] {route.NetAddress.ToString(), route.MaskAddress.ToString(),
				route.NextHop.ToString(), route.Level.ToString(), route.OutInterface.Description});
				listView1.Items.Add(item);
			}
		}
	}
}
