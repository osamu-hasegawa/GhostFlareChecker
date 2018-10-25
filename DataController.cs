using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace GhostFlareChecker
{
	public class DataController
	{
		public TriangleInfo[] triInfo;
		public CircleInfo[] circleInfo;
		public SumInfo sumInfo;
		public LogInfo logInfo;
		public AF_INFO[] af_info;
		public LedTable[] ledtable;
		public static int gPos = 0xFF;
		public static int colorIndex = 1;

		static public SYSSET SETDATA = new SYSSET();
		
		public class SYSSET:System.ICloneable
		{
			//Image関連 start
			public int sMin;
			public int sMax;
			public int lMin;
			public int lMax;
			public double cMin;
			public double cMax;
			public int stepCount;
			public int distanceThresh;
			public int center_x_Thresh;
			public int center_y_Thresh;
			public int center_x_hosei;
			public int center_y_hosei;
			public int FstThreshMin;
			public int FstThreshMax;
			public int FstStepCount;
			public int SndThreshMin;
			public int SndThreshMax;
			public int SndStepCount;
			public int TrdThreshMin;
			public int TrdThreshMax;
			public int TrdStepCount;
			
			public int center_x_zure;
			public int center_y_zure;

			public int[] led_x = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0};
			public int[] led_y = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
								  0, 0, 0};
			public int[] led_area_limit = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
										  0, 0, 0};
			public int lineFlareThresh;

			public int circleArea;


			//Image関連 end

			//Camera関連 start
            public int waitTime;
            public bool isMirrorV;
            public bool isMirrorH;
            public int brightness;
            public int contrast;
            public int hue;
            public int saturation;

            public int sharpness;
            public int bayerGainRed;
            public int bayerGainGreen;
            public int bayerGainBlue;
			public bool auto;
            public int gamma;

            public int globalGain;
            public int gainRed;
            public int gainGreen1;
            public int gainGreen2;
            public int colorGainBlue;
            public int exposureTime;
            public int halfClock;

            public int MonitorColorMode;
            public int cameraType;
            public int width;
            public int height;
            public int Fps;
            public long dllVersion;
            public int cameraInfo;
            public int cameraStatus;

			public int af_left;//AF時の矩形：左端X座標
			public int af_top;//AF時の矩形：左端Y座標
			public int af_right;//AF時の矩形：右X座標
			public int af_buttom;//AF時の矩形：右Y座標

			public int AfThreshMin;
			public int AfThreshMax;
			//Camera関連 end


			//Motor関連 start
			public int[] Unit = { 0, 0, 0, 0 };
			public int[] Axis = { 0, 0, 0, 0 };
			public int[] Origin = { 0, 0, 0, 0 };
			public int[] ORG_drv = { 0, 0, 0, 0 };
			public int[] Jspd = { 0, 0, 0, 0 };
			public int[] Hspd = { 0, 0, 0, 0 };
			public int[] Lspd = { 0, 0, 0, 0 };
			public int[] StdPLS = { 0, 0, 0, 0 };
			public int[] L_LimitPLS = { 0, 0, 0, 0 };
			public int[] U_LimitPLS = { 0, 0, 0, 0 };

			public string MasterSerial;
			public int MasDepth;
			public int AreaDepth;
			public double MasMTF1;
			public double AreaMTF1;
			public double MasMTF2;
			public double AreaMTF2;
			public double MasMTF3;
			public double AreaMTF3;
			public double MasFOV;
			public double AreaFOV;
			public double MasTilt;
			public double AreaTilt;
			public int MasDrct;
			public int AreaDrct;
			public int MasCof;
			public int AreaCof;
			public int MasHens;
			public int AreaHens;
			public int MasHenV;
			public int AreaHenV;
			public int MasHenH;
			public int AreaHenH;
			public int MasFcs;
			public int AreaFcs;

			public double[][] MTFHosei;
			public double[] sp_MTFCent2 = { 0, 0, 0};
			public int sp_DataMatrix;
			public int sp_Depth2;
            public double MasterLastCheckF;
            public string MasterLastCheckS;
            public string model;
            public int gouki;
            public int[] B_OffSet = { 0, 0, 0, 0 };
            public int[] F_OffSet = { 0, 0, 0, 0 };

			public int limit_1;
			public int limit_2;
			public int limit_3;
			public int limit_4;
			public int limit_5;
			public int limit_6;
			public int limit_7;
			public int step_1_2;
			public int step_2_3;
			public int step_3_4;
			public int step_4_5;
			public int step_5_6;
			public int step_6_7;
			public int defaultStep;

			//Motor関連 end

            public bool load(ref SYSSET ss)
			{
				string path = GET_DOC_PATH("SettingData.xml");
				bool ret = false;
				try {
					XmlSerializer sz = new XmlSerializer(typeof(SYSSET));
					System.IO.StreamReader fs = new System.IO.StreamReader(path, System.Text.Encoding.Default);
					SYSSET obj;
					obj = (SYSSET)sz.Deserialize(fs);
					fs.Close();
					obj = (SYSSET)obj.Clone();
					ss = obj;
					ret = true;
				}
				catch (Exception /*ex*/) {
				}
				return(ret);
			}

			public Object Clone()
			{
				SYSSET cln = (SYSSET)this.MemberwiseClone();
				return (cln);
			}

			public bool save(SYSSET ss)
			{
				string path = GET_DOC_PATH("SettingData.xml");
				bool ret = false;
				try {
					XmlSerializer sz = new XmlSerializer(typeof(SYSSET));
					System.IO.StreamWriter fs = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
					sz.Serialize(fs, ss);
					fs.Close();
					ret = true;
				}
				catch (Exception /*ex*/) {
				}
				return (ret);
			}
		}
			
		static public string GET_DOC_PATH(string file)
		{
			string path;
			path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			path += @"\KOP";
			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}
			path += @"\" + Application.ProductName;
			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}
			if (!string.IsNullOrEmpty(file)) {
				if (file[0] != '\\') {
					path += "\\";
				}
				path += file;
			}

			return (path);
		}
		
		static private void ReadDataFromXml()
		{
            SETDATA.load(ref DataController.SETDATA);
		}

		public DataController()
		{
		}

		public void Init()
		{
			ReadDataFromXml();
			InitTriangleInfo();
			InitCircleInfo();
			InitAfInfo();
			InitLedTable();
			CSV_HEADER_WRITE();
		}

		public void Release()
		{
			WriteDataToXml();
		}


		public string GetLedCSVFileName(string str, int ledPos)
		{
			//日付、時間を取得
			DateTime dt = DateTime.Now;
			string pathstr = string.Format("{0:D04}{1:D02}{2:D02}{3:D02}{4:D02}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute);
			string pathdst = GET_DOC_PATH(pathstr);

			//S/Nをヘッダに付ける
			string lenzID = GetLenzeID();
			pathdst = pathdst + "_" + lenzID;

			if(!Directory.Exists(pathdst))
			{
				Directory.CreateDirectory(pathdst);
			}

            string pathAndName = pathdst + "\\" + pathstr + "_" + lenzID + "_" + ledPos.ToString() + str;
            return pathAndName;
		}

		public string GetLenzeID()
		{
			return "L12345678";//TODO
		}

		public string GetCSVFileName(string str)
		{
			//日付を取得
			DateTime dt = DateTime.Now;
			string pathstr = string.Format("{0:D04}{1:D02}{2:D02}", dt.Year, dt.Month, dt.Day);
			string pathdst = GET_DOC_PATH(pathstr);

			if(!Directory.Exists(pathdst))
			{
				Directory.CreateDirectory(pathdst);
			}

            string pathAndName = pathdst + "\\" + pathstr + str;
            return pathAndName;
		}

		public string GetImageFileName(int sign)
		{
			//日付を取得
			DateTime dt = DateTime.Now;

			string pathstr = string.Format("{0:D04}{1:D02}{2:D02}", dt.Year, dt.Month, dt.Day);
			string pathdst = GET_DOC_PATH(pathstr);

			if(!Directory.Exists(pathdst))
			{
				Directory.CreateDirectory(pathdst);
			}

			string signStr = null;
			if(sign == 0)
			{
				signStr = "_ETC";
			}else if(sign == 1)
			{
				signStr = "_RAW";
			}else if(sign == 2)
			{
				signStr = "_ANA";
			}else if(sign == 3)
			{
				signStr = "_CHART_CENTER";
			}else if(sign == 4)
			{
				signStr = "_CHART_RADIAL";
			}
			pathdst += "\\" + signStr;
			if(!Directory.Exists(pathdst))
			{
				Directory.CreateDirectory(pathdst);
			}

			pathstr += "_";
			pathstr += string.Format("{0:D02}{1:D02}{2:D02}{3:D03}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
			pathstr += signStr;
			pathstr += ".png";

            string pathAndName = pathdst + "\\" + pathstr;
            return pathAndName;
		}

		static public void WriteDataToXml()
		{
            SETDATA.save(DataController.SETDATA);
		}

		
		public bool CopySettings(string filename)
		{
			string path;
			path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			path += @"\KOP";
			if (!System.IO.Directory.Exists(path)) {
				return(false);
			}
			path += @"\" + Application.ProductName;
			if (!System.IO.Directory.Exists(path)) {
				return(false);
			}
			path += @"\";
			path += filename;
			if (!System.IO.File.Exists(path)) {
				return(false);
			}
			try {
				string path_dst = GET_DOC_PATH(filename);
				if (System.IO.File.Exists(path_dst)) {
					//backupを作成
					DateTime dt = System.IO.File.GetLastWriteTime(path_dst);
					string file_base = System.IO.Path.GetFileNameWithoutExtension(filename);
					string file_ext = System.IO.Path.GetExtension(filename);
					string path_bak = file_base;
					path_bak += string.Format("-{0:D04}{1:D02}{2:D02}", dt.Year, dt.Month, dt.Day);
					path_bak += string.Format("-{0:D02}h{1:D02}m{2:D02}s", dt.Hour, dt.Minute, dt.Second);
					path_bak += file_ext;
					path_bak = GET_DOC_PATH(path_bak);
					System.IO.File.Copy(path_dst, path_bak, true);
				}
				System.IO.File.Copy(path, path_dst, true);
				System.IO.File.Delete(path);
			}
			catch (Exception ex) {
                ex.GetType();
				return(false);
			}
			return(true);
		}
		
		public struct TriangleInfo
		{
			public int	x;			//X座標
			public int	y;			//Y座標
			public int	yokosen;	//横線：底辺
			public int	tatesen;	//縦線
			public int	shasen;		//斜線
			public int	theta;		//斜線と底辺に挟まれた角度
		}

		public struct CircleInfo
		{
			public double	s;		//面積
			public double	l;		//周囲長
			public double	c;		//円形度
			public int	x;		//円の中心X座標
			public int	y;		//円の中心Y座標
			public int rgb;		//描画色
		}

		public struct SumInfo//カメラ１回の撮影ごとのデータ
		{
			public int n;				//総数
            public int sum_x;			//X座標の合計
			public int sum_y;			//Y座標の合計
			public int sum_xv;			//X座標の二乗の合計
			public int sum_yv;			//Y座標の二乗の合計
			public double mean_x;		//X座標の平均
			public double variance_x;	//X座標の分散
			public double stddev_x;	//X座標の標準偏差
			public double mean_y;		//Y座標の平均
			public double variance_y;	//Y座標の分散
			public double stddev_y;	//Y座標の標準偏差
			public double enkeido;	//円形度(最外形)
			public double shuicho;	//周囲長(最外形)
			public double menseki;	//面積(最外形)

			public int fst_most_outside_x;			//最外形の1番目の島のX座標
			public int fst_most_outside_y;			//最外形の1番目の島のY座標
			public double fst_most_outside_menseki;	//最外形の1番目の島の面積
			public double fst_most_outside_shuicho;	//最外形の1番目の島の周囲長
			public double fst_most_outside_enkeido;	//最外形の1番目の島の円形度

			public int snd_most_outside_x;			//最外形の2番目の島のX座標
			public int snd_most_outside_y;			//最外形の2番目の島のY座標
			public double snd_most_outside_menseki;	//最外形の2番目の島の面積
			public double snd_most_outside_shuicho;	//最外形の2目の島の周囲長
			public double snd_most_outside_enkeido;	//最外形の2番目の島の円形度

			public int trd_most_outside_x;			//最外形の3番目の島のX座標
			public int trd_most_outside_y;			//最外形の3番目の島のY座標
			public double trd_most_outside_menseki; //最外形の3番目の島の面積
			public double trd_most_outside_shuicho; //最外形の3目の島の周囲長
			public double trd_most_outside_enkeido; //最外形の3番目の島の円形度

			public double sum_most_outside_menseki; //最外形の面積合計

			public int fst_most_inside_x;			//1つ内側の1番目の島のX座標
			public int fst_most_inside_y;			//1つ内側の1番目の島のY座標
			public double fst_most_inside_menseki;	//1つ内側の1番目の島の面積
			public double fst_most_inside_shuicho;	//1つ内側の1番目の島の周囲長
			public double fst_most_inside_enkeido;	//1つ内側の1番目の島の円形度

			public int snd_most_inside_x;			//1つ内側の2番目の島のX座標
			public int snd_most_inside_y;			//1つ内側の2番目の島のY座標
			public double snd_most_inside_menseki;	//1つ内側の2番目の島の面積
			public double snd_most_inside_shuicho;	//1つ内側の2目の島の周囲長
			public double snd_most_inside_enkeido;	//1つ内側の2番目の島の円形度

			public int trd_most_inside_x;			//1つ内側の3番目の島のX座標
			public int trd_most_inside_y;			//1つ内側の3番目の島のY座標
			public double trd_most_inside_menseki; //1つ内側の3番目の島の面積
			public double trd_most_inside_shuicho; //1つ内側の3目の島の周囲長
			public double trd_most_inside_enkeido; //1つ内側の3番目の島の円形度

			public double sum_most_inside_menseki; //1つ内側の面積合計

			public int numberOfoutside;				//最外形の島の数
			public int numberOfinside;				//1つ内側の島の数

			public int max_brightness_center_x;//最大輝度から求めたX座標
			public int max_brightness_center_y;//最大輝度から求めたX座標
			public int ledPosition;				//点灯したLEDの位置
			public int maxBrightnessCount;		//最大輝度の数
			
			public int maxLine;//最大輝度中心から外形までの最大距離
			public int minLine;//最大輝度中心から外形までの最小距離
		}

		public struct LogInfo
		{
			public int FstThreshMin;//(前半)2値化の閾値MIN
			public int FstThreshMax;//(前半)2値化の閾値MAX
			public int FstStepCount;//(前半)刻み幅
			public int SndThreshMin;//(中盤)2値化の閾値MIN
			public int SndThreshMax;//(中盤)2値化の閾値MAX
			public int SndStepCount;//(中盤)刻み幅
			public int TrdThreshMin;//(後半)2値化の閾値MIN
			public int TrdThreshMax;//(後半)2値化の閾値MAX
			public int TrdStepCount;//(後半)刻み幅
			public string z_address;	//Z軸のアドレス
			public string hankei_kakudoFile;//半径-角度グラフのファイル名
			public string chu_shintenFile;//中心点座標グラフのファイル名
			public string rawFileName;//生画像のファイル名
			public string anaFileName;//解析画像のファイル名
			public string ghost_or_not;//GHOSTの判定
			public int flare_hankei_avarage;//FLARE向け半径の平均値
			public int flare_thresh;//FLAREの閾値
			public string flare_or_not;//FLAREの判定
			public double UpperMenseki;//検出する面積の上限
			public string lenzeNumber;//レンズのシリアルナンバー
		}

		public struct AF_INFO
		{
			public int step;
			public int address;
			public int pulse;
			public int whiteCount;
			public double contrast;
			public double menseki;
			public double shuicho;
			public double enkeido;
		}

		//63個のLED毎の情報を、構造体配列で格納
		public struct LedTable
		{
			public int	center_x;			//画面から設定したX座標
			public int	center_y;			//画面から設定したY座標

			public int	outside_center_x;		//最外形のX座標(最初に検出した最外形のもの)：GHOST検出用
			public int	outside_center_y;		//最外形のY座標(最初に検出した最外形のもの)：GHOST検出用

			public int	max_brightness_center_x;//最大輝度から求めたX座標
			public int	max_brightness_center_y;//最大輝度から求めたX座標

			public bool isPass;					//そのLED光源を通過したか
			public int  result;					//OK or NG(Ghost or Flare)

			public int fst_most_outside_x;			//最外形の1番目の島のX座標
			public int fst_most_outside_y;			//最外形の1番目の島のY座標
			public double fst_most_outside_menseki;	//最外形の1番目の島の面積
			public double fst_most_outside_shuicho;	//最外形の1番目の島の周囲長
			public double fst_most_outside_enkeido;	//最外形の1番目の島の円形度
			public int snd_most_outside_x;			//最外形の2番目の島のX座標
			public int snd_most_outside_y;			//最外形の2番目の島のY座標
			public double snd_most_outside_menseki;	//最外形の2番目の島の面積
			public double snd_most_outside_shuicho;	//最外形の2目の島の周囲長
			public double snd_most_outside_enkeido;	//最外形の2番目の島の円形度
			public int trd_most_outside_x;			//最外形の3番目の島のX座標
			public int trd_most_outside_y;			//最外形の3番目の島のY座標
			public double trd_most_outside_menseki; //最外形の3番目の島の面積
			public double trd_most_outside_shuicho; //最外形の3目の島の周囲長
			public double trd_most_outside_enkeido; //最外形の3番目の島の円形度

			public double sum_most_outside_menseki; //最外形の面積合計
			public int numberOfoutside;				//最外形の島の数

			public int maxBrightnessCount;			//最大輝度の数

			public int fst_most_inside_x;			//1つ内側の1番目の島のX座標
			public int fst_most_inside_y;			//1つ内側の1番目の島のY座標
			public double fst_most_inside_menseki;	//1つ内側の1番目の島の面積
			public double fst_most_inside_shuicho;	//1つ内側の1番目の島の周囲長
			public double fst_most_inside_enkeido;	//1つ内側の1番目の島の円形度
			public int snd_most_inside_x;			//1つ内側の2番目の島のX座標
			public int snd_most_inside_y;			//1つ内側の2番目の島のY座標
			public double snd_most_inside_menseki;	//1つ内側の2番目の島の面積
			public double snd_most_inside_shuicho;	//1つ内側の2目の島の周囲長
			public double snd_most_inside_enkeido;	//1つ内側の2番目の島の円形度
			public int trd_most_inside_x;			//1つ内側の3番目の島のX座標
			public int trd_most_inside_y;			//1つ内側の3番目の島のY座標
			public double trd_most_inside_menseki; //1つ内側の3番目の島の面積
			public double trd_most_inside_shuicho; //1つ内側の3目の島の周囲長
			public double trd_most_inside_enkeido; //1つ内側の3番目の島の円形度

			public double sum_most_inside_menseki; //1つ内側の面積合計
			public int numberOfinside;				//1つ内側の島の数

			public int maxLine;//最大輝度中心から外形までの最大距離
			public int minLine;//最大輝度中心から外形までの最小距離
		}

		public void InitTriangleInfo()
		{
			triInfo = new TriangleInfo[2048];
		}

		public void InitCircleInfo()
		{
            circleInfo = new CircleInfo[2048];
		}

		public void InitAfInfo()
		{
			af_info = new AF_INFO[1024];
		}

		public void InitLedTable()
		{
			ledtable = new LedTable[63];//63:LED全数

            for (int i = 0; i < ledtable.Length; i++)
            {
				ledtable[i].center_x = SETDATA.led_x[i];
				ledtable[i].center_y = SETDATA.led_y[i];
			}


		}

		public int MakeDrawColor(int shellColor)
		{
			int rgb = 0;
#if false//ランダムに色を設定する場合
			Random rnd = new System.Random(); // インスタンスを生成
			rgb = rnd.Next(0xFFFFFF);        // RGBの乱数を取得
#else//固定色にする場合
			//後で変換するので逆になる
			switch(shellColor)
			{
			case 1:
				rgb = 0xFF0000;//青
				break;
			case 2:
				rgb = 0x00FF00;//緑
				break;
			case 3:
				rgb = 0x00FFFF;//黄
				break;
			case 4:
				rgb = 0xCC00C4;//紫
				break;
			case 5:
				rgb = 0x4CB7FF;//オレンジ
				break;
			case 6:
				rgb = 0x7600E5;//マゼンダ
				break;
			case 7:
				rgb = 0xE98743;//ターコイズブルー
				break;
			case 8:
				rgb = 0x436F88;//カーキ
				break;
			case 9:
				rgb = 0x7EEA47;//エメラルドグリーン
				break;
			default:
				Random rnd = new System.Random(); // インスタンスを生成
				rgb = rnd.Next(0xFFFFFF);        // RGBの乱数を取得
				break;
			}
			colorIndex++;
#endif
			rgb.ToString("x4");
			return rgb;
		}

		public void SetCircleColor()
		{
			colorIndex = 1;
		}

		public int GetDrawColor(int i)
		{
			int color = circleInfo[i].rgb;
			
			int r = ((color & 0x0000FF) << 16);
			int g = (color & 0x00FF00);
			int b = ((color & 0xFF0000) >> 16);

			color = r | g | b;
			color.ToString("x4");
			return color;
		}

		public void  SetValueXY(int cnt, int x, int y)
		{
			triInfo[cnt].x = x;
			triInfo[cnt].y = y;
		}

		public void  SetCircleInfo(int i, double s, double l, double c, int x, int y, int rgb)
		{
			circleInfo[i].s = s;
			circleInfo[i].l = l;
			circleInfo[i].c = c;
			circleInfo[i].x = x;
			circleInfo[i].y = y;
			circleInfo[i].rgb = rgb;
		}

		public void InitLogData()
		{
			logInfo.FstThreshMin = 0;
			logInfo.FstThreshMax = 0;
			logInfo.FstStepCount = 0;
			logInfo.SndThreshMin = 0;
			logInfo.SndThreshMax = 0;
			logInfo.SndStepCount = 0;
			logInfo.TrdThreshMin = 0;
			logInfo.TrdThreshMax = 0;
			logInfo.TrdStepCount = 0;
			logInfo.z_address = "";
			logInfo.hankei_kakudoFile = "";
			logInfo.chu_shintenFile = "";
			logInfo.rawFileName = "";
			logInfo.anaFileName = "";
			logInfo.ghost_or_not = "";
			logInfo.flare_hankei_avarage = 0;
			logInfo.flare_thresh = 0;
			logInfo.flare_or_not = "";
			logInfo.UpperMenseki = 0;
			logInfo.lenzeNumber = "";

			sumInfo.n = 0;
			sumInfo.sum_x = 0;
			sumInfo.sum_y = 0;
			sumInfo.sum_xv = 0;
			sumInfo.sum_yv = 0;
			sumInfo.mean_x = 0;
			sumInfo.variance_x = 0;
			sumInfo.stddev_x = 0;
			sumInfo.mean_y = 0;
			sumInfo.variance_y = 0;
			sumInfo.stddev_y = 0;

			sumInfo.numberOfoutside = 0;
			sumInfo.fst_most_outside_x = 0;
			sumInfo.fst_most_outside_y = 0;
			sumInfo.fst_most_outside_menseki = 0;
			sumInfo.fst_most_outside_shuicho = 0;
			sumInfo.fst_most_outside_enkeido = 0;
			sumInfo.snd_most_outside_x = 0;
			sumInfo.snd_most_outside_y = 0;
			sumInfo.snd_most_outside_menseki = 0;
			sumInfo.snd_most_outside_shuicho = 0;
			sumInfo.snd_most_outside_enkeido = 0;

			sumInfo.trd_most_outside_x = 0;
			sumInfo.trd_most_outside_y = 0;			
			sumInfo.trd_most_outside_menseki = 0;
			sumInfo.trd_most_outside_shuicho = 0;
			sumInfo.trd_most_outside_enkeido = 0;

			sumInfo.sum_most_outside_menseki = 0;

			sumInfo.fst_most_inside_x = 0;
			sumInfo.fst_most_inside_y = 0;
			sumInfo.fst_most_inside_menseki = 0;
			sumInfo.fst_most_inside_shuicho = 0;
			sumInfo.fst_most_inside_enkeido = 0;
			sumInfo.snd_most_inside_x = 0;
			sumInfo.snd_most_inside_y = 0;
			sumInfo.snd_most_inside_menseki = 0;
			sumInfo.snd_most_inside_shuicho = 0;
			sumInfo.snd_most_inside_enkeido = 0;
			sumInfo.trd_most_inside_x = 0;
			sumInfo.trd_most_inside_y = 0;
			sumInfo.trd_most_inside_menseki = 0;
			sumInfo.trd_most_inside_shuicho = 0;
			sumInfo.trd_most_inside_enkeido = 0;

			sumInfo.sum_most_inside_menseki = 0;

			sumInfo.numberOfinside = 0;

			sumInfo.max_brightness_center_x = 0;
			sumInfo.max_brightness_center_y = 0;
			sumInfo.ledPosition = 0;
			sumInfo.maxBrightnessCount = 0;

			sumInfo.maxLine = 0;
			sumInfo.minLine = 0;
		}

		public void CSV_HEADER_WRITE()
		{
			string buf = "";
			string csvpath = GetCSVFileName("log.csv");
			StreamWriter wr;
			DateTime dt = DateTime.Now;
			try {
				wr = new StreamWriter(csvpath, true, Encoding.Default);

				buf = string.Format("時刻");
				buf += string.Format(",レンズ番号");
				buf += string.Format(",LED位置");
				buf += string.Format(",境界数");
				buf += string.Format(",X座標の合計");
				buf += string.Format(",Y座標の合計");
				buf += string.Format(",X座標の二乗の合計");
				buf += string.Format(",Y座標の二乗の合計");
				buf += string.Format(",X座標の平均");
				buf += string.Format(",X座標の分散");
				buf += string.Format(",X座標の標準偏差");
				buf += string.Format(",Y座標の平均");
				buf += string.Format(",Y座標の分散");
				buf += string.Format(",Y座標の標準偏差");

				buf += string.Format(",赤の島数");
				buf += string.Format(",赤1番目X座標");
				buf += string.Format(",赤1番目Y座標");
				buf += string.Format(",赤1番目面積");
				buf += string.Format(",赤1番目周囲長");
				buf += string.Format(",赤1番目円形度");
				buf += string.Format(",赤2番目X座標");
				buf += string.Format(",赤2番目Y座標");
				buf += string.Format(",赤2番目面積");
				buf += string.Format(",赤2番目周囲長");
				buf += string.Format(",赤2番目円形度");
				buf += string.Format(",赤3番目X座標");
				buf += string.Format(",赤3番目Y座標");
				buf += string.Format(",赤3番目面積");
				buf += string.Format(",赤3番目周囲長");
				buf += string.Format(",赤3番目円形度");
				buf += string.Format(",赤面積合計");

				buf += string.Format(",青の島数");
				buf += string.Format(",青1番目X座標");
				buf += string.Format(",青1番目Y座標");
				buf += string.Format(",青1番目面積");
				buf += string.Format(",青1番目周囲長");
				buf += string.Format(",青1番目円形度");
				buf += string.Format(",青2番目X座標");
				buf += string.Format(",青2番目Y座標");
				buf += string.Format(",青2番目面積");
				buf += string.Format(",青2番目周囲長");
				buf += string.Format(",青2番目円形度");
				buf += string.Format(",青3番目X座標");
				buf += string.Format(",青3番目Y座標");
				buf += string.Format(",青3番目面積");
				buf += string.Format(",青3番目周囲長");
				buf += string.Format(",青3番目円形度");
				buf += string.Format(",青面積合計");

				buf += string.Format(",最大輝度のX座標");
				buf += string.Format(",最大輝度のY座標");
				buf += string.Format(",最大輝度数");

				buf += string.Format(",最大輝度中心からの最大距離");
				buf += string.Format(",最大輝度中心からの最小距離");
				buf += string.Format(",最大距離―最小距離");
				buf += string.Format(",距離差の閾値");

				buf += string.Format(",(低)GRAY SCALEの閾値MIN");
				buf += string.Format(",(低)GRAY SCALEの閾値MAX");
				buf += string.Format(",(低)刻み幅");
				buf += string.Format(",(中)GRAY SCALEの閾値MIN");
				buf += string.Format(",(中)GRAY SCALEの閾値MAX");
				buf += string.Format(",(中)刻み幅");
				buf += string.Format(",(高)GRAY SCALEの閾値MIN");
				buf += string.Format(",(高)GRAY SCALEの閾値MAX");
				buf += string.Format(",(高)刻み幅");

				buf += string.Format(",面積上限");

				buf += string.Format(",中心X座標の補正値");
				buf += string.Format(",中心Y座標の補正値");

				buf += string.Format(",Z軸のアドレス");
				buf += string.Format(",半径-角度グラフのファイル名");
				buf += string.Format(",中心点座標グラフのファイル名");
				buf += string.Format(",生画像のファイル名");
				buf += string.Format(",解析画像のファイル名");
				buf += string.Format(",GHOSTのX座標の閾値");
				buf += string.Format(",GHOSTのY座標の閾値");
				buf += string.Format(",GHOST ?");
				buf += string.Format(",FLARE向け半径の平均値");
				buf += string.Format(",FLAREの閾値");
				buf += string.Format(",FLARE ?");

				wr.WriteLine(buf);
				wr.Close();
			}
			catch (Exception) {
			}
		}

		public void CSV_WRITE()
		{
//TODO
			if(sumInfo.ledPosition == 0)
			{
//				return;
			}
			
			string buf = "";
			string csvpath = GetCSVFileName("log.csv");
			StreamWriter wr;
			DateTime dt = DateTime.Now;
			try {
				wr = new StreamWriter(csvpath, true, Encoding.Default);
				buf = string.Format("{0:D02}時{1:D02}分{2:D02}秒{3:D03}", dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

				buf += ",";
				buf += logInfo.lenzeNumber;

				buf += string.Format(",{0}", sumInfo.ledPosition);
				buf += string.Format(",{0}", sumInfo.n);
				buf += string.Format(",{0}", sumInfo.sum_x);
				buf += string.Format(",{0}", sumInfo.sum_y);
				buf += string.Format(",{0}", sumInfo.sum_xv);
				buf += string.Format(",{0}", sumInfo.sum_yv);
				buf += string.Format(",{0:F2}", sumInfo.mean_x);
				buf += string.Format(",{0:F2}", sumInfo.variance_x);
				buf += string.Format(",{0:F2}", sumInfo.stddev_x);
				buf += string.Format(",{0:F2}", sumInfo.mean_y);
				buf += string.Format(",{0:F2}", sumInfo.variance_y);
				buf += string.Format(",{0:F2}", sumInfo.stddev_y);

				buf += string.Format(",{0}", sumInfo.numberOfoutside);
				buf += string.Format(",{0}", sumInfo.fst_most_outside_x);
				buf += string.Format(",{0}", sumInfo.fst_most_outside_y);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_outside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_outside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_outside_enkeido);
				buf += string.Format(",{0}", sumInfo.snd_most_outside_x);
				buf += string.Format(",{0}", sumInfo.snd_most_outside_y);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_outside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_outside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_outside_enkeido);
				buf += string.Format(",{0}", sumInfo.trd_most_outside_x);
				buf += string.Format(",{0}", sumInfo.trd_most_outside_y);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_outside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_outside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_outside_enkeido);
				buf += string.Format(",{0:F2}", sumInfo.sum_most_outside_menseki);

				buf += string.Format(",{0}", sumInfo.numberOfinside);
				buf += string.Format(",{0}", sumInfo.fst_most_inside_x);
				buf += string.Format(",{0}", sumInfo.fst_most_inside_y);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_inside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_inside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.fst_most_inside_enkeido);
				buf += string.Format(",{0}", sumInfo.snd_most_inside_x);
				buf += string.Format(",{0}", sumInfo.snd_most_inside_y);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_inside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_inside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.snd_most_inside_enkeido);
				buf += string.Format(",{0}", sumInfo.trd_most_inside_x);
				buf += string.Format(",{0}", sumInfo.trd_most_inside_y);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_inside_menseki);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_inside_shuicho);
				buf += string.Format(",{0:F2}", sumInfo.trd_most_inside_enkeido);
				buf += string.Format(",{0:F2}", sumInfo.sum_most_inside_menseki);

				buf += string.Format(",{0}", sumInfo.max_brightness_center_x);
				buf += string.Format(",{0}", sumInfo.max_brightness_center_y);
				buf += string.Format(",{0}", sumInfo.maxBrightnessCount);

				buf += string.Format(",{0}", sumInfo.maxLine);
				buf += string.Format(",{0}", sumInfo.minLine);
				buf += string.Format(",{0}", (sumInfo.maxLine - sumInfo.minLine));
				buf += string.Format(",{0}", SETDATA.lineFlareThresh);

				buf += string.Format(",{0}", logInfo.FstThreshMin);
				buf += string.Format(",{0}", logInfo.FstThreshMax);
				buf += string.Format(",{0}", logInfo.FstStepCount);
				buf += string.Format(",{0}", logInfo.SndThreshMin);
				buf += string.Format(",{0}", logInfo.SndThreshMax);
				buf += string.Format(",{0}", logInfo.SndStepCount);
				buf += string.Format(",{0}", logInfo.TrdThreshMin);
				buf += string.Format(",{0}", logInfo.TrdThreshMax);
				buf += string.Format(",{0}", logInfo.TrdStepCount);

				buf += string.Format(",{0:F2}", logInfo.UpperMenseki);

				buf += string.Format(",{0}", SETDATA.center_x_hosei);
				buf += string.Format(",{0}", SETDATA.center_y_hosei);

				buf += ",";
				buf += logInfo.z_address;
				buf += ",";
				buf += logInfo.hankei_kakudoFile;
				buf += ",";
				buf += logInfo.chu_shintenFile;
				buf += ",";
				buf += logInfo.rawFileName;
				buf += ",";
				buf += logInfo.anaFileName;

				buf += string.Format(",{0}", SETDATA.center_x_Thresh);
				buf += string.Format(",{0}", SETDATA.center_y_Thresh);

				buf += ",";
				buf += logInfo.ghost_or_not;

				buf += string.Format(",{0}", logInfo.flare_hankei_avarage);
				buf += string.Format(",{0}", SETDATA.distanceThresh);

				buf += ",";
				buf += logInfo.flare_or_not;

				wr.WriteLine(buf);
				wr.Close();
			}
			catch (Exception) {
			}
		}

		public void CSV_LED_RESULT_WRITE()
		{
			for(int i = 0; i < ledtable.Length; i++)
			{
				string csvpath = GetLedCSVFileName(".csv", (i + 1));
				string buf = "";
				StreamWriter wr;
				try {
					wr = new StreamWriter(csvpath, true, Encoding.Default);

					buf += string.Format("LED番号");
					buf += string.Format(",中心X座標設定値");
					buf += string.Format(",中心Y座標設定値");

					buf += string.Format(",赤の島数");

					buf += string.Format(",赤1番目X座標");
					buf += string.Format(",赤1番目Y座標");
					buf += string.Format(",赤1番目面積");
					buf += string.Format(",赤1番目周囲長");
					buf += string.Format(",赤1番目円形度");

					buf += string.Format(",赤2番目X座標");
					buf += string.Format(",赤2番目Y座標");
					buf += string.Format(",赤2番目面積");
					buf += string.Format(",赤2番目周囲長");
					buf += string.Format(",赤2番目円形度");

					buf += string.Format(",赤3番目X座標");
					buf += string.Format(",赤3番目Y座標");
					buf += string.Format(",赤3番目面積");
					buf += string.Format(",赤3番目周囲長");
					buf += string.Format(",赤3番目円形度");
					buf += string.Format(",赤面積合計");

					buf += string.Format(",青の島数");

					buf += string.Format(",青1番目X座標");
					buf += string.Format(",青1番目Y座標");
					buf += string.Format(",青1番目面積");
					buf += string.Format(",青1番目周囲長");
					buf += string.Format(",青1番目円形度");

					buf += string.Format(",青2番目X座標");
					buf += string.Format(",青2番目Y座標");
					buf += string.Format(",青2番目面積");
					buf += string.Format(",青2番目周囲長");
					buf += string.Format(",青2番目円形度");

					buf += string.Format(",青3番目X座標");
					buf += string.Format(",青3番目Y座標");
					buf += string.Format(",青3番目面積");
					buf += string.Format(",青3番目周囲長");
					buf += string.Format(",青3番目円形度");
					buf += string.Format(",青面積合計");

					buf += string.Format(",最大輝度のX座標");
					buf += string.Format(",最大輝度のY座標");
					buf += string.Format(",最大輝度数");

					buf += string.Format(",最大輝度中心からの最大距離");
					buf += string.Format(",最大輝度中心からの最小距離");
					buf += string.Format(",最大距離―最小距離");

					wr.WriteLine(buf);

					buf = "";
					buf = string.Format("{0}", (i + 1));
					buf += string.Format(",{0}", ledtable[i].center_x);
					buf += string.Format(",{0}", ledtable[i].center_y);

					buf += string.Format(",{0}", ledtable[i].numberOfoutside);

					buf += string.Format(",{0}", ledtable[i].fst_most_outside_x);
					buf += string.Format(",{0}", ledtable[i].fst_most_outside_y);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_outside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_outside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_outside_enkeido);

					buf += string.Format(",{0}", ledtable[i].snd_most_outside_x);
					buf += string.Format(",{0}", ledtable[i].snd_most_outside_y);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_outside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_outside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_outside_enkeido);

					buf += string.Format(",{0}", ledtable[i].trd_most_outside_x);
					buf += string.Format(",{0}", ledtable[i].trd_most_outside_y);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_outside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_outside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_outside_enkeido);
					buf += string.Format(",{0:F2}", ledtable[i].sum_most_outside_menseki);

					buf += string.Format(",{0}", ledtable[i].numberOfinside);

					buf += string.Format(",{0}", ledtable[i].fst_most_inside_x);
					buf += string.Format(",{0}", ledtable[i].fst_most_inside_y);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_inside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_inside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].fst_most_inside_enkeido);

					buf += string.Format(",{0}", ledtable[i].snd_most_inside_x);
					buf += string.Format(",{0}", ledtable[i].snd_most_inside_y);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_inside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_inside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].snd_most_inside_enkeido);

					buf += string.Format(",{0}", ledtable[i].trd_most_inside_x);
					buf += string.Format(",{0}", ledtable[i].trd_most_inside_y);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_inside_menseki);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_inside_shuicho);
					buf += string.Format(",{0:F2}", ledtable[i].trd_most_inside_enkeido);
					buf += string.Format(",{0:F2}", ledtable[i].sum_most_inside_menseki);

					buf += string.Format(",{0}", ledtable[i].max_brightness_center_x);
					buf += string.Format(",{0}", ledtable[i].max_brightness_center_y);
					buf += string.Format(",{0}", ledtable[i].maxBrightnessCount);

					buf += string.Format(",{0}", ledtable[i].maxLine);
					buf += string.Format(",{0}", ledtable[i].minLine);
					buf += string.Format(",{0}", (ledtable[i].maxLine - ledtable[i].minLine));

					wr.WriteLine(buf);

					wr.Close();
				}
				catch (Exception) {
				}
			}
		}



