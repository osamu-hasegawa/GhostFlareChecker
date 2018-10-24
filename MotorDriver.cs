//*******************************************************************************************************************
//
//
//Å@MPL-36-01v2.00/USBW32 Dynamic Link Library Structure, Function Declarations             5.3601.13.1310
//
//
//*******************************************************************************************************************
using System;
using System.Runtime.InteropServices;

namespace MotorDriver
{
    /// <summary>
    /// </summary>
    ///
    //*** RESULT STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_RESULT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] MC07_Result;
        public MC07_S_RESULT(ushort dummy)
        {
            MC07_Result = new ushort[4];
        }
    }

    //*** COMMAND DATA STRUCTURE ***
    public struct MC07_S_COMMAND_DATA
    {
        public ushort Command;
        public uint Data;
    }

    //*** STATUS DATA STRUCTURE ***
    public struct MC07_S_STATUS_DATA
    {
        public ushort Status1;
        public uint Data;
    }

    //*** UNIT INFO STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_UNIT_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] UnitType;
        public MC07_S_UNIT_INFO(ushort dummy)
        {
            UnitType = new ushort[10];
        }
    }

    //*** COMMAND BUFFER STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_COMMAND_BUF
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] Command;
        public MC07_S_COMMAND_BUF(ushort dummy)
        {
            Command = new ushort[16];
        }
    }

    //*** DATA BUFFER STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_DATA_BUF
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public uint[] Data;
        public MC07_S_DATA_BUF(uint dummy)
        {
            Data = new uint[16];
        }
    }

    //*** STATUS BUFFER STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_STATUS_BUF
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] Status;
        public MC07_S_STATUS_BUF(ushort dummy)
        {
            Status = new ushort[16];
        }
    }

    //*** DATA STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_DATA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] MC07_Data;
        public MC07_S_DATA(ushort dummy)
        {
            MC07_Data = new ushort[4];
        }
    }

    //*** SPEEDÅARATE STRUCTURE ***
    public struct MC07_S_SPEED_RATE
    {
        public uint FSpd;
        public uint HighSpeed;
        public uint LowSpeed;
        public uint EndLowSpeed;
        public uint SUArea;
        public uint SDArea;
        public uint URateNo;
        public uint DRateNo;
    }
    //*** XY POSITION STRUCTURE ***
    public struct MC07_S_XY_POSITION
    {
        public int X;
        public int Y;
    }

    //*** ORIGIN PARAMETER STRUCTURE ***
    public struct MC07_S_ORG_PARAM
    {
        public uint Spec;
        public uint MarginPulse;
        public uint LimitDelay;
        public uint ScanDelay;
        public uint PulseDelay;
        public uint CScanErrorPulse;
        public uint PulseErrorPulse;
        public uint OffsetPulse;
        public int PresetPulse;
    }

    //*** A/D DATA STRUCTURE ***
    [StructLayout(LayoutKind.Sequential)]		// LAYOUT INFORMATION
    public struct MC07_S_AD_DATA
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ushort[] Data;
        public MC07_S_AD_DATA(ushort dummy)
        {
            Data = new ushort[16];
        }
    }

    //*** IN PORT STRUCTURE ***
    public struct MC07_S_IN_PORT
    {
        public ushort Gpin;
        public ushort Gp0in;
        public ushort Gp1in;
        public ushort Exp0in;
        public ushort Exp1in;
        public ushort Ctlp0in;
    }

    //*** OUT PORT STRUCTURE ***
    public struct MC07_S_OUT_PORT
    {
        public ushort Gpout;
        public ushort Gp0out;
        public ushort Gp1out;
        public ushort Exp0out;
        public ushort Exp1out;
        public ushort Ctlp0out;
    }

    //*** UNIT STATUS STRUCTURE ***
    public struct MC07_S_UNIT_STATUS
    {
        public MC07_S_STATUS_DATA X;
        public MC07_S_STATUS_DATA Y;
        public MC07_S_STATUS_DATA Z;
        public MC07_S_STATUS_DATA A;
        public MC07_S_IN_PORT InPort;
    }

    //*** UNIT COMMAND STRUCTURE ***
    public struct MC07_S_UNIT_COMMAND
    {
        public MC07_S_COMMAND_DATA X;
        public MC07_S_COMMAND_DATA Y;
        public MC07_S_COMMAND_DATA Z;
        public MC07_S_COMMAND_DATA A;
        public MC07_S_OUT_PORT OutPort;
    }

    public class MC07
    {
        public MC07()
        {
        }
        //*** DEFINITION ***
        public const ushort MC07_USB_UNIT_0 = 0x2000;
        public const ushort MC07_USB_UNIT_1 = 0x2001;
        public const ushort MC07_USB_UNIT_2 = 0x2002;
        public const ushort MC07_USB_UNIT_3 = 0x2003;
        public const ushort MC07_USB_UNIT_4 = 0x2004;
        public const ushort MC07_USB_UNIT_5 = 0x2005;
        public const ushort MC07_USB_UNIT_6 = 0x2006;
        public const ushort MC07_USB_UNIT_7 = 0x2007;
        public const ushort MC07_USB_UNIT_8 = 0x2008;
        public const ushort MC07_USB_UNIT_9 = 0x2009;

        public const ushort MC07_X = 0;
        public const ushort MC07_Y = 1;
        public const ushort MC07_Z = 2;
        public const ushort MC07_A = 3;

        public const ushort MC07_EX_UNIT_COMM_RATE_5 = 2;

        public const ushort MC07_EX_UNIT_COMM_16BIT = 0;
        public const ushort MC07_EX_UNIT_COMM_32BIT = 1;

        public const ushort MC07_EX_UNIT_COMM_START = 0;
        public const ushort MC07_EX_UNIT_COMM_STOP = 1;
        public const ushort MC07_EX_UNIT_COMM_DISC_LATCH_CLR = 2;

        public const ushort MC07_STATUS1 = 0;
        public const ushort MC07_STATUS2 = 1;
        public const ushort MC07_STATUS3 = 2;
        public const ushort MC07_STATUS4 = 3;
        public const ushort MC07_STATUS5 = 4;
        public const ushort MC07_ORG_STATUS = 5;

        public const ushort MC07_GP_IN = 0x0;
        public const ushort MC07_GP_OUT = 0x1;
        public const ushort MC07_EXP0_IN = 0x2;
        public const ushort MC07_EXP1_IN = 0x3;
        public const ushort MC07_EXP0_OUT = 0x4;
        public const ushort MC07_EXP1_OUT = 0x5;
        public const ushort MC07_CTLP0_IN = 0x6;
        public const ushort MC07_CTLP0_OUT = 0x7;

        public const ushort MC07_ORG0 = 0;
        public const ushort MC07_ORG1 = 1;
        public const ushort MC07_ORG2 = 2;
        public const ushort MC07_ORG3 = 3;
        public const ushort MC07_ORG4 = 4;
        public const ushort MC07_ORG5 = 5;
        public const ushort MC07_ORG10 = 10;
        public const ushort MC07_ORG11 = 11;
        public const ushort MC07_ORG12 = 12;

        public const ushort MC07_CCW = 0;
        public const ushort MC07_CW = 1;

        public const uint MC07_SEL_X = 0x00000001;
        public const uint MC07_SEL_Y = 0x00000002;
        public const uint MC07_SEL_Z = 0x00000004;
        public const uint MC07_SEL_A = 0x00000008;
        public const uint MC07_SEL_X_Y = 0x00000003;
        public const uint MC07_SEL_X_Z = 0x00000005;
        public const uint MC07_SEL_X_A = 0x00000009;
        public const uint MC07_SEL_Y_Z = 0x00000006;
        public const uint MC07_SEL_Y_A = 0x0000000a;
        public const uint MC07_SEL_Z_A = 0x0000000c;
        public const uint MC07_SEL_X_Y_Z = 0x00000007;
        public const uint MC07_SEL_X_Y_A = 0x0000000b;
        public const uint MC07_SEL_X_Z_A = 0x0000000d;
        public const uint MC07_SEL_Y_Z_A = 0x0000000e;
        public const uint MC07_SEL_X_Y_Z_A = 0x0000000f;

        public const uint MC07_SEL_GP_OUT = 0x00000001;
        public const uint MC07_SEL_EXP0_OUT = 0x00000004;
        public const uint MC07_SEL_EXP1_OUT = 0x00000008;
        public const uint MC07_SEL_CTLP0_OUT = 0x00000002;
        public const uint MC07_SEL_EXP0_EXP1_OUT = 0x0000000c;

        public const uint MC07_SEL_GP_IN = 0x00010000;
        public const uint MC07_SEL_EXP0_IN = 0x00040000;
        public const uint MC07_SEL_EXP1_IN = 0x00080000;
        public const uint MC07_SEL_CTLP0_IN = 0x00020000;
        public const uint MC07_SEL_EXP0_EXP1_IN = 0x000c0000;

        //*** MPL Function Declarations ***
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Environment")]
        public static extern bool Environment(ushort UnitNo, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ReadUnitInfo")]
        public static extern bool ReadUnitInfo(ushort UnitNo, ref MC07_S_UNIT_INFO psUnitInfo, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UOpen")]
        public static extern bool UOpen(ushort UnitNo, ref uint phUnit, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UClose")]
        public static extern bool UClose(uint hUnit, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UClrError")]
        public static extern bool UClrError(uint hUnit, uint AxisSel, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UWExUnitCommMode")]
        public static extern bool UWExUnitCommMode(uint hUnit, ushort CommRate, ushort RetryCount, ushort IoBit, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UWExUnitCommControl")]
        public static extern bool UWExUnitCommControl(uint hUnit, ushort ControlSel, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_URExUnitCommStatus")]
        public static extern bool URExUnitCommStatus(uint hUnit, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_URExUnitCommMode")]
        public static extern bool URExUnitCommMode(uint hUnit, ref ushort pCommRate, ref ushort pRetryCount, ref ushort pIoBit, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRADData")]
        public static extern bool BRADData(uint hUnit, ushort Signal, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_IRADData")]
        public static extern bool IRADData(uint hUnit, ref MC07_S_AD_DATA psData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UWDriveIo")]
        public static extern bool UWDriveIo(uint hUnit, uint AxisSel, uint IoPortSel, ref MC07_S_UNIT_COMMAND psUnitCmd, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UWRDrive")]
        public static extern bool UWRDrive(uint hUnit, uint AxisSel, ref MC07_S_UNIT_COMMAND psUnitCommand, ref MC07_S_UNIT_STATUS psUnitStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_URStatus1Io")]
        public static extern bool URStatus1Io(uint hUnit, uint AxisSel, uint IoPortSel, ref MC07_S_UNIT_STATUS psUnitStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_URStatus1PcntIo")]
        public static extern bool URStatus1PcntIo(uint hUnit, uint AxisSel, uint IoPortSel, ref MC07_S_UNIT_STATUS psUnitStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UPortOut")]
        public static extern bool UPortOut(uint hUnit, uint IoPortSel, ref MC07_S_OUT_PORT psOutPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UPortOrOut")]
        public static extern bool UPortOrOut(uint hUnit, uint IoPortSel, ref MC07_S_OUT_PORT psOutPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UPortAndOut")]
        public static extern bool UPortAndOut(uint hUnit, uint IoPortSel, ref MC07_S_OUT_PORT psOutPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_UPortIn")]
        public static extern bool UPortIn(uint hUnit, uint IoPortSel, ref MC07_S_IN_PORT psInPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BOpen")]
        public static extern bool BOpen(ushort UnitNo, ushort Axis, ref uint phDev, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BClose")]
        public static extern bool BClose(uint hDev, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ClrError")]
        public static extern bool ClrError(uint hDev, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BufWDrive")]
        public static extern bool BufWDrive(uint hDev, ref MC07_S_COMMAND_BUF psCmdBuf, ref MC07_S_DATA_BUF psDataBuf, ushort Count, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_LWDrive")]
        public static extern bool LWDrive(uint hDev, ushort Cmd, ref uint pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_LWData")]
        public static extern bool LWData(uint hDev, ref uint pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_IWDrive")]
        public static extern bool IWDrive(uint hDev, ushort Cmd, ref MC07_S_DATA psData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_IWData")]
        public static extern bool IWData(uint hDev, ref MC07_S_DATA psData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_LWDriveBuf")]
        public static extern bool LWDriveBuf(uint hDev, MC07_S_COMMAND_DATA[] CmdDataBuf, ushort Count, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BWDriveCommand")]
        public static extern bool BWDriveCommand(uint hDev, ref ushort pCmd, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BWDriveData1")]
        public static extern bool BWDriveData1(uint hDev, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BWDriveData2")]
        public static extern bool BWDriveData2(uint hDev, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_AllRStatus")]
        public static extern bool AllRStatus(uint hDev, ref MC07_S_STATUS_BUF psStsBuf, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatus1")]
        public static extern bool BRStatus1(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatus2")]
        public static extern bool BRStatus2(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatus3")]
        public static extern bool BRStatus3(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatus4")]
        public static extern bool BRStatus4(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatus5")]
        public static extern bool BRStatus5(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRStatusBuf")]
        public static extern bool BRStatusBuf(uint hDev, ushort[] StatusBuf, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_LRDrive")]
        public static extern bool LRDrive(uint hDev, ref uint pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_IRDrive")]
        public static extern bool IRDrive(uint hDev, ref MC07_S_DATA psData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRDriveData1")]
        public static extern bool BRDriveData1(uint hDev, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BRDriveData2")]
        public static extern bool BRDriveData2(uint hDev, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_LWRDrive")]
        public static extern bool LWRDrive(uint hDev, ushort Cmd, ref uint pWriteData, ref uint pReadData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BWaitDriveCommand")]
        public static extern bool BWaitDriveCommand(uint hDev, ushort WaitTime, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BWaitComregNotFull")]
        public static extern bool BWaitComregNotFull(uint hDev, ushort WaitTime, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BIsWait")]
        public static extern bool BIsWait(uint hDev, ref ushort pWaitSts, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BBreakWait")]
        public static extern bool BBreakWait(uint hDev, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetSpeedRate")]
        public static extern bool SetSpeedRate(uint hDev, ushort ResolNo, ref MC07_S_SPEED_RATE psSpeedRate, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ReadSpeedRate")]
        public static extern bool ReadSpeedRate(uint hDev, ref ushort pResolNo, ref MC07_S_SPEED_RATE psSpeedRate, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_McIncStrCp")]
        public static extern bool McIncStrCp(uint hDevX, uint hDevY, ushort DrvSpec, ref MC07_S_XY_POSITION psTargetPosition, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_McIncCirCp")]
        public static extern bool McIncCirCp(uint hDevX, uint hDevY, ushort DrvSpec, ushort Dir, ref MC07_S_XY_POSITION psCenterPosition, ref MC07_S_XY_POSITION psTargetPosition, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_GetCirShortPulse")]
        public static extern bool GetCirShortPulse(ushort Dir, ref MC07_S_XY_POSITION psCenterPosition, ref MC07_S_XY_POSITION psTargetPosition, ref int pShortPulse, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_GetCirCenterPosition")]
        public static extern bool GetCirCenterPosition(ref MC07_S_XY_POSITION psPassPosition, ref MC07_S_XY_POSITION psTargetPosition, ref ushort pDir, ref MC07_S_XY_POSITION psCenterPosition, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_IncFromAbs")]
        public static extern bool IncFromAbs(uint hDevX, uint hDevY, ref MC07_S_XY_POSITION psAbsPosition, ref MC07_S_XY_POSITION psIncPosition, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ReadOrgStatus")]
        public static extern bool ReadOrgStatus(uint hDev, ref ushort pStatus, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgSpec")]
        public static extern bool SetOrgSpec(uint hDev, ushort Spec, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgMarginPulse")]
        public static extern bool SetOrgMarginPulse(uint hDev, uint MarginPulse, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgDelay")]
        public static extern bool SetOrgDelay(uint hDev, ushort LimitDelay, ushort ScanDelay, ushort PulseDelay, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgErrorPulse")]
        public static extern bool SetOrgErrorPulse(uint hDev, uint CScanErrorPulse, uint PulseErrorPulse, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgOffsetPulse")]
        public static extern bool SetOrgOffsetPulse(uint hDev, uint OffsetPulse, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetOrgPresetPulse")]
        public static extern bool SetOrgPresetPulse(uint hDev, int PresetPulse, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ReadOrgParam")]
        public static extern bool ReadOrgParam(uint hDev, ref MC07_S_ORG_PARAM psOrgParam, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_ResetOrgFlag")]
        public static extern bool ResetOrgFlag(uint hDev, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Org")]
        public static extern bool Org(uint hDev, ushort OrgType, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_SetData")]
        public static extern void SetData(uint Data, ref MC07_S_DATA psData);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_GetData")]
        public static extern uint GetData(ref MC07_S_DATA psData);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortOpen")]
        public static extern bool BPortOpen(ushort UnitNo, ushort IoPort, ref uint phPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortClose")]
        public static extern bool BPortClose(uint hPort, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortOut")]
        public static extern bool BPortOut(uint hPort, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortOrOut")]
        public static extern bool BPortOrOut(uint hPort, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortAndOut")]
        public static extern bool BPortAndOut(uint hPort, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_BPortIn")]
        public static extern bool BPortIn(uint hPort, ref ushort pData, ref MC07_S_RESULT psResult);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Unsigned16")]
        public static extern ushort Unsigned16(short Data);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Signed16")]
        public static extern short Signed16(ushort Data);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Unsigned32")]
        public static extern uint Unsigned32(int Data);
        [DllImport("Mc07UsbC.dll", EntryPoint = "MC07_Signed32")]
        public static extern int Signed32(uint Data);
    }
}