using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using CameraDriver;
using System.Windows.Forms.DataVisualization.Charting;

namespace GhostFlareChecker
{
    public partial class Form1 : Form
    {

		private const Int32 WM_USER = 0x400;
		private const Int32 WM_IPC_MESSAGE = 0x8010;
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		private int m_fps_cnt = 1;
		private int m_fps_tk1 = 0;
		private int m_fps_tk2 = 0;
		private double m_fps = 0;
		private int m_fps_wid = 10;

		protected override void WndProc(ref Message m)
		{
			// WM_GRAPHPAINT
			if(DLL_MESSAGE.WM_GRAPHPAINT == (DLL_MESSAGE)m.Msg)
			{
				//FPS計算
				if(m_fps_cnt == 1)
				{
					m_fps_tk1 = Environment.TickCount;
				}
				
				if(m_fps_cnt == m_fps_wid)
				{
					m_fps_tk2 = Environment.TickCount;
					m_fps = (double)(m_fps_wid / ((m_fps_tk2 - m_fps_tk1) / 1000.0));
					m_fps_cnt = 0;
					textBox11.Text = string.Format("{0:F2}", m_fps);
				}
				m_fps_cnt++;

#if false
                Thread.Sleep(20);//タイミング調整
				PostMessage(this.Handle, WM_IPC_MESSAGE, IntPtr.Zero, IntPtr.Zero);
#else
				StartTimer();
#endif
            }
            else if(WM_IPC_MESSAGE == m.Msg)
			{
#if false
				StartTimer();
#else
//				CheckerManager.m_CameraController.SaveCaptureBuffer();//受けた瞬間の画像を即保存test-->
//				CheckerManager.m_CameraController.CopyCaptureBuffer();
	            if(CheckerManager.GetCurrentMode() == 1)//AF
	            {
					DoImageContrast();
		            textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
					return;
				}

				DoImageAnalyze();

				if(checkBox8.Checked)
				{
					CheckerManager.m_DataController.CSV_WRITE();
				}

				this.Invalidate();//ここで再描画しないと例外発生する
#endif
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		public Label[] labelmatrix;

		static int FstThreshMin = 0;
		static int FstThreshMax = 0;
		static int SndThreshMin = 0;
		static int SndThreshMax = 0;
		static int TrdThreshMin = 0;
		static int TrdThreshMax = 0;
		private int FstStepCount = 0;
		private int SndStepCount = 0;
		private int TrdStepCount = 0;

		private int m_PreviewMode = -1;
		static string strFileName = null;
		private Bitmap grabBitmap = null;
		private bool isDoWork = false;

		System.Drawing.Bitmap ok_img;
		System.Drawing.Bitmap ng_img;
		public string gShasenFileName = "";
		public string gRawFileName = "";
		public string gAnaFileName = "";
		System.Drawing.Bitmap ngbox_ok_img;
		System.Drawing.Bitmap ngbox_ng_img;

		static double gCONTRAST = 0;
		static double gArea = 0;
		static int gWhiteCount = 0;
		static int gStep = 0;
		static int AfCount = 0;
		static int gBestAddress = 0;
		static bool isFirstAF = true;

		private Bitmap localimage = null;

		public enum IMAGE_TYPE
		{
			RAW = 1,
			ANA = 2,
			CHART_CENTER = 3,
			CHART_RADIAL  = 4
		}

		
        #region Win32 API
        [FlagsAttribute]
        public enum ExecutionState : uint
        {
            // 関数が失敗した時の戻り値
            Null = 0,
            // スタンバイを抑止(Vista以降は効かない？)
            SystemRequired = 1,
            // 画面OFFを抑止
            DisplayRequired = 2,
            // 効果を永続させる。ほかオプションと併用する。
            Continuous = 0x80000000,
        }

        [DllImport("user32.dll")]
        extern static uint SendInput(
            uint nInputs,   // INPUT 構造体の数(イベント数)
            INPUT[] pInputs,   // INPUT 構造体
            int cbSize     // INPUT 構造体のサイズ
            );

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct INPUT
        {
            public int type;  // 0 = INPUT_MOUSE(デフォルト), 1 = INPUT_KEYBOARD
            public MOUSEINPUT mi;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        [StructLayout(LayoutKind.Sequential)]  // アンマネージ DLL 対応用 struct 記述宣言
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;  // amount of wheel movement
            public int dwFlags;
            public int time;  // time stamp for the event
            public IntPtr dwExtraInfo;
            // Note: struct の場合、デフォルト(パラメータなしの)コンストラクタは、
            //       言語側で定義済みで、フィールドを 0 に初期化する。
        }

        // dwFlags
        const int MOUSEEVENTF_MOVED = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;  // 左ボタン Down
        const int MOUSEEVENTF_LEFTUP = 0x0004;  // 左ボタン Up
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;  // 右ボタン Down
        const int MOUSEEVENTF_RIGHTUP = 0x0010;  // 右ボタン Up
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  // 中ボタン Down
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;  // 中ボタン Up
        const int MOUSEEVENTF_WHEEL = 0x0080;
        const int MOUSEEVENTF_XDOWN = 0x0100;
        const int MOUSEEVENTF_XUP = 0x0200;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        const int screen_length = 0x10000;  // for MOUSEEVENTF_ABSOLUTE
        [DllImport("kernel32.dll")]
        static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);
        #endregion


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

			//Form1上にForm2を割付
			CheckerManager.form2 = new Form2();
			CheckerManager.form2.TopLevel = false;

			this.groupBox4.Controls.Add(CheckerManager.form2);

			CheckerManager.form2.Show();
			CheckerManager.form2.BringToFront();

			//Form1上にForm3を割付
			CheckerManager.form3 = new Form3();
			CheckerManager.form3.TopLevel = false;

			this.groupBox5.Controls.Add(CheckerManager.form3);

			CheckerManager.form3.Show();
			CheckerManager.form3.BringToFront();
			
			CheckerManager.form3.Visible = true;
			pictureBox1.Visible = false;

            //ディスプレイの解像度取得
            var screen = System.Windows.Forms.Screen.FromControl(this);
			int width  = screen.Bounds.Width;
			int height = screen.Bounds.Height;

			//開発PCより小さな解像度のPCで動作する場合
			//解像度×拡大率1.25倍を考慮して縮小
			int original_width = 1920;
			int original_height = 1080;
			double bairitsu = 1.25;
			if(width < (original_width / bairitsu))
			{
				double tempwidth = (double)(width / (original_width / bairitsu));
				double tempwidth2 = (double)CheckerManager.form2.Width;
				double resultwidth = tempwidth2 * tempwidth;
				CheckerManager.form2.Width = (int)resultwidth;
				CheckerManager.form3.Width = (int)resultwidth;

				pictureBox1.Width = (int)resultwidth;
			}
			if(height < (original_height / bairitsu))
			{
				double tempheight = (double)(height / (original_height / bairitsu));
				double tempheight2 = (double)CheckerManager.form2.Height;
				double resulheight = tempheight2 * tempheight;
				CheckerManager.form2.Height = (int)resulheight;
				CheckerManager.form3.Height = (int)resulheight;

				pictureBox1.Height = (int)resulheight;
			}

            this.timer1.Enabled = true;//timer1 for can't sleep
			this.timer2.Enabled = false;//timer2 for image analyze
			this.timer3.Enabled = false;//timer3 for 原点から上限まで上昇方向に移動
			this.timer4.Enabled = false;//timer4
			this.timer5.Enabled = false;//timer5 no use
			this.timer6.Enabled = false;//timer6 for callback & auto focus
			this.timer7.Enabled = false;//timer7 for ng box
			this.timer8.Enabled = false;//timer8 no use

			CheckerManager.Init();

            textBox14.Text = CheckerManager.m_MotorController.button_Env_Click(sender, e);
            textBox14.Text = CheckerManager.m_MotorController.button_Open_Click(sender, e);
            textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();

			//カメラ関連設定値の読み込み start
            numericUpDown8.Value = DataController.SETDATA.waitTime;
            numericUpDown9.Value = DataController.SETDATA.brightness;
            numericUpDown10.Value = DataController.SETDATA.contrast;
            numericUpDown11.Value = DataController.SETDATA.hue;
            numericUpDown12.Value = DataController.SETDATA.saturation;
            numericUpDown13.Value = DataController.SETDATA.sharpness;

            if (DataController.SETDATA.auto == true)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex = 1;
            }

            numericUpDown14.Value = DataController.SETDATA.gamma;
            numericUpDown15.Value = DataController.SETDATA.globalGain;
            numericUpDown16.Value = DataController.SETDATA.gainRed;
            numericUpDown17.Value = DataController.SETDATA.gainGreen1;
            numericUpDown18.Value = DataController.SETDATA.gainGreen2;
            numericUpDown19.Value = DataController.SETDATA.colorGainBlue;
            numericUpDown20.Value = DataController.SETDATA.exposureTime;
            numericUpDown21.Value = DataController.SETDATA.halfClock;
			numericUpDown22.Value = DataController.SETDATA.bayerGainRed;
			numericUpDown23.Value = DataController.SETDATA.bayerGainGreen;
			numericUpDown24.Value = DataController.SETDATA.bayerGainBlue;

			numericUpDown1.Value = DataController.SETDATA.MonitorColorMode;

			numericUpDown38.Value = DataController.SETDATA.Fps;

			textBox29.Text = DataController.SETDATA.width.ToString();
			textBox30.Text = DataController.SETDATA.height.ToString();

			numericUpDown44.Value = DataController.SETDATA.af_left;
			numericUpDown45.Value = DataController.SETDATA.af_top;
			numericUpDown46.Value = DataController.SETDATA.af_right;
			numericUpDown47.Value = DataController.SETDATA.af_buttom;

			numericUpDown196.Value = DataController.SETDATA.AfThreshMin;
			numericUpDown195.Value = DataController.SETDATA.AfThreshMax;

			//カメラ関連設定値の読み込み end

			//モーター関連設定値の読み込み start
			numericUpDown28.Value = DataController.SETDATA.Unit[0];
			numericUpDown29.Value = DataController.SETDATA.Axis[0];
			numericUpDown30.Value = DataController.SETDATA.Origin[0];
			numericUpDown31.Value = DataController.SETDATA.ORG_drv[0];
			numericUpDown32.Value = DataController.SETDATA.Jspd[0];
			numericUpDown37.Value = DataController.SETDATA.Hspd[0];
			numericUpDown36.Value = DataController.SETDATA.Lspd[0];
			numericUpDown35.Value = DataController.SETDATA.StdPLS[0];
			numericUpDown34.Value = DataController.SETDATA.L_LimitPLS[0];
			numericUpDown33.Value = DataController.SETDATA.U_LimitPLS[0];

			numericUpDown191.Value = DataController.SETDATA.defaultStep;
			numericUpDown177.Value = DataController.SETDATA.limit_1;
			numericUpDown179.Value = DataController.SETDATA.limit_2;
			numericUpDown180.Value = DataController.SETDATA.limit_3;
			numericUpDown181.Value = DataController.SETDATA.limit_4;
			numericUpDown182.Value = DataController.SETDATA.limit_5;
			numericUpDown183.Value = DataController.SETDATA.limit_6;
			numericUpDown184.Value = DataController.SETDATA.limit_7;
			numericUpDown185.Value = DataController.SETDATA.step_1_2;
			numericUpDown186.Value = DataController.SETDATA.step_2_3;
			numericUpDown187.Value = DataController.SETDATA.step_3_4;
			numericUpDown188.Value = DataController.SETDATA.step_4_5;
			numericUpDown189.Value = DataController.SETDATA.step_5_6;
			numericUpDown190.Value = DataController.SETDATA.step_6_7;
			//モーター関連設定値の読み込み end


			//画像関連設定値の読み込み start
			numericUpDown2.Value = DataController.SETDATA.sMin;
			numericUpDown5.Value = DataController.SETDATA.sMax;
			numericUpDown3.Value = DataController.SETDATA.lMin;
            numericUpDown6.Value = DataController.SETDATA.lMax;

			numericUpDown25.Value = FstThreshMin = DataController.SETDATA.FstThreshMin;
			numericUpDown26.Value = FstThreshMax = DataController.SETDATA.FstThreshMax;
			numericUpDown27.Value = FstStepCount = DataController.SETDATA.FstStepCount;
			numericUpDown48.Value = SndThreshMin = DataController.SETDATA.SndThreshMin;
			numericUpDown49.Value = SndThreshMax = DataController.SETDATA.SndThreshMax;
			numericUpDown50.Value = SndStepCount = DataController.SETDATA.SndStepCount;
			numericUpDown192.Value = TrdThreshMin = DataController.SETDATA.TrdThreshMin;
			numericUpDown193.Value = TrdThreshMax = DataController.SETDATA.TrdThreshMax;
			numericUpDown194.Value = TrdStepCount = DataController.SETDATA.TrdStepCount;

            numericUpDown39.Value = DataController.SETDATA.distanceThresh;
            numericUpDown40.Value = DataController.SETDATA.center_x_Thresh;
            numericUpDown43.Value = DataController.SETDATA.center_y_Thresh;

            numericUpDown41.Value = DataController.SETDATA.center_x_hosei;
            numericUpDown42.Value = DataController.SETDATA.center_y_hosei;
			//画像関連設定値の読み込み end

			textBox32.Text = CheckerManager.form2.Width.ToString();
			textBox33.Text = CheckerManager.form2.Height.ToString();

