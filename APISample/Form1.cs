using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMARTQuery
{
    public partial class Form1 : Form
    {
        #region EFileAccess
        public enum EFileAccess : uint
        {
            //
            // Standard Section
            //

            AccessSystemSecurity = 0x1000000,   // AccessSystemAcl access type
            MaximumAllowed = 0x2000000,     // MaximumAllowed access type

            Delete = 0x10000,
            ReadControl = 0x20000,
            WriteDAC = 0x40000,
            WriteOwner = 0x80000,
            Synchronize = 0x100000,

            StandardRightsRequired = 0xF0000,
            StandardRightsRead = ReadControl,
            StandardRightsWrite = ReadControl,
            StandardRightsExecute = ReadControl,
            StandardRightsAll = 0x1F0000,
            SpecificRightsAll = 0xFFFF,

            FILE_READ_DATA = 0x0001,        // file & pipe
            FILE_LIST_DIRECTORY = 0x0001,       // directory
            FILE_WRITE_DATA = 0x0002,       // file & pipe
            FILE_ADD_FILE = 0x0002,         // directory
            FILE_APPEND_DATA = 0x0004,      // file
            FILE_ADD_SUBDIRECTORY = 0x0004,     // directory
            FILE_CREATE_PIPE_INSTANCE = 0x0004, // named pipe
            FILE_READ_EA = 0x0008,          // file & directory
            FILE_WRITE_EA = 0x0010,         // file & directory
            FILE_EXECUTE = 0x0020,          // file
            FILE_TRAVERSE = 0x0020,         // directory
            FILE_DELETE_CHILD = 0x0040,     // directory
            FILE_READ_ATTRIBUTES = 0x0080,      // all
            FILE_WRITE_ATTRIBUTES = 0x0100,     // all

            //
            // Generic Section
            //

            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,

            SPECIFIC_RIGHTS_ALL = 0x00FFFF,
            FILE_ALL_ACCESS =
                StandardRightsRequired |
                Synchronize |
                0x1FF,

            FILE_GENERIC_READ =
                StandardRightsRead |
                FILE_READ_DATA |
                FILE_READ_ATTRIBUTES |
                FILE_READ_EA |
                Synchronize,

            FILE_GENERIC_WRITE =
                StandardRightsWrite |
                FILE_WRITE_DATA |
                FILE_WRITE_ATTRIBUTES |
                FILE_WRITE_EA |
                FILE_APPEND_DATA |
                Synchronize,

            FILE_GENERIC_EXECUTE =
                StandardRightsExecute |
                FILE_READ_ATTRIBUTES |
                FILE_EXECUTE |
                Synchronize
        }
        #endregion

        #region EFileShare
        [Flags]
        public enum EFileShare : uint
        {
            /// <summary>
            ///
            /// </summary>
            None = 0x00000000,
            /// <summary>
            /// Enables subsequent open operations on an object to request read access.
            /// Otherwise, other processes cannot open the object if they request read access.
            /// If this flag is not specified, but the object has been opened for read access, the function fails.
            /// </summary>
            Read = 0x00000001,
            /// <summary>
            /// Enables subsequent open operations on an object to request write access.
            /// Otherwise, other processes cannot open the object if they request write access.
            /// If this flag is not specified, but the object has been opened for write access, the function fails.
            /// </summary>
            Write = 0x00000002,
            /// <summary>
            /// Enables subsequent open operations on an object to request delete access.
            /// Otherwise, other processes cannot open the object if they request delete access.
            /// If this flag is not specified, but the object has been opened for delete access, the function fails.
            /// </summary>
            Delete = 0x00000004
        }
        #endregion

        #region ECreationDisposition
        public enum ECreationDisposition : uint
        {
            /// <summary>
            /// Creates a new file. The function fails if a specified file exists.
            /// </summary>
            New = 1,
            /// <summary>
            /// Creates a new file, always.
            /// If a file exists, the function overwrites the file, clears the existing attributes, combines the specified file attributes,
            /// and flags with FILE_ATTRIBUTE_ARCHIVE, but does not set the security descriptor that the SECURITY_ATTRIBUTES structure specifies.
            /// </summary>
            CreateAlways = 2,
            /// <summary>
            /// Opens a file. The function fails if the file does not exist.
            /// </summary>
            OpenExisting = 3,
            /// <summary>
            /// Opens a file, always.
            /// If a file does not exist, the function creates a file as if dwCreationDisposition is CREATE_NEW.
            /// </summary>
            OpenAlways = 4,
            /// <summary>
            /// Opens a file and truncates it so that its size is 0 (zero) bytes. The function fails if the file does not exist.
            /// The calling process must open the file with the GENERIC_WRITE access right.
            /// </summary>
            TruncateExisting = 5
        }
        #endregion

        #region EFileAttributes
        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }
        #endregion


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
           string lpFileName,
           EFileAccess dwDesiredAccess,
           EFileShare dwShareMode,
           IntPtr lpSecurityAttributes,
           ECreationDisposition dwCreationDisposition,
           EFileAttributes dwFlagsAndAttributes,
           IntPtr hTemplateFile);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);


        public struct SIbuffer
        {
            public byte[] id;
            public byte[] smart;
        }


        Dictionary<string, string> driveMapping = new Dictionary<string, string>();
        Dictionary<string, RAIDClass.RaidInfo> raidDict = new Dictionary<string, RAIDClass.RaidInfo>();
        Dictionary<string, SIbuffer> siBufDict = new Dictionary<string, SIbuffer>();
        bool isUSB = false;
        bool isNVMe = false;
        bool isWindows10 = false;

        byte[] driveInfo = new byte[512];
        byte[] SMARTInfo = new byte[512];

        public Form1()
        {
            InitializeComponent();
            getDiskInfo();
        }

        #region Change interface to ATA
        private void radioButton_ATA_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton_ATA.Checked)
                return;
            volumeComboBox.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            volumeComboBox.Items.Clear();
            siBufDict.Clear();
            driveMapping.Clear();
            raidDict.Clear();
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                if (wmi_HD["Size"] != null)
                {
                    if (wmi_HD["InterfaceType"].ToString() != "USB" && (!isWindows10 || !wmi_HD["Caption"].ToString().Contains("SCSI")) && !wmi_HD["PNPDeviceID"].ToString().ToUpper().Contains("NVME"))
                    {
                        if (!wmi_HD["Caption"].ToString().ToUpper().Contains("RAID"))
                        {
                            string info = wmi_HD["Name"].ToString() + " " + wmi_HD["Caption"].ToString();
                            volumeComboBox.Items.Add(info);
                            driveMapping.Add(info, wmi_HD["PNPDeviceID"].ToString());
                        }
                    }
                }
            }
            isUSB = false;
            isNVMe = false;
            

            List<string> sataSNList = new List<string>();    // pre make SATA serial number list to filter duplicate item in SATA RAID list
            byte[] idBuf = new byte[512];
            byte[] smartBuf = new byte[512];

            foreach (var fullPath in volumeComboBox.Items)
            {
                getDriveIDandSmartTable(fullPath.ToString(), ref idBuf, ref smartBuf);
                SIbuffer si = new SIbuffer();
                si.id = idBuf;
                si.smart = smartBuf;
                siBufDict.Add(fullPath.ToString(), si);
                sataSNList.Add(getSN(idBuf));
            }


            //Get SATA RAID devices info
            RAIDClass raid = new RAIDClass();
            List<RAIDClass.RaidInfo> sataRaidInfoList = new List<RAIDClass.RaidInfo>();

            sataRaidInfoList = raid.GetRaidDeviceList_SATA();
            int cnt = 1;
            foreach (var item in sataRaidInfoList)
            {
                bool duplicate = false;
                foreach (var sataSN in sataSNList)
                {
                    if (sataSN == getSN(item.id))
                    {
                        duplicate = true;
                    }
                }
                if (!duplicate)
                {
                    string fullPath = "RAID_Device[" + cnt + "] " + UtilityClass.getModelName(item.id, false);
                    volumeComboBox.Items.Add(fullPath);
                    raidDict.Add(fullPath, item);
                    cnt++;
                }
            }
        }
        #endregion

        #region Change interface to USB
        private void radioButton_USB_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton_USB.Checked)
                return;
            volumeComboBox.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            volumeComboBox.Items.Clear();
            siBufDict.Clear();
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                if (wmi_HD["Size"] != null)
                {
                    if (wmi_HD["InterfaceType"].ToString() == "USB" || (isWindows10 && wmi_HD["Caption"].ToString().Contains("SCSI")) && !wmi_HD["PNPDeviceID"].ToString().ToUpper().Contains("NVME"))
                    {
                        if (!wmi_HD["Caption"].ToString().ToUpper().Contains("RAID"))
                        {
                            string info = wmi_HD["Name"].ToString() + " " + wmi_HD["Caption"].ToString();
                            volumeComboBox.Items.Add(info);
                        }
                    }
                }
            }
            isUSB = true;
            isNVMe = false;
            
            byte[] idBuf = new byte[512];
            byte[] smartBuf = new byte[512];

            foreach (var fullPath in volumeComboBox.Items)
            {
                getDriveIDandSmartTable(fullPath.ToString(), ref idBuf, ref smartBuf);
                SIbuffer si = new SIbuffer();
                si.id = idBuf;
                si.smart = smartBuf;
                siBufDict.Add(fullPath.ToString(), si);
            }
        }
        #endregion

        #region Change interface to NVMe
        private void radioButton_NVMe_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton_NVMe.Checked)
                return;
            volumeComboBox.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            volumeComboBox.Items.Clear();
            siBufDict.Clear();
            driveMapping.Clear();
            raidDict.Clear();
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                if (wmi_HD["Size"] != null)
                {
                    if (wmi_HD["InterfaceType"].ToString() != "USB" && /*(!isWindows10()||wmi_HD["InterfaceType"].ToString() != "SCSI") &&*/ wmi_HD["PNPDeviceID"].ToString().ToUpper().Contains("NVME"))
                    {
                        if (!wmi_HD["Caption"].ToString().ToUpper().Contains("RAID"))
                        {
                            string info = wmi_HD["Name"].ToString() + " " + wmi_HD["Caption"].ToString();
                            volumeComboBox.Items.Add(info);
                        }
                    }
                }
            }
            isUSB = false;
            isNVMe = true;

      
            List<string> nvmeSNList = new List<string>();    // pre make NVMe serial number list to filter duplicate item in NVMe RAID list
            byte[] idBuf = new byte[4096];
            byte[] smartBuf = new byte[512];

            foreach (var fullPath in volumeComboBox.Items)
            {
                getDriveIDandSmartTable(fullPath.ToString(), ref idBuf, ref smartBuf);
                SIbuffer si = new SIbuffer();
                si.id = idBuf;
                si.smart = smartBuf;
                siBufDict.Add(fullPath.ToString(), si);
                nvmeSNList.Add(getSN_NVMe(idBuf));
            }
            

            //Get NVMe RAID devices info
            RAIDClass raid = new RAIDClass();
            List<RAIDClass.RaidInfo> nvmeRaidInfoList = new List<RAIDClass.RaidInfo>();

            nvmeRaidInfoList = raid.GetRaidDeviceList_NVMe();
            int cnt = 1;
            foreach (var item in nvmeRaidInfoList)
            {
                if (item.nvmeDiskSize != 0)
                {
                    bool duplicate = false;
                    foreach (var nvmeSN in nvmeSNList)
                    {
                        if (nvmeSN == getSN_NVMe(item.id))
                        {
                            duplicate = true;
                        }
                    }
                    if (!duplicate)
                    {
                        string fullPath = "RAID_Device[" + cnt + "] " + UtilityClass.getModelName(item.id, true);
                        volumeComboBox.Items.Add(fullPath);
                        raidDict.Add(fullPath, item);
                        cnt++;
                    }
                }
            }

        }
        #endregion

        #region comboBox select
        private void volumeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            driveInfo = new byte[4096];
            SMARTInfo = new byte[512];

            int nvme_raidType = -1;
            int nvme_raidStatus = -1;

            if (volumeComboBox.Text != null && volumeComboBox.Text != "")
            {
                if (volumeComboBox.Text.ToUpper().Contains("RAID"))  //RAID device
                {
                    RAIDClass.RaidInfo raidInfo = new RAIDClass.RaidInfo();
                    raidDict.TryGetValue(volumeComboBox.Text, out raidInfo);
                    driveInfo = raidInfo.id;
                    SMARTInfo = raidInfo.smart;
                    if (isNVMe)
                    {
                        nvme_raidType = raidInfo.raidType;
                        nvme_raidStatus = raidInfo.raidStatus;
                    }
                }
                else
                {
                    SIbuffer si = new SIbuffer();
                    siBufDict.TryGetValue(volumeComboBox.Text, out si);
                    driveInfo = si.id;
                    SMARTInfo = si.smart;
                }

                List<string[]> rowCollection = new List<string[]>();
                if (!isNVMe)
                {
                    showDriveInfo(driveInfo);
                    parseSmartInformation(SMARTInfo, ref rowCollection);
                }
                else
                {
                    if (volumeComboBox.Text.ToUpper().Contains("RAID"))
                    {
                        showDriveInfo_NVMe(driveInfo, nvme_raidType, nvme_raidStatus);
                        parseSmartInformation_NVMe(SMARTInfo, ref rowCollection);
                    }
                    else
                    {
                        showDriveInfo_NVMe(driveInfo);
                        parseSmartInformation_NVMe(SMARTInfo, ref rowCollection);
                    }
                }

                showSmartInfo(rowCollection);
            }

            this.Cursor = Cursors.Default;
        }
        #endregion


        private bool checkWindows10()
        {
            bool result = false;
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            if (productName.StartsWith("Windows 10") || productName.StartsWith("Windows 8"))
                result = true;
            return result;
        }

        private void getDiskInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
            volumeComboBox.Items.Clear();
            siBufDict.Clear();
            isWindows10 = checkWindows10();

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                if (wmi_HD["Size"] != null)
                {
                    if (wmi_HD["InterfaceType"].ToString() != "USB" && (!isWindows10 || !wmi_HD["Caption"].ToString().Contains("SCSI")) && !wmi_HD["PNPDeviceID"].ToString().ToUpper().Contains("NVME"))
                    {
                        if (!wmi_HD["Caption"].ToString().ToUpper().Contains("RAID"))
                        {
                            string info = wmi_HD["Name"].ToString() + " " + wmi_HD["Caption"].ToString();
                            volumeComboBox.Items.Add(info);
                            driveMapping.Add(info, wmi_HD["PNPDeviceID"].ToString());
                        }
                    }
                }
            }


            List<string> sataSNList = new List<string>();    // pre make SATA serial number list to filter duplicate item in SATA RAID list
            byte[] idBuf = new byte[512];
            byte[] smartBuf = new byte[512];

            foreach (var fullPath in volumeComboBox.Items)
            {
                getDriveIDandSmartTable(fullPath.ToString(), ref idBuf, ref smartBuf);
                SIbuffer si = new SIbuffer();
                si.id = idBuf;
                si.smart = smartBuf;
                siBufDict.Add(fullPath.ToString(), si);
                sataSNList.Add(getSN(idBuf));
            }


            //Get SATA RAID devices info
            RAIDClass raid = new RAIDClass();
            List<RAIDClass.RaidInfo> sataRaidInfoList = new List<RAIDClass.RaidInfo>();

            sataRaidInfoList = raid.GetRaidDeviceList_SATA();
            int cnt = 1;
            foreach(var item in sataRaidInfoList) 
            {
                bool duplicate = false;
                foreach (var sataSN in sataSNList) {
                    if (sataSN == getSN(item.id))
                    {
                        duplicate = true;
                    }
                }
                if (!duplicate)
                {
                    string fullPath = "RAID_Device[" + cnt + "] " + UtilityClass.getModelName(item.id, false);
                    volumeComboBox.Items.Add(fullPath);
                    raidDict.Add(fullPath, item);
                    cnt++;
                }
            }


        }

        #region Get ID/Smart Data Array
        private void getDriveIDandSmartTable(string fullPath, ref byte[] deviceInfoArray, ref byte[] smartInfoArray)
        {
            string physicalPath = fullPath.Substring(4, fullPath.IndexOf(" ") - 4);
            Console.Write(physicalPath + "\n");
            IntPtr handle = CreateFile(
                            "\\\\.\\" + physicalPath,
                                        EFileAccess.GenericAll,
                            EFileShare.Write | EFileShare.Read,
                            IntPtr.Zero,
                            ECreationDisposition.OpenExisting,
                            EFileAttributes.NoBuffering | EFileAttributes.RandomAccess,
                            IntPtr.Zero);


            if (isUSB == false && isNVMe == false)   //ATA
            {
                string deviceID = "";
                modifyBufferSize(ref deviceInfoArray, 512);
                driveMapping.TryGetValue(fullPath, out deviceID);
                if (deviceID.IndexOf("SCSI", StringComparison.OrdinalIgnoreCase) == -1 || isWindows10)
                {
                    if (isWindows10)
                    {
                        ATADFPClass ATADFP = new ATADFPClass();
                        ATADFP.GetIDBuffer(handle, ref deviceInfoArray);
                        ATADFP.GetSMARTBuffer(handle, ref smartInfoArray);
                    }
                    else
                    {
                        ATAClass ATA = new ATAClass();
                        ATA.GetIDBuffer(handle, ref deviceInfoArray);
                        ATA.GetSMARTBuffer(handle, ref smartInfoArray);
                    }
                }
                else
                {
                    SCSIClass SCSI = new SCSIClass();
                    SCSI.getIDBuffer(handle, ref deviceInfoArray);
                    SCSI.getSMARTBuffer(handle, ref smartInfoArray);

                }
                CloseHandle(handle);
            }
            else if (isNVMe)   //NVMe device
            {
                string[] tmp = fullPath.Split(' ');
                string Phydisk = tmp[0];
                modifyBufferSize(ref deviceInfoArray, 4096);

                NVMeClass NVMe = new NVMeClass();
                NVMe.getIDBuffer(handle, ref deviceInfoArray);
                NVMe.getSMARTBuffer(handle, ref smartInfoArray);

                CloseHandle(handle);
            }
            else    //SCSI
            {
                modifyBufferSize(ref deviceInfoArray, 512);

                SCSIClass SCSI = new SCSIClass();
                SCSI.getIDBuffer(handle, ref deviceInfoArray);
                SCSI.getSMARTBuffer(handle, ref smartInfoArray);

                CloseHandle(handle);
            }
        }

        private void modifyBufferSize(ref byte[] buffer, int size)
        {
            if (buffer.Length != size)
            {
                Array.Resize(ref buffer, size);
            }
        }
        #endregion

        #region Parse Smart Information
        private void parseSmartInformation(byte[] SMARTinfo, ref List<string[]> rowCollection)
        {
            SMARTLog smartLog = new SMARTLog();
            separatSmartValue(SMARTinfo, ref smartLog);
            organizeSmartValue(smartLog, ref rowCollection);
        }


        private void parseSmartInformation_NVMe(byte[] SMARTinfo, ref List<string[]> rowCollection)
        {
            SMARTLog_NVMe smartLog_nvme = new SMARTLog_NVMe();
            separatSmartValue_NVMe(SMARTinfo, ref smartLog_nvme);
            organizeSmartValue_NVMe(smartLog_nvme, ref rowCollection);
        }

        private void separatSmartValue(byte[] buf_smart, ref SMARTLog smartLog)
        {
            smartLog.raw_read_err.content = getTableData(buf_smart, "01", false);
            smartLog.reallocate_sector_cnt.content = getTableData(buf_smart, "05", false);
            smartLog.pwr_on_hrs.content = getTableData(buf_smart, "09", false);
            smartLog.pwr_cycle_cnt.content = getTableData(buf_smart, "0C", false);
            smartLog.uncorrect_sector_cnt.content = getTableData(buf_smart, "A0", false);
            smartLog.vaild_spare.content = getTableData(buf_smart, "A1", false);
            smartLog.init_invalid_blks.content = getTableData(buf_smart, "A3", false);
            smartLog.total_tlc_erase_cnt.content = getTableData(buf_smart, "A4", false);
            smartLog.max_tlc_erase_cnt.content = getTableData(buf_smart, "A5", false);
            smartLog.min_tlc_erase_cnt.content = getTableData(buf_smart, "A6", false);
            smartLog.avg_tlc_erase_cnt.content = getTableData(buf_smart, "A7", false);
            smartLog.rsvd_a8.content = getTableData(buf_smart, "A8", false);
            smartLog.percent_life_remaning.content = getTableData(buf_smart, "A9", false);
            smartLog.rsvd_af.content = getTableData(buf_smart, "AF", false);
            smartLog.rsvd_b0.content = getTableData(buf_smart, "B0", false);
            smartLog.rsvd_b1.content = getTableData(buf_smart, "B1", false);
            smartLog.rsvd_b2.content = getTableData(buf_smart, "B2", false);
            smartLog.prog_fail_cnt.content = getTableData(buf_smart, "B5", false);
            smartLog.erase_fail_cnt.content = getTableData(buf_smart, "B6", false);
            smartLog.pwr_off_retract_cnt.content = getTableData(buf_smart, "C0", false);
            smartLog.temperature.content = getTableData(buf_smart, "C2", false);
            smartLog.cumulat_ecc_bit_correct_cnt.content = getTableData(buf_smart, "C3", false);
            smartLog.reallocation_event_cnt.content = getTableData(buf_smart, "C4", false);
            smartLog.current_pending_sector_cnt.content = getTableData(buf_smart, "C5", false);
            smartLog.off_line_scan_uncorrect_cnt.content = getTableData(buf_smart, "C6", false);
            smartLog.ultra_dma_crc_err_rate.content = getTableData(buf_smart, "C7", false);
            smartLog.avail_rsv_space.content = getTableData(buf_smart, "E8", false);
            smartLog.total_lba_write.content = getTableData(buf_smart, "F1", false);
            smartLog.total_lba_read.content = getTableData(buf_smart, "F2", false);
            smartLog.cumulat_prog_nand_pg.content = getTableData(buf_smart, "F5", false);



            smartLog.raw_read_err.raw = getTableData(buf_smart, "01", true);
            smartLog.reallocate_sector_cnt.raw = getTableData(buf_smart, "05", true);
            smartLog.pwr_on_hrs.raw = getTableData(buf_smart, "09", true);
            smartLog.pwr_cycle_cnt.raw = getTableData(buf_smart, "0C", true);
            smartLog.uncorrect_sector_cnt.raw = getTableData(buf_smart, "A0", true);
            smartLog.vaild_spare.raw = getTableData(buf_smart, "A1", true);
            smartLog.init_invalid_blks.raw = getTableData(buf_smart, "A3", true);
            smartLog.total_tlc_erase_cnt.raw = getTableData(buf_smart, "A4", true);
            smartLog.max_tlc_erase_cnt.raw = getTableData(buf_smart, "A5", true);
            smartLog.min_tlc_erase_cnt.raw = getTableData(buf_smart, "A6", true);
            smartLog.avg_tlc_erase_cnt.raw = getTableData(buf_smart, "A7", true);
            smartLog.rsvd_a8.raw = getTableData(buf_smart, "A8", true);
            smartLog.percent_life_remaning.raw = getTableData(buf_smart, "A9", true);
            smartLog.rsvd_af.raw = getTableData(buf_smart, "AF", true);
            smartLog.rsvd_b0.raw = getTableData(buf_smart, "B0", true);
            smartLog.rsvd_b1.raw = getTableData(buf_smart, "B1", true);
            smartLog.rsvd_b2.raw = getTableData(buf_smart, "B2", true);
            smartLog.prog_fail_cnt.raw = getTableData(buf_smart, "B5", true);
            smartLog.erase_fail_cnt.raw = getTableData(buf_smart, "B6", true);
            smartLog.pwr_off_retract_cnt.raw = getTableData(buf_smart, "C0", true);
            smartLog.temperature.raw = getTableData(buf_smart, "C2", true);
            smartLog.cumulat_ecc_bit_correct_cnt.raw = getTableData(buf_smart, "C3", true);
            smartLog.reallocation_event_cnt.raw = getTableData(buf_smart, "C4", true);
            smartLog.current_pending_sector_cnt.raw = getTableData(buf_smart, "C5", true);
            smartLog.off_line_scan_uncorrect_cnt.raw = getTableData(buf_smart, "C6", true);
            smartLog.ultra_dma_crc_err_rate.raw = getTableData(buf_smart, "C7", true);
            smartLog.avail_rsv_space.raw = getTableData(buf_smart, "E8", true);
            smartLog.total_lba_write.raw = getTableData(buf_smart, "F1", true);
            smartLog.total_lba_read.raw = getTableData(buf_smart, "F2", true);
            smartLog.cumulat_prog_nand_pg.raw = getTableData(buf_smart, "F5", true);
        }

        private void separatSmartValue_NVMe(byte[] buf_smart, ref SMARTLog_NVMe smartLog)
        {
            smartLog.critical_warning.content = (buf_smart[0]).ToString();
            smartLog.temperature.content = ((buf_smart[2] << 8) + buf_smart[1] - 273).ToString();
            smartLog.avail_spare.content = (buf_smart[3]).ToString();
            smartLog.spare_thresh.content = (buf_smart[4]).ToString();
            smartLog.percent_used.content = (buf_smart[5]).ToString();
            smartLog.endu_grp_crit_warn_sumry.content = Convert.ToString(buf_smart[6], 2).PadLeft(8, '0');
            smartLog.rsvd7.content = getTableData_NVMe(buf_smart, 7, 31, false);
            smartLog.data_units_read.content = getTableData_NVMe(buf_smart, 32, 47, false);
            smartLog.data_units_written.content = getTableData_NVMe(buf_smart, 48, 63, false);
            smartLog.host_reads.content = getTableData_NVMe(buf_smart, 64, 79, false);
            smartLog.host_writes.content = getTableData_NVMe(buf_smart, 80, 95, false);
            smartLog.ctrl_busy_time.content = getTableData_NVMe(buf_smart, 96, 111, false);
            smartLog.power_cycles.content = getTableData_NVMe(buf_smart, 112, 127, false);
            smartLog.power_on_hours.content = getTableData_NVMe(buf_smart, 128, 143, false);
            smartLog.unsafe_shutdowns.content = getTableData_NVMe(buf_smart, 144, 159, false);
            smartLog.media_errors.content = getTableData_NVMe(buf_smart, 160, 175, false);
            smartLog.num_err_log_entries.content = getTableData_NVMe(buf_smart, 176, 191, false);
            smartLog.warning_temp_time.content = getTableData_NVMe(buf_smart, 192, 195, false);
            smartLog.critical_comp_time.content = getTableData_NVMe(buf_smart, 196, 199, false);



            smartLog.critical_warning.raw = (buf_smart[0]).ToString("X8");
            smartLog.temperature.raw = ((buf_smart[2] << 8) + buf_smart[1]).ToString("X8");
            smartLog.avail_spare.raw = (buf_smart[3]).ToString("X8");
            smartLog.spare_thresh.raw = (buf_smart[4]).ToString("X8");
            smartLog.percent_used.raw = (buf_smart[5]).ToString("X8");
            smartLog.endu_grp_crit_warn_sumry.raw = (buf_smart[6]).ToString("X8");
            smartLog.rsvd7.raw = getTableData_NVMe(buf_smart, 7, 31, true);
            smartLog.data_units_read.raw = getTableData_NVMe(buf_smart, 32, 47, true);
            smartLog.data_units_written.raw = getTableData_NVMe(buf_smart, 48, 63, true);
            smartLog.host_reads.raw = getTableData_NVMe(buf_smart, 64, 79, true);
            smartLog.host_writes.raw = getTableData_NVMe(buf_smart, 80, 95, true);
            smartLog.ctrl_busy_time.raw = getTableData_NVMe(buf_smart, 96, 111, true);
            smartLog.power_cycles.raw = getTableData_NVMe(buf_smart, 112, 127, true);
            smartLog.power_on_hours.raw = getTableData_NVMe(buf_smart, 128, 143, true);
            smartLog.unsafe_shutdowns.raw = getTableData_NVMe(buf_smart, 144, 159, true);
            smartLog.media_errors.raw = getTableData_NVMe(buf_smart, 160, 175, true);
            smartLog.num_err_log_entries.raw = getTableData_NVMe(buf_smart, 176, 191, true);
            smartLog.warning_temp_time.raw = getTableData_NVMe(buf_smart, 192, 195, true);
            smartLog.critical_comp_time.raw = getTableData_NVMe(buf_smart, 196, 199, true);
        }

        private string getTableData(byte[] smartTableArray, string smartID, bool getRaw)
        {
            string res = "";
            for (int i = 2; i < smartTableArray.Length; i += 12)
            {
                if (Convert.ToInt32(smartID, 16) == smartTableArray[i])
                {
                    if (getRaw)
                        res = ((smartTableArray[i + 8] << 24) + (smartTableArray[i + 7] << 16) + (smartTableArray[i + 6] << 8) + (smartTableArray[i + 5])).ToString("X8");
                    else
                        res = ((smartTableArray[i + 8] << 24) + (smartTableArray[i + 7] << 16) + (smartTableArray[i + 6] << 8) + (smartTableArray[i + 5])).ToString();
                    break;
                }
            }
            return res;
        }

        private string getTableData_NVMe(byte[] smartTableArray, int begin, int finish, bool getRaw)
        {
            ulong tmp = 0;
            for (int i = finish; i >= begin; i--)
            {
                tmp = ((tmp << 8) + smartTableArray[i]);
            }
            if (getRaw)
                return tmp.ToString("X8");
            else
                return tmp.ToString();
        }


        private void organizeSmartValue(SMARTLog smartLog, ref List<string[]> rowCollection)
        {
            rowCollection.Add(new string[4] { "01", "Raw Read Error Rate", smartLog.raw_read_err.content, smartLog.raw_read_err.raw });
            rowCollection.Add(new string[4] { "05", "Reallocated Sectors Count", smartLog.reallocate_sector_cnt.content, smartLog.reallocate_sector_cnt.raw });
            rowCollection.Add(new string[4] { "09", "Power-On Hours", smartLog.pwr_on_hrs.content, smartLog.pwr_on_hrs.raw });
            rowCollection.Add(new string[4] { "0C", "Power Cycle Count", smartLog.pwr_cycle_cnt.content, smartLog.pwr_cycle_cnt.raw });
            rowCollection.Add(new string[4] { "A0", "Uncorrectable Sector Count", smartLog.uncorrect_sector_cnt.content, smartLog.uncorrect_sector_cnt.raw });
            rowCollection.Add(new string[4] { "A1", "Valid Spare Blocks", smartLog.vaild_spare.content, smartLog.vaild_spare.raw });
            rowCollection.Add(new string[4] { "A3", "Initial Invalid Blocks", smartLog.init_invalid_blks.content, smartLog.init_invalid_blks.raw });
            rowCollection.Add(new string[4] { "A4", "Total TLC Erase Count", smartLog.total_tlc_erase_cnt.content, smartLog.total_tlc_erase_cnt.raw });
            rowCollection.Add(new string[4] { "A5", "Maximum TLC Erase Count", smartLog.max_tlc_erase_cnt.content, smartLog.max_tlc_erase_cnt.raw });
            rowCollection.Add(new string[4] { "A6", "Minimum TLC Erase Count", smartLog.min_tlc_erase_cnt.content, smartLog.min_tlc_erase_cnt.raw });
            rowCollection.Add(new string[4] { "A7", "Average TLC Erase Count", smartLog.avg_tlc_erase_cnt.content, smartLog.avg_tlc_erase_cnt.raw });
            rowCollection.Add(new string[4] { "A8", "Vendor Specific", smartLog.rsvd_a8.content, smartLog.rsvd_a8.raw });
            rowCollection.Add(new string[4] { "A9", "Percentage Lifetime Remaning", smartLog.percent_life_remaning.content, smartLog.percent_life_remaning.raw });
            rowCollection.Add(new string[4] { "AF", "Vendor Specific", smartLog.rsvd_af.content, smartLog.rsvd_af.raw });
            rowCollection.Add(new string[4] { "B0", "Vendor Specific", smartLog.rsvd_b0.content, smartLog.rsvd_b0.raw });
            rowCollection.Add(new string[4] { "B1", "Vendor Specific", smartLog.rsvd_b1.content, smartLog.rsvd_b1.raw });
            rowCollection.Add(new string[4] { "B2", "Vendor Specific", smartLog.rsvd_b2.content, smartLog.rsvd_b2.raw });
            rowCollection.Add(new string[4] { "B5", "Program Fail Count", smartLog.prog_fail_cnt.content, smartLog.prog_fail_cnt.raw });
            rowCollection.Add(new string[4] { "B6", "Erase Fail Count", smartLog.erase_fail_cnt.content, smartLog.erase_fail_cnt.raw });
            rowCollection.Add(new string[4] { "C0", "Power-off Retract Count", smartLog.pwr_off_retract_cnt.content, smartLog.pwr_off_retract_cnt.raw });
            rowCollection.Add(new string[4] { "C2", "Temperature", smartLog.temperature.content, smartLog.temperature.raw });
            rowCollection.Add(new string[4] { "C3", "Cumulative ECC Bit Correction Count", smartLog.cumulat_ecc_bit_correct_cnt.content, smartLog.cumulat_ecc_bit_correct_cnt.raw });
            rowCollection.Add(new string[4] { "C4", "Reallocation Event Count", smartLog.reallocation_event_cnt.content, smartLog.reallocation_event_cnt.raw });
            rowCollection.Add(new string[4] { "C5", "Current Pending Sector Count", smartLog.current_pending_sector_cnt.content, smartLog.current_pending_sector_cnt.raw });
            rowCollection.Add(new string[4] { "C6", "Smart Off-line Scan Uncorrectable Error Count", smartLog.off_line_scan_uncorrect_cnt.content, smartLog.off_line_scan_uncorrect_cnt.raw });
            rowCollection.Add(new string[4] { "C7", "Ultra DMA CRC Error Rate", smartLog.ultra_dma_crc_err_rate.content, smartLog.ultra_dma_crc_err_rate.raw });
            rowCollection.Add(new string[4] { "E8", "Available Reserved Space", smartLog.avail_rsv_space.content, smartLog.avail_rsv_space.raw });
            rowCollection.Add(new string[4] { "F1", "Total LBA Write", smartLog.total_lba_write.content, smartLog.total_lba_write.raw });
            rowCollection.Add(new string[4] { "F2", "Total LBA Read", smartLog.total_lba_read.content, smartLog.total_lba_read.raw });
            rowCollection.Add(new string[4] { "F5", "Cumulative Program NAND Page", smartLog.cumulat_prog_nand_pg.content, smartLog.cumulat_prog_nand_pg.raw });
        }

        private void organizeSmartValue_NVMe(SMARTLog_NVMe smartLog, ref List<string[]> rowCollection)
        {
            rowCollection.Add(new string[4] { "0", "Critical Warning", smartLog.critical_warning.content, smartLog.critical_warning.raw });
            rowCollection.Add(new string[4] { "1-2", "Composite Temperature", smartLog.temperature.content, smartLog.temperature.raw });
            rowCollection.Add(new string[4] { "3", "Available Spare", smartLog.avail_spare.content, smartLog.avail_spare.raw });
            rowCollection.Add(new string[4] { "4", "Available Spare Threshold", smartLog.spare_thresh.content, smartLog.spare_thresh.raw });
            rowCollection.Add(new string[4] { "5", "Percentage Use", smartLog.percent_used.content, smartLog.percent_used.raw });
            rowCollection.Add(new string[4] { "6", "Endurance Group Critical Warning Summary", smartLog.endu_grp_crit_warn_sumry.content, smartLog.endu_grp_crit_warn_sumry.raw });
            rowCollection.Add(new string[4] { "7-31", "Reserved", smartLog.rsvd7.content, smartLog.rsvd7.raw });
            rowCollection.Add(new string[4] { "32-47", "Data Units Read", smartLog.data_units_read.content, smartLog.data_units_read.raw });
            rowCollection.Add(new string[4] { "48-63", "Data Units Written", smartLog.data_units_written.content, smartLog.data_units_written.raw });
            rowCollection.Add(new string[4] { "64-79", "Host Read Commands", smartLog.host_reads.content, smartLog.host_reads.raw });
            rowCollection.Add(new string[4] { "80-95", "Host Write Commands", smartLog.host_writes.content, smartLog.host_writes.raw });
            rowCollection.Add(new string[4] { "96-111", "Controller Busy Time", smartLog.ctrl_busy_time.content, smartLog.ctrl_busy_time.raw });
            rowCollection.Add(new string[4] { "112-127", "Power Cycles", smartLog.power_cycles.content, smartLog.power_cycles.raw });
            rowCollection.Add(new string[4] { "128-143", "Power On Hours", smartLog.power_on_hours.content, smartLog.power_on_hours.raw });
            rowCollection.Add(new string[4] { "144-159", "Unsafe Shutdowns", smartLog.unsafe_shutdowns.content, smartLog.unsafe_shutdowns.raw });
            rowCollection.Add(new string[4] { "160-175", "Media and Data integrity Errors", smartLog.media_errors.content, smartLog.media_errors.raw });
            rowCollection.Add(new string[4] { "176-191", "Number of Error information Log Entries", smartLog.num_err_log_entries.content, smartLog.num_err_log_entries.raw });
            rowCollection.Add(new string[4] { "192-195", "Warning Composite Temperature Time", smartLog.warning_temp_time.content, smartLog.warning_temp_time.raw });
            rowCollection.Add(new string[4] { "196-199", "Critical Composite Temperature Time", smartLog.critical_comp_time.content, smartLog.critical_comp_time.raw });
        }
        #endregion


        private string getSN(byte[] driveinfo)
        {
            string sn = "";  //Word 10~19
            for (int i = 0; i < 10; i++)
            {
                sn += Encoding.Default.GetString(driveinfo, (10 * 2) + (i * 2 + 1), 1) + Encoding.Default.GetString(driveinfo, (10 * 2) + (i * 2), 1);
            }
            return sn.Trim();
        }
        private string getSN_NVMe(byte[] driveinfo)
        {
            string sn = "";
            ASCIIEncoding ascii = new ASCIIEncoding();
            for (int i = 4; i <= 23; i++)
            {
                if (!driveinfo[i].Equals(0))
                    sn += ascii.GetString(driveinfo, i, 1);
                else
                    break;
            }
            return sn.Trim();
        }

        #region Show ID Info
        private void showDriveInfo(byte[] driveinfo)
        {
            dataGridView1.Rows.Clear();
            string Serial_number = "";  //Word 10~19
            for (int i = 0; i < 10; i++)
            {
                Serial_number += Encoding.Default.GetString(driveinfo, (10 * 2) + (i * 2 + 1), 1) + Encoding.Default.GetString(driveinfo, (10 * 2) + (i * 2), 1);
            }
            string[] row1 = new string[] { "Serial Number", Serial_number.Trim() };
            dataGridView1.Rows.Add(row1);

            string Firmware_revision = "";  //Word 23~26
            for (int i = 0; i < 4; i++)
            {
                Firmware_revision += Encoding.Default.GetString(driveinfo, (23 * 2) + (i * 2 + 1), 1) + Encoding.Default.GetString(driveinfo, (23 * 2) + (i * 2), 1);
            }
            string[] row2 = new string[] { "Firmware Revision", Firmware_revision.Trim() };
            dataGridView1.Rows.Add(row2);

            string Model_number = "";  //Word 27~46
            for (int i = 0; i < 20; i++)
            {
                Model_number += Encoding.Default.GetString(driveinfo, (27 * 2) + (i * 2 + 1), 1) + Encoding.Default.GetString(driveinfo, (27 * 2) + (i * 2), 1);
            }
            string[] row3 = new string[] { "Model number", Model_number.Trim() };
            dataGridView1.Rows.Add(row3);

            string strNCQSup;   //Word 76
            if ((driveinfo[153] & 0x01) == 1)
                strNCQSup = "Supported";
            else
                strNCQSup = "Not Supported";
            string[] row4 = new string[] { "NCQ Support", strNCQSup.Trim() };
            dataGridView1.Rows.Add(row4);

            string strATAPIInfo = "N/A";    //Word 80
            if ((driveinfo[160] & 0x10) == 0x10)
            {
                strATAPIInfo = "Support ATA/ATAPI-4";
            }
            if ((driveinfo[160] & 0x20) == 0x20)
            {
                strATAPIInfo = "Support ATA/ATAPI-5";
            }
            if ((driveinfo[160] & 0x40) == 0x40)
            {
                strATAPIInfo = "Support ATA/ATAPI-6";
            }
            if ((driveinfo[160] & 0x80) == 0x80)
            {
                strATAPIInfo = "Support ATA/ATAPI-7";
            }
            if ((driveinfo[161] & 0x01) == 0x01)
            {
                strATAPIInfo = "Support ATA/ATAPI-8";
            }
            string[] row5 = new string[] { "Major Version Number", strATAPIInfo.Trim() };
            dataGridView1.Rows.Add(row5);

            // Command and feature sets supported
            string strSMARTSup, strSecuritySup;
            // W83.14=1 & W84.14=1 & W83.15=0 & W84.15=0
            if (((driveinfo[0xA7] & 0x40) == 0x40) &&
                 ((driveinfo[0xA9] & 0x40) == 0x40) &&
                 ((driveinfo[0xA7] & 0x80) == 0x00) &&
                 ((driveinfo[0xA9] & 0x80) == 0x00))
            {
                // Support SMART Feture Set or Not
                // W82.0
                if ((driveinfo[0xA4] & 0x01) == 0x01)
                    strSMARTSup = "Supported";
                else
                    strSMARTSup = "Not Supported";
                string[] row6 = new string[] { "SMART Support", strSMARTSup };
                dataGridView1.Rows.Add(row6);

                // Support Security Mode Feature or Not
                // W82.1
                if ((driveinfo[0xA4] & 0x02) == 0x02)
                    strSecuritySup = "Supported";
                else
                    strSecuritySup = "Not Supported";
                string[] row7 = new string[] { "Security Support", strSecuritySup };
                dataGridView1.Rows.Add(row7);
            }

            // W87.14=1, W87.15=0  Word[85]
            string strSMARTEnable, strSecurityEnable;
            if (((driveinfo[0xAF] & 0x40) == 0x40) && ((driveinfo[0xAF] & 0x80) == 0x00))
            {
                // Enable SMART Feature Set or Not
                // W85.0
                if ((driveinfo[0xAA] & 0x01) == 0x01)
                    strSMARTEnable = "Enabled";
                else
                    strSMARTEnable = "Disabled";
                string[] row8 = new string[] { "SMART Enable", strSMARTEnable };
                dataGridView1.Rows.Add(row8);

                // Enable  Security Mode Feature Set or Not
                // W85.1
                if ((driveinfo[0xAA] & 0x02) == 0x02)
                    strSecurityEnable = "Enabled";
                else
                    strSecurityEnable = "Disabled";
                string[] row9 = new string[] { "Security Enable", strSecurityEnable };
                dataGridView1.Rows.Add(row9);
            }

            // Command and feature sets supported or enable (Depend on the host enabling)  Word[83]
            // W83.3
            string strAPMSup = "Not Supported";
            if ((driveinfo[0xA6] & 0x08) == 0x08)
                strAPMSup = "Supported";
            string[] row10 = new string[] { "APM Support", strAPMSup };
            dataGridView1.Rows.Add(row10);

            // Command and feature sets supported or enable (Depend on the host enabling)  Word[86]
            // W86.3
            string strAPMEnable = "Disabled";
            if ((driveinfo[0xAC] & 0x08) == 0x08)
                strAPMEnable = "Enabled";
            string[] row11 = new string[] { "APM Status", strAPMEnable };
            dataGridView1.Rows.Add(row11);

            // Suppot Trim or not
            // Word 169
            string strTrimSup = "Not Supported";
            if ((driveinfo[0x152] & 0x01) == 0x01)
                strTrimSup = "Supported";
            string[] row12 = new string[] { "Trim Command Support", strTrimSup };
            dataGridView1.Rows.Add(row12);

            string strCurrentTranMode = ""; //Word 88
            if ((driveinfo[0xB1] & 0x40) == 0x40)   // W88.14
            { strCurrentTranMode = "Ultra DMA mode 6"; }
            else if ((driveinfo[0xB1] & 0x20) == 0x20)   // W88.13
            { strCurrentTranMode = "Ultra DMA mode 5"; }
            else if ((driveinfo[0xB1] & 0x10) == 0x10)   // W88.12
            { strCurrentTranMode = "Ultra DMA mode 4"; }
            else if ((driveinfo[0xB1] & 0x08) == 0x08)   // W88.11
            { strCurrentTranMode = "Ultra DMA mode 3"; }
            else if ((driveinfo[0xB1] & 0x04) == 0x04)   // W88.10
            { strCurrentTranMode = "Ultra DMA mode 2"; }
            else if ((driveinfo[0xB1] & 0x02) == 0x02)   // W88.9
            { strCurrentTranMode = "Ultra DMA mode 1"; }
            else if ((driveinfo[0xB1] & 0x01) == 0x01)   // W88.8
            { strCurrentTranMode = "Ultra DMA mode 0"; }

            if ((driveinfo[0xB0] & 0x40) == 0x40)   // W88.6
            { strCurrentTranMode = "Ultra DMA mode 6 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x20) == 0x20)   // W88.5
            { strCurrentTranMode = "Ultra DMA mode 5 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x10) == 0x10)   // W88.4
            { strCurrentTranMode = "Ultra DMA mode 4 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x08) == 0x08)   // W88.3
            { strCurrentTranMode = "Ultra DMA mode 3 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x04) == 0x04)   // W88.2
            { strCurrentTranMode = "Ultra DMA mode 2 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x02) == 0x02)   // W88.1
            { strCurrentTranMode = "Ultra DMA mode 1 and below are supported"; }
            else if ((driveinfo[0xB0] & 0x01) == 0x01)   // W88.0
            { strCurrentTranMode = "Ultra DMA mode 0 and below are supported"; }
            string[] row13 = new string[] { "Transfer Mode", strCurrentTranMode };
            dataGridView1.Rows.Add(row13);
        }


        private void showDriveInfo_NVMe(byte[] driveinfo, int raidType = -1, int raidStatus = -1)
        {
            dataGridView1.Rows.Clear();

            if (raidType != -1 || raidStatus != -1)
            {
                string nvmeRaidMode = "";
                if (raidStatus == 0)
                {
                    if (raidType == 1)
                    {
                        nvmeRaidMode = "RAID 0";
                    }
                    else if (raidType == 2)
                    {
                        nvmeRaidMode = "RAID 1";
                    }
                    else
                    {
                        nvmeRaidMode = "RAID";
                    }
                }
                else if (raidStatus == 1)
                {
                    nvmeRaidMode = "Degraded";
                }
                else if (raidStatus == 2)
                {
                    nvmeRaidMode = "Rebuild";
                }
                else
                {
                    nvmeRaidMode = "Fail";
                }

                dataGridView1.Rows.Add(new string[] { "RAID Mode", nvmeRaidMode });
            }

            dataGridView1.Rows.Add(new string[] { "PCI Vendor ID", getTableData_NVMe(driveinfo, 0, 1, false) });
            dataGridView1.Rows.Add(new string[] { "PCI Subsystem Vendor ID", getTableData_NVMe(driveinfo, 2, 3, false) });

            ASCIIEncoding ascii = new ASCIIEncoding();
            ///SN
            string _sn = "";
            for (int i = 4; i <= 23; i++)
            {
                if (!driveinfo[i].Equals(0))
                    _sn += ascii.GetString(driveinfo, i, 1);
                else
                    break;
            }
            dataGridView1.Rows.Add(new string[] { "Serial Number", _sn});


            /// Model Number
            string _model = "";
            for (int i = 24; i <= 63; i++)
            {
                if (!driveinfo[i].Equals(0))
                    _model += ascii.GetString(driveinfo, i, 1);
                else
                    break;
            }
            dataGridView1.Rows.Add(new string[] { "Model Number", _model });


            /// FW Revision
            string _fw = "";
            for (int i = 64; i <= 71; i++)
            {
                if (!driveinfo[i].Equals(0))
                    _fw += ascii.GetString(driveinfo, i, 1);
                else
                    break;
            }
            dataGridView1.Rows.Add(new string[] { "Firmware Revision", _fw});


            dataGridView1.Rows.Add(new string[] { "Recommended Arbitration Burst(RAB)", driveinfo[72].ToString() });
            dataGridView1.Rows.Add(new string[] { "IEEE OUI Identifier", getTableData_NVMe(driveinfo, 73, 75, false) });
            dataGridView1.Rows.Add(new string[] { "Controller Multi-Path I/O and Namespace Sharing Capabilities", driveinfo[76].ToString() });
            dataGridView1.Rows.Add(new string[] { "Max Data Transfer Size", driveinfo[77].ToString() });
            dataGridView1.Rows.Add(new string[] { "Controller ID", getTableData_NVMe(driveinfo, 78, 79, false) });
            dataGridView1.Rows.Add(new string[] { "Version", getTableData_NVMe(driveinfo, 80, 83, false) });
            dataGridView1.Rows.Add(new string[] { "RTD3 Resume Latency", getTableData_NVMe(driveinfo, 84, 87, false) });
            dataGridView1.Rows.Add(new string[] { "RDT3 Entry Latency", getTableData_NVMe(driveinfo, 88, 91, false) });
            dataGridView1.Rows.Add(new string[] { "Optional Asynchronous Events Supported", getTableData_NVMe(driveinfo, 91, 95, false) });
            dataGridView1.Rows.Add(new string[] { "Controller Attributes", getTableData_NVMe(driveinfo, 96, 99, false) });
            dataGridView1.Rows.Add(new string[] { "Reserved", getTableData_NVMe(driveinfo, 100, 239, false) });
        }
        #endregion

        #region Show Smart Info
        private void showSmartInfo(List<string[]> rowCollection)
        {
            dataGridView2.Rows.Clear();

            foreach (string[] item in rowCollection)
            {
                dataGridView2.Rows.Add(item);
            }
        }
        #endregion
    }

}
