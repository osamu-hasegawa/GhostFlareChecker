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

using MotorDriver;

namespace GhostFlareChecker
{
    public class MotorController
	{
		// デバイスハンドル変数　
		// ＲＥＳＵＬＴ格納エリア
		static MC07_S_RESULT sResult1; //1

		static ushort pData = 0;
		// 表示用バッファ
		char[] buf = new char[20];
		//static int Jstep,Cstep;

		int[] IoPort = {0,0,0,0};

		uint[] HDEV = {0,0,0,0};
		bool[] ORG_MOVE = {false,false,false,false}; //原点動作したかどうか

		int [] TotalPLS = {0,0,0,0}; //モータ移動総パルス
		int[] Base = {0,0,0,0};
		int[] ST = {0,0,0};
		int[] IO = new int[16];
		static int gRData = 0;
		static int gPulse = 0;

		public static int CcdF = 0;//CCD_Focus
		public static int LnsF = 1;//Lens_Focus
		public static int LnsR = 2;//Lens_Rotation
		public static int Chrt = 3;//Chart

		public const int UNIT_0 = 0x2000;
		public const int UNIT_1 = 0x2001;

		private static uint hDev;                                   // Device Handle

		public uint hPort = 0;
		public uint hUnit = 0;

		public int M_NgBoxCheck()
		{
            bool result = false;
			string resultStr = "";
//			ushort extUnitStatus = 0;

			//*** UNIT OPEN ***
            result = MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.UOpen() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}

	        //*** UNIT COMMUNICATION MODE ***
            result = MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.UWExUnitCommMode() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}

	        //*** UNIT COMMUNICATION CONTROL ***
            result = MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.UWExUnitCommControl(START) \r\n : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}


			//*** WAIT CONNECT BIT = 1 ***
