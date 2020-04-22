using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMARTQuery
{
    class ATAClass
    {
        #region ATA_relate
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATA_PASS_THROUGH_DIRECT
        {
            ///The ATA_PASS_THROUGH_DIRECT structure is used in conjunction with an IOCTL_ATA_PASS_THROUGH_DIRECT 
            ///request to instruct the port driver to send an embedded ATA command to the target device. 
            public ushort Length;
            public ushort AtaFlags;
            public byte PathId;
            public byte TargetId;
            public byte Lun;
            public byte ReservedAsUchar;
            public uint DataTransferLength;
            public uint TimeOutValue;
            public uint ReservedAsUlong;
            public IntPtr offset;
            ///Specifies the contents of the input task file register prior to the current pass-through command.
            ///This member is not used when the ATA_FLAGS_48BIT_COMMAND flag is not set. 
            public TaskFile PreviousTaskFile;
            ///Specifies the content of the task file register on both input and output. 
            ///When IOCTL_ATA_PASS_THROUGH_DIRECT completes, the port driver updates CurrentTaskFile with the values that are 
            ///present in the device's output registers at the completion of the embedded command. The array values in CurrentTaskFile 
            ///correspond to the following task file output registers.
            ///Byte 0 is Error Register
            ///Byte 6 is Status Register
            public TaskFile CurrentTaskFile;
        }

        public const Int32 ATA_FLAGS_DRDY_REQUIRED = 0x01;
        public const Int32 ATA_FLAGS_DATA_IN = 0x02;
        public const Int32 ATA_FLAGS_DATA_OUT = 0x04;
        public const Int32 ATA_FLAGS_48BIT_COMMAND = 0x08;
        public const Int32 ATA_FLAGS_USE_DMA = 0x10;

        public const Int32 IOCTL_ATA_PASS_THROUGH_DIRECT = 0x4d030;
        public const Int32 FSCTL_LOCK_VOLUME = 0x90018;
        public const Int32 FSCTL_UNLOCK_VOLUME = 0x9001C;
        public const Int32 DFP_RECEIVE_DRIVE_DATA = 0x0007c088;
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


        public struct TaskFile
        {
            public byte Feature;
            public byte Count;
            public byte Sector_Number_Register;
            public byte Cylinder_Low_Register;
            public byte Cylinder_High_Register;
            public byte Devic_Register;
            public byte Command;
            public byte Reserved;
        }

        public bool GetIDBuffer(IntPtr handle, ref byte[] buffer)
        {
            TaskFile currentTaskFile = new TaskFile();
            TaskFile preTaskFile = new TaskFile();

            currentTaskFile.Command = 0xEC;
            currentTaskFile.Devic_Register = 0xA0;

            return ATACommand(handle, ref currentTaskFile, ref preTaskFile, 512, ref buffer, true, false, false, false);
        }

        public bool GetSMARTBuffer(IntPtr handle, ref byte[] buffer)
        {
            IntPtr result = IntPtr.Zero;
            IntPtr buff1 = IntPtr.Zero;

            ATA_PASS_THROUGH_DIRECT input = new ATA_PASS_THROUGH_DIRECT();

            input.Length = (UInt16)Marshal.SizeOf(input);
            input.AtaFlags = ATA_FLAGS_DATA_IN;
            input.TimeOutValue = 2;

            TaskFile currentTaskFile = new TaskFile();
            TaskFile preTaskFile = new TaskFile();
            currentTaskFile.Devic_Register = 0xA0;
            currentTaskFile.Feature = 0xD0;
            currentTaskFile.Command = 0xB0;
            currentTaskFile.Cylinder_Low_Register = 0x4F;
            currentTaskFile.Cylinder_High_Register = 0xC2;

            return ATACommand(handle, ref currentTaskFile, ref preTaskFile, 512, ref buffer, true, false, false, false);
        }


        #region ATACommand
        private bool ATACommand(IntPtr handle, ref TaskFile currentTaskFile, ref TaskFile preTaskFile, int dataTransferSzie, ref byte[] buffer, bool isRead, bool isDMA, bool isExCmd, bool isnodata)
        {
            //If parameter isRead is true, the command is a read command.
            //Otherwise it's a write command.
            ATA_PASS_THROUGH_DIRECT input;
            bool status;
            Int32 nBytesReturned;
            IntPtr result = IntPtr.Zero;
            IntPtr buff1 = IntPtr.Zero;
            int errorMessage;

            input = new ATA_PASS_THROUGH_DIRECT();
            input.Length = (UInt16)Marshal.SizeOf(input);
            if (!isRead)
                input.AtaFlags = ATA_FLAGS_DATA_OUT;    //For write command
            else
                input.AtaFlags = ATA_FLAGS_DATA_IN;     //For read command

            if (isnodata)
            {
                input.AtaFlags = ATA_FLAGS_DRDY_REQUIRED;     //For no data
            }

            if (isDMA)
                input.AtaFlags |= ATA_FLAGS_USE_DMA;    //For direct memory access
            if (isExCmd)
                input.AtaFlags |= ATA_FLAGS_48BIT_COMMAND;  //For 48 bit command

            input.TimeOutValue = 60;
            input.DataTransferLength = (uint)(dataTransferSzie);    //The buffer size that you want to read or write

            input.CurrentTaskFile = currentTaskFile;
            input.PreviousTaskFile = preTaskFile;

            buff1 = Marshal.AllocHGlobal((int)(dataTransferSzie));
            Marshal.Copy(buffer, 0, buff1, (int)(dataTransferSzie));
            input.offset = buff1;
            result = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ATA_PASS_THROUGH_DIRECT)));
            Marshal.StructureToPtr(input, result, false);


            status = DeviceIoControl(handle,
                IOCTL_ATA_PASS_THROUGH_DIRECT,
                result,
                Marshal.SizeOf(input),
                result,
                Marshal.SizeOf(input),
                out nBytesReturned,
                IntPtr.Zero);

            if (status == false)
            {
                errorMessage = Marshal.GetLastWin32Error();
            }

            input = (ATA_PASS_THROUGH_DIRECT)Marshal.PtrToStructure(result, typeof(ATA_PASS_THROUGH_DIRECT));
            currentTaskFile = input.CurrentTaskFile;
            preTaskFile = input.PreviousTaskFile;

            Marshal.Copy(buff1, buffer, 0, dataTransferSzie);

            Marshal.FreeHGlobal(buff1);
            Marshal.FreeHGlobal(result);

            if (status == false)
                return false;
            else
                return true;
        }
        #endregion
    }
}
