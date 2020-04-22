using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMARTQuery
{
    class SCSIClass
    {
        #region SCSI_related
        public struct SCSI_PASS_THROUGH_DIRECT
        {
            //The SCSI_PASS_THROUGH_DIRECT structure is used in conjunction with an IOCTL_SCSI_PASS_THROUGH_DIRECT 
            //request to instruct the port driver to send an embedded SCSI command to the target device. 
            public ushort Length;
            public byte ScsiStatus;
            public byte PathId;
            public byte TargetId;
            public byte Lun;
            public byte Cdblength;
            public byte SenseInfoLength;
            public byte DataIn;
            public uint DataTransferLength;
            public uint TimeOutValue;
            public IntPtr DataBufferOffset;
            public uint SenseInfoOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] cdb;
        };

        //windows 8
        public struct SCSI_PASS_THROUGH_DIRECT_EX
        {
            public ushort Version;
            public ushort Length;
            public uint CdbLength;
            public byte StorAddressLength;
            public byte ScsiStatus;
            public byte SenseInfoLength;
            public byte DataDirection;
            public byte Reserved;
            public uint TimeOutValue;
            public uint StorAddressOffset;
            public uint SenseInfoOffset;
            public uint DataOutTransferLength;
            public uint DataInTransferLength;
            public IntPtr DataOutBuffer;
            public IntPtr DataInBuffer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] cdb;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class SCSI_PASS_THROUGH_WITH_BUFFERS
        {
            internal SCSI_PASS_THROUGH_DIRECT sptd = new SCSI_PASS_THROUGH_DIRECT();
            // // adapt size to suit your needs!!!!!! 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal byte[] sense;
        };

        public const uint FILE_READ_ACCESS = (0x0001);
        public const uint FILE_WRITE_ACCESS = (0x0002);
        public const uint METHOD_BUFFERED = 0;
        public const uint IOCTL_SCSI_BASE = 0x00000004;

        // SCSI Pass Through Flags
        public static uint IOCTL_SCSI_PASS_THROUGH_DIRECT = CTL_CODE(IOCTL_SCSI_BASE, 0x0405, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);
        public static byte SCSI_IOCTL_DATA_OUT = 0;
        public static byte SCSI_IOCTL_DATA_IN = 1;

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method);
        }
        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern Boolean DeviceIoControl(
            IntPtr hFile,
            Int32 dwIoControlCode,
            IntPtr lpInBuffer,
            Int32 nInBufferSize,
            IntPtr lpOutBuffer,
            Int32 nOutBufferSize,
            out Int32 nBytesReturned,
            IntPtr lpOverlapped
            );

        public bool getIDBuffer(IntPtr handle, ref byte[] buffer)
        {
            byte[] cdb = new byte[16];
            cdb[0] = 0xA1;
            cdb[1] = 0x08;
            cdb[2] = 0x0E;
            cdb[4] = 0x01;
            cdb[8] = 0xA0;
            cdb[9] = 0xEC;

            return SCSICommand(handle, 16, cdb, 512, ref buffer, SCSI_IOCTL_DATA_IN);
        }
        public bool getSMARTBuffer(IntPtr handle, ref byte[] buffer)
        {
            buffer = new byte[512];
            byte[] cdb = new byte[16];

            cdb[0] = 0x85;
            cdb[1] = 0x08;
            cdb[2] = 0x0E;
            cdb[3] = 0x00;
            cdb[4] = 0xD0;
            cdb[5] = 0x00;
            cdb[6] = 0x01;
            cdb[7] = 0x00;
            cdb[8] = 0x00;
            cdb[9] = 0x00;
            cdb[10] = 0x4F;
            cdb[11] = 0x00;
            cdb[12] = 0xC2;
            cdb[13] = 0x00;
            cdb[14] = 0xB0;
            cdb[15] = 0x00;

            return SCSICommand(handle, 16, cdb, 512, ref buffer, SCSI_IOCTL_DATA_IN);
        }

        #region SCSICommand
        protected bool SCSICommand(IntPtr handle, byte cdbLength, byte[] cdb, int dataLength, ref byte[] dataBuffer, byte dataInOut)
        {
            bool status;
            Int32 nBytesReturned;
            IntPtr result = IntPtr.Zero;
            int errorMessage;

            SCSI_PASS_THROUGH_WITH_BUFFERS info = new SCSI_PASS_THROUGH_WITH_BUFFERS();

            info.sptd.cdb = new byte[cdbLength];
            info.sptd.cdb = cdb;
            info.sptd.Cdblength = cdbLength;
            info.sense = new byte[32];
            info.sptd.Length = (ushort)Marshal.SizeOf(typeof(SCSI_PASS_THROUGH_DIRECT));
            info.sptd.SenseInfoOffset = (uint)Marshal.OffsetOf(typeof(SCSI_PASS_THROUGH_WITH_BUFFERS), "sense");
            info.sptd.SenseInfoLength = 32;
            info.sptd.DataTransferLength = (uint)dataLength;
            info.sptd.DataBufferOffset = Marshal.AllocHGlobal(dataLength);
            info.sptd.TimeOutValue = 3;
            info.sptd.DataIn = dataInOut;

            if (dataInOut == SCSI_IOCTL_DATA_OUT)
                Marshal.Copy(dataBuffer, 0, info.sptd.DataBufferOffset, dataLength);

            result = Marshal.AllocHGlobal(Marshal.SizeOf(info));
            Marshal.StructureToPtr(info, result, false);

            status = DeviceIoControl(handle,
                (int)IOCTL_SCSI_PASS_THROUGH_DIRECT,
                result,
                Marshal.SizeOf(info),
                result,
                Marshal.SizeOf(info),
                out nBytesReturned,
                IntPtr.Zero);

            if (status == false)
            {
                errorMessage = Marshal.GetLastWin32Error();
                return false;
            }

            info = (SCSI_PASS_THROUGH_WITH_BUFFERS)Marshal.PtrToStructure(result, typeof(SCSI_PASS_THROUGH_WITH_BUFFERS));

            Marshal.Copy(info.sptd.DataBufferOffset, dataBuffer, 0, dataLength);

            Marshal.FreeHGlobal(result);
            Marshal.FreeHGlobal(info.sptd.DataBufferOffset);
            return true;
        }
        #endregion
    }
}