//			do
//			{
//				//*** READ UNIT COMMUNICATION STATUS ***
//				if (MC07.URExUnitCommStatus(hUnit, ref extUnitStatus, ref sResult1) == false)
//				{
//					resultStr = "It failed in MC07.URExUnitCommStatus() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
//					//Because it is avoided double Unit Open Error
//					MC07.UClose(hUnit,ref sResult1);
//				}
//
//				//*** CHECK POLLING BIT ***
//				if ((extUnitStatus & 0x1) == 0)
//				{
//					resultStr = "EXTEND UNIT STATUS is not Polling. \r\nPlease Check EXTEND UNIT connection";
//					//Because it is avoided double Unit Open Error
//					MC07.UClose(hUnit,ref sResult1);
//				}
//
//				//*** CHECK DISCONNECT LATCH BIT ***
//				if ((extUnitStatus & 0x4) !=0)
//				{
//					resultStr = "EXTEND UNIT STATUS is DISCONNECT LATCH. \r\n  Please Check EXTEND UNIT connection";
//
//					//*** UNIT COMMUNICATION CONTROL ***
//					if (MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_DISC_LATCH_CLR,ref sResult1) == false)
//					{
//						resultStr = "It failed in MC07.UWExUnitCommControl(LATCH_CLR)  \r\n : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
//						//Because it is avoided double Unit Open Error
//						MC07.UClose(hUnit,ref sResult1);
//					}
//
//					//Because it is avoided double Unit Open Error
//					MC07.UClose(hUnit,ref sResult1);
//				}
//				//*** CHECK CONNECT BIT ***
//			}while((extUnitStatus & 0x2) == 0);
//
			//*** BPORT OPEN ***
            result = MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_IN, ref hPort, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.BPortOpen() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}
		
			//*** BPORT READ ***
			pData = 0;
			result = MC07.BPortIn(hPort, ref pData, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.BPortIn() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}
			
			//*** BPORT CLOSE ***
			result = MC07.BPortClose(hPort, ref sResult1);
			if(!result)
			{
				resultStr = "It failed in MC07.BPortClose() : MC07.Result(1)=" + sResult1.MC07_Result[1].ToString();
			}

			short retData = (short)pData;
			retData &= 0x0100;
			retData >>= 8;

			return retData;
		}

		public void LedReset()//LEDリセット
		{
			//*** UNIT OPEN ***
            MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
	        //*** UNIT COMMUNICATION MODE ***
            MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
	        //*** UNIT COMMUNICATION CONTROL ***
            MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);

			LedLightOff();
			
			Thread.Sleep(20);
			//*** BPORT OPEN ***
            MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_OUT, ref hPort, ref sResult1);
		
			//*** BPORT WRITE ***
			pData = 0x00;
			MC07.BPortOut(hPort, ref pData, ref sResult1);
			
			//*** BPORT CLOSE ***
			MC07.BPortClose(hPort, ref sResult1);
		}

		public void LedLightOn()//LED点灯(1つずつ)
		{
			//*** UNIT OPEN ***
            MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
	        //*** UNIT COMMUNICATION MODE ***
            MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
	        //*** UNIT COMMUNICATION CONTROL ***
            MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);

			LedLightOff();
			
			Thread.Sleep(20);
			//*** BPORT OPEN ***
            MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_OUT, ref hPort, ref sResult1);
		
			//*** BPORT WRITE ***
			pData = 0x01;
			MC07.BPortOut(hPort, ref pData, ref sResult1);
			
			//*** BPORT CLOSE ***
			MC07.BPortClose(hPort, ref sResult1);
		}

		public void LedLightOff()//LED消灯
		{
			//*** UNIT OPEN ***
            MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
	        //*** UNIT COMMUNICATION MODE ***
            MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
	        //*** UNIT COMMUNICATION CONTROL ***
            MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);

			Thread.Sleep(20);
			//*** BPORT OPEN ***
            MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_OUT, ref hPort, ref sResult1);
		
			//*** BPORT WRITE ***
			pData = 0x02;
			MC07.BPortOut(hPort, ref pData, ref sResult1);
			
			//*** BPORT CLOSE ***
			MC07.BPortClose(hPort, ref sResult1);
		}

		public void LedColumnLightOn()//LED行毎点灯
		{
			//*** UNIT OPEN ***
            MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
	        //*** UNIT COMMUNICATION MODE ***
            MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
	        //*** UNIT COMMUNICATION CONTROL ***
            MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);

			LedLightOff();

			Thread.Sleep(20);
			//*** BPORT OPEN ***
            MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_OUT, ref hPort, ref sResult1);
		
			//*** BPORT WRITE ***
			pData = 0x04;
			MC07.BPortOut(hPort, ref pData, ref sResult1);
			
			//*** BPORT CLOSE ***
			MC07.BPortClose(hPort, ref sResult1);
		}

		public void LedUnitLightOn()//LED単体の点灯
		{
			//*** UNIT OPEN ***
            MC07.UOpen(MC07.MC07_USB_UNIT_0, ref hUnit, ref sResult1);
	        //*** UNIT COMMUNICATION MODE ***
            MC07.UWExUnitCommMode(hUnit, MC07.MC07_EX_UNIT_COMM_RATE_5, 1, MC07.MC07_EX_UNIT_COMM_16BIT, ref sResult1);
	        //*** UNIT COMMUNICATION CONTROL ***
            MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_START, ref sResult1);

			LedLightOff();

			Thread.Sleep(20);
			//*** BPORT OPEN ***
            MC07.BPortOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_EXP0_OUT, ref hPort, ref sResult1);
		
			//*** BPORT WRITE ***
			pData = 0x08;
			MC07.BPortOut(hPort, ref pData, ref sResult1);
			
			//*** BPORT CLOSE ***
			MC07.BPortClose(hPort, ref sResult1);
		}

        public MotorController()
        {
        }

        public void Init()
		{
		}

		public void Release()
		{
#if false
            MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			Boolean Ret = false;

			Ret = MC07.BClose(hDev, ref sResult);
			
			Ret = MC07.UWExUnitCommControl(hUnit, MC07.MC07_EX_UNIT_COMM_STOP, ref sResult);
			
			Ret = MC07.UClose(hUnit, ref sResult);
			
			Ret = MC07.BPortClose(hInPort, ref sResult);
			
			Ret = MC07.BPortClose(hOutPort, ref sResult);
#endif
		}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string button_Env_Click(object sender, EventArgs e)
        {
            MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            MC07_S_UNIT_INFO unitInfo = new MC07_S_UNIT_INFO();

			string resultStr = "Environment OK";

            if(MC07.Environment(MC07.MC07_USB_UNIT_0, ref sResult) == false)
            {
                resultStr = "It failed in MC07_Environment() : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
            }

            if (MC07.ReadUnitInfo(MC07.MC07_USB_UNIT_0, ref unitInfo, ref sResult) == false)
            {
                resultStr = "It failed in MC07_ReadUnitInfo() : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
            }

            if (unitInfo.UnitType[0] == 0xFF)
            {
                resultStr = "NO CONTROLLER UNIT:Please Check USB connection";
            }

			return resultStr;
        }


        public string button_Open_Click(object sender, EventArgs e)
        {
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);

			string resultStr = "Open OK";

			//DEVICE OPEN
			if (MC07.BOpen(MC07.MC07_USB_UNIT_0, MC07.MC07_X, ref hDev, ref sResult) == false){
				resultStr = "It failed in MC07_BOpen() : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}

        public string button_Close_Click(object sender, EventArgs e)
        {
			string resultStr = "Close OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);

			//*** DEVICE CLOSE ***
			if (MC07.BClose(hDev, ref sResult) == false) {
				resultStr = "It failed in MC07_BClose() : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}

        public string button_Reset_Click(object sender, EventArgs e)
        {
			string resultStr = "Reset OK";
            uint LData = 0;
			ushort Cmd = 0;
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);

			//*** ADDRESS COUNTER PRESET ***
			Cmd = 0x80;
			LData = 0;
			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07_LWDrive(H'" + Cmd.ToString("X") +  ") : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}

        public string button_FastStop_Click(object sender, EventArgs e)
        {
			string resultStr = "FastStop OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            ushort Cmd = 0;

			//*** FAST STOP COMMAND ***
			Cmd = 0xFF;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07_BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}

        public string button_SlowStop_Click(object sender, EventArgs e)
        {
			string resultStr = "SlowStop OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			ushort Cmd = 0;

			//*** SLOW STOP COMMAND ***
			Cmd = 0xFE;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07_BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07_Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}

        public string button_DataSet_Click(object sender, EventArgs e)
        {
			string resultStr = "DataSet OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            ushort Data = 0;
            ushort Cmd = 0;
            uint LData = 0;
			string errStr = "";
		
			//*** FSPD SET COMMAND       : 5000Hz ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0x5;
			LData = 5000;
			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** JSPD SET COMMAND       : 1000Hz ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0xC;
			LData = 1000;
			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** HIGH SPEED SET COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Data = 5000;                // HSPD       : 5000Hz;
			if (MC07.BWDriveData1(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData1() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Data = 0x3;                 // RESOL No.  : 3;
			if (MC07.BWDriveData2(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData2() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Cmd = 0x6;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** LOW SPEED SET COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Data = 1000;                // LSPD       : 1000Hz
			if (MC07.BWDriveData1(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData1() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Data = 1000;                 // ELSPD      : 1000Hz
			if (MC07.BWDriveData2(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData2() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Cmd = 0x7;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** RATE SET COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Data = 200;                // UCYCLE     : 100μs
			if (MC07.BWDriveData1(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData1() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Data = 200;                 // DCYCLE     : 100μs
			if (MC07.BWDriveData2(hDev, ref Data, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveData2() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			Cmd = 0x8;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** ORIGIN SPEC SET ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			if (MC07.SetOrgSpec(hDev, 0x8000, ref sResult) == false){
				resultStr = "It failed in MC07.SetOrgSpec() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
			
			//*** MARGIN PULSE            : 5 Pulse ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			if (MC07.SetOrgMarginPulse(hDev, 5, ref sResult) == false){
				resultStr = "It failed in MC07.SetOrgMarginPulse() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}

			return resultStr;
		}

        public string button_CcwScan_Click(object sender, EventArgs e)
        {
			string resultStr = "CcwScan OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			ushort Cmd = 0;
            byte StopCode = 0;
			string errStr = "";

			//*** -CCW SCAN DRIVE COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0x13;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}			
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

        public string button_CwScan_Click(object sender, EventArgs e)
        {
			string resultStr = "CwScan OK";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			ushort Cmd = 0;
            byte StopCode = 0;
			string errStr = "";

			//*** +CW SCAN DRIVE COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0x12;
			if (MC07.BWDriveCommand(hDev, ref Cmd, ref sResult) == false){
				resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

        public string button_Org_Click(object sender, EventArgs e)
        {
			string resultStr = "Org OK";
			string errStr = "";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            byte StopCode = 0;

			//*** ORIGIN DRIVE ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			if (MC07.Org(hDev, MC07.MC07_ORG3, ref sResult) == false){
				resultStr = "It failed in MC07.Org() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

//        public string button_Rtn_Click(object sender, EventArgs e)
//        {
//			string resultStr = "Rtn OK";
//			string errStr = "";
//			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
//            byte StopCode = 0;
//            ushort Cmd = 0;
//			uint LData = 0;
//
//			//*** ABSOLUTE INDEX DRIVE COMMAND ***
//			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
//			Cmd = 0x15;
//			LData = 0;
//			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
//				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
//				return resultStr;
//			}
//            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
//            if(!isResult)
//            {
//				resultStr = errStr;
//			}
//			return resultStr;
//		}

        public string button_CcwIndex_Click(object sender, EventArgs e)
        {
			string resultStr = "CcwIndex OK";
			string errStr = "";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            byte StopCode = 0;
			ushort Cmd = 0;
			uint LData = 0;

			//*** INCREMENTAL INDEX DRIVE COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0x14;
			LData = MC07.Unsigned32(-10000);
			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

        public string button_CwIndex_Click(object sender, EventArgs e)
        {
			string resultStr = "CwIndex OK";
			string errStr = "";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            byte StopCode = 0;
			ushort Cmd = 0;
			uint LData = 0;

			//*** INCREMENTAL INDEX DRIVE COMMAND ***
			if (CheckStatus1(hDev, ref errStr) == false) return errStr;
			Cmd = 0x14;
			LData = 10000;
			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

        public string button_ClrError_Click(object sender, EventArgs e)
        {
			string resultStr = "ClrError OK";

			MC07_S_RESULT sResult = new MC07_S_RESULT(0);

			if (MC07.ClrError(hDev, ref sResult) == false){
				resultStr = "It failed in MC07.ClrError() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
			}
			return resultStr;
		}


		private Boolean CheckStatus1(uint hDevice, ref string errStr) 
		{
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			ushort Cmd = 0;
			ushort Status1 = 0;
			ushort Data1 = 0;

			//*** CHECK STATUS1 BUSY BIT = 0 (WAIT TIME = 1000ms)***
			if (MC07.BWaitDriveCommand(hDevice, 1000, ref sResult) == false){
				errStr = "It failed in MC07.BWaitDriveCommand() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return false;
			}
			
			//*** READ STATUS1 ***
			if (MC07.BRStatus1(hDevice, ref Status1, ref sResult) == false){
				errStr = "It failed in MC07.BRStatus1() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return false;
			}
			
			//*** CHECK STATUS1 ERROR BIT = 0 ***
			if ( ( Status1 & 0x0010 ) != 0 ){
				//*** READ ERROR STATUS ***
				Cmd = 0xD1;
				if (MC07.BWDriveCommand(hDevice, ref Cmd, ref sResult) == false){
					errStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
					return false;
				}
			
				if (MC07.BRDriveData1(hDevice, ref Data1, ref sResult) == false){
					errStr = "It failed in MC07.BRDriveData1(): MC07.Result(1) = " + sResult.MC07_Result[1].ToString();
					return false;
				}
			
				errStr = "STATUS1 ERROR = 1 : ERROR STATUS = H'" + Data1.ToString("X")  + "\r\n Please click 'Clear Error'";
			
				return false;
			
			}
			
			return true;

		}

        private bool Ready_Wait(uint hDevice, ref byte StopCode, bool OriginDriveFlag, ref string errStr)
		{
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
			ushort Status1 = 0;

            do{
//				Now_Address_Display( hDevice );
				Application.DoEvents();
//				Now_Address_Display( hDevice );
				if (MC07.BRStatus1(hDevice, ref Status1, ref sResult) == false){
					errStr = "It failed in MC07.BRStatus1() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
					StopCode = 7;
					return false;
				}
			}while( ( Status1 & 0x0001 ) != 0 );

//            Now_Address_Display( hDevice );

            if (OriginDriveFlag){
				if (MC07.ReadOrgStatus(hDevice, ref Status1, ref sResult) == false){
					errStr = "It failed in MC07.ReadOrgStatus() : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
					StopCode = 7;
					return false;
				}
                if ( ( Status1 & 0x8000) != 0 )
                    StopCode = 6;                                            // ADDRESS ERROR
                else if ( ( Status1 & 0x4000 ) != 0)
                    StopCode = 5;                                            // ERROR PULSE ERROR
                else if ( ( Status1 & 0x2000 ) != 0 )
                    StopCode = 4;                                            // SENSOR ERROR
                else if ( ( Status1 & 0x0020 ) != 0 )
                    StopCode = 3;                                            // LSEND
                else if ( ( Status1 & 0x0040 ) != 0 )
                    StopCode = 2;                                            // SSEND
                else if ( ( Status1 & 0x0080 ) != 0 )
                    StopCode = 1;                                            // FSEND
                else
                    StopCode = 0;                                            // PASS
            }else{
                if ( ( Status1 & 0x0020 ) != 0 )
                    StopCode = 3;                                            // LSEND
                else if ( ( Status1 & 0x0040 ) != 0 )
                    StopCode = 2;                                            // SSEND
                else if ((Status1 & 0x0080 ) != 0)
                    StopCode = 1;                                            // FSEND
                else
                    StopCode = 0;                                            // PASS
            }
            return true;
        }


		public string DisplayAddress()
		{
            MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            uint WData = 0;
            uint RData = 0;
            ushort Cmd = 0;
            string resultStr = "";

            //*** ADDRESS COUNTER READ COMMAND ***
            Cmd = 0xD8;
            if (MC07.LWRDrive(hDev, Cmd, ref WData, ref RData, ref sResult) == false)
            {
                resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
                return resultStr;
            }
            
            gRData = (int)RData;
            
			resultStr = string.Format("{0:#,###,###,##0}", MC07.Signed32(RData));
            return resultStr;
        }

		public string DisplayPulse()
		{
            MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            uint WData = 0;
            uint RData = 0;
            ushort Cmd = 0;
            string resultStr = "";

            //*** PULSE COUNTER READ COMMAND ***
            Cmd = 0xD9;
            if (MC07.LWRDrive(hDev, Cmd, ref WData, ref RData, ref sResult) == false)
            {
                resultStr = "It failed in MC07.BWDriveCommand(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
                return resultStr;
            }
            
            gPulse = (int)RData;
            
			resultStr = string.Format("{0:#,###,###,##0}", MC07.Signed32(RData));
            return resultStr;
        }

		public int GetCurrentAddress()
		{
			return gRData;
		}

		public int GetCurrentPulse()
		{
			return gPulse;
		}


		public string StepDrive(int step, out bool isRes)
		{
			DisplayAddress();
			
			isRes = true;
		    //移動後パルスチェック
		    if(DataController.SETDATA.Origin[CcdF] == 1){
		        if((gRData + step) < DataController.SETDATA.L_LimitPLS[CcdF]
		           || (gRData + step) > DataController.SETDATA.U_LimitPLS[CcdF]){
					isRes = false;
		            return "StepDrive Range Over Error";
		        }
		    }

			string resultStr = "StepDrive OK";
			string errStr = "";
			MC07_S_RESULT sResult = new MC07_S_RESULT(0);
            byte StopCode = 0;
			ushort Cmd = 0;
			uint LData = 0;

			//*** INCREMENTAL INDEX DRIVE COMMAND ***
			if(CheckStatus1(hDev, ref errStr) == false)
			{
				return errStr;
			}
			
			Cmd = 0x14;

			if(step < 0)
			{
				LData = MC07.Unsigned32(step);
			}else
			{
				LData = (uint)step;
			}

			if (MC07.LWDrive(hDev, Cmd, ref LData, ref sResult) == false){
				resultStr = "It failed in MC07.LWDrive(H'" + Cmd.ToString("X") + ") : MC07.Result(1)=" + sResult.MC07_Result[1].ToString();
				return resultStr;
			}
            bool isResult = Ready_Wait( hDev, ref StopCode, false, ref errStr);
            if(!isResult)
            {
				resultStr = errStr;
			}
			return resultStr;
		}

		public void MotorMotionTest()
		{
			ushort pStatus = 0;
			bool resStatus = MC07.ReadOrgStatus(hDev, ref pStatus, ref sResult1);

			MC07_S_ORG_PARAM psOrgParam = new MC07_S_ORG_PARAM();

			bool resParam = MC07.ReadOrgParam(hDev, ref psOrgParam, ref sResult1);
		}

    }
}

