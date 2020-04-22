using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMARTQuery
{
    class ATADFPClass
    {
        #region ATADFP_relate
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct IdeRegs
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DriverStatus
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdInParams
        {
            public uint cBufferSize;
            public IdeRegs irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
            public byte bBuffer;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdOutParams
        {
            public uint cBufferSize;
            public DriverStatus DriverStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] bBuffer;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Boolean DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            ref SendCmdInParams lpInBuffer,
            uint nInBufferSize,
            ref SendCmdOutParams lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            [Out] IntPtr lpOverlapped);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(byte[] dest, byte[] src, int count);
        #endregion

        #region param define
        public const Int32 IOCTL_ATA_PASS_THROUGH_DIRECT = 0x4d030;
        public const Int32 FSCTL_LOCK_VOLUME = 0x90018;
        public const Int32 FSCTL_UNLOCK_VOLUME = 0x9001C;
        public const Int32 DFP_RECEIVE_DRIVE_DATA = 0x0007c088;

        public const Int32 ATA_FLAGS_DRDY_REQUIRED = 0x01;
        public const Int32 ATA_FLAGS_DATA_IN = 0x02;
        public const Int32 ATA_FLAGS_DATA_OUT = 0x04;
        public const Int32 ATA_FLAGS_48BIT_COMMAND = 0x08;
        public const Int32 ATA_FLAGS_USE_DMA = 0x10;

        public const Int32 IDENTIFY_BUFFER_SIZE = 512;

        public const Int32 INVALID_HANDLE_VALUE = -1;

        #endregion

        public bool GetIDBuffer(IntPtr handle, ref byte[] buffer)
        {
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            bool status;
            uint bytesReturned = 0;
            byte bDriveNum = 0;

            inParam.irDriveRegs.bFeaturesReg = 0;
            inParam.irDriveRegs.bSectorCountReg = 0;
            inParam.irDriveRegs.bSectorNumberReg = 0;
            inParam.irDriveRegs.bCylLowReg = 0;
            inParam.irDriveRegs.bCylHighReg = 0;
            inParam.irDriveRegs.bDriveHeadReg = (byte)(0xA0 | ((bDriveNum & 1) << 4));
            inParam.irDriveRegs.bCommandReg = 0xEC;
            inParam.bDriveNumber = bDriveNum;
            inParam.cBufferSize = IDENTIFY_BUFFER_SIZE;

            status = DeviceIoControl(handle,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero);

            if (status == false)
            {
                return false;
            }
            memcpy(buffer, outParam.bBuffer, 512);
            return true;
        }
        public bool GetSMARTBuffer(IntPtr handle, ref byte[] buffer)
        {
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            bool status;
            uint bytesReturned = 0;
            byte bDriveNum = 0;

            inParam.irDriveRegs.bFeaturesReg = 0xD0;
            inParam.irDriveRegs.bSectorCountReg = 0;
            inParam.irDriveRegs.bSectorNumberReg = 0;
            inParam.irDriveRegs.bCylLowReg = 0x4F;
            inParam.irDriveRegs.bCylHighReg = 0xC2;
            inParam.irDriveRegs.bDriveHeadReg = (byte)(0xA0 | ((bDriveNum & 1) << 4));
            inParam.irDriveRegs.bCommandReg = 0xB0;
            inParam.bDriveNumber = bDriveNum;
            inParam.cBufferSize = IDENTIFY_BUFFER_SIZE;

            status = DeviceIoControl(handle,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero);

            if (status == false)
            {
                int errorMessage;
                errorMessage = Marshal.GetLastWin32Error();
                return false;
            }
            memcpy(buffer, outParam.bBuffer, 512);
            return true;
        }
    }
}
