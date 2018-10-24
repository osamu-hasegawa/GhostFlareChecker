using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Drawing;

namespace GhostFlareChecker
{
    public class CheckerManager
    {
		public delegate void CALL_VOID();

        static public Form1 form1 = null;
        static public Form2 form2 = null;
        static public Form3 form3 = null;
        static public CameraController m_CameraController = null;
        static public CheckerManager m_CheckerManager = null;
        static public DataController m_DataController = null;
        static public ImageController m_ImageController = null;
        static public MotorController m_MotorController = null;
        static public int afCount = 0;

		static public void Init()
		{
			m_CameraController = new CameraController();
            m_DataController = new DataController();
            m_ImageController = new ImageController();
            m_MotorController = new MotorController();

			//\C:\Program Files (x86)\KOP\GhostFlareChecker (<-セットアップにてコピーされる)⇒
			//C:\Users\xxxxx\Documents\KOP\GhostFlareChecker    xxxxxはPCのログイン名
			//へ"SettingData.xml"をコピーし、元の"SettingData.xml"を削除する
			//C:\Users\xxxxx\Documents\KOP\GhostFlareCheckerに既に"SettingData.xml"があれば日付をファイル名に付けて
			//リネームしておく⇒上書きしないように。
			m_DataController.CopySettings("SettingData.xml");

            m_DataController.Init();//各設定値をSetting.xmlから読み込み
            m_ImageController.Init();
            m_MotorController.Init();
            m_CameraController.Init();
			
            m_CameraController.LoadSetting();//Camera関連設定値をSetting.xmlから読み込み
		}

		static public void Close()
		{
            m_CameraController.Terminate();
            m_DataController.Release();
            m_ImageController.Release();
            m_MotorController.Release();
		}

		static int mode = 0;
		static public void SetCurrentMode(int md)
		{
			mode = md;
		}
		static public int GetCurrentMode()
		{
			return mode;
		}
		
		static int AfMotion = 0;
		static public void SetAfMotion(int motion)
		{
			AfMotion = motion;
		}
		static public int GetAfMotion()
		{
			return AfMotion;
		}

		static public void SetAfCount()
		{
			if(afCount == 1000000)
			{
				afCount = 0;
			}
			afCount++;
		}

		static public bool IsOriginBack()
		{
			if((afCount % 10) == 0)//10回毎
			{
				return true;
			}
			return false;
		}



    }

}	