			//OK/NG　imageの読み込み
			string stCurrentDir = System.IO.Directory.GetCurrentDirectory();
			string okFileDir = stCurrentDir + "\\" + "OK.JPG";
			string ngFileDir = stCurrentDir + "\\" + "NG.JPG";
			
			ok_img = (Bitmap)System.Drawing.Image.FromFile(okFileDir);
			ng_img = (Bitmap)System.Drawing.Image.FromFile(ngFileDir);
			pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

			//NG BOX imageの読み込み
			string ngbox_okFileDir = stCurrentDir + "\\" + "NGBOX_OK.JPG";
			string ngbox_ngFileDir = stCurrentDir + "\\" + "NGBOX_NG.PNG";
			
			ngbox_ok_img = (Bitmap)System.Drawing.Image.FromFile(ngbox_okFileDir);
			ngbox_ng_img = (Bitmap)System.Drawing.Image.FromFile(ngbox_ngFileDir);
			pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

		    textBox15.ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
			textBox15.BackColor = SystemColors.Control;
			textBox15.Text = "レンズを置いて[測　定]を押下して下さい";

			//LEDの光源座標の読み込み
			numericUpDown51.Value = CheckerManager.m_DataController.ledtable[0].center_x;
			numericUpDown52.Value = CheckerManager.m_DataController.ledtable[0].center_y;
			numericUpDown53.Value = CheckerManager.m_DataController.ledtable[1].center_x;
			numericUpDown54.Value = CheckerManager.m_DataController.ledtable[1].center_y;
			numericUpDown55.Value = CheckerManager.m_DataController.ledtable[2].center_x;
			numericUpDown56.Value = CheckerManager.m_DataController.ledtable[2].center_y;
			numericUpDown57.Value = CheckerManager.m_DataController.ledtable[3].center_x;
			numericUpDown58.Value = CheckerManager.m_DataController.ledtable[3].center_y;
			numericUpDown59.Value = CheckerManager.m_DataController.ledtable[4].center_x;
			numericUpDown60.Value = CheckerManager.m_DataController.ledtable[4].center_y;
			numericUpDown61.Value = CheckerManager.m_DataController.ledtable[5].center_x;
			numericUpDown62.Value = CheckerManager.m_DataController.ledtable[5].center_y;
			numericUpDown63.Value = CheckerManager.m_DataController.ledtable[6].center_x;
			numericUpDown64.Value = CheckerManager.m_DataController.ledtable[6].center_y;
			numericUpDown65.Value = CheckerManager.m_DataController.ledtable[7].center_x;
			numericUpDown66.Value = CheckerManager.m_DataController.ledtable[7].center_y;
			numericUpDown82.Value = CheckerManager.m_DataController.ledtable[8].center_x;
			numericUpDown81.Value = CheckerManager.m_DataController.ledtable[8].center_y;
			numericUpDown79.Value = CheckerManager.m_DataController.ledtable[9].center_x;
			numericUpDown80.Value = CheckerManager.m_DataController.ledtable[9].center_y;
			numericUpDown77.Value = CheckerManager.m_DataController.ledtable[10].center_x;
			numericUpDown78.Value = CheckerManager.m_DataController.ledtable[10].center_y;
			numericUpDown75.Value = CheckerManager.m_DataController.ledtable[11].center_x;
			numericUpDown76.Value = CheckerManager.m_DataController.ledtable[11].center_y;
			numericUpDown73.Value = CheckerManager.m_DataController.ledtable[12].center_x;
			numericUpDown74.Value = CheckerManager.m_DataController.ledtable[12].center_y;
			numericUpDown71.Value = CheckerManager.m_DataController.ledtable[13].center_x;
			numericUpDown72.Value = CheckerManager.m_DataController.ledtable[13].center_y;
			numericUpDown69.Value = CheckerManager.m_DataController.ledtable[14].center_x;
			numericUpDown70.Value = CheckerManager.m_DataController.ledtable[14].center_y;
			numericUpDown67.Value = CheckerManager.m_DataController.ledtable[15].center_x;
			numericUpDown68.Value = CheckerManager.m_DataController.ledtable[15].center_y;
			numericUpDown83.Value = CheckerManager.m_DataController.ledtable[16].center_x;
			numericUpDown84.Value = CheckerManager.m_DataController.ledtable[16].center_y;
			numericUpDown85.Value = CheckerManager.m_DataController.ledtable[17].center_x;
			numericUpDown86.Value = CheckerManager.m_DataController.ledtable[17].center_y;
			numericUpDown103.Value = CheckerManager.m_DataController.ledtable[18].center_x;
			numericUpDown104.Value = CheckerManager.m_DataController.ledtable[18].center_y;
			numericUpDown101.Value = CheckerManager.m_DataController.ledtable[19].center_x;
			numericUpDown102.Value = CheckerManager.m_DataController.ledtable[19].center_y;
			numericUpDown99.Value = CheckerManager.m_DataController.ledtable[20].center_x;
			numericUpDown100.Value = CheckerManager.m_DataController.ledtable[20].center_y;
			numericUpDown97.Value = CheckerManager.m_DataController.ledtable[21].center_x;
			numericUpDown98.Value = CheckerManager.m_DataController.ledtable[21].center_y;
			numericUpDown95.Value = CheckerManager.m_DataController.ledtable[22].center_x;
			numericUpDown96.Value = CheckerManager.m_DataController.ledtable[22].center_y;
			numericUpDown93.Value = CheckerManager.m_DataController.ledtable[23].center_x;
			numericUpDown94.Value = CheckerManager.m_DataController.ledtable[23].center_y;
			numericUpDown91.Value = CheckerManager.m_DataController.ledtable[24].center_x;
			numericUpDown92.Value = CheckerManager.m_DataController.ledtable[24].center_y;
			numericUpDown89.Value = CheckerManager.m_DataController.ledtable[25].center_x;
			numericUpDown90.Value = CheckerManager.m_DataController.ledtable[25].center_y;
			numericUpDown87.Value = CheckerManager.m_DataController.ledtable[26].center_x;
			numericUpDown88.Value = CheckerManager.m_DataController.ledtable[26].center_y;
			numericUpDown121.Value = CheckerManager.m_DataController.ledtable[27].center_x;
			numericUpDown122.Value = CheckerManager.m_DataController.ledtable[27].center_y;
			numericUpDown119.Value = CheckerManager.m_DataController.ledtable[28].center_x;
			numericUpDown120.Value = CheckerManager.m_DataController.ledtable[28].center_y;
			numericUpDown117.Value = CheckerManager.m_DataController.ledtable[29].center_x;
			numericUpDown118.Value = CheckerManager.m_DataController.ledtable[29].center_y;
			numericUpDown115.Value = CheckerManager.m_DataController.ledtable[30].center_x;
			numericUpDown116.Value = CheckerManager.m_DataController.ledtable[30].center_y;
			numericUpDown113.Value = CheckerManager.m_DataController.ledtable[31].center_x;
			numericUpDown114.Value = CheckerManager.m_DataController.ledtable[31].center_y;
			numericUpDown111.Value = CheckerManager.m_DataController.ledtable[32].center_x;
			numericUpDown112.Value = CheckerManager.m_DataController.ledtable[32].center_y;
			numericUpDown109.Value = CheckerManager.m_DataController.ledtable[33].center_x;
			numericUpDown110.Value = CheckerManager.m_DataController.ledtable[33].center_y;
			numericUpDown107.Value = CheckerManager.m_DataController.ledtable[34].center_x;
			numericUpDown108.Value = CheckerManager.m_DataController.ledtable[34].center_y;
			numericUpDown105.Value = CheckerManager.m_DataController.ledtable[35].center_x;
			numericUpDown106.Value = CheckerManager.m_DataController.ledtable[35].center_y;
			numericUpDown139.Value = CheckerManager.m_DataController.ledtable[36].center_x;
			numericUpDown140.Value = CheckerManager.m_DataController.ledtable[36].center_y;
			numericUpDown137.Value = CheckerManager.m_DataController.ledtable[37].center_x;
			numericUpDown138.Value = CheckerManager.m_DataController.ledtable[37].center_y;
			numericUpDown135.Value = CheckerManager.m_DataController.ledtable[38].center_x;
			numericUpDown136.Value = CheckerManager.m_DataController.ledtable[38].center_y;
			numericUpDown133.Value = CheckerManager.m_DataController.ledtable[39].center_x;
			numericUpDown134.Value = CheckerManager.m_DataController.ledtable[39].center_y;
			numericUpDown131.Value = CheckerManager.m_DataController.ledtable[40].center_x;
			numericUpDown132.Value = CheckerManager.m_DataController.ledtable[40].center_y;
			numericUpDown129.Value = CheckerManager.m_DataController.ledtable[41].center_x;
			numericUpDown130.Value = CheckerManager.m_DataController.ledtable[41].center_y;
			numericUpDown127.Value = CheckerManager.m_DataController.ledtable[42].center_x;
			numericUpDown128.Value = CheckerManager.m_DataController.ledtable[42].center_y;
			numericUpDown125.Value = CheckerManager.m_DataController.ledtable[43].center_x;
			numericUpDown126.Value = CheckerManager.m_DataController.ledtable[43].center_y;
			numericUpDown123.Value = CheckerManager.m_DataController.ledtable[44].center_x;
			numericUpDown124.Value = CheckerManager.m_DataController.ledtable[44].center_y;
			numericUpDown157.Value = CheckerManager.m_DataController.ledtable[45].center_x;
			numericUpDown158.Value = CheckerManager.m_DataController.ledtable[45].center_y;
			numericUpDown155.Value = CheckerManager.m_DataController.ledtable[46].center_x;
			numericUpDown156.Value = CheckerManager.m_DataController.ledtable[46].center_y;
			numericUpDown153.Value = CheckerManager.m_DataController.ledtable[47].center_x;
			numericUpDown154.Value = CheckerManager.m_DataController.ledtable[47].center_y;
			numericUpDown151.Value = CheckerManager.m_DataController.ledtable[48].center_x;
			numericUpDown152.Value = CheckerManager.m_DataController.ledtable[48].center_y;
			numericUpDown149.Value = CheckerManager.m_DataController.ledtable[49].center_x;
			numericUpDown150.Value = CheckerManager.m_DataController.ledtable[49].center_y;
			numericUpDown147.Value = CheckerManager.m_DataController.ledtable[50].center_x;
			numericUpDown148.Value = CheckerManager.m_DataController.ledtable[50].center_y;
			numericUpDown145.Value = CheckerManager.m_DataController.ledtable[51].center_x;
			numericUpDown146.Value = CheckerManager.m_DataController.ledtable[51].center_y;
			numericUpDown143.Value = CheckerManager.m_DataController.ledtable[52].center_x;
			numericUpDown144.Value = CheckerManager.m_DataController.ledtable[52].center_y;
			numericUpDown141.Value = CheckerManager.m_DataController.ledtable[53].center_x;
			numericUpDown142.Value = CheckerManager.m_DataController.ledtable[53].center_y;
			numericUpDown175.Value = CheckerManager.m_DataController.ledtable[54].center_x;
			numericUpDown176.Value = CheckerManager.m_DataController.ledtable[54].center_y;
			numericUpDown173.Value = CheckerManager.m_DataController.ledtable[55].center_x;
			numericUpDown174.Value = CheckerManager.m_DataController.ledtable[55].center_y;
			numericUpDown171.Value = CheckerManager.m_DataController.ledtable[56].center_x;
			numericUpDown172.Value = CheckerManager.m_DataController.ledtable[56].center_y;
			numericUpDown169.Value = CheckerManager.m_DataController.ledtable[57].center_x;
			numericUpDown170.Value = CheckerManager.m_DataController.ledtable[57].center_y;
			numericUpDown167.Value = CheckerManager.m_DataController.ledtable[58].center_x;
			numericUpDown168.Value = CheckerManager.m_DataController.ledtable[58].center_y;
			numericUpDown165.Value = CheckerManager.m_DataController.ledtable[59].center_x;
			numericUpDown166.Value = CheckerManager.m_DataController.ledtable[59].center_y;
			numericUpDown163.Value = CheckerManager.m_DataController.ledtable[60].center_x;
			numericUpDown164.Value = CheckerManager.m_DataController.ledtable[60].center_y;
			numericUpDown161.Value = CheckerManager.m_DataController.ledtable[61].center_x;
			numericUpDown162.Value = CheckerManager.m_DataController.ledtable[61].center_y;
			numericUpDown159.Value = CheckerManager.m_DataController.ledtable[62].center_x;
			numericUpDown160.Value = CheckerManager.m_DataController.ledtable[62].center_y;
			//LEDの光源座標の読み込み


