using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoftRouter
{
	public partial class MainForm : Form
	{
		static public SoftRouter softRoute;
		public MainForm()
		{
			InitializeComponent();
			softRoute = new SoftRouter();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			softRoute.outputWindow = richTextBox1;
			toolStripButton2.Enabled = false;
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
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			softRoute.StopCapture();
			toolStripButton1.Enabled = true;
			toolStripButton2.Enabled = false;
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			AddRoute dia = new AddRoute();
			dia.ShowDialog();
		}

		public RouteTableList GetRoute()
		{
			return softRoute.StaticRouting;
		}
	}
}
