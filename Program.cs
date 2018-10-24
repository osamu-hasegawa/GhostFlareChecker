using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostFlareChecker
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CheckerManager.form1 = new Form1();
            Application.Run(CheckerManager.form1);
        }

		//ThreadExceptionイベントハンドラ
		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
		    try
		    {
		    }
		    finally
		    {
		        //アプリケーションを終了する
		        Application.Exit();
		    }
		}

    }
}