			//LEDの光源面積閾値の読み込み
			numericUpDown199.Value = DataController.SETDATA.led_area_limit[0];
			numericUpDown200.Value = DataController.SETDATA.led_area_limit[1];
			numericUpDown201.Value = DataController.SETDATA.led_area_limit[2];
			numericUpDown202.Value = DataController.SETDATA.led_area_limit[3];
			numericUpDown203.Value = DataController.SETDATA.led_area_limit[4];
			numericUpDown204.Value = DataController.SETDATA.led_area_limit[5];
			numericUpDown205.Value = DataController.SETDATA.led_area_limit[6];
			numericUpDown206.Value = DataController.SETDATA.led_area_limit[7];
			numericUpDown207.Value = DataController.SETDATA.led_area_limit[8];
			numericUpDown208.Value = DataController.SETDATA.led_area_limit[9];
			numericUpDown209.Value = DataController.SETDATA.led_area_limit[10];
			numericUpDown210.Value = DataController.SETDATA.led_area_limit[11];
			numericUpDown211.Value = DataController.SETDATA.led_area_limit[12];
			numericUpDown212.Value = DataController.SETDATA.led_area_limit[13];
			numericUpDown213.Value = DataController.SETDATA.led_area_limit[14];
			numericUpDown214.Value = DataController.SETDATA.led_area_limit[15];
			numericUpDown215.Value = DataController.SETDATA.led_area_limit[16];
			numericUpDown216.Value = DataController.SETDATA.led_area_limit[17];
			numericUpDown217.Value = DataController.SETDATA.led_area_limit[18];
			numericUpDown218.Value = DataController.SETDATA.led_area_limit[19];
			numericUpDown219.Value = DataController.SETDATA.led_area_limit[20];
			numericUpDown220.Value = DataController.SETDATA.led_area_limit[21];
			numericUpDown221.Value = DataController.SETDATA.led_area_limit[22];
			numericUpDown222.Value = DataController.SETDATA.led_area_limit[23];
			numericUpDown223.Value = DataController.SETDATA.led_area_limit[24];
			numericUpDown224.Value = DataController.SETDATA.led_area_limit[25];
			numericUpDown225.Value = DataController.SETDATA.led_area_limit[26];
			numericUpDown226.Value = DataController.SETDATA.led_area_limit[27];
			numericUpDown227.Value = DataController.SETDATA.led_area_limit[28];
			numericUpDown228.Value = DataController.SETDATA.led_area_limit[29];
			numericUpDown229.Value = DataController.SETDATA.led_area_limit[30];
			numericUpDown230.Value = DataController.SETDATA.led_area_limit[31];
			numericUpDown231.Value = DataController.SETDATA.led_area_limit[32];
			numericUpDown232.Value = DataController.SETDATA.led_area_limit[33];
			numericUpDown233.Value = DataController.SETDATA.led_area_limit[34];
			numericUpDown234.Value = DataController.SETDATA.led_area_limit[35];
			numericUpDown235.Value = DataController.SETDATA.led_area_limit[36];
			numericUpDown236.Value = DataController.SETDATA.led_area_limit[37];
			numericUpDown237.Value = DataController.SETDATA.led_area_limit[38];
			numericUpDown238.Value = DataController.SETDATA.led_area_limit[39];
			numericUpDown239.Value = DataController.SETDATA.led_area_limit[40];
			numericUpDown240.Value = DataController.SETDATA.led_area_limit[41];
			numericUpDown241.Value = DataController.SETDATA.led_area_limit[42];
			numericUpDown242.Value = DataController.SETDATA.led_area_limit[43];
			numericUpDown243.Value = DataController.SETDATA.led_area_limit[44];
			numericUpDown244.Value = DataController.SETDATA.led_area_limit[45];
			numericUpDown245.Value = DataController.SETDATA.led_area_limit[46];
			numericUpDown246.Value = DataController.SETDATA.led_area_limit[47];
			numericUpDown247.Value = DataController.SETDATA.led_area_limit[48];
			numericUpDown248.Value = DataController.SETDATA.led_area_limit[49];
			numericUpDown249.Value = DataController.SETDATA.led_area_limit[50];
			numericUpDown250.Value = DataController.SETDATA.led_area_limit[51];
			numericUpDown251.Value = DataController.SETDATA.led_area_limit[52];
			numericUpDown252.Value = DataController.SETDATA.led_area_limit[53];
			numericUpDown253.Value = DataController.SETDATA.led_area_limit[54];
			numericUpDown254.Value = DataController.SETDATA.led_area_limit[55];
			numericUpDown255.Value = DataController.SETDATA.led_area_limit[56];
			numericUpDown256.Value = DataController.SETDATA.led_area_limit[57];
			numericUpDown257.Value = DataController.SETDATA.led_area_limit[58];
			numericUpDown258.Value = DataController.SETDATA.led_area_limit[59];
			numericUpDown259.Value = DataController.SETDATA.led_area_limit[60];
			numericUpDown260.Value = DataController.SETDATA.led_area_limit[61];
			numericUpDown261.Value = DataController.SETDATA.led_area_limit[62];
			//LEDの光源面積閾値の読み込み

			numericUpDown262.Value = DataController.SETDATA.lineFlareThresh;//線状フレア判定に使用：光源中心からの最大半径ー最小半径の差

			numericUpDown197.Value = DataController.SETDATA.center_x_zure;
			numericUpDown198.Value = DataController.SETDATA.center_y_zure;

			numericUpDown178.Value = DataController.SETDATA.circleArea;

			initResultLabel();

			CheckerManager.m_MotorController.LedReset();//LEDリセット

            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                label48.Text = "外形面積";
                label74.Text = "最少外形面積";
            }
            else
            {
                radioButton2.Checked = true;
                label48.Text = "白色数";
                label74.Text = "最少白色数";
            }

//Setting.xml初回書き込み用
#if false
#endif

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
			CheckerManager.Close();

