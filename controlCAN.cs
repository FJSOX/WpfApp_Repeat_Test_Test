using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WpfApp_Repeat_Test_Test
{
    //1.ZLGCAN系列接口卡信息的数据类型。
    [StructLayout(LayoutKind.Sequential)]
    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
    }


    /////////////////////////////////////////////////////
    //2.定义CAN信息帧的数据类型。
    [StructLayout(LayoutKind.Sequential)]
    public struct VCI_CAN_OBJ
    {
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;//是否是远程帧
        public byte ExternFlag;//是否是扩展帧
        public byte DataLen;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;
    }
    ////2.定义CAN信息帧的数据类型。
    //[StructLayout(LayoutKind.Sequential)]
    //public struct VCI_CAN_OBJ 
    //{
    //    public UInt32 ID;
    //    public UInt32 TimeStamp;
    //    public byte TimeFlag;
    //    public byte SendType;
    //    public byte RemoteFlag;//是否是远程帧
    //    public byte ExternFlag;//是否是扩展帧
    //    public byte DataLen;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    //    public byte[] Data;
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    //    public byte[] Reserved;

    //    public void Init()
    //    {
    //        Data = new byte[8];
    //        Reserved = new byte[3];
    //    }
    //}

    //3.定义CAN控制器状态的数据类型。
    [StructLayout(LayoutKind.Sequential)]
    public struct VCI_CAN_STATUS
    {
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    //4.定义错误信息的数据类型。
    [StructLayout(LayoutKind.Sequential)]
    public struct VCI_ERR_INFO
    {
        public UInt32 ErrCode;
        public byte Passive_ErrData1;
        public byte Passive_ErrData2;
        public byte Passive_ErrData3;
        public byte ArLost_ErrData;
    }

    //5.定义初始化CAN的数据类型
    [StructLayout(LayoutKind.Sequential)]
    public struct VCI_INIT_CONFIG
    {
        public UInt32 AccCode;
        public UInt32 AccMask;
        public UInt32 Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }
    public struct controlCAN
    {
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, VCI_CAN_OBJ[] pSend, UInt32 Len);

        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        public static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
    };


    // 函数调用返回状态值
    public enum STATUS
    {
        STATUS_OK = 1,
        STATUS_ERR = 0,
    };

    // 函数调用返回状态值
    public enum CMD
    {
        CMD_DESIP = 0,
        CMD_DESPORT = 1,
        CMD_SRCPORT = 2,
        CMD_CHGDESIPANDPORT = 2,
        CMD_TCP_TYPE = 4,//tcp 工作方式，服务器:1 或是客户端:0
    };

    public enum TCPTYPE
    {
        TCP_CLIENT = 0,
        TCP_SERVER = 1,
    }

    public enum REF
    {
        REFERENCE_BAUD = 1,
        REFERENCE_SET_TRANSMIT_TIMEOUT = 2,
        REFERENCE_ADD_FILTER = 3,
        REFERENCE_SET_FILTER = 4,
    };
}
