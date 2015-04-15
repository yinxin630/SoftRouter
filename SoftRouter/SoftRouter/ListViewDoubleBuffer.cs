using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SoftRouter
{
	public class ListViewDoubleBuffer : ListView
	{
		public ListViewDoubleBuffer()
		{
			SetStyle(ControlStyles.DoubleBuffer |
			   ControlStyles.OptimizedDoubleBuffer |
			   ControlStyles.AllPaintingInWmPaint, true);
			UpdateStyles();
		}
	}
}