			m_PreviewMode = -1;
//			m_StopFlg = false;
        }

        private void button27_Click(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            //画面暗転阻止
            SetThreadExecutionState(ExecutionState.DisplayRequired);

            // ドラッグ操作の準備 (struct 配列の宣言)
            INPUT[] input = new INPUT[1];  // イベントを格納

            // ドラッグ操作の準備 (イベントの定義 = 相対座標へ移動)
            input[0].mi.dx = 0;  // 相対座標で0　つまり動かさない
            input[0].mi.dy = 0;  // 相対座標で0 つまり動かさない
            input[0].mi.dwFlags = MOUSEEVENTF_MOVED;

            // ドラッグ操作の実行 (イベントの生成)
            SendInput(1, input, Marshal.SizeOf(input[0]));

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
			this.button1.Enabled = false;
			this.button40.Enabled = false;

//			pictureBox3.Image = ok_img;
//			textBox31.Text = "";
			textBox41.Text = "";

			CheckerManager.m_DataController.SetCircleColor();

			Bitmap keepImage = null;
			//1番目の範囲の閾値
			while(FstThreshMin < FstThreshMax)
//			while(threshMin < threshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
				int totalContour;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(FstThreshMin, FstThreshMax, null, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;
#if false
				CheckerManager.form3.BackgroundImage = m_bmpZ;
				CheckerManager.form3.ClientSize = m_bmpZ.Size;
#else
				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}
#endif
				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

#if false
				textBox9.Text = threshMin.ToString();
				textBox26.Text = string.Format("{0:f16}", CONTRAST);
#endif
	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);
				
	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            FstThreshMin += FstStepCount;
	        }

			//2番目の範囲の閾値
			while(SndThreshMin < SndThreshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
				int totalContour;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(SndThreshMin, SndThreshMax, null, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;

				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}

				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);
				
	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            SndThreshMin += SndStepCount;
	        }

			//3番目の範囲の閾値
			while(TrdThreshMin < TrdThreshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
				int totalContour;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(TrdThreshMin, TrdThreshMax, null, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;

				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}

				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);
				
	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            TrdThreshMin += TrdStepCount;
	        }

			CheckerManager.form3.BackgroundImage = keepImage;
			CheckerManager.form3.ClientSize = keepImage.Size;

			CheckerManager.m_ImageController.GetCenterFromMaxBrightness(keepImage);//TODO　この場所は暫定

			this.timer2.Enabled = false;
			this.button1.Enabled = true;
			this.button40.Enabled = true;

			if(checkBox7.Checked)
			{	
				gAnaFileName = CheckerManager.m_CameraController.SaveImageFile((Bitmap)CheckerManager.form3.BackgroundImage, (int)IMAGE_TYPE.ANA);
			}

			int total = CheckerManager.m_ImageController.GetCenterCount();
			if(total > 0)
			{
				DrawInfoChart(total);
			}

			if(checkBox8.Checked)
			{
				if(total > 0)
				{
					CheckerManager.m_DataController.CSV_WRITE();
				}
			}

        }

        private void button26_Click(object sender, EventArgs e)
        {
			m_PreviewMode = 0;
//			m_StopFlg = false;

			CheckerManager.m_MotorController.LedColumnLightOn();
			CheckerManager.m_CameraController.Preview_Click(sender, e);
        }

        private void button28_Click(object sender, EventArgs e)//Camera Capture
        {
			m_PreviewMode = 1;
//			m_StopFlg = false;

			button28.Enabled = false;
			button58.Enabled = false;
			button26.Enabled = false;
			button33.Enabled = false;

			button1.Enabled = false;
			button40.Enabled = false;

			pictureBox4.Image = null;
			pictureBox3.Image = null;

			textBox3.Text = "";
			textBox4.Text = "";
			textBox9.Text = "";

		    textBox15.ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
			textBox15.BackColor = SystemColors.Control;
			textBox15.Text = "GHOST / FLARE" + "\r\n" + "検査中...";

			CheckerManager.m_DataController.SetLedInit();
			clearLabel();

            CheckerManager.m_CameraController.Callback_Click(sender, e, this.checkBox5.Checked);//キャプチャ開始
            CheckerManager.SetCurrentMode(2);//2:Camera Grabbing

#if true
			CheckerManager.m_MotorController.LedLightOn();//LED点灯開始
#else
			CheckerManager.m_MotorController.LedColumnLightOn//LED行毎点灯開始
#endif
        }

        private void button32_Click(object sender, EventArgs e)//STOP
        {
//test start
//CheckerManager.m_DataController.CSV_LED_RESULT_WRITE();
//test end

			this.timer2.Enabled = false;
			this.timer4.Enabled = false;
			this.timer6.Enabled = false;
			this.timer8.Enabled = false;

			button28.Enabled = true;
			button58.Enabled = true;
			button26.Enabled = true;
			button33.Enabled = true;

			button1.Enabled = true;
			button40.Enabled = true;

			// 動画の時のみ止める
			if(0 == m_PreviewMode || 1 == m_PreviewMode)
			{
				CheckerManager.m_CameraController.StopPreview();
				CheckerManager.m_MotorController.LedReset();//LEDリセット
			}

        }

        private void StopCapture()//STOP
        {
			this.timer2.Enabled = false;
			this.timer4.Enabled = false;
			this.timer6.Enabled = false;
			this.timer8.Enabled = false;

			button28.Enabled = true;
			button58.Enabled = true;
			button26.Enabled = true;
			button33.Enabled = true;

			button1.Enabled = true;
			button40.Enabled = true;

			// 動画の時のみ止める
			if(0 == m_PreviewMode || 1 == m_PreviewMode)
			{
				CheckerManager.m_CameraController.StopPreview();
				CheckerManager.m_MotorController.LedReset();//LEDリセット
			}

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
			timer3.Enabled = false;
			CheckerManager.m_MotorController.button_CwIndex_Click(sender, e);
			CheckerManager.SetAfMotion(2);

			double minValue;
			int minTotal;
			bool isAf;
			int bestAddress = 0;
			if(radioButton1.Checked)
			{
				//最外形が最小面積となる位置を求める場合
				bestAddress = CheckerManager.m_ImageController.GetAfBestAddress(AfCount, 1, out minValue, out minTotal, out isAf);//1:面積
				textBox2.Text = string.Format("{0:f2}", minValue);//最外形の最小面積
			}
			else
			{
				//中央矩形部の白色部(0xFF)が最少となる位置を求める場合
				bestAddress = CheckerManager.m_ImageController.GetAfBestAddress(AfCount, 2, out minValue, out minTotal, out isAf);//2:白色数
				textBox2.Text = minTotal.ToString();
			}

			if(isFirstAF)
			{
				gBestAddress = bestAddress;
				isFirstAF = false;
			}

			if(isAf)//AF成功
			{
				textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
				int currentAddress = CheckerManager.m_MotorController.GetCurrentAddress();

				int finalStep = 0;
#if false
				if(System.Math.Abs(gBestAddress - bestAddress) > 100)//初回との差分が100を超えている時。100は任意
				{
					finalStep = currentAddress - gBestAddress;//初回のベスト位置に移動。ガード
				}
				else
				{
					finalStep = currentAddress - bestAddress;
				}
#else
				finalStep = currentAddress - bestAddress;
#endif

				finalStep *= -1;
				bool isResult;
				CheckerManager.m_MotorController.StepDrive(finalStep, out isResult);

				button32_Click(sender, e);//キャプチャ停止

                textBox15.ForeColor = Color.ForestGreen;
				textBox15.BackColor = SystemColors.Control;
				textBox15.Text = "AF成功。自動検査を開始します";
				
				Thread.Sleep(50);

				button28_Click(sender, e);//検査開始
			}
			else//AF失敗
			{
				button32_Click(sender, e);//キャプチャ停止

			    textBox15.ForeColor = Color.FromArgb(0xFF, 0x00, 0x00);
				textBox15.BackColor = SystemColors.Control;
				textBox15.Text = "AF失敗！。再度AFを実行して下さい";
			}
        }

        private void button33_Click(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SaveImageFile();
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetBrightness((int)numericUpDown9.Value);
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetContrast((int)numericUpDown10.Value);
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetHue((int)numericUpDown11.Value);
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetSaturation((int)numericUpDown12.Value);
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetSharpness((int)numericUpDown13.Value);
        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetGamma((int)numericUpDown14.Value);
        }

        private void numericUpDown15_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetGlobalGain((int)numericUpDown15.Value);
        }

        private void numericUpDown16_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorGainRed((int)numericUpDown16.Value);
        }

        private void numericUpDown17_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorGainGreen1((int)numericUpDown17.Value);
        }

        private void numericUpDown18_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorGainGreen2((int)numericUpDown18.Value);
        }

        private void numericUpDown19_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorGainBlue((int)numericUpDown19.Value);
        }

        private void numericUpDown20_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetExposureTime((int)numericUpDown20.Value);
        }

        private void numericUpDown21_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetHalfClock((int)numericUpDown21.Value);
        }

        private void numericUpDown22_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetBayerGainRed((int)numericUpDown22.Value);
        }

        private void numericUpDown23_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetBayerGainGreen((int)numericUpDown23.Value);
        }

        private void numericUpDown24_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetBayerGainBlue((int)numericUpDown24.Value);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
			int i = comboBox1.SelectedIndex;
			bool autoData = false;
			if(i == 0)
			{
				autoData = true;
			}
			CheckerManager.m_CameraController.SetBayerGainAuto(autoData);
        }

        private void button50_Click(object sender, EventArgs e)
        {
        }

        private void button54_Click(object sender, EventArgs e)
        {
        }

        private void button58_Click(object sender, EventArgs e)//Auto Focus
        {
			m_PreviewMode = 1;
//			m_StopFlg = false;

			button28.Enabled = false;
			button58.Enabled = false;
			button26.Enabled = false;
			button33.Enabled = false;

			button1.Enabled = false;
			button40.Enabled = false;

		    textBox15.ForeColor = Color.FromArgb(0x00, 0x00, 0xFF);
			textBox15.BackColor = SystemColors.Control;
			textBox15.Text = "Auto Focussing...";
			pictureBox4.Image = null;

			AfCount = 0;

//			addressCount = 0;

			textBox2.Text = "";
			textBox26.Text = "";

			for(int i = 0; i < 1024; i++)//ゼロ初期化
			{
				CheckerManager.m_DataController.af_info[i].step = 0;
				CheckerManager.m_DataController.af_info[i].address = 0;
				CheckerManager.m_DataController.af_info[i].pulse = 0;
				CheckerManager.m_DataController.af_info[i].contrast = 0;
				CheckerManager.m_DataController.af_info[i].menseki = 0;
				CheckerManager.m_DataController.af_info[i].shuicho = 0;
				CheckerManager.m_DataController.af_info[i].enkeido = 0;
			}

            CheckerManager.m_CameraController.Callback_Click(sender, e, this.checkBox5.Checked);
            CheckerManager.SetCurrentMode(1);//Auto Focus

			CheckerManager.m_MotorController.LedUnitLightOn();//LED単体の点灯

			CheckerManager.SetAfMotion(0);
#if false
			if(CheckerManager.IsOriginBack())//初回以降、10回毎
			{
				button3_Click_2(sender, e);//一旦原点へ移動
			}
			else
			{
				bool isResult;
				CheckerManager.m_MotorController.StepDrive(-2000, out isResult);//少し戻る
			}
			CheckerManager.SetAfCount();
#else
			button3_Click_2(sender, e);//一旦原点へ移動
#endif
			CheckerManager.SetAfMotion(1);
			timer3.Enabled = true;

        }

        private void button62_Click(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.deviceClose();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
			button28.Enabled = true;
			button58.Enabled = true;
			button26.Enabled = true;
			button33.Enabled = true;

			bool isResult;
			CheckerManager.m_MotorController.StepDrive(gStep, out isResult);
			timer4.Enabled = false;

			textBox26.Text = string.Format("{0:f2}", gArea);//最外形の面積

			textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
			textBox15.Text = "AF終了。[CHECK START]して下さい";

			CheckerManager.m_MotorController.LedReset();//LEDリセット
			button32_Click(sender, e);
		}

		public void DoImageContrast()
		{
			isDoWork = true;

			if(grabBitmap != null)
			{
				grabBitmap.Dispose();
			}
			
			grabBitmap = CheckerManager.m_CameraController.GetImage();

			CheckerManager.m_ImageController.SetImageFile("");

			int thMin = 128;//like uSCOPE
			int thMax = 254;//like uSCOPE

			gCONTRAST = CheckerManager.m_ImageController.GetImageContrast(thMin, thMax, CheckerManager.form3.Width, CheckerManager.form3.Height, grabBitmap, 
			(int)numericUpDown44.Value, (int)numericUpDown45.Value, (int)numericUpDown46.Value, (int)numericUpDown47.Value);
			textBox26.Text = string.Format("{0:f16}", gCONTRAST);

			isDoWork = false;
		}


		public void DoImageShapeLine()
		{
			isDoWork = true;

			if(grabBitmap != null)
			{
				grabBitmap.Dispose();
			}
			
			grabBitmap = CheckerManager.m_CameraController.GetImage();

			CheckerManager.m_ImageController.SetImageFile("");

            CheckerManager.m_ImageController.SetLimitForAnalysis(
                                        (int)numericUpDown2.Value,
                                        (int)numericUpDown5.Value,
                                        (int)numericUpDown3.Value,
                                        (int)numericUpDown6.Value,
                                        (int)numericUpDown4.Value,
                                        (int)numericUpDown7.Value);

			int thMin = DataController.SETDATA.AfThreshMin;
			int thMax = DataController.SETDATA.AfThreshMax;

			double s, l, c;
			int x, y;
			Bitmap graymap = CheckerManager.m_ImageController.GetShapeLineInfo(thMin, thMax, grabBitmap, out s, out l, out c, out x, out y, checkBox3.Checked);
			gArea = s;//最外形の面積取得

			//最外形内の白部分を計算
			int xx = (int)numericUpDown44.Value;
			int yy = (int)numericUpDown45.Value;
			int wide = (int)(numericUpDown46.Value - numericUpDown44.Value);
			int height = (int)(numericUpDown47.Value - numericUpDown45.Value);
			gWhiteCount = CheckerManager.m_ImageController.GetDataInScopeArea(graymap, xx, yy, wide, height);

			if(radioButton1.Checked)
			{
				textBox26.Text = string.Format("{0:f2}", gArea);//最外形の面積
			}
			else
			{
				textBox26.Text = gWhiteCount.ToString();//白色部分の数
			}

			Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;
			CheckerManager.form3.BackgroundImage = graymap;
			if(oldImage != null)
			{
				oldImage.Dispose();
				oldImage = null;
			}


			isDoWork = false;
		}

		public void DoImageAnalyze()
		{
			isDoWork = true;

			if(grabBitmap != null)
			{
				grabBitmap.Dispose();
			}
			
			grabBitmap = CheckerManager.m_CameraController.GetImage();

			CheckerManager.m_DataController.InitLogData();
			CheckerManager.m_ImageController.GetCenterFromMaxBrightness(grabBitmap);

			CheckerManager.m_ImageController.SetImageFile("");

			CheckerManager.m_ImageController.SetLimitForAnalysis(
										(int)numericUpDown2.Value, 
										(int)numericUpDown5.Value, 
										(int)numericUpDown3.Value, 
										(int)numericUpDown6.Value, 
										(int)numericUpDown4.Value, 
										(int)numericUpDown7.Value);

			FstThreshMin = (int)this.numericUpDown25.Value;
			FstThreshMax = (int)this.numericUpDown26.Value;
			SndThreshMin = (int)this.numericUpDown48.Value;
			SndThreshMax = (int)this.numericUpDown49.Value;
			TrdThreshMin = (int)this.numericUpDown192.Value;
			TrdThreshMax = (int)this.numericUpDown193.Value;

			CheckerManager.m_DataController.SetCircleColor();

			int totalContour = 0;
//			pictureBox3.Image = ok_img;
//			textBox31.Text = "";
			textBox41.Text = "";
			Bitmap keepImage = null;
			//1番目の範囲の閾値
			while(FstThreshMin < FstThreshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(FstThreshMin, FstThreshMax, grabBitmap, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;

				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}

				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);

	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            FstThreshMin += FstStepCount;

//TODO
				if(totalContour == 0)//一つも外形検出できなかった→真黒画面等
				{
//					CheckerManager.form3.BackgroundImage = keepImage;
//					CheckerManager.form3.ClientSize = keepImage.Size;
//					return;
				}
           
            }

			//2番目の範囲の閾値
			while(SndThreshMin < SndThreshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(SndThreshMin, SndThreshMax, grabBitmap, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;

				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}

				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);
				
	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            SndThreshMin += SndStepCount;
	        }

			//3番目の範囲の閾値
			while(TrdThreshMin < TrdThreshMax)
			{
				double s, l, c;
				int x, y;
				double CONTRAST;
	            Bitmap m_bmpZ = CheckerManager.m_ImageController.ImageAnalysis(TrdThreshMin, TrdThreshMax, grabBitmap, out s, out l, out c, out x, out y, out CONTRAST, out totalContour, checkBox3.Checked);

				Bitmap oldImage = (Bitmap)CheckerManager.form3.BackgroundImage;

				if(keepImage != null)
				{
					keepImage.Dispose();
					keepImage = null;
				}
				keepImage = (Bitmap)m_bmpZ.Clone();

				if(m_bmpZ != null)
				{
					m_bmpZ.Dispose();
					m_bmpZ = null;
				}

				if(oldImage != null)
				{
					oldImage.Dispose();
					oldImage = null;
				}

	            textBox5.Text = string.Format("{0:f2}", s);
	            textBox6.Text = string.Format("{0:f2}", l);
	            textBox7.Text = string.Format("{0:f2}", c);
				
	            textBox27.Text = Convert.ToString(x);
				textBox28.Text = Convert.ToString(y);

	            TrdThreshMin += TrdStepCount;
	        }

			CheckerManager.form3.BackgroundImage = keepImage;
			CheckerManager.form3.ClientSize = keepImage.Size;


			if(checkBox6.Checked)
			{	
				gRawFileName = CheckerManager.m_CameraController.SaveImageFile(grabBitmap, (int)IMAGE_TYPE.RAW);
			}

			if(checkBox7.Checked)
			{	
				gAnaFileName = CheckerManager.m_CameraController.SaveImageFile((Bitmap)CheckerManager.form3.BackgroundImage, (int)IMAGE_TYPE.ANA);
			}

			int total = CheckerManager.m_ImageController.GetCenterCount();
			if(total > 0)
			{
				DrawInfoChart(total);
			}

			isDoWork = false;
		}

		public bool IsDoWork()
		{
#if false
			return isDoWork;//busyならハンドラ側へ返す
#else
			return false;
#endif
		}

		//カメラの割り込みハンドラから割り込んでくる契機
        private void timer6_Tick(object sender, EventArgs e)
        {
			timer6.Enabled = false;

            if(CheckerManager.GetCurrentMode() == 1)//AF
            {
				textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();

				int motion = CheckerManager.GetAfMotion();
				if(motion == 0)
				{
					CheckerManager.form3.BackgroundImage = null;
				}
				else
				{
					DoImageShapeLine();//最外円の最小面積で判断

					CheckerManager.m_DataController.af_info[AfCount].address = CheckerManager.m_MotorController.GetCurrentAddress();
					CheckerManager.m_DataController.af_info[AfCount].menseki = gArea;
					CheckerManager.m_DataController.af_info[AfCount].whiteCount = gWhiteCount;
					AfCount++;
				}
				return;
			}

			DoImageAnalyze();//画像解析

			if(checkBox8.Checked)
			{
				int total = CheckerManager.m_ImageController.GetCenterCount();
				if(total > 0)
				{
					CheckerManager.m_DataController.CSV_WRITE();
				}
			}

			this.Invalidate();//例外発生防止

			int countOk = 0;
			int countNg = 0;
			int countPass = 0;
			CheckerManager.m_DataController.GetGhostFlareCount(ref countOk, ref countNg, ref countPass);
			textBox3.Text = countPass.ToString() + " / 63";
			textBox4.Text = countOk.ToString();
			textBox9.Text = countNg.ToString();

			//LED 63個点灯が網羅されたか
			if(CheckerManager.m_DataController.IsAllLed())
			{
				CheckerManager.m_MotorController.LedReset();//LEDリセット
				button32_Click(sender, e);//Captureを止める

				CheckerManager.m_DataController.CSV_LED_RESULT_WRITE();//全LED情報をLED毎のCSVに保存する

				if(CheckerManager.m_DataController.IsGhostFlareCheck())
				{
					pictureBox3.Image = ok_img;
				    textBox15.ForeColor = Color.FromArgb(0x00, 0xFF, 0x00);
					textBox15.BackColor = SystemColors.Control;
					textBox15.Text = "検査OK。次のレンズをセットして[測　定]を押下して下さい";
				}
				else
				{
					pictureBox3.Image = ng_img;
				    textBox15.ForeColor = Color.FromArgb(0xFF, 0x00, 0x00);
					textBox15.BackColor = SystemColors.Control;
					textBox15.Text = "検査NG。レンズをNGBOXに入れて下さい";

					button28.Enabled = false;
					button58.Enabled = false;
					button26.Enabled = false;
					button33.Enabled = false;

					button1.Enabled = false;
					button40.Enabled = false;

		            //NG BOX関連の初期化
//		            button63_Click(sender, e);
					timer5.Enabled = true;
				}
			}
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
			timer5.Enabled = false;
			if(!timer7.Enabled)
			{
				timer7.Enabled = true;
			}else
			{
				timer7.Enabled = false;
				pictureBox4.Image = null;
			}
        }

        private void button63_Click(object sender, EventArgs e)
        {

			if(!timer7.Enabled)
			{
				timer7.Enabled = true;
			}else
			{
				timer7.Enabled = false;
				pictureBox4.Image = null;
			}
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            int result = CheckerManager.m_MotorController.M_NgBoxCheck();

			if(result == 1)
			{
				pictureBox4.Image = null;
			}
			else if(result == 0)
			{
				button28.Enabled = true;
				button58.Enabled = true;
				button26.Enabled = true;
				button33.Enabled = true;

				button1.Enabled = true;
				button40.Enabled = true;

				timer7.Enabled = false;
				pictureBox4.Image = ngbox_ok_img;
				textBox15.Text = "NGBOX通過。次のレンズをセットして[測　定]を押下して下さい";
			}

        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown38_ValueChanged(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorMode((int)numericUpDown38.Value);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
			this.Invalidate();
        }

		public void StartTimer()
		{
			timer6.Enabled = true;
		}

		public void Display(string str)
		{
			textBox15.Text = str;
		}

		public void DrawShasenChart(int n)
		{
		    chart1.Series.Clear();
		    chart1.ChartAreas.Clear();

            // ChartにChartAreaを追加
            string chart_area1 = "Area1";
		    chart1.ChartAreas.Add(new ChartArea(chart_area1));

            string legend1 = "中心からの距離";
		    chart1.Series.Add(legend1);
		    // グラフの種別を指定
		    chart1.Series[legend1].ChartType = SeriesChartType.Point;
            chart1.Series[legend1].MarkerSize = 4;
            chart1.Series[legend1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

            //X軸最小値、最大値、目盛間隔の設定
            chart1.ChartAreas[0].AxisX.Title = "角度[deg]";
            chart1.ChartAreas[0].AxisX.Minimum = -180;
			chart1.ChartAreas[0].AxisX.Maximum = 180;
			chart1.ChartAreas[0].AxisX.Interval = 60;

            //Y軸最小値、最大値、目盛間隔の設定
            chart1.ChartAreas[0].AxisY.Title = "半径";
            chart1.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Stacked;

#if false
			int rgb = CheckerManager.m_DataController.GetDrawColor();
#else
			int rgb = 0xFF0000;//red
#endif
			int R = (rgb & 0xFF0000) >> 16;
			int G = (rgb & 0x00FF00) >> 8;
			int B = (rgb & 0x0000FF);
			Color color = Color.FromArgb(R,G,B);

            chart1.Series[legend1].Color = color;

			int sum = 0;
            // データをシリーズにセット
            for (int i = 0; i < n; i++)
		    {
		        chart1.Series[legend1].Points.AddXY(CheckerManager.m_DataController.triInfo[i].theta, CheckerManager.m_DataController.triInfo[i].shasen);
				sum += CheckerManager.m_DataController.triInfo[i].shasen;
		    }
			
			//平均を求める
			int average = sum / n;
			textBox34.Text = average.ToString();

			//平均線を重ね合わせる
			Color ave_color = Color.FromArgb(0x00, 0x00, 0xFF);

            string legend2 = "平均";
		    chart1.Series.Add(legend2);
		    // グラフの種別を指定
		    chart1.Series[legend2].ChartType = SeriesChartType.Point;
            chart1.Series[legend2].MarkerSize = 2;
            chart1.Series[legend2].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
			chart1.Series[legend2].Color = ave_color;
            for (int i = -180; i < 180; i++)
		    {
		        chart1.Series[legend2].Points.AddXY(i, average);
		    }

			if(checkBox1.Checked)
			{
				gShasenFileName = CheckerManager.m_DataController.GetImageFileName((int)IMAGE_TYPE.CHART_RADIAL);
				chart1.SaveImage(gShasenFileName, ChartImageFormat.Png);
			}

#if false
            //平均からの最大ずれ量が閾値を超えた場合、FLAREと判断
            textBox41.Text = "";
            textBox41.ForeColor = Color.Red;

			int pos = CheckerManager.m_DataController.GetLedPos();
			if(pos != 0xFF)
			{
				CheckerManager.m_DataController.SetLedPos(pos, 0x01);
				paintCell(pos);
			}
            for (int i = 0; i < n; i++)
			{
                if ((CheckerManager.m_DataController.triInfo[i].shasen - average) > DataController.SETDATA.distanceThresh)
				{
				    textBox41.ForeColor = Color.FromArgb(0xFF, 0x00, 0x00);
					textBox41.BackColor = SystemColors.Control;
					textBox41.Text = "FLARE 検出 !";
//                    pictureBox3.Image = ng_img;
					
					if(pos != 0xFF)
					{
						CheckerManager.m_DataController.SetLedPos(pos, 0x04);
						paintCell(pos);
					}

					if(checkBox8.Checked)
					{
						CheckerManager.m_DataController.logInfo.flare_hankei_avarage = average;
						CheckerManager.m_DataController.logInfo.flare_or_not = "FLARE検出";
					}

					break;
				}
			}
#endif
		
		}

		public void DrawInfoChart(int n)
		{

            chart3.Series.Clear();
		    chart3.ChartAreas.Clear();

            // ChartにChartAreaを追加
            string chart_area1 = "Area1";
		    chart3.ChartAreas.Add(new ChartArea(chart_area1));

            string legend1 = "中心座標";
		    chart3.Series.Add(legend1);

		    // グラフの種別を指定
		    chart3.Series[legend1].ChartType = SeriesChartType.Point;
            chart3.Series[legend1].MarkerSize = 8;
            chart3.Series[legend1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

            //X軸最小値、最大値、目盛間隔の設定
            chart3.ChartAreas[0].AxisX.Title = "中心点のX座標";
            chart3.ChartAreas[0].AxisX.Minimum = 0;
			chart3.ChartAreas[0].AxisX.Maximum = CheckerManager.form2.Width;
			chart3.ChartAreas[0].AxisX.Interval = 100;

            //Y軸最小値、最大値、目盛間隔の設定
            chart3.ChartAreas[0].AxisY.Minimum = 0;
			chart3.ChartAreas[0].AxisY.Maximum = CheckerManager.form2.Height;
			chart3.ChartAreas[0].AxisY.Interval = 100;
            chart3.ChartAreas[0].AxisY.Title = "中心点のY座標";
            chart3.ChartAreas[0].AxisY.IsReversed = true;
            chart3.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Stacked;

            int sum_x = 0;
			int sum_y = 0;
			int sum_xv = 0;
			int sum_yv = 0;
            // データをシリーズにセット
			string csvpath = CheckerManager.m_DataController.GetCSVFileName("log.csv");

            for (int i = 0; i < n; i++)
		    {
				int xx = CheckerManager.m_DataController.circleInfo[i].x;
				int yy = CheckerManager.m_DataController.circleInfo[i].y;
                chart3.Series[legend1].Points.AddXY(xx, yy);

				if(checkBox8.Checked)
				{
//					CheckerManager.m_DataController.f_write(csvpath, CheckerManager.m_DataController.circleInfo[i]);
				}

                int rgb = CheckerManager.m_DataController.GetDrawColor(i);

				int R = (rgb & 0xFF0000) >> 16;
				int G = (rgb & 0x00FF00) >> 8;
				int B = (rgb & 0x0000FF);
				Color color = Color.FromArgb(R,G,B);

	            chart3.Series[legend1].Points[i].Color = color;

				sum_x += xx;
				sum_y += yy;
				sum_xv += xx * xx;
				sum_yv += yy * yy;
		    }

            CheckerManager.m_DataController.sumInfo.n = n;
            CheckerManager.m_DataController.sumInfo.sum_x = sum_x;
            CheckerManager.m_DataController.sumInfo.sum_y = sum_y;
            CheckerManager.m_DataController.sumInfo.sum_xv = sum_xv;
            CheckerManager.m_DataController.sumInfo.sum_yv = sum_yv;

            if (n > 0)
			{
				//xの平均、分散、標準偏差
				CheckerManager.m_DataController.sumInfo.mean_x = sum_x / n;
				CheckerManager.m_DataController.sumInfo.variance_x = (double)((sum_xv / n) - (CheckerManager.m_DataController.sumInfo.mean_x * CheckerManager.m_DataController.sumInfo.mean_x));
				CheckerManager.m_DataController.sumInfo.stddev_x = Math.Sqrt(CheckerManager.m_DataController.sumInfo.variance_x);

				//yの平均、分散、標準偏差
				CheckerManager.m_DataController.sumInfo.mean_y = sum_y / n;
				CheckerManager.m_DataController.sumInfo.variance_y = (double)((sum_yv / n) - (CheckerManager.m_DataController.sumInfo.mean_y * CheckerManager.m_DataController.sumInfo.mean_y));
				CheckerManager.m_DataController.sumInfo.stddev_y = Math.Sqrt(CheckerManager.m_DataController.sumInfo.variance_y);
			}

			textBox35.Text = CheckerManager.m_DataController.sumInfo.mean_x.ToString();
			textBox36.Text = CheckerManager.m_DataController.sumInfo.variance_x.ToString();
			int dev_x = (int)CheckerManager.m_DataController.sumInfo.stddev_x;
			textBox37.Text = dev_x.ToString();
			textBox38.Text = CheckerManager.m_DataController.sumInfo.mean_y.ToString();
			textBox39.Text = CheckerManager.m_DataController.sumInfo.variance_y.ToString();
			int dev_y = (int)CheckerManager.m_DataController.sumInfo.stddev_y;
			textBox40.Text = dev_y.ToString();

			if(checkBox8.Checked)
			{
				CheckerManager.m_DataController.logInfo.lenzeNumber = textBox31.Text;

				CheckerManager.m_DataController.logInfo.FstThreshMin = (int)numericUpDown25.Value;
				CheckerManager.m_DataController.logInfo.FstThreshMax = (int)numericUpDown26.Value;
				CheckerManager.m_DataController.logInfo.FstStepCount = (int)numericUpDown27.Value;
				CheckerManager.m_DataController.logInfo.SndThreshMin = (int)numericUpDown48.Value;
				CheckerManager.m_DataController.logInfo.SndThreshMax = (int)numericUpDown49.Value;
				CheckerManager.m_DataController.logInfo.SndStepCount = (int)numericUpDown50.Value;
				CheckerManager.m_DataController.logInfo.TrdThreshMin = (int)numericUpDown192.Value;
				CheckerManager.m_DataController.logInfo.TrdThreshMax = (int)numericUpDown193.Value;
				CheckerManager.m_DataController.logInfo.TrdStepCount = (int)numericUpDown194.Value;

				CheckerManager.m_DataController.logInfo.UpperMenseki = (int)numericUpDown5.Value;

				CheckerManager.m_DataController.logInfo.z_address = textBox13.Text;

				CheckerManager.m_DataController.logInfo.hankei_kakudoFile= gShasenFileName;
				CheckerManager.m_DataController.logInfo.rawFileName = gRawFileName;
				CheckerManager.m_DataController.logInfo.anaFileName = gAnaFileName;
			}

			if(checkBox1.Checked)
			{
				CheckerManager.m_DataController.logInfo.hankei_kakudoFile= gShasenFileName;
			}

			if(checkBox2.Checked)
			{
				string fileName = CheckerManager.m_DataController.GetImageFileName((int)IMAGE_TYPE.CHART_CENTER);
				chart3.SaveImage(fileName, ChartImageFormat.Png);
				CheckerManager.m_DataController.logInfo.chu_shintenFile = fileName;
			}

            textBox41.Text = "";
			bool ghostFlag = false;//NG判定に使用
#if false
            //中心XY座標の分散のいずれかが閾値を超えた場合、GHOSTと判断
			int pos = CheckerManager.m_DataController.GetLedPos();
			if(pos != 0xFF)
			{
	            if (CheckerManager.m_DataController.sumInfo.variance_x > DataController.SETDATA.center_x_Thresh || 
	                CheckerManager.m_DataController.sumInfo.variance_y > DataController.SETDATA.center_y_Thresh)
	            {
					ghostFlag = true;
				}
			}
#else
			//光源の中心座標と最外形の中心座標の差が閾値以上ある場合、GHOSTと判断
			int pos = CheckerManager.m_DataController.GetLedPos();
			if(pos != 0xFF)
			{
//				int sabun_x = CheckerManager.m_DataController.ledtable[pos].center_x - CheckerManager.m_DataController.ledtable[pos].outside_center_x;
//				int sabun_y = CheckerManager.m_DataController.ledtable[pos].center_y - CheckerManager.m_DataController.ledtable[pos].outside_center_y;
//
//				if(System.Math.Abs(sabun_x) > DataController.SETDATA.center_x_zure || System.Math.Abs(sabun_y) > DataController.SETDATA.center_y_zure)
//				{
//					ghostFlag = true;
//				}

				if(CheckerManager.m_DataController.ledtable[pos].numberOfoutside > 1)//島が2つ以上ある場合
				{
					ghostFlag = true;
				}
				else if(CheckerManager.m_DataController.ledtable[pos].fst_most_outside_menseki > DataController.SETDATA.led_area_limit[pos])//LEDポジションの面積閾値より大きい場合
				{
					ghostFlag = true;
				}
				else if((CheckerManager.m_DataController.ledtable[pos].maxLine - CheckerManager.m_DataController.ledtable[pos].minLine) > DataController.SETDATA.lineFlareThresh)
				{
					ghostFlag = true;
				}


				if(ghostFlag)
				{
					CheckerManager.m_DataController.SetLedPos(pos, 0x02);
					paintCell(pos);

				    textBox41.ForeColor = Color.FromArgb(0xFF, 0x00, 0x00);
					textBox41.BackColor = SystemColors.Control;
	//				textBox41.Text = "GHOST 検出 !";
					textBox41.Text = "NG 検出 !";
	//				pictureBox3.Image = ng_img;

					if(checkBox8.Checked)
					{
	//					CheckerManager.m_DataController.logInfo.ghost_or_not = "GHOST検出";
						CheckerManager.m_DataController.logInfo.ghost_or_not = "NG検出";
					}
				}
				else
				{
					if(checkBox8.Checked)
					{
						CheckerManager.m_DataController.logInfo.ghost_or_not = "OK";
					}

					CheckerManager.m_DataController.SetLedPos(pos, 0x01);
					paintCell(pos);
				}
			}
#endif

        }

        private void button1_Click_1(object sender, EventArgs e)//File Open
        {
			//OpenFileDialogクラスのインスタンスを作成
			OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image File(*.bmp,*.jpg,*.png,*.tif)|*.bmp;*.jpg;*.png;*.tif|Bitmap(*.bmp)|*.bmp|Jpeg(*.jpg)|*.jpg|PNG(*.png)|*.png";
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
			{
                //OKボタンがクリックされたとき、選択されたファイルを読み取り専用で開く
                strFileName = ofd.FileName;
				CheckerManager.m_ImageController.SetImageFile(strFileName);

				string filename = Path.GetFileName(strFileName);
				textBox8.Text = filename;

				CheckerManager.m_ImageController.SetLimitForAnalysis(
											(int)numericUpDown2.Value, 
											(int)numericUpDown5.Value, 
											(int)numericUpDown3.Value, 
											(int)numericUpDown6.Value, 
											(int)numericUpDown4.Value, 
											(int)numericUpDown7.Value);

				FstThreshMin = (int)this.numericUpDown25.Value;
				FstThreshMax = (int)this.numericUpDown26.Value;
				SndThreshMin = (int)this.numericUpDown48.Value;
				SndThreshMax = (int)this.numericUpDown49.Value;
				TrdThreshMin = (int)this.numericUpDown192.Value;
				TrdThreshMax = (int)this.numericUpDown193.Value;


				this.timer2.Enabled = true;

				Bitmap img = new Bitmap(strFileName);

				int width, height;
				CheckerManager.form2.GetPreviewWindow(out width, out height);
				int resizeWidth = width;
				int resizeHeight = height;

				if(localimage != null)
				{
					localimage.Dispose();
				}
				localimage = new Bitmap(resizeWidth, resizeHeight);
				Graphics g = Graphics.FromImage(localimage);

				g.DrawImage(img, 0, 0, resizeWidth, resizeHeight);
				g.Dispose();
				img.Dispose();

				Bitmap oldImage = (Bitmap)CheckerManager.form2.BackgroundImage;
				CheckerManager.form2.BackgroundImage = localimage;
				CheckerManager.form2.ClientSize = localimage.Size;
				if(oldImage != null)
				{
					oldImage.Dispose();
				}

				if(checkBox6.Checked)
				{	
					gRawFileName = CheckerManager.m_CameraController.SaveImageFile(localimage, (int)IMAGE_TYPE.RAW);
				}

			}

        }

        private void button40_Click_1(object sender, EventArgs e)//Retry
        {
            if (textBox8.Text == "")
            {
                MessageBox.Show("Please select image file!\n");
                return;
            }

            CheckerManager.m_ImageController.SetLimitForAnalysis(
                                        (int)numericUpDown2.Value,
                                        (int)numericUpDown5.Value,
                                        (int)numericUpDown3.Value,
                                        (int)numericUpDown6.Value,
                                        (int)numericUpDown4.Value,
                                        (int)numericUpDown7.Value);

			FstThreshMin = (int)this.numericUpDown25.Value;
			FstThreshMax = (int)this.numericUpDown26.Value;
			SndThreshMin = (int)this.numericUpDown48.Value;
			SndThreshMax = (int)this.numericUpDown49.Value;
			TrdThreshMin = (int)this.numericUpDown192.Value;
			TrdThreshMax = (int)this.numericUpDown193.Value;

            CheckerManager.m_ImageController.SetImageFile(strFileName);

			if(checkBox6.Checked)
			{	
				gRawFileName = CheckerManager.m_CameraController.SaveImageFile(localimage, (int)IMAGE_TYPE.RAW);
			}
            this.timer2.Enabled = true;
        }

        private void numericUpDown39_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.distanceThresh = (int)numericUpDown39.Value;
        }

        private void numericUpDown40_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.center_x_Thresh = (int)numericUpDown40.Value;
        }

        private void numericUpDown41_ValueChanged(object sender, EventArgs e)
        {
            DataController.SETDATA.center_x_hosei = (int)numericUpDown41.Value;
        }

        private void numericUpDown42_ValueChanged(object sender, EventArgs e)
        {
            DataController.SETDATA.center_y_hosei = (int)numericUpDown42.Value;
        }


        private void button3_Click_2(object sender, EventArgs e)
        {
        	textBox14.Text = CheckerManager.m_MotorController.button_Org_Click(sender, e);
        }

        private void button59_Click_2(object sender, EventArgs e)
        {
            textBox14.Text = CheckerManager.m_MotorController.button_Env_Click(sender, e);
            textBox14.Text = CheckerManager.m_MotorController.button_Open_Click(sender, e);
        }

        private void button60_Click_2(object sender, EventArgs e)
        {
            textBox14.Text = CheckerManager.m_MotorController.button_FastStop_Click(sender, e);

			button32_Click(sender, e);//カメラのPreviewを一時停止する
			timer6.Enabled = false;
			timer4.Enabled = false;
			timer8.Enabled = false;
        }

        private void button61_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            bool isResult;
            CheckerManager.m_MotorController.StepDrive(textData, out isResult);
            textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
        }

        private void button22_Click(object sender, EventArgs e)
        {
			textBox1.Text = "100";
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            textData *= -1;
            textBox1.Text = Convert.ToString(textData);
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            textData /= 10;
            textBox1.Text = Convert.ToString(textData);
        }

        private void button7_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            textData /= 2;
            textBox1.Text = Convert.ToString(textData);
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            textData *= 2;
            textBox1.Text = Convert.ToString(textData);
        }

        private void button9_Click_2(object sender, EventArgs e)
        {
            string textStr = textBox1.Text;
            int textData = int.Parse(textStr);
            textData *= 5;
            textBox1.Text = Convert.ToString(textData);
        }

        private void numericUpDown43_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.center_y_Thresh = (int)numericUpDown40.Value;
        }

        private void numericUpDown25_ValueChanged_1(object sender, EventArgs e)
        {
			if(numericUpDown25.Value < numericUpDown25.Minimum)
			{
				numericUpDown25.Value = numericUpDown25.Minimum;
			}
			if((numericUpDown26.Value - 1) < numericUpDown25.Value)
			{
				numericUpDown25.Value = numericUpDown26.Value - 1;
			}
			DataController.SETDATA.FstThreshMin = FstThreshMin = (int)numericUpDown25.Value;
        }

        private void numericUpDown26_ValueChanged_1(object sender, EventArgs e)
        {
			if(numericUpDown26.Value < (numericUpDown25.Value + 1))
			{
				numericUpDown26.Value = numericUpDown25.Value + 1;
			}

			numericUpDown48.Value = numericUpDown26.Value + 1;
			DataController.SETDATA.FstThreshMax = FstThreshMax = (int)numericUpDown26.Value;
        }

        private void numericUpDown27_ValueChanged_1(object sender, EventArgs e)
        {
			if(numericUpDown27.Value == 0)
			{
				numericUpDown27.Value++;
			}
			DataController.SETDATA.FstStepCount = FstStepCount = (int)numericUpDown27.Value;
        }

        private void numericUpDown5_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.sMax = (int)numericUpDown5.Value;
        }

        private void numericUpDown2_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.sMin = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.lMin = (int)numericUpDown3.Value;
        }

        private void numericUpDown6_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.lMax = (int)numericUpDown6.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
        }

        private void numericUpDown1_ValueChanged_2(object sender, EventArgs e)
        {
			CheckerManager.m_CameraController.SetColorMode((int)numericUpDown1.Value);
        }

        private void numericUpDown34_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.L_LimitPLS[0] = (int)numericUpDown34.Value;
        }

        private void numericUpDown33_ValueChanged_1(object sender, EventArgs e)
        {
			DataController.SETDATA.U_LimitPLS[0] = (int)numericUpDown33.Value;
        }

        private void button57_Click(object sender, EventArgs e)
        {
        }

        private void button52_Click_2(object sender, EventArgs e)
        {
            textBox14.Text = CheckerManager.m_MotorController.button_CcwIndex_Click(sender, e);
            textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
        }

        private void button53_Click_2(object sender, EventArgs e)
        {
            textBox14.Text = CheckerManager.m_MotorController.button_CwIndex_Click(sender, e);
            textBox13.Text = CheckerManager.m_MotorController.DisplayAddress();
        }

        private void button55_Click(object sender, EventArgs e)
        {
            textBox14.Text = CheckerManager.m_MotorController.button_ClrError_Click(sender, e);
        }

        private void timer8_Tick_1(object sender, EventArgs e)//AFでステージを移動し焦点に近づける
        {
        }

        private void numericUpDown44_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.af_left = (int)numericUpDown44.Value;
        }

        private void numericUpDown45_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.af_top = (int)numericUpDown45.Value;
        }

        private void numericUpDown46_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.af_right = (int)numericUpDown46.Value;
        }

        private void numericUpDown47_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.af_buttom = (int)numericUpDown47.Value;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void numericUpDown48_ValueChanged(object sender, EventArgs e)
        {
			if((numericUpDown49.Value - 1) < numericUpDown48.Value)
			{
				numericUpDown48.Value = numericUpDown49.Value - 1;
			}

			numericUpDown26.Value = numericUpDown48.Value - 1;
			DataController.SETDATA.SndThreshMin = SndThreshMin = (int)numericUpDown48.Value;
        }

        private void numericUpDown49_ValueChanged(object sender, EventArgs e)
        {
			if(numericUpDown49.Value < (numericUpDown48.Value + 1))
			{
				numericUpDown49.Value = numericUpDown48.Value + 1;
			}

			numericUpDown192.Value = numericUpDown49.Value + 1;
			DataController.SETDATA.SndThreshMax = SndThreshMax = (int)numericUpDown49.Value;
        }

        private void numericUpDown50_ValueChanged(object sender, EventArgs e)
        {
			if(numericUpDown50.Value == 0)
			{
				numericUpDown50.Value++;
			}
		
			DataController.SETDATA.SndStepCount = SndStepCount = (int)numericUpDown50.Value;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
			checkBox4.Checked = checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
			checkBox3.Checked = checkBox4.Checked;
        }

		private void initResultLabel()
		{
			labelmatrix = new Label[]{
			label72,label76,label77,label78,label79,label80,label81,label82,label83,label84,
			label85,label86,label87,label88,label89,label90,label91,label92,label93,label94,
			label95,label96,label97,label98,label99,label100,label101,label102,label103,label104,
			label105,label106,label107,label108,label109,label110,label111,label112,label113,label114,
			label115,label116,label117,label118,label119,label120,label121,label122,label123,label124,
			label125,label126,label127,label128,label129,label130,label131,label132,label133,label134,
			label135,label136,label137};

			clearLabel();
		}

		private void clearLabel()
		{
			for(int i = 0; i < labelmatrix.Length; i++)
			{
				labelmatrix[i].Text = "";
                labelmatrix[i].BackColor = Color.White;
			}
		}

		private void paintCell(int pos)
		{
			int type = CheckerManager.m_DataController.GetLedResult(pos);
			if(type == 1)
			{
				labelmatrix[pos].BackColor = Color.Green;
				labelmatrix[pos].Text = "   OK   ";
			}
			else if(type == 2)
			{
				labelmatrix[pos].BackColor = Color.Red;
//				labelmatrix[pos].Text = "GHOST";
				labelmatrix[pos].Text = "   NG   ";
			}
			else if(type == 4)
			{
				labelmatrix[pos].BackColor = Color.Yellow;
				labelmatrix[pos].Text = "FLARE";
			}
			else if(type == 6)
			{
				labelmatrix[pos].BackColor = Color.Orange;
				labelmatrix[pos].Text = "G/F";
			}
		}

        private void numericUpDown51_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[0].center_x = DataController.SETDATA.led_x[0] = (int)numericUpDown51.Value;
        }

        private void numericUpDown52_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[0].center_y = DataController.SETDATA.led_y[0] = (int)numericUpDown52.Value;
        }

        private void numericUpDown53_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[1].center_x = DataController.SETDATA.led_x[1] = (int)numericUpDown53.Value;
        }

        private void numericUpDown54_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[1].center_y = DataController.SETDATA.led_y[1] = (int)numericUpDown54.Value;
        }

        private void numericUpDown55_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[2].center_x = DataController.SETDATA.led_x[2] = (int)numericUpDown55.Value;
        }

        private void numericUpDown56_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[2].center_y = DataController.SETDATA.led_y[2] = (int)numericUpDown56.Value;
        }

        private void numericUpDown57_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[3].center_x = DataController.SETDATA.led_x[3] = (int)numericUpDown57.Value;
        }

        private void numericUpDown58_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[3].center_y = DataController.SETDATA.led_y[3] = (int)numericUpDown58.Value;
        }

        private void numericUpDown59_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[4].center_x = DataController.SETDATA.led_x[4] = (int)numericUpDown59.Value;
        }

        private void numericUpDown60_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[4].center_y = DataController.SETDATA.led_y[4] = (int)numericUpDown60.Value;
        }

        private void numericUpDown61_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[5].center_x = DataController.SETDATA.led_x[5] = (int)numericUpDown61.Value;
        }

        private void numericUpDown62_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[5].center_y = DataController.SETDATA.led_y[5] = (int)numericUpDown62.Value;
        }

        private void numericUpDown63_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[6].center_x = DataController.SETDATA.led_x[6] = (int)numericUpDown63.Value;
        }

        private void numericUpDown64_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[6].center_y = DataController.SETDATA.led_y[6] = (int)numericUpDown64.Value;
        }

        private void numericUpDown65_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[7].center_x = DataController.SETDATA.led_x[7] = (int)numericUpDown65.Value;
        }

        private void numericUpDown66_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[7].center_y = DataController.SETDATA.led_y[7] = (int)numericUpDown66.Value;
        }

        private void numericUpDown67_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[15].center_x = DataController.SETDATA.led_x[15] = (int)numericUpDown67.Value;
        }

        private void numericUpDown68_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[15].center_y = DataController.SETDATA.led_y[15] = (int)numericUpDown68.Value;
        }

        private void numericUpDown69_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[14].center_x = DataController.SETDATA.led_x[14] = (int)numericUpDown69.Value;
        }

        private void numericUpDown70_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[14].center_y = DataController.SETDATA.led_y[14] = (int)numericUpDown70.Value;
        }

        private void numericUpDown71_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[13].center_x = DataController.SETDATA.led_x[13] = (int)numericUpDown71.Value;
        }

        private void numericUpDown72_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[13].center_y = DataController.SETDATA.led_y[13] = (int)numericUpDown72.Value;
        }

        private void numericUpDown73_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[12].center_x = DataController.SETDATA.led_x[12] = (int)numericUpDown73.Value;
        }

        private void numericUpDown74_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[12].center_y = DataController.SETDATA.led_y[12] = (int)numericUpDown74.Value;
        }

        private void numericUpDown75_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[11].center_x = DataController.SETDATA.led_x[11] = (int)numericUpDown75.Value;
        }

        private void numericUpDown76_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[11].center_y = DataController.SETDATA.led_y[11] = (int)numericUpDown76.Value;
        }

        private void numericUpDown77_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[10].center_x = DataController.SETDATA.led_x[10] = (int)numericUpDown77.Value;
        }

        private void numericUpDown78_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[10].center_y = DataController.SETDATA.led_y[10] = (int)numericUpDown78.Value;
        }

        private void numericUpDown79_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[9].center_x = DataController.SETDATA.led_x[9] = (int)numericUpDown79.Value;
        }

        private void numericUpDown80_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[9].center_y = DataController.SETDATA.led_y[9] = (int)numericUpDown80.Value;
        }

        private void numericUpDown81_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[8].center_y = DataController.SETDATA.led_y[8] = (int)numericUpDown81.Value;
        }

        private void numericUpDown82_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[8].center_x = DataController.SETDATA.led_x[8] = (int)numericUpDown82.Value;
        }

        private void numericUpDown83_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[16].center_x = DataController.SETDATA.led_x[16] = (int)numericUpDown83.Value;
        }

        private void numericUpDown84_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[16].center_y = DataController.SETDATA.led_y[16] = (int)numericUpDown84.Value;
        }

        private void numericUpDown85_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[17].center_x = DataController.SETDATA.led_x[17] = (int)numericUpDown85.Value;
        }

        private void numericUpDown86_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[17].center_y = DataController.SETDATA.led_y[17] = (int)numericUpDown86.Value;
        }

        private void numericUpDown97_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[21].center_x = DataController.SETDATA.led_x[21] = (int)numericUpDown97.Value;
        }

        private void numericUpDown98_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[21].center_y = DataController.SETDATA.led_y[21] = (int)numericUpDown98.Value;
        }

        private void numericUpDown99_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[20].center_x = DataController.SETDATA.led_x[20] = (int)numericUpDown99.Value;
        }

        private void numericUpDown100_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[20].center_y = DataController.SETDATA.led_y[20] = (int)numericUpDown100.Value;
        }

        private void numericUpDown101_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[19].center_x = DataController.SETDATA.led_x[19] = (int)numericUpDown101.Value;
        }

        private void numericUpDown102_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[19].center_y = DataController.SETDATA.led_y[19] = (int)numericUpDown102.Value;
        }
        private void numericUpDown103_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[18].center_x = DataController.SETDATA.led_x[18] = (int)numericUpDown103.Value;
        }

        private void numericUpDown104_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[18].center_y = DataController.SETDATA.led_y[18] = (int)numericUpDown104.Value;
        }

        private void numericUpDown95_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[22].center_x = DataController.SETDATA.led_x[22] = (int)numericUpDown95.Value;
        }

        private void numericUpDown96_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[22].center_y = DataController.SETDATA.led_y[22] = (int)numericUpDown96.Value;
        }

        private void numericUpDown93_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[23].center_x = DataController.SETDATA.led_x[23] = (int)numericUpDown93.Value;
        }

        private void numericUpDown94_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[23].center_y = DataController.SETDATA.led_y[23] = (int)numericUpDown94.Value;
        }

        private void numericUpDown91_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[24].center_x = DataController.SETDATA.led_x[24] = (int)numericUpDown91.Value;
        }

        private void numericUpDown92_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[24].center_y = DataController.SETDATA.led_y[24] = (int)numericUpDown92.Value;
        }

        private void numericUpDown89_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[25].center_x = DataController.SETDATA.led_x[25] = (int)numericUpDown89.Value;
        }

        private void numericUpDown90_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[25].center_y = DataController.SETDATA.led_y[25] = (int)numericUpDown90.Value;
        }

        private void numericUpDown87_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[26].center_x = DataController.SETDATA.led_x[26] = (int)numericUpDown87.Value;
        }

        private void numericUpDown88_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[26].center_y = DataController.SETDATA.led_y[26] = (int)numericUpDown88.Value;
        }

        private void numericUpDown121_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[27].center_x = DataController.SETDATA.led_x[27] = (int)numericUpDown121.Value;
        }

        private void numericUpDown122_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[27].center_y = DataController.SETDATA.led_y[27] = (int)numericUpDown122.Value;
        }

        private void numericUpDown119_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[28].center_x = DataController.SETDATA.led_x[28] = (int)numericUpDown119.Value;
        }

        private void numericUpDown120_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[28].center_y = DataController.SETDATA.led_y[28] = (int)numericUpDown120.Value;
        }

        private void numericUpDown117_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[29].center_x = DataController.SETDATA.led_x[29] = (int)numericUpDown117.Value;
        }

        private void numericUpDown118_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[29].center_y = DataController.SETDATA.led_y[29] = (int)numericUpDown118.Value;
        }

        private void numericUpDown115_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[30].center_x = DataController.SETDATA.led_x[30] = (int)numericUpDown115.Value;
        }

        private void numericUpDown116_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[30].center_y = DataController.SETDATA.led_y[30] = (int)numericUpDown116.Value;
        }

        private void numericUpDown113_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[31].center_x = DataController.SETDATA.led_x[31] = (int)numericUpDown113.Value;
        }

        private void numericUpDown114_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[31].center_y = DataController.SETDATA.led_y[31] = (int)numericUpDown114.Value;
        }

        private void numericUpDown111_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[32].center_x = DataController.SETDATA.led_x[32] = (int)numericUpDown111.Value;
        }

        private void numericUpDown112_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[32].center_y = DataController.SETDATA.led_y[32] = (int)numericUpDown112.Value;
        }

        private void numericUpDown109_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[33].center_x = DataController.SETDATA.led_x[33] = (int)numericUpDown109.Value;
        }

        private void numericUpDown110_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[33].center_y = DataController.SETDATA.led_y[33] = (int)numericUpDown110.Value;
        }

        private void numericUpDown107_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[34].center_x = DataController.SETDATA.led_x[34] = (int)numericUpDown107.Value;
        }

        private void numericUpDown108_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[34].center_y = DataController.SETDATA.led_y[34] = (int)numericUpDown108.Value;
        }

        private void numericUpDown105_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[35].center_x = DataController.SETDATA.led_x[35] = (int)numericUpDown105.Value;
        }

        private void numericUpDown106_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[35].center_y = DataController.SETDATA.led_y[35] = (int)numericUpDown106.Value;
        }

        private void numericUpDown139_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[36].center_x = DataController.SETDATA.led_x[36] = (int)numericUpDown139.Value;
        }

        private void numericUpDown140_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[36].center_y = DataController.SETDATA.led_y[36] = (int)numericUpDown140.Value;
        }

        private void numericUpDown137_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[37].center_x = DataController.SETDATA.led_x[37] = (int)numericUpDown137.Value;
        }

        private void numericUpDown138_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[37].center_y = DataController.SETDATA.led_y[37] = (int)numericUpDown138.Value;
        }

        private void numericUpDown135_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[38].center_x = DataController.SETDATA.led_x[38] = (int)numericUpDown135.Value;
        }

        private void numericUpDown136_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[38].center_y = DataController.SETDATA.led_y[38] = (int)numericUpDown136.Value;
        }

        private void numericUpDown123_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[44].center_x = DataController.SETDATA.led_x[44] = (int)numericUpDown123.Value;
        }

        private void numericUpDown125_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[43].center_x = DataController.SETDATA.led_x[43] = (int)numericUpDown125.Value;
        }

        private void numericUpDown126_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[43].center_y = DataController.SETDATA.led_y[43] = (int)numericUpDown126.Value;
        }

        private void numericUpDown127_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[42].center_x = DataController.SETDATA.led_x[42] = (int)numericUpDown127.Value;
        }

        private void numericUpDown128_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[42].center_y = DataController.SETDATA.led_y[42] = (int)numericUpDown128.Value;
        }

        private void numericUpDown129_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[41].center_x = DataController.SETDATA.led_x[41] = (int)numericUpDown129.Value;
        }

        private void numericUpDown130_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[41].center_y = DataController.SETDATA.led_y[41] = (int)numericUpDown130.Value;
        }

        private void numericUpDown131_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[40].center_x = DataController.SETDATA.led_x[40] = (int)numericUpDown131.Value;
        }

        private void numericUpDown132_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[40].center_y = DataController.SETDATA.led_y[40] = (int)numericUpDown132.Value;
        }

        private void numericUpDown133_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[39].center_x = DataController.SETDATA.led_x[39] = (int)numericUpDown133.Value;
        }

        private void numericUpDown134_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[39].center_y = DataController.SETDATA.led_y[39] = (int)numericUpDown134.Value;
        }

        private void numericUpDown141_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[53].center_x = DataController.SETDATA.led_x[53] = (int)numericUpDown141.Value;
        }

        private void numericUpDown142_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[53].center_y = DataController.SETDATA.led_y[53] = (int)numericUpDown142.Value;
        }

        private void numericUpDown143_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[52].center_x = DataController.SETDATA.led_x[52] = (int)numericUpDown143.Value;
        }

        private void numericUpDown144_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[52].center_y = DataController.SETDATA.led_y[52] = (int)numericUpDown144.Value;
        }

        private void numericUpDown145_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[51].center_x = DataController.SETDATA.led_x[51] = (int)numericUpDown145.Value;
        }

        private void numericUpDown146_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[51].center_y = DataController.SETDATA.led_y[51] = (int)numericUpDown146.Value;
        }

        private void numericUpDown147_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[50].center_x = DataController.SETDATA.led_x[50] = (int)numericUpDown147.Value;
        }

        private void numericUpDown148_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[50].center_y = DataController.SETDATA.led_y[50] = (int)numericUpDown148.Value;
        }

        private void numericUpDown149_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[49].center_x = DataController.SETDATA.led_x[49] = (int)numericUpDown149.Value;
        }

        private void numericUpDown150_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[49].center_y = DataController.SETDATA.led_y[49] = (int)numericUpDown150.Value;
        }

        private void numericUpDown151_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[48].center_x = DataController.SETDATA.led_x[48] = (int)numericUpDown151.Value;
        }

        private void numericUpDown152_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[48].center_y = DataController.SETDATA.led_y[48] = (int)numericUpDown152.Value;
        }

        private void numericUpDown153_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[47].center_x = DataController.SETDATA.led_x[47] = (int)numericUpDown153.Value;
        }

        private void numericUpDown154_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[47].center_y = DataController.SETDATA.led_y[47] = (int)numericUpDown154.Value;
        }

        private void numericUpDown155_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[46].center_x = DataController.SETDATA.led_x[46] = (int)numericUpDown155.Value;
        }

        private void numericUpDown156_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[46].center_y = DataController.SETDATA.led_y[46] = (int)numericUpDown156.Value;
        }

        private void numericUpDown157_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[45].center_x = DataController.SETDATA.led_x[45] = (int)numericUpDown157.Value;
        }

        private void numericUpDown158_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[45].center_y = DataController.SETDATA.led_y[45] = (int)numericUpDown158.Value;
        }

        private void numericUpDown159_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[62].center_x = DataController.SETDATA.led_x[62] = (int)numericUpDown159.Value;
        }

        private void numericUpDown161_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[61].center_x = DataController.SETDATA.led_x[61] = (int)numericUpDown161.Value;
        }

        private void numericUpDown162_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[61].center_y = DataController.SETDATA.led_y[61] = (int)numericUpDown162.Value;
        }

        private void numericUpDown163_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[60].center_x = DataController.SETDATA.led_x[60] = (int)numericUpDown163.Value;
        }

        private void numericUpDown164_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[60].center_y = DataController.SETDATA.led_y[60] = (int)numericUpDown164.Value;
        }

        private void numericUpDown165_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[59].center_x = DataController.SETDATA.led_x[59] = (int)numericUpDown165.Value;
        }

        private void numericUpDown166_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[59].center_y = DataController.SETDATA.led_y[59] = (int)numericUpDown166.Value;
        }

        private void numericUpDown167_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[58].center_x = DataController.SETDATA.led_x[58] = (int)numericUpDown167.Value;
        }

        private void numericUpDown168_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[58].center_y = DataController.SETDATA.led_y[58] = (int)numericUpDown168.Value;
        }

        private void numericUpDown169_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[57].center_x = DataController.SETDATA.led_x[57] = (int)numericUpDown169.Value;
        }

        private void numericUpDown170_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[57].center_y = DataController.SETDATA.led_y[57] = (int)numericUpDown170.Value;
        }

        private void numericUpDown171_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[56].center_x = DataController.SETDATA.led_x[56] = (int)numericUpDown171.Value;
        }

        private void numericUpDown172_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[56].center_y = DataController.SETDATA.led_y[56] = (int)numericUpDown172.Value;
        }

        private void numericUpDown173_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[55].center_x = DataController.SETDATA.led_x[55] = (int)numericUpDown173.Value;
        }

        private void numericUpDown174_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[55].center_y = DataController.SETDATA.led_y[55] = (int)numericUpDown174.Value;
        }

        private void numericUpDown175_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[54].center_x = DataController.SETDATA.led_x[54] = (int)numericUpDown175.Value;
        }

        private void numericUpDown176_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[54].center_y = DataController.SETDATA.led_y[54] = (int)numericUpDown176.Value;
        }

        private void numericUpDown124_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[44].center_y = DataController.SETDATA.led_y[44] = (int)numericUpDown124.Value;
        }

        private void numericUpDown160_ValueChanged(object sender, EventArgs e)
        {
            CheckerManager.m_DataController.ledtable[62].center_y = DataController.SETDATA.led_y[62] = (int)numericUpDown160.Value;
        }

        private void numericUpDown178_ValueChanged(object sender, EventArgs e)
        {
            DataController.SETDATA.circleArea = (int)numericUpDown178.Value;
		}

        private void numericUpDown191_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.defaultStep = (int)numericUpDown191.Value;
        }

        private void numericUpDown177_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown177.Value < (numericUpDown179.Value + 1))
            {
                numericUpDown177.Value = numericUpDown179.Value + 1;
            }
            if (numericUpDown177.Value > numericUpDown177.Maximum)
            {
                numericUpDown177.Value = numericUpDown177.Maximum;
            }
            DataController.SETDATA.limit_1 = (int)numericUpDown177.Value;
        }

        private void numericUpDown179_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown179.Value > (numericUpDown177.Value - 1))
            {
                numericUpDown179.Value = numericUpDown177.Value - 1;
            }
            if (numericUpDown179.Value < (numericUpDown180.Value + 1))
            {
                numericUpDown179.Value = numericUpDown180.Value + 1;
            }
