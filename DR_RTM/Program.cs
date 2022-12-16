using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DR_RTM
{

	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new Form1());
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
			static void OnProcessExit(object sender, EventArgs e)
            {
				if (TimeSkip.UpdateTimer.Enabled == true)
				{
					TimeSkip.RestoreCode();
				}
            }
		}
    }
}
