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
		SoftRouter softRoute;
		public MainForm()
		{
			InitializeComponent();
			softRoute = new SoftRouter();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			softRoute.StartCapture();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			softRoute.StopCapture();
		}
	}
}