//            numericUpDown177.Value = numericUpDown179.Value + 1;
            DataController.SETDATA.limit_2 = (int)numericUpDown179.Value;
        }

        private void numericUpDown180_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown180.Value > (numericUpDown179.Value - 1))
            {
                numericUpDown180.Value = numericUpDown179.Value - 1;
            }
            if (numericUpDown180.Value < (numericUpDown181.Value + 1))
            {
                numericUpDown180.Value = numericUpDown181.Value + 1;
            }
//            numericUpDown179.Value = numericUpDown180.Value + 1;
            DataController.SETDATA.limit_3 = (int)numericUpDown180.Value;
        }

        private void numericUpDown181_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown181.Value > (numericUpDown180.Value - 1))
            {
                numericUpDown181.Value = numericUpDown180.Value - 1;
            }
            if (numericUpDown181.Value < (numericUpDown182.Value + 1))
            {
                numericUpDown181.Value = numericUpDown182.Value + 1;
            }
//            numericUpDown180.Value = numericUpDown181.Value + 1;
            DataController.SETDATA.limit_4 = (int)numericUpDown181.Value;
        }

        private void numericUpDown182_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown182.Value > (numericUpDown181.Value - 1))
            {
                numericUpDown182.Value = numericUpDown181.Value - 1;
            }
            if (numericUpDown182.Value < (numericUpDown183.Value + 1))
            {
                numericUpDown182.Value = numericUpDown183.Value + 1;
            }
