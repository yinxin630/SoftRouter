using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;
using PacketDotNet;
using System.Net;
using System.Net.NetworkInformation;

namespace SoftRouter
{
	public partial class MainForm : Form
	{
		static public SoftRouter softRoute = null;
		public MainForm()
		{
			InitializeComponent();
			softRoute = new SoftRouter();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			softRoute.OnAppendPacketInfo += AppendPacketInfo;
			softRoute.OnAddListviewItem += AddListviewItem;
			toolStripButton2.Enabled = false;

			comboBox1.Items.AddRange(new string[] {"ARP", "IP", "TCP", "UDP", "ICMP"});
			comboBox1.SelectedIndex = 0;

			foreach (Device dev in Device.GetDeviceList())
			{
				toolStripComboBox1.Items.Add(dev.Name);
				comboBox2.Items.Add(dev.Name);
			}
			toolStripComboBox1.SelectedIndex = 0;
			comboBox2.SelectedIndex = 0;
			richTextBox3.AppendText("程序启动>>>>\n");
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			softRoute.CloseDevice();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			softRoute.StartCapture();
			toolStripButton2.Enabled = true;
			toolStripButton1.Enabled = false;
			richTextBox3.AppendText(DateTime.Now.ToString() + ":开始监听\n");
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			softRoute.StopCapture();
			toolStripButton1.Enabled = true;
			toolStripButton2.Enabled = false;
			richTextBox3.AppendText(DateTime.Now.ToString() + ":停止监听\n");
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			AddRoute dia = new AddRoute();
			dia.ShowDialog();
			richTextBox3.AppendText(DateTime.Now.ToString() + ":修改路由信息\n");
		}
		private void AppendPacketInfo(string info)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				richTextBox1.AppendText(info);
				richTextBox1.Focus();
				richTextBox1.Select(richTextBox1.TextLength, 0);
			}));
		}

		private void AddListviewItem(ListViewItem item)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				listView1.Items.Add(item);
			}));
		}

		public RouteTableList GetRoute()
		{
			return softRoute.StaticRouting;
		}

		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			int selectNumber = toolStripComboBox1.SelectedIndex;
			MessageBox.Show(softRoute.deviceList[selectNumber].ToString(), "网卡信息");
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (comboBox1.SelectedIndex)
			{
				case 0:
					{
						textBox1.Enabled = true;
						textBox2.Enabled = true;
						ipAddressBox1.Enabled = true;
						ipAddressBox2.Enabled = true;
						numericUpDown1.Enabled = false;
						numericUpDown2.Enabled = false;
					}
					break;
				case 1:
				case 4:
					{
						textBox1.Enabled = false;
						textBox2.Enabled = false;
						ipAddressBox1.Enabled = true;
						ipAddressBox2.Enabled = true;
						numericUpDown1.Enabled = false;
						numericUpDown2.Enabled = false;
					}
					break;
				case 2:
				case 3:
					{
						textBox1.Enabled = false;
						textBox2.Enabled = false;
						ipAddressBox1.Enabled = true;
						ipAddressBox2.Enabled = true;
						numericUpDown1.Enabled = true;
						numericUpDown2.Enabled = true;
					}
					break;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//ICMP
			if (comboBox1.SelectedIndex == 4)
			{
				byte[] buffer = new byte[32];
				ICMPv4Packet icmp = new ICMPv4Packet(new PacketDotNet.Utils.ByteArraySegment(buffer));
				icmp.TypeCode = ICMPv4TypeCodes.EchoRequest;
				//icmp.Checksum = ;

				IPAddress sourceIP = IPAddress.Parse(ipAddressBox1.Text);
				IPAddress destinationIP = IPAddress.Parse(ipAddressBox2.Text);
				IPv4Packet ipv4 = new IPv4Packet(sourceIP, destinationIP);
				ipv4.PayloadPacket = icmp;

				if (!softRoute.macAddress.ContainsKey(sourceIP))
				{
					MacAddress.GetMacAddress(sourceIP);
					MessageBox.Show("源MAC不存在，获取中，请稍候再试！", "提示");
					return;
				}
				PhysicalAddress sourceMac = softRoute.macAddress[sourceIP];
				if (!softRoute.macAddress.ContainsKey(destinationIP))
				{
					MacAddress.GetMacAddress(destinationIP);
					MessageBox.Show("目的MAC不存在，获取中，请稍候再试！", "提示");
					return;
				}
				PhysicalAddress destinationMac = softRoute.macAddress[destinationIP];
				EthernetPacket eth = new EthernetPacket(sourceMac, destinationMac, EthernetPacketType.IpV4);
				eth.PayloadPacket = ipv4;
				softRoute.deviceList[comboBox2.SelectedIndex].Interface.SendPacket(eth);
			}
			//ARP
			else if (comboBox1.SelectedIndex == 0)
			{
				//ARPPacket arp = new ARPPacket();
			}
			//IP
			else if (comboBox1.SelectedIndex == 1)
			{
				//IPv4Packet ipv4 = new IPv4Packet();
			}
			//TCP
			else if (comboBox1.SelectedIndex == 2)
			{
				//TcpPacket tcp = new TcpPacket(1, 2);
			}
			//UDP
			else if (comboBox1.SelectedIndex == 3)
			{

			}
		}


		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			richTextBox2.Clear();
			Packet packet = softRoute.packets[listView1.SelectedItems[0].Index];
			byte[] data = packet.Bytes;

			int lineNumber = 0;
			for (int i = 0; i < data.Length; i++)
			{
				if (i % 16 == 0)
				{
					richTextBox2.AppendText(string.Format("{0:X4}  ", lineNumber));
					lineNumber += 16;
				}
				richTextBox2.AppendText(string.Format("{0:X2} ", data[i]));

				if (i % 16 == 7)
				{
					richTextBox2.AppendText("  ");
				}

				if (i % 16 == 15)
				{
					//richTextBox2.AppendText("   ");
					//for (int j = i - 15; j <= i; j++)
					//{
					//	richTextBox2.AppendText(string.Format("{0}", (char)data[j]));
					//}
					richTextBox2.AppendText("\n");
				}
			}
		}
	}
}
