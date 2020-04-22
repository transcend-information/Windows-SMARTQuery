using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMARTQuery
{
    class NVMeClass
    {
        #region Define Const
        public const Int32 IOCTL_STORAGE_QUERY_PROPERTY = 0x2d1400;
        public const UInt32 STORAGE_PROTOCOL_STRUCTURE_VERSION = 0x01;

        public const UInt32 STORAGE_PROTOCOL_COMMAND_FLAG_ADAPTER_REQUEST = 0x80000000;
        public const UInt32 STORAGE_PROTOCOL_COMMAND_LENGTH = 0x40;//NVME commands are always 64 bytes
        //
        //Command specific Information for Storage Protocols - "CommandSpecific" field
        //
        public const UInt32 STORAGE_PROTOCOL_SPECIFIC_NVME_ADMIN_COMMAND = 0x01;
        public const UInt32 STORAGE_PROTOCOL_SPECIFIC_NVME_NVM_COMMAND = 0x02;

        public const UInt32 FILE_DEVICE_MASS_STORAGE = 0x2D;//45
        public const UInt32 METHOD_BUFFERED = 0;
        public const UInt32 IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;

        public const UInt32 FILE_ANY_ACCESS = 0;
        public const UInt32 FILE_READ_ACCESS = 0x01;
        public const UInt32 FILE_WRITE_ACCESS = 0x02;

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            return ((DeviceType) << 16 | (Access) << 14 | (Function) << 2 | (Method));
        }

        public UInt32 IOCTL_STORAGE_PROTOCOL_COMMAND = CTL_CODE(IOCTL_STORAGE_BASE, 0x04F0, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS);
        #endregion

        #region NVMe_related

        public enum STORAGE_PROPERTY_ID
        {
            StorageDeviceProperty = 0,
            StorageAdapterProperty,
            StorageDeviceIdProperty,
            StorageDeviceUniqueIdProperty,
            StorageDeviceWriteCacheProperty,
            StorageMiniportProperty,
            StorageAccessAlignmentProperty,
            StorageDeviceSeekPenaltyProperty,
            StorageDeviceTrimProperty,
            StorageDeviceWriteAggregationProperty,
            StorageDeviceDeviceTelemetryProperty,
            StorageDeviceLBProvisioningProperty,
            StorageDevicePowerProperty,
            StorageDeviceCopyOffloadProperty,
            StorageDeviceResiliencyProperty,
            StorageDeviceMediumProductType,
            StorageDeviceIoCapabilityProperty = 48,
            StorageAdapterProtocolSpecificProperty,
            StorageDeviceProtocolSpecificProperty,
            StorageAdapterTemperatureProperty,
            StorageDeviceTemperatureProperty,
            StorageAdapterPhysicalTopologyProperty,
            StorageDevicePhysicalTopologyProperty,
            StorageDeviceAttributesProperty
        };
        public enum STORAGE_QUERY_TYPE
        {
            PropertyStandardQuery = 0,
            PropertyExistsQuery,
            PropertyMaskQuery,
            PropertyQueryMaxDefined
        };
        public enum STORAGE_PROTOCOL_TYPE
        {
            ProtocolTypeUnknown = 0,   // 0x0
            ProtocolTypeScsi,
            ProtocolTypeAta,
            ProtocolTypeNvme,
            ProtocolTypeSd,
            ProtocolTypeProprietary = 126, // 0x7E
            ProtocolTypeMaxReserved = 127 // 0x7F
        } ;

        /* ------Identify command CNV value  Spec 5.11 ----- */
        public enum NVME_IDENTIFY_CNS
        {

            /** Identify namespace indicated in CDW1.NSID */
            NVME_IDENTIFY_NS = 0x00,

            /** Identify controller */
            NVME_IDENTIFY_CTRLR = 0x01,

            /** List active NSIDs greater than CDW1.NSID */
            NVME_IDENTIFY_ACTIVE_NS_LIST = 0x02,

            /** List allocated NSIDs greater than CDW1.NSID */
            NVME_IDENTIFY_ALLOCATED_NS_LIST = 0x10,

            /** Identify namespace if CDW1.NSID is allocated */
            NVME_IDENTIFY_NS_ALLOCATED = 0x11,

            /** Get list of controllers starting at CDW10.CNTID that are attached to CDW1.NSID */
            NVME_IDENTIFY_NS_ATTACHED_CTRLR_LIST = 0x12,

            /** Get list of controllers starting at CDW10.CNTID */
            NVME_IDENTIFY_CTRLR_LIST = 0x13,
        };

        public enum NVME_Log_Page_Identifier
        {
            ERROR_INFORMATION = 0x01,
            SMART_HEALTH_INFORMATION = 0x02,
            FIRMWARE_SLOT_INFORMATION = 0x03,
            CHANGED_NAMESPEACE_LIST = 0x04,
            COMMAND_EFFECTS_LOG = 0x05
        };

        public enum STORAGE_PROTOCOL_NVME_DATA_TYPE
        {
            NVMeDataTypeUnknown = 0,
            NVMeDataTypeIdentify,
            NVMeDataTypeLogPage,
            NVMeDataTypeFeature
        } ;

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_PROTOCOL_SPECIFIC_DATA
        {

            public STORAGE_PROTOCOL_TYPE ProtocolType;
            public int DataType;
            public int ProtocolDataRequestValue;
            public int ProtocolDataRequestSubValue;
            public int ProtocolDataOffset;
            public int ProtocolDataLength;
            public int FixedProtocolReturnData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] Reserved;

        } ;

        public struct STORAGE_PROTOCOL_DATA_DESCRIPTOR
        {
            int Version;
            int Size;
            STORAGE_PROTOCOL_SPECIFIC_DATA ProtocolSpecificData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_PROPERTY_QUERY
        {
            public STORAGE_PROPERTY_ID PropertyId;
            public STORAGE_QUERY_TYPE QueryType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STRAGE_QUERY_WITH_BUFFER
        {
            public STORAGE_PROPERTY_QUERY Query;
            public STORAGE_PROTOCOL_SPECIFIC_DATA ProtocolData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public byte[] Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NVME_HEALTH_INFO_LOGO
        {
            public Byte CriticalWarning;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public Byte[] Temperature;                 // Temperature: Contains the temperature of the overall device (controller and NVM included) in units of Kelvin. If the temperature exceeds the temperature threshold, refer to section 5.12.1.4, then an asynchronous event completion may occur
            public Byte AvailableSpare;                 // Available Spare:  Contains a normalized percentage (0 to 100%) of the remaining spare capacity available
            public Byte AvailableSpareThreshold;        // Available Spare Threshold:  When the Available Spare falls below the threshold indicated in this field, an asynchronous event  completion may occur. The value is indicated as a normalized percentage (0 to 100%).
            public Byte PercentageUsed;                 // Percentage Used
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public Byte[] Reserved0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] DataUnitRead;              // Data Units Read:  Contains the number of 512 byte data units the host has read from the controller; this value does not include metadata. This value is reported in thousands (i.e., a value of 1 corresponds to 1000 units of 512 bytes read)  and is rounded up.  When the LBA size is a value other than 512 bytes, the controller shall convert the amount of data read to 512 byte units. For the NVM command set, logical blocks read as part of Compare and Read operations shall be included in this value
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] DataUnitWritten;            // Data Units Written: Contains the number of 512 byte data units the host has written to the controller; this value does not include metadata. This value is reported in thousands (i.e., a value of 1 corresponds to 1000 units of 512 bytes written)  and is rounded up.  When the LBA size is a value other than 512 bytes, the controller shall convert the amount of data written to 512 byte units. For the NVM command set, logical blocks written as part of Write operations shall be included in this value. Write Uncorrectable commands shall not impact this value.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] HostReadCommands;           // Host Read Commands:  Contains the number of read commands  completed by  the controller. For the NVM command set, this is the number of Compare and Read commands. 
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] HostWrittenCommands;         // Host Write Commands:  Contains the number of write commands  completed by  the controller. For the NVM command set, this is the number of Write commands.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] ControllerBusyTime;         // Controller Busy Time:  Contains the amount of time the controller is busy with I/O commands. The controller is busy when there is a command outstanding to an I/O Queue (specifically, a command was issued via an I/O Submission Queue Tail doorbell write and the corresponding  completion queue entry  has not been posted yet to the associated I/O Completion Queue). This value is reported in minutes.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] PowerCycle;                 // Power Cycles: Contains the number of power cycles.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] PowerOnHours;               // Power On Hours: Contains the number of power-on hours. This does not include time that the controller was powered and in a low power state condition.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] UnsafeShutdowns;            // Unsafe Shutdowns: Contains the number of unsafe shutdowns. This count is incremented when a shutdown notification (CC.SHN) is not received prior to loss of power.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] MediaErrors;                // Media Errors:  Contains the number of occurrences where the controller detected an unrecovered data integrity error. Errors such as uncorrectable ECC, CRC checksum failure, or LBA tag mismatch are included in this field.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public Byte[] ErrorInfoLogEntryCount;     // Number of Error Information Log Entries:  Contains the number of Error Information log entries over the life of the controller
            public uint WarningCompositeTemperatureTime;     // Warning Composite Temperature Time: Contains the amount of time in minutes that the controller is operational and the Composite Temperature is greater than or equal to the Warning Composite Temperature Threshold (WCTEMP) field and less than the Critical Composite Temperature Threshold (CCTEMP) field in the Identify Controller data structure
            public uint CriticalCompositeTemperatureTime;    // Critical Composite Temperature Time: Contains the amount of time in minutes that the controller is operational and the Composite Temperature is greater the Critical Composite Temperature Threshold (CCTEMP) field in the Identify Controller data structure
            public ushort TemperatureSensor1;          // Contains the current temperature reported by temperature sensor 1.
            public ushort TemperatureSensor2;          // Contains the current temperature reported by temperature sensor 2.
            public ushort TemperatureSensor3;          // Contains the current temperature reported by temperature sensor 3.
            public ushort TemperatureSensor4;          // Contains the current temperature reported by temperature sensor 4.
            public ushort TemperatureSensor5;          // Contains the current temperature reported by temperature sensor 5.
            public ushort TemperatureSensor6;          // Contains the current temperature reported by temperature sensor 6.
            public ushort TemperatureSensor7;          // Contains the current temperature reported by temperature sensor 7.
            public ushort TemperatureSensor8;          // Contains the current temperature reported by temperature sensor 8.
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 296)]
            public Byte[] Reserved1;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NVMe_Command_DWord_0
        {
            public Byte OPC;
            public Byte Byte2;//Fused Operation bit 8:9 Reserved 10:13 PSDT 14:15
            public short CID;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct Nvme_Command
        {
            public NVMe_Command_DWord_0 CDW0;
            public UInt32 NSID;
            public ulong Reserved;
            public ulong MPTR;
            public ulong PRP1;
            public ulong PRP2;
            public UInt32 DW10;
            public UInt32 DW11;
            public UInt32 DW12;
            public UInt32 DW13;
            public UInt32 DW14;
            public UInt32 DW15;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct STORAGE_PROTOCOL_COMMAND
        {
            public UInt32 Version;
            public UInt32 Length;
            public STORAGE_PROTOCOL_TYPE protocolType;
            public UInt32 Flags;
            public UInt32 ReturnStatus;
            public UInt32 ErroCode;
            public UInt32 CommandLength;
            public UInt32 ErrorInfolength;
            public UInt32 DataToDeviceTransferLength;
            public UInt32 DataFromDeviceTransferLength;
            public UInt32 TimeoutValue;
            public UInt32 ErrorInfoOffset;
            public UInt32 DataToDeviceBufferOffset;
            public UInt32 DataFromDeviceBufferOffset;
            public UInt32 CommandSpecific;
            public UInt32 Reserved0;
            public UInt32 FixedProtocolResturnData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public UInt32[] Reserved1;
            public Nvme_Command command;
            public IntPtr Data_Buffer;

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

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemSet(IntPtr dest, int c, int byteCount);

        public bool getIDBuffer(IntPtr handle, ref byte[] Table)
        {
            IntPtr buffer = IntPtr.Zero;
            int BufferSize = 4096;
            int nBytesReturned;
            int bufferLength = 0;
            bool result;

            Table = new byte[4096];
            STRAGE_QUERY_WITH_BUFFER QUERY_BUFFER = new STRAGE_QUERY_WITH_BUFFER();
            if (handle == null)
            {
                Console.WriteLine("Create Handle Fail");
            }

            //
            // Allocate buffer for use.
            //
            buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(STRAGE_QUERY_WITH_BUFFER)));
            bufferLength = Marshal.SizeOf(typeof(STRAGE_QUERY_WITH_BUFFER));
            MemSet(buffer, 0x00, bufferLength);


            // Initialize query data structure to get Identify Controller Data.

            QUERY_BUFFER.Query.PropertyId = STORAGE_PROPERTY_ID.StorageAdapterProtocolSpecificProperty;
            QUERY_BUFFER.Query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;

            QUERY_BUFFER.ProtocolData.ProtocolType = STORAGE_PROTOCOL_TYPE.ProtocolTypeNvme;
            QUERY_BUFFER.ProtocolData.DataType = (int)STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeIdentify;
            QUERY_BUFFER.ProtocolData.ProtocolDataRequestValue = (int)NVME_IDENTIFY_CNS.NVME_IDENTIFY_CTRLR;
            QUERY_BUFFER.ProtocolData.ProtocolDataRequestSubValue = 0;
            QUERY_BUFFER.ProtocolData.ProtocolDataOffset = Marshal.SizeOf(QUERY_BUFFER.ProtocolData);
            QUERY_BUFFER.ProtocolData.ProtocolDataLength = BufferSize;

            Marshal.StructureToPtr(QUERY_BUFFER, buffer, false);
            result = DeviceIoControl(handle, IOCTL_STORAGE_QUERY_PROPERTY, buffer, bufferLength, buffer, bufferLength, out nBytesReturned, IntPtr.Zero);

            if (result == false)
            {
                System.Console.WriteLine("ERROR");
                System.Console.WriteLine(Marshal.GetLastWin32Error().ToString());
                Marshal.FreeHGlobal(buffer);
                return false;
            }

            //pull back the data to sturctur
            QUERY_BUFFER = (STRAGE_QUERY_WITH_BUFFER)Marshal.PtrToStructure(buffer, typeof(STRAGE_QUERY_WITH_BUFFER));
            Array.Copy(QUERY_BUFFER.Buffer, Table, BufferSize);

            //close handle and free the memory
            Marshal.FreeHGlobal(buffer);
            return true;
        }

        public bool getSMARTBuffer(IntPtr handle, ref byte[] Table)
        {
            //The data structure is 512 bytes in size
            IntPtr buffer = IntPtr.Zero;
            int BufferSize = 512;
            int nBytesReturned;
            int bufferLength = 0;
            bool result;


            STRAGE_QUERY_WITH_BUFFER QUERY_BUFFER = new STRAGE_QUERY_WITH_BUFFER();

            if (handle == null)
            {
                Console.WriteLine("Create Handle Fail");
            }

            buffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(STRAGE_QUERY_WITH_BUFFER)));
            bufferLength = Marshal.SizeOf(typeof(STRAGE_QUERY_WITH_BUFFER));
            MemSet(buffer, 0x00, bufferLength);

            if (buffer == IntPtr.Zero)
            {
                System.Console.WriteLine("GetIDTable allocate buffer failed");

            }

            // Initialize query data structure to get Identify Controller Data.

            QUERY_BUFFER.Query.PropertyId = STORAGE_PROPERTY_ID.StorageAdapterProtocolSpecificProperty;
            QUERY_BUFFER.Query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;

            QUERY_BUFFER.ProtocolData.ProtocolType = STORAGE_PROTOCOL_TYPE.ProtocolTypeNvme;
            QUERY_BUFFER.ProtocolData.DataType = (int)STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage;
            QUERY_BUFFER.ProtocolData.ProtocolDataRequestValue = (int)NVME_Log_Page_Identifier.SMART_HEALTH_INFORMATION;
            QUERY_BUFFER.ProtocolData.ProtocolDataRequestSubValue = 0;
            QUERY_BUFFER.ProtocolData.ProtocolDataOffset = Marshal.SizeOf(QUERY_BUFFER.ProtocolData);
            QUERY_BUFFER.ProtocolData.ProtocolDataLength = BufferSize;

            Marshal.StructureToPtr(QUERY_BUFFER, buffer, false);
            result = DeviceIoControl(handle, IOCTL_STORAGE_QUERY_PROPERTY, buffer, bufferLength, buffer, bufferLength, out nBytesReturned, IntPtr.Zero);

            if (result == false)
            {
                System.Console.WriteLine("ERROR");
                System.Console.WriteLine(Marshal.GetLastWin32Error().ToString());
                Marshal.FreeHGlobal(buffer);
                return false;
            }

            //pull back the data to sturctur
            QUERY_BUFFER = (STRAGE_QUERY_WITH_BUFFER)Marshal.PtrToStructure(buffer, typeof(STRAGE_QUERY_WITH_BUFFER));
            Array.Copy(QUERY_BUFFER.Buffer, Table, BufferSize);

            //close handle and free the memory
            Marshal.FreeHGlobal(buffer);
            return true;
        }
    }
}