#if false
		public void f_write(string path, CircleInfo dat)
		{
			string buf;
			StreamWriter wr;
			try {
				/*				rd = new StreamReader(filename, Encoding.GetEncoding("Shift_JIS"));*/
				wr = new StreamWriter(path, true, Encoding.Default);

				buf = string.Format("{0}", dat.x);
				buf += string.Format(",{0}", dat.y);
				buf += string.Format(",{0:F2}", dat.s);
				buf += string.Format(",{0:F2}", dat.l);
				buf += string.Format(",{0:F2}", dat.c);

				wr.WriteLine(buf);
				wr.Close();
			}
			catch (Exception) {
			}
		}
#endif	

		//特定の点が、任意の点を中心とした円の内側にあるか。青色＝２番目の外形
		public void IsPointInsideArea(int center_x, int center_y, int numOfin, int outer_x, int outer_y)
		{
            // 円グラフの幅
            int pie_w = SETDATA.circleArea;
            int pie_h = SETDATA.circleArea;

            int half_pie_w = pie_w / 2;
            int half_pie_h = pie_h / 2;
            double point_val = 0;
            double r2 = 0;
            // 円グラフのxy座標
            for(int i = 0; i < ledtable.Length; i++)
            {
	            // 中心座標
	            int o_x = ledtable[i].center_x;
	            int o_y = ledtable[i].center_y;

	            // 半径
	            int r = half_pie_w;
	            
	            // 公式　(x -a)^2 + (y-b)^2 = r^2
	            point_val = (center_x - o_x) * (center_x - o_x) + (center_y - o_y) * (center_y - o_y);
	            r2 = r * r;

	            // 円内か？
	            if (point_val < r2)
	            {
					sumInfo.numberOfinside = numOfin;

						bool isChange = false;
						if(ledtable[i].maxBrightnessCount <= sumInfo.maxBrightnessCount)//認識していても、次の光源最高輝度面積が既存のそれより大きい場合、書き換える
						{
							isChange = true;
						}
					
						if(numOfin == 1)//GHOSTの1つ目の島の場合
						{
							sumInfo.fst_most_inside_x = outer_x;//中心X座標を保存。
							sumInfo.fst_most_inside_y = outer_y;//中心Y座標を保存。

							sumInfo.fst_most_inside_menseki = sumInfo.menseki;
							sumInfo.fst_most_inside_shuicho = sumInfo.shuicho;
							sumInfo.fst_most_inside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_inside_menseki += sumInfo.menseki;//面積の合計
							if(isChange)
							{
								ledtable[i].numberOfinside = numOfin;
								ledtable[i].sum_most_inside_menseki = sumInfo.sum_most_inside_menseki;

								ledtable[i].fst_most_inside_x = outer_x;//中心X座標を保存。
								ledtable[i].fst_most_inside_y = outer_y;//中心Y座標を保存。

								ledtable[i].fst_most_inside_menseki = sumInfo.menseki;
								ledtable[i].fst_most_inside_shuicho = sumInfo.shuicho;
								ledtable[i].fst_most_inside_enkeido = sumInfo.enkeido;

								ledtable[i].snd_most_inside_x = 0;
								ledtable[i].snd_most_inside_y = 0;

								ledtable[i].snd_most_inside_menseki = 0;
								ledtable[i].snd_most_inside_shuicho = 0;
								ledtable[i].snd_most_inside_enkeido = 0;

								ledtable[i].trd_most_inside_x = 0;
								ledtable[i].trd_most_inside_y = 0;

								ledtable[i].trd_most_inside_menseki = 0;
								ledtable[i].trd_most_inside_shuicho = 0;
								ledtable[i].trd_most_inside_enkeido = 0;

							}
						}
						else if(numOfin == 2)
						{
							sumInfo.snd_most_inside_x = outer_x;//中心X座標を保存。
							sumInfo.snd_most_inside_y = outer_y;//中心Y座標を保存。

							sumInfo.snd_most_inside_menseki = sumInfo.menseki;
							sumInfo.snd_most_inside_shuicho = sumInfo.shuicho;
							sumInfo.snd_most_inside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_inside_menseki += sumInfo.menseki;//面積の合計
							if(isChange)
							{
								ledtable[i].numberOfinside = numOfin;
								ledtable[i].sum_most_inside_menseki = sumInfo.sum_most_inside_menseki;

								ledtable[i].snd_most_inside_x = outer_x;//中心X座標を保存。
								ledtable[i].snd_most_inside_y = outer_y;//中心Y座標を保存。

								ledtable[i].snd_most_inside_menseki = sumInfo.menseki;
								ledtable[i].snd_most_inside_shuicho = sumInfo.shuicho;
								ledtable[i].snd_most_inside_enkeido = sumInfo.enkeido;
							}
						}
						else if(numOfin == 3)
						{
							sumInfo.trd_most_inside_x = outer_x;//中心X座標を保存。
							sumInfo.trd_most_inside_y = outer_y;//中心Y座標を保存。

							sumInfo.trd_most_inside_menseki = sumInfo.menseki;
							sumInfo.trd_most_inside_shuicho = sumInfo.shuicho;
							sumInfo.trd_most_inside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_inside_menseki += sumInfo.menseki;//面積の合計
							if(isChange)
							{
								ledtable[i].numberOfinside = numOfin;
								ledtable[i].sum_most_inside_menseki = sumInfo.sum_most_inside_menseki;

								ledtable[i].trd_most_inside_x = outer_x;//中心X座標を保存。
								ledtable[i].trd_most_inside_y = outer_y;//中心Y座標を保存。

								ledtable[i].trd_most_inside_menseki = sumInfo.menseki;
								ledtable[i].trd_most_inside_shuicho = sumInfo.shuicho;
								ledtable[i].trd_most_inside_enkeido = sumInfo.enkeido;
							}
						}
						else//GHOSTの島が4つ以上ある場合
						{
							sumInfo.sum_most_inside_menseki += sumInfo.menseki;//面積の合計
							if(isChange)
							{
								ledtable[i].numberOfinside = numOfin;
								ledtable[i].sum_most_inside_menseki = sumInfo.sum_most_inside_menseki;
							}
						}
						
						break;
				}
			}
		}
					
					

		//特定の点が、任意の点を中心とした円の内側にあるか。赤色＝最外形
		public void IsPointInArea(int center_x, int center_y, bool outside, int numOfout, int most_outer_x, int most_outer_y, int maxLine, int minLine)
		{
            // 円グラフの幅
            int pie_w = SETDATA.circleArea;
            int pie_h = SETDATA.circleArea;

            int half_pie_w = pie_w / 2;
            int half_pie_h = pie_h / 2;
            double point_val = 0;
            double r2 = 0;
			gPos = 0xFF;
			bool isChange = false;
            // 円グラフのxy座標
            for(int i = 0; i < ledtable.Length; i++)
            {
	            // 中心座標
	            int o_x = ledtable[i].center_x;
	            int o_y = ledtable[i].center_y;

	            // 半径
	            int r = half_pie_w;
	            
	            // 公式　(x -a)^2 + (y-b)^2 = r^2
	            point_val = (center_x - o_x) * (center_x - o_x) + (center_y - o_y) * (center_y - o_y);
	            r2 = r * r;

	            // 円内か？
	            if (point_val < r2)
	            {
					sumInfo.numberOfoutside = numOfout;
					
					//そのLED光源をまだ認識していなければ通過と判定
					if(ledtable[i].isPass == false)
					{
						ledtable[i].numberOfoutside = numOfout;
						ledtable[i].isPass = true;//円内
						gPos = i;
						sumInfo.ledPosition = i + 1;//LED位置を保存

			            ledtable[i].max_brightness_center_x = center_x;//最大輝度の中心座標を保存
			            ledtable[i].max_brightness_center_y = center_y;//最大輝度の中心座標を保存
			            sumInfo.max_brightness_center_x = center_x;//最大輝度の中心座標を保存
			            sumInfo.max_brightness_center_y = center_y;//最大輝度の中心座標を保存
	
			            ledtable[i].maxBrightnessCount = sumInfo.maxBrightnessCount;//最大輝度の数を保存

			            ledtable[i].outside_center_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
			            ledtable[i].outside_center_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

						sumInfo.fst_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
						sumInfo.fst_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

						sumInfo.fst_most_outside_menseki = sumInfo.menseki;
						sumInfo.fst_most_outside_shuicho = sumInfo.shuicho;
						sumInfo.fst_most_outside_enkeido = sumInfo.enkeido;

						sumInfo.sum_most_outside_menseki += sumInfo.menseki;//面積の合計
						ledtable[i].sum_most_outside_menseki = sumInfo.sum_most_outside_menseki;

						ledtable[i].fst_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
						ledtable[i].fst_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

						ledtable[i].fst_most_outside_menseki = sumInfo.menseki;
						ledtable[i].fst_most_outside_shuicho = sumInfo.shuicho;
						ledtable[i].fst_most_outside_enkeido = sumInfo.enkeido;

						sumInfo.maxLine = maxLine;//最大輝度中心から外形までの最大距離
						sumInfo.minLine = minLine;//最大輝度中心から外形までの最小距離
						ledtable[i].maxLine = maxLine;//最大輝度中心から外形までの最大距離
						ledtable[i].minLine = minLine;//最大輝度中心から外形までの最小距離
			        }
					else if(ledtable[i].isPass == true)
					{
						gPos = i;
						sumInfo.ledPosition = i + 1;//LED位置を保存
			            sumInfo.max_brightness_center_x = center_x;//最大輝度の中心座標を保存
			            sumInfo.max_brightness_center_y = center_y;//最大輝度の中心座標を保存
						sumInfo.maxLine = maxLine;//最大輝度中心から外形までの最大距離
						sumInfo.minLine = minLine;//最大輝度中心から外形までの最小距離

						if(ledtable[i].maxBrightnessCount <= sumInfo.maxBrightnessCount)//認識していても、次の光源最高輝度面積が既存のそれより大きい場合、書き換える
						{
							ledtable[i].numberOfoutside = numOfout;
				            ledtable[i].max_brightness_center_x = center_x;//最大輝度の中心座標を保存
				            ledtable[i].max_brightness_center_y = center_y;//最大輝度の中心座標を保存

				            ledtable[i].maxBrightnessCount = sumInfo.maxBrightnessCount;//最大輝度の数を保存

							ledtable[i].numberOfoutside = numOfout;
							ledtable[i].maxLine = maxLine;//最大輝度中心から外形までの最大距離
							ledtable[i].minLine = minLine;//最大輝度中心から外形までの最小距離
							isChange = true;
						}

						if(numOfout == 1)//GHOSTの1つ目の島の場合
						{
							sumInfo.fst_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							sumInfo.fst_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							sumInfo.fst_most_outside_menseki = sumInfo.menseki;
							sumInfo.fst_most_outside_shuicho = sumInfo.shuicho;
							sumInfo.fst_most_outside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_outside_menseki += sumInfo.menseki;//面積の合計

							if(isChange)
							{
								ledtable[i].sum_most_outside_menseki = sumInfo.sum_most_outside_menseki;

								ledtable[i].fst_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
								ledtable[i].fst_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

								ledtable[i].fst_most_outside_menseki = sumInfo.menseki;
								ledtable[i].fst_most_outside_shuicho = sumInfo.shuicho;
								ledtable[i].fst_most_outside_enkeido = sumInfo.enkeido;

								ledtable[i].snd_most_outside_x = 0;
								ledtable[i].snd_most_outside_y = 0;

								ledtable[i].snd_most_outside_menseki = 0;
								ledtable[i].snd_most_outside_shuicho = 0;
								ledtable[i].snd_most_outside_enkeido = 0;

								ledtable[i].trd_most_outside_x = 0;
								ledtable[i].trd_most_outside_y = 0;

								ledtable[i].trd_most_outside_menseki = 0;
								ledtable[i].trd_most_outside_shuicho = 0;
								ledtable[i].trd_most_outside_enkeido = 0;

							}
						}
						else if(numOfout == 2)//GHOSTの2つ目の島の場合
						{
							sumInfo.snd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							sumInfo.snd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							sumInfo.snd_most_outside_menseki = sumInfo.menseki;
							sumInfo.snd_most_outside_shuicho = sumInfo.shuicho;
							sumInfo.snd_most_outside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_outside_menseki += sumInfo.menseki;//面積の合計

							if(isChange)
							{
								ledtable[i].sum_most_outside_menseki = sumInfo.sum_most_outside_menseki;

								ledtable[i].snd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
								ledtable[i].snd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

								ledtable[i].snd_most_outside_menseki = sumInfo.menseki;
								ledtable[i].snd_most_outside_shuicho = sumInfo.shuicho;
								ledtable[i].snd_most_outside_enkeido = sumInfo.enkeido;
							}
						}
						else if(numOfout == 3)//GHOSTの3つ目の島の場合
						{
							sumInfo.trd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							sumInfo.trd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							sumInfo.trd_most_outside_menseki = sumInfo.menseki;
							sumInfo.trd_most_outside_shuicho = sumInfo.shuicho;
							sumInfo.trd_most_outside_enkeido = sumInfo.enkeido;

							sumInfo.sum_most_outside_menseki += sumInfo.menseki;//面積の合計

							if(isChange)
							{
								ledtable[i].sum_most_outside_menseki = sumInfo.sum_most_outside_menseki;

								ledtable[i].trd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
								ledtable[i].trd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

								ledtable[i].trd_most_outside_menseki = sumInfo.menseki;
								ledtable[i].trd_most_outside_shuicho = sumInfo.shuicho;
								ledtable[i].trd_most_outside_enkeido = sumInfo.enkeido;
							}
						}
						else//GHOSTの島が4つ以上ある場合
						{
							sumInfo.sum_most_outside_menseki += sumInfo.menseki;//面積の合計
							if(isChange)
							{
								ledtable[i].sum_most_outside_menseki = sumInfo.sum_most_outside_menseki;
							}
						}
					}

#if false
					if(ledtable[i].isPass == true && ledtable[i].maxBrightnessCount < sumInfo.maxBrightnessCount)//認識していても、次の光源最高輝度面積が既存のそれより大きい場合、書き換える
					{
						gPos = i;
						sumInfo.ledPosition = i + 1;//LED位置を保存
			            ledtable[i].max_brightness_center_x = center_x;//最大輝度の中心座標を保存
			            ledtable[i].max_brightness_center_y = center_y;//最大輝度の中心座標を保存
			            ledtable[i].maxBrightnessCount = sumInfo.maxBrightnessCount;//最大輝度の数を保存

						ledtable[i].numberOfoutside = numOfout;
						ledtable[i].maxLine = maxLine;//最大輝度中心から外形までの最大距離
						ledtable[i].minLine = minLine;//最大輝度中心から外形までの最小距離

						if(numOfout == 1)
						{
							ledtable[i].sum_most_outside_menseki = sumInfo.menseki;

							ledtable[i].fst_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							ledtable[i].fst_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							ledtable[i].fst_most_outside_menseki = sumInfo.menseki;
							ledtable[i].fst_most_outside_shuicho = sumInfo.shuicho;
							ledtable[i].fst_most_outside_enkeido = sumInfo.enkeido;

						}
						else if(numOfout == 2)
						{
							ledtable[i].sum_most_outside_menseki = sumInfo.menseki;

							ledtable[i].snd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							ledtable[i].snd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							ledtable[i].snd_most_outside_menseki = sumInfo.menseki;
							ledtable[i].snd_most_outside_shuicho = sumInfo.shuicho;
							ledtable[i].snd_most_outside_enkeido = sumInfo.enkeido;

						}
						else if(numOfout == 3)
						{
							ledtable[i].sum_most_outside_menseki = sumInfo.menseki;

							ledtable[i].trd_most_outside_x = most_outer_x;//中心X座標を保存。GHOST判定に使用
							ledtable[i].trd_most_outside_y = most_outer_y;//中心Y座標を保存。GHOST判定に使用

							ledtable[i].trd_most_outside_menseki = sumInfo.menseki;
							ledtable[i].trd_most_outside_shuicho = sumInfo.shuicho;
							ledtable[i].trd_most_outside_enkeido = sumInfo.enkeido;
						}
						else//GHOSTの島が4つ以上ある場合
						{
							ledtable[i].sum_most_outside_menseki = sumInfo.menseki;
						}
					}
#endif

					break;
				}
			}

		}

		//63個全てのLED点灯を網羅したか。
		public bool IsFullLedCheck()
		{
			bool allLedflag = true;
			for(int i = 0; i < ledtable.Length; i++)
            {
				if(ledtable[i].result == 0)//初期値が一つでもあったら
				{
					allLedflag = false;
					break;
				}
			}
			return allLedflag;
		}

		//63個全てのLED光源として認識したか(光源内に入ったか)。
		public bool IsAllLed()
		{
			bool allLedflag = true;
			for(int i = 0; i < ledtable.Length; i++)
            {
				if(!ledtable[i].isPass)
				{
					allLedflag = false;
					break;
				}
			}
			return allLedflag;
		}

		//63個全てのLED点灯でNGが1つでも含まれているか
		public bool IsGhostFlareCheck()
		{
			bool checkflag = true;
			for(int i = 0; i < ledtable.Length; i++)
            {
				if(ledtable[i].result != 1)//1:OK, 2:Ghost, 3:Flare
				{
					checkflag = false;
					break;
				}
			}
			return checkflag;
		}

		//63個全てのLED点灯で、点灯が確認できた数、OK、NGの数を数えて返す
		public void GetGhostFlareCount(ref int ok, ref int ng, ref int pass)
		{
			for(int i = 0; i < ledtable.Length; i++)
            {
				//0:初期値, 1:OK, 2:Ghost, 4:Flare, 6:Ghost&Flare
				if(ledtable[i].result == 1)
				{
					ok++;
				}
				else if(ledtable[i].result == 2 || ledtable[i].result == 4 || ledtable[i].result == 6)
				{
					ng++;
				}
				
				if(ledtable[i].isPass)//true:pass
				{
					pass++;
				}
			}
		}

		public void SetLedInit()
		{
			for(int i = 0; i < ledtable.Length; i++)
            {
				ledtable[i].isPass = false;
				ledtable[i].result = 0;
				ledtable[i].outside_center_x = 0;
				ledtable[i].outside_center_y = 0;

				ledtable[i].fst_most_outside_x = 0;
				ledtable[i].fst_most_outside_y = 0;
				ledtable[i].fst_most_outside_menseki = 0;
				ledtable[i].fst_most_outside_shuicho = 0;
				ledtable[i].fst_most_outside_enkeido = 0;

				ledtable[i].snd_most_outside_x = 0;
				ledtable[i].snd_most_outside_y = 0;
				ledtable[i].snd_most_outside_menseki = 0;
				ledtable[i].snd_most_outside_shuicho = 0;
				ledtable[i].snd_most_outside_enkeido = 0;

				ledtable[i].trd_most_outside_x = 0;
				ledtable[i].trd_most_outside_y = 0;
				ledtable[i].trd_most_outside_menseki = 0;
				ledtable[i].trd_most_outside_shuicho = 0;
				ledtable[i].trd_most_outside_enkeido = 0;

				ledtable[i].sum_most_outside_menseki = 0;
				ledtable[i].numberOfoutside = 0;

				ledtable[i].max_brightness_center_x = 0;
				ledtable[i].max_brightness_center_y = 0;
				ledtable[i].maxBrightnessCount = 0;

//
				ledtable[i].fst_most_inside_x = 0;
				ledtable[i].fst_most_inside_y = 0;
				ledtable[i].fst_most_inside_menseki = 0;
				ledtable[i].fst_most_inside_shuicho = 0;
				ledtable[i].fst_most_inside_enkeido = 0;

				ledtable[i].snd_most_inside_x = 0;
				ledtable[i].snd_most_inside_y = 0;
				ledtable[i].snd_most_inside_menseki = 0;
				ledtable[i].snd_most_inside_shuicho = 0;
				ledtable[i].snd_most_inside_enkeido = 0;

				ledtable[i].trd_most_inside_x = 0;
				ledtable[i].trd_most_inside_y = 0;
				ledtable[i].trd_most_inside_menseki = 0;
				ledtable[i].trd_most_inside_shuicho = 0;
				ledtable[i].trd_most_inside_enkeido = 0;

				ledtable[i].sum_most_inside_menseki = 0;
				ledtable[i].numberOfinside = 0;

				ledtable[i].maxLine = 0;
				ledtable[i].minLine = 0;
			}
		}

		public int GetLedPos()
		{
			return gPos;
		}

		public void SetLedPos(int pos, int result)
		{
			if(result == 0x01)
			{
				ledtable[pos].result = result;//OK
			}
			else
			{
				ledtable[pos].result &= 0x06;
				ledtable[pos].result |= result;//GHOST or FLARE
			}
		}

		public int GetLedResult(int pos)
		{
			return ledtable[pos].result;
		}

	}
	
}
