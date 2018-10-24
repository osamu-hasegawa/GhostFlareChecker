using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CameraDriver;

using System.Runtime.InteropServices;

namespace GhostFlareChecker
{
    public partial class Form2 : Form
    {
		private const Int32 WM_IPC_MESSAGE = 0x8010;
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		IntPtr callbackhandle;

		protected override void WndProc(ref Message m)
		{
			// WM_GRAPHNOTIFY
			if(DLL_MESSAGE.WM_GRAPHNOTIFY == (DLL_MESSAGE)m.Msg)
			{
			}
				// WM_ERROR
			else if(DLL_MESSAGE.WM_ERROR == (DLL_MESSAGE)m.Msg)
			{
				int error = (int)m.LParam;
//				switch((ARTCAMSDK_ERROR)error){
//					case ARTCAMSDK_ERROR.ARTCAMSDK_NOERROR :		textBox15.Text = "正常";										break;	
//					case ARTCAMSDK_ERROR.ARTCAMSDK_NOT_INITIALIZE:	textBox15.Text = "初期化されてません";						break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_DISABLEDDEVICE:	textBox15.Text = "利用不可能なデバイス";						break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CREATETHREAD:	textBox15.Text = "画像取り込み用スレッド作成に失敗";			break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CREATEWINDOW:	textBox15.Text = "ウィンドウ作成に失敗";						break;
//
//					case ARTCAMSDK_ERROR.ARTCAMSDK_OUTOFMEMORY:
//						textBox15.Text = "メモリの確保に失敗";
//						break;
//
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CAMERASET:		textBox15.Text = "カメラ（デバイス）の設定でエラー";			break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CAMERASIZE:		textBox15.Text = "カメラ（デバイス）のサイズ設定でエラー";	break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CAPTURE:			textBox15.Text = "映像取り込みで失敗";						break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_PARAM:			textBox15.Text = "引数が間違ってます";						break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_DIRECTSHOW:		textBox15.Text = "DirectShow 初期化エラー";					break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_UNSUPPORTED:		textBox15.Text = "この機能はサポートされていません";			break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_UNKNOWN:			textBox15.Text = "不明のエラー";								break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_CAPTURELOST:		textBox15.Text = "デバイスが消失";							break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_FILENOTFOUND:	textBox15.Text = "指定ファイルが見つからない";				break;
//					case ARTCAMSDK_ERROR.ARTCAMSDK_FPGASET:			textBox15.Text = "FPGAの設定でエラー";						break;
//					default:										textBox15.Text = "不明のエラー";								break;
//				}
			}
				// WM_GRAPHPAINT
			else if(DLL_MESSAGE.WM_GRAPHPAINT == (DLL_MESSAGE)m.Msg)
			{
				if(CheckerManager.form1.IsDoWork())
				{
					CheckerManager.form1.Display("BUSY !!! ");
					return;
				}

				CheckerManager.form1.GetPreviewWindow(out callbackhandle);
				PostMessage(callbackhandle, WM_IPC_MESSAGE, IntPtr.Zero, IntPtr.Zero);
				
//				CheckerManager.m_CameraController.SaveCaptureBuffer();//受けた瞬間の画像を即保存 test
//				CheckerManager.m_CameraController.CopyCaptureBuffer();//受けた瞬間、書き換えられてもよいようにバックアップにコピー test
//				CheckerManager.form1.StartTimer();
			}
			else
			{
				base.WndProc(ref m);
			}
		}

        public Form2()
        {
            InitializeComponent();
        }

        public void GetPreviewWindow(out IntPtr imagehandle, out int width, out int height)
        {
            imagehandle = this.Handle;
            width = this.Width;
            height = this.Height;
        }

        public void GetPreviewWindow(out int width, out int height)
        {
            width = this.Width;
            height = this.Height;
        }

    }
}