//            numericUpDown181.Value = numericUpDown182.Value + 1;
            DataController.SETDATA.limit_5 = (int)numericUpDown182.Value;
        }

        private void numericUpDown183_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown183.Value > (numericUpDown182.Value - 1))
            {
                numericUpDown183.Value = numericUpDown182.Value - 1;
            }
            if (numericUpDown183.Value < (numericUpDown184.Value + 1))
            {
                numericUpDown183.Value = numericUpDown184.Value + 1;
            }
//            numericUpDown182.Value = numericUpDown183.Value + 1;
            DataController.SETDATA.limit_6 = (int)numericUpDown183.Value;
        }

        private void numericUpDown184_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown184.Value < numericUpDown184.Minimum)
            {
                numericUpDown184.Value = numericUpDown184.Minimum;
            }
            if ((numericUpDown183.Value - 1) < numericUpDown184.Value)
            {
                numericUpDown184.Value = numericUpDown183.Value - 1;
            }
            DataController.SETDATA.limit_7 = (int)numericUpDown184.Value;
        }

        private void numericUpDown185_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_1_2 = (int)numericUpDown185.Value;
        }

        private void numericUpDown186_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_2_3 = (int)numericUpDown186.Value;
        }

        private void numericUpDown187_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_3_4 = (int)numericUpDown187.Value;
        }

        private void numericUpDown188_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_4_5 = (int)numericUpDown188.Value;
        }

        private void numericUpDown189_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_5_6 = (int)numericUpDown189.Value;
        }

        private void numericUpDown190_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.step_6_7 = (int)numericUpDown190.Value;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                label48.Text = "外形面積";
                label74.Text = "最少外形面積";
            }
            else
            {
                radioButton2.Checked = true;
                label48.Text = "白色数";
                label74.Text = "最少白色数";
            }
        }

        private void numericUpDown192_ValueChanged(object sender, EventArgs e)
        {
			if((numericUpDown193.Value - 1) < numericUpDown192.Value)
			{
				numericUpDown192.Value = numericUpDown193.Value - 1;
			}

			numericUpDown49.Value = numericUpDown192.Value - 1;
			DataController.SETDATA.TrdThreshMin = TrdThreshMin = (int)numericUpDown192.Value;
        }

        private void numericUpDown193_ValueChanged(object sender, EventArgs e)
        {
			if(numericUpDown193.Value < (numericUpDown192.Value + 1))
			{
				numericUpDown193.Value = numericUpDown192.Value + 1;
			}
			if(numericUpDown193.Value > numericUpDown193.Maximum)
			{
				numericUpDown193.Value = numericUpDown193.Maximum;
			}
			DataController.SETDATA.TrdThreshMax = TrdThreshMax = (int)numericUpDown193.Value;
        }

        private void numericUpDown194_ValueChanged(object sender, EventArgs e)
        {
			if(numericUpDown194.Value == 0)
			{
				numericUpDown194.Value++;
			}
			DataController.SETDATA.TrdStepCount = TrdStepCount = (int)numericUpDown194.Value;
        }

        private void button17_Click(object sender, EventArgs e)
        {
			CheckerManager.m_MotorController.LedReset();
        }

        private void button16_Click(object sender, EventArgs e)
        {
			CheckerManager.m_MotorController.LedLightOn();
        }

        private void button15_Click(object sender, EventArgs e)
        {
			CheckerManager.m_MotorController.LedLightOff();
        }

        private void button14_Click(object sender, EventArgs e)
        {
			CheckerManager.m_MotorController.LedColumnLightOn();
        }

        private void button13_Click(object sender, EventArgs e)
        {
			CheckerManager.m_MotorController.LedUnitLightOn();
        }

        private void numericUpDown196_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.AfThreshMin = (int)numericUpDown196.Value;
        }

        private void numericUpDown195_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.AfThreshMax = (int)numericUpDown195.Value;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CheckerManager.m_MotorController.MotorMotionTest();
        }

        private void numericUpDown197_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.center_x_zure = (int)numericUpDown197.Value;
        }

        private void numericUpDown198_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.center_y_zure = (int)numericUpDown198.Value;
        }

        private void numericUpDown199_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[0] = (int)numericUpDown199.Value;
        }

        private void numericUpDown200_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[1] = (int)numericUpDown200.Value;
        }

        private void numericUpDown201_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[2] = (int)numericUpDown201.Value;
        }

        private void numericUpDown202_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[3] = (int)numericUpDown202.Value;
        }

        private void numericUpDown203_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[4] = (int)numericUpDown203.Value;
        }

        private void numericUpDown204_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[5] = (int)numericUpDown204.Value;
        }

        private void numericUpDown205_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[6] = (int)numericUpDown205.Value;
        }

        private void numericUpDown206_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[7] = (int)numericUpDown206.Value;
        }

        private void numericUpDown207_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[8] = (int)numericUpDown207.Value;
        }

        private void numericUpDown208_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[9] = (int)numericUpDown208.Value;
        }

        private void numericUpDown209_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[10] = (int)numericUpDown209.Value;
        }

        private void numericUpDown210_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[11] = (int)numericUpDown210.Value;
        }

        private void numericUpDown211_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[12] = (int)numericUpDown211.Value;
        }

        private void numericUpDown212_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[13] = (int)numericUpDown212.Value;
        }

        private void numericUpDown213_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[14] = (int)numericUpDown213.Value;
        }

        private void numericUpDown214_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[15] = (int)numericUpDown214.Value;
        }

        private void numericUpDown215_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[16] = (int)numericUpDown215.Value;
        }

        private void numericUpDown216_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[17] = (int)numericUpDown216.Value;
        }

        private void numericUpDown217_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[18] = (int)numericUpDown217.Value;
        }

        private void numericUpDown218_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[19] = (int)numericUpDown218.Value;
        }

        private void numericUpDown219_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[20] = (int)numericUpDown219.Value;
        }

        private void numericUpDown220_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[21] = (int)numericUpDown220.Value;
        }

        private void numericUpDown221_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[22] = (int)numericUpDown221.Value;
        }

        private void numericUpDown222_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[23] = (int)numericUpDown222.Value;
        }

        private void numericUpDown223_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[24] = (int)numericUpDown223.Value;
        }

        private void numericUpDown224_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[25] = (int)numericUpDown224.Value;
        }

        private void numericUpDown225_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[26] = (int)numericUpDown225.Value;
        }

        private void numericUpDown226_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[27] = (int)numericUpDown226.Value;
        }

        private void numericUpDown227_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[28] = (int)numericUpDown227.Value;
        }

        private void numericUpDown228_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[29] = (int)numericUpDown228.Value;
        }

        private void numericUpDown229_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[30] = (int)numericUpDown229.Value;
        }

        private void numericUpDown230_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[31] = (int)numericUpDown230.Value;
        }

        private void numericUpDown231_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[32] = (int)numericUpDown231.Value;
        }

        private void numericUpDown232_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[33] = (int)numericUpDown232.Value;
        }

        private void numericUpDown233_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[34] = (int)numericUpDown233.Value;
        }

        private void numericUpDown234_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[35] = (int)numericUpDown234.Value;
        }

        private void numericUpDown235_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[36] = (int)numericUpDown235.Value;
        }

        private void numericUpDown236_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[37] = (int)numericUpDown236.Value;
        }

        private void numericUpDown237_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[38] = (int)numericUpDown237.Value;
        }

        private void numericUpDown238_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[39] = (int)numericUpDown238.Value;
        }

        private void numericUpDown239_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[40] = (int)numericUpDown239.Value;
        }

        private void numericUpDown240_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[41] = (int)numericUpDown240.Value;
        }

        private void numericUpDown241_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[42] = (int)numericUpDown241.Value;
        }

        private void numericUpDown242_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[43] = (int)numericUpDown242.Value;
        }

        private void numericUpDown243_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[44] = (int)numericUpDown243.Value;
        }

        private void numericUpDown244_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[45] = (int)numericUpDown244.Value;
        }

        private void numericUpDown245_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[46] = (int)numericUpDown245.Value;
        }

        private void numericUpDown246_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[47] = (int)numericUpDown246.Value;
        }

        private void numericUpDown247_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[48] = (int)numericUpDown247.Value;
        }

        private void numericUpDown248_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[49] = (int)numericUpDown248.Value;
        }

        private void numericUpDown249_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[50] = (int)numericUpDown249.Value;
        }

        private void numericUpDown250_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[51] = (int)numericUpDown250.Value;
        }

        private void numericUpDown251_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[52] = (int)numericUpDown251.Value;
        }

        private void numericUpDown252_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[53] = (int)numericUpDown252.Value;
        }

        private void numericUpDown253_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[54] = (int)numericUpDown253.Value;
        }

        private void numericUpDown254_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[55] = (int)numericUpDown254.Value;
        }

        private void numericUpDown255_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[56] = (int)numericUpDown255.Value;
        }

        private void numericUpDown256_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[57] = (int)numericUpDown256.Value;
        }

        private void numericUpDown257_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[58] = (int)numericUpDown257.Value;
        }

        private void numericUpDown258_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[59] = (int)numericUpDown258.Value;
        }

        private void numericUpDown259_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[60] = (int)numericUpDown259.Value;
        }

        private void numericUpDown260_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[61] = (int)numericUpDown260.Value;
        }

        private void numericUpDown261_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.led_area_limit[62] = (int)numericUpDown261.Value;
        }

        private void numericUpDown262_ValueChanged(object sender, EventArgs e)
        {
			DataController.SETDATA.lineFlareThresh = (int)numericUpDown262.Value;
        }

        public void GetPreviewWindow(out IntPtr imagehandle)
        {
            imagehandle = this.Handle;
        }
    }

}
