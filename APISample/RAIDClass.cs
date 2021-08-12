using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SMARTQuery
{
    class RAIDClass
    {

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemSet(IntPtr dest, int c, int byteCount);

        [DllImport(@"Dlls\SSD_Raid64.dll", EntryPoint = "getPhyPortNVME", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getPhyPortNVME64(int scsiPort, IntPtr phyPort, ref int raidType, ref int raidStatus);
        [DllImport(@"Dlls\SSD_Raid32.dll", EntryPoint = "getPhyPortNVME", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getPhyPortNVME32(int scsiPort, IntPtr phyPort, ref int raidType, ref int raidStatus);

        [DllImport(@"Dlls\SSD_Raid64.dll", EntryPoint = "getNVMEIdSmart", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getNVMEIdSmart64(int scsiPort, int scsiTargetId, IntPtr id, IntPtr smart, ref ulong disksize);
        [DllImport(@"Dlls\SSD_Raid32.dll", EntryPoint = "getNVMEIdSmart", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getNVMEIdSmart32(int scsiPort, int scsiTargetId, IntPtr id, IntPtr smart, ref ulong disksize);

        public static bool getNvmePhyNPort(int scsiPort, IntPtr Phy, ref int raidType, ref int raidStatus)
        {
            return IntPtr.Size == 8 /* 64bit */ ? getPhyPortNVME64(scsiPort, Phy, ref raidType, ref raidStatus) : getPhyPortNVME32(scsiPort, Phy, ref raidType, ref raidStatus);
        }

        public static bool getNVMEIdSmart(int scsiPort, int scsiTargetId, IntPtr id, IntPtr smart, ref ulong disksize)
        {
            return IntPtr.Size == 8 /* 64bit */ ? getNVMEIdSmart64(scsiPort, scsiTargetId, id, smart, ref disksize) : getNVMEIdSmart32(scsiPort, scsiTargetId, id, smart, ref disksize);
        }




        [DllImport(@"Dlls\SSD_Raid64.dll", EntryPoint = "getPhyPort", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getPhyPort64(int scsiPort, ref CSMI_SAS_PHY_INFO csni_sas_phy_info);
        [DllImport(@"Dlls\SSD_Raid32.dll", EntryPoint = "getPhyPort", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getPhyPort32(int scsiPort, ref CSMI_SAS_PHY_INFO csni_sas_phy_info);

        [DllImport(@"Dlls\SSD_Raid64.dll", EntryPoint = "getIdSmart", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getIdSmart64(int scsiPort, int Phy, int Identify, IntPtr address, IntPtr id, IntPtr snart);
        [DllImport(@"Dlls\SSD_Raid32.dll", EntryPoint = "getIdSmart", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool getIdSmart32(int scsiPort, int Phy, int Identify, IntPtr address, IntPtr id, IntPtr snart);

        private bool getPhyPort(int scsiPort, ref CSMI_SAS_PHY_INFO csni_sas_phy_info)
        {
            return IntPtr.Size == 8 /* 64bit */ ? getPhyPort64(scsiPort, ref csni_sas_phy_info) : getPhyPort32(scsiPort, ref csni_sas_phy_info);
        }

        private bool getIdSmart(int scsiPort, int Phy, int Identify, IntPtr address, IntPtr id, IntPtr snart)
        {
            return IntPtr.Size == 8 /* 64bit */ ? getIdSmart64(scsiPort, Phy, Identify, address, id, snart) : getIdSmart32(scsiPort, Phy, Identify, address, id, snart);
        }



        public struct RaidInfo
        {
            public byte[] id;
            public byte[] smart;
            public string ctl;
            public int raidType;
            public int raidStatus;
            public ulong nvmeDiskSize;
        }


        public struct CSMI_SAS_PHY_INFO
        {
            public char bNumberOfPhys;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public char[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public CSMI_SAS_PHY_ENTITY[] Phy;
        }

        public struct CSMI_SAS_PHY_ENTITY
        {
            public CSMI_SAS_IDENTIFY Identify;
            public char bPortIdentifier;
            public char bNegotiatedLinkRate;
            public char bMinimumLinkRate;
            public char bMaximumLinkRate;
            public char bPhyChangeCount;
            public char bAutoDiscover;
            public char bPhyFeatures;
            public char bReserved;
            public CSMI_SAS_IDENTIFY Attached;
        }

        public struct CSMI_SAS_IDENTIFY
        {
            public char bDeviceType;
            public char bRestricted;
            public char bInitiatorPortProtocol;
            public char bTargetPortProtocol;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] bRestricted2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public char[] bSASAddress;
            public char bPhyIdentifier;
            public char bSignalClass;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public char[] bReserved;
        }

        public List<RaidInfo> GetRaidDeviceList_SATA()
        {
            List<RaidInfo> infoList = new List<RaidInfo>();
            byte[] id_buf = new byte[512];
            byte[] smart_buf = new byte[512];
            //scan SATA raid device
            const int MAX_SEARCH_SCSI_PORT = 16;

            for (int index = 0; index < MAX_SEARCH_SCSI_PORT; index++)
            {
                CSMI_SAS_PHY_INFO csni_sas_phy_info = new CSMI_SAS_PHY_INFO();
                bool res = getPhyPort(index, ref csni_sas_phy_info);

                //////////////////////////////////////////////////////////////
                IntPtr intPtr_id = IntPtr.Zero;
                IntPtr intPtr_smart = IntPtr.Zero;
                IntPtr intPtr_addr = IntPtr.Zero;

                for (int i = 0; i < csni_sas_phy_info.bNumberOfPhys; i++)
                {
                    byte[] id = new byte[512];
                    GCHandle handle_id = GCHandle.Alloc(id, GCHandleType.Pinned);
                    intPtr_id = handle_id.AddrOfPinnedObject();

                    byte[] smart = new byte[512];
                    GCHandle handle_smart = GCHandle.Alloc(smart, GCHandleType.Pinned);
                    intPtr_smart = handle_smart.AddrOfPinnedObject();

                    byte[] addr = Encoding.GetEncoding("UTF-8").GetBytes(csni_sas_phy_info.Phy[i].Attached.bSASAddress);
                    GCHandle handle_addr = GCHandle.Alloc(addr, GCHandleType.Pinned);
                    intPtr_addr = handle_addr.AddrOfPinnedObject();

                    res = getIdSmart(index, csni_sas_phy_info.Phy[i].Attached.bPhyIdentifier, csni_sas_phy_info.Phy[i].bPortIdentifier, intPtr_addr, intPtr_id, intPtr_smart);

                    if (res)
                    {
                        Marshal.Copy(id, 0, intPtr_id, id.Length);
                        Marshal.Copy(smart, 0, intPtr_smart, smart.Length);
                        string Model_number = "";
                        Model_number = UtilityClass.getModelName(id, false);
                        Model_number = Model_number.Replace("\0", "").Trim();

                        if (Model_number.Length > 0)
                        {
                            string ctlName = "";
                            for (int j = 400; j < 410; j = j + 2)   //get controller
                            {
                                if (smart[j] != 0 || smart[j + 1] != 0)
                                {
                                    ctlName = string.Concat(ctlName, Convert.ToString((char)smart[j]), Convert.ToString((char)smart[j + 1]));
                                }
                            }
                            if (ctlName.IndexOf("\0", StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                ctlName = ctlName.Replace("\0", "");
                            }

                            RaidInfo info = new RaidInfo();
                            info.id = id;
                            info.smart = smart;
                            info.ctl = ctlName.Trim(); ;

                            infoList.Add(info);
                        }

                    }
                }
            }
            return infoList;
        }


        public List<RaidInfo> GetRaidDeviceList_NVMe()
        {
            //scan NVME raid device
            List<RaidInfo> infoList = new List<RaidInfo>();
            IntPtr intPtr_nvmeid = IntPtr.Zero;
            IntPtr intPtr_nvmesmart = IntPtr.Zero;
            IntPtr intPtr_port = IntPtr.Zero;
            byte[] port = new byte[16];
            int raid_type = -1;
            int raid_status = -1;

            GCHandle handle_port = GCHandle.Alloc(port, GCHandleType.Pinned);
            intPtr_port = handle_port.AddrOfPinnedObject();
            for (int i = 0; i < 16; i++)
            {
                int scsiport = i;

                bool res = getNvmePhyNPort(i, intPtr_port, ref raid_type, ref raid_status);
                Marshal.Copy(port, 0, intPtr_port, port.Length);
                if (res)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        if (port[k] == 0)
                        {
                            continue;
                        }
                        intPtr_nvmeid = IntPtr.Zero;
                        byte[] id = new byte[4096];
                        GCHandle id_addr = GCHandle.Alloc(id, GCHandleType.Pinned);
                        intPtr_nvmeid = id_addr.AddrOfPinnedObject();

                        intPtr_nvmesmart = IntPtr.Zero;
                        byte[] smart = new byte[512];
                        GCHandle smart_addr = GCHandle.Alloc(smart, GCHandleType.Pinned);
                        intPtr_nvmesmart = smart_addr.AddrOfPinnedObject();
                        ulong disksize = 0;
                        bool isgetData = getNVMEIdSmart(scsiport, port[k], intPtr_nvmeid, intPtr_nvmesmart, ref disksize);
                        if (isgetData)
                        {
                            Marshal.Copy(id, 0, intPtr_nvmeid, id.Length);
                            ASCIIEncoding ascii = new ASCIIEncoding();
                            string modelName = "", serial_number = "", firmware_version = "";
                            for (int j = 24; j <= 63; j++)
                            {
                                modelName += ascii.GetString(id, j, 1);
                                modelName = modelName.Replace("\0", "");

                            }

                            for (int j = 4; j <= 23; j++)
                            {
                                if (!id[i].Equals(0))
                                    serial_number += ascii.GetString(id, j, 1);
                                else
                                    break;
                            }
                            ///Model 
                            for (int j = 64; j <= 71; j++)
                            {
                                firmware_version += ascii.GetString(id, j, 1);
                            }
                            Console.WriteLine(modelName.Trim());
                            Console.WriteLine(serial_number.Trim());
                            Console.WriteLine(firmware_version.Trim());
                            Marshal.Copy(smart, 0, intPtr_nvmesmart, smart.Length);

                            RaidInfo info = new RaidInfo();
                            info.id = id;
                            info.smart = smart;
                            info.raidType = raid_type;
                            info.raidStatus = raid_status;
                            info.nvmeDiskSize = disksize;

                            infoList.Add(info);
                        }
                    }
                }
            }
            return infoList;
        }
    }
}
