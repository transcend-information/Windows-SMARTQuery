#include <atlstr.h>
#include "SSD_Raid.h"
#include <winioctl.h>

 
HANDLE GetIoCtrlHandleCsmi(INT scsiPort)
{
	HANDLE hScsiDriveIOCTL = 0;
	CString driveName;

	driveName.Format(_T("\\\\.\\Scsi%d:"), scsiPort);
	hScsiDriveIOCTL = CreateFile(driveName, GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
	return hScsiDriveIOCTL;
}



bool GetScsiAddress(int Path, int *PortNumber, int *PathId)
{
	CString path;
	path.Format(L"\\\\.\\PhysicalDrive%d", Path);

	TCHAR* path_tchar = new TCHAR[100];

	path_tchar = path.GetBuffer(path.GetLength());


	HANDLE hDevice = CreateFile(path_tchar,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE,
		nullptr,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		nullptr);
	DWORD dwReturned;
	SCSI_ADDRESS ScsiAddr;

	memset(&ScsiAddr, 0, sizeof(ScsiAddr));
	BOOL bRet = DeviceIoControl(hDevice, IOCTL_SCSI_GET_ADDRESS,
		nullptr, 0, &ScsiAddr, sizeof(ScsiAddr), &dwReturned, NULL);

	CloseHandle(hDevice);

	*PortNumber = ScsiAddr.PortNumber;
	*PathId = ScsiAddr.PathId;

	return bRet == TRUE;
}

BOOL  DoBadBlockNVMe(INT scsiPort, INT scsiTargetId, PBYTE data)
{
	CString path;
	BYTE portNumber, pathId, targetId, lun;
	CString drive;
	portNumber = scsiPort;
	pathId = scsiTargetId;
	drive.Format(L"\\\\.\\Scsi%d:", portNumber);

	HANDLE hIoCtrl = CreateFile(drive,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr,
		OPEN_EXISTING, 0, nullptr);

	if (hIoCtrl != INVALID_HANDLE_VALUE)
	{
		INTEL_NVME_PASS_THROUGH NVMeData;
		memset(&NVMeData, 0, sizeof(NVMeData));

		NVMeData.SRB.HeaderLength = sizeof(SRB_IO_CONTROL);
		memcpy(NVMeData.SRB.Signature, "IntelNvm", 8);
		NVMeData.SRB.Timeout = 10;
		NVMeData.SRB.ControlCode = IOCTL_INTEL_NVME_PASS_THROUGH;
		NVMeData.SRB.Length = sizeof(INTEL_NVME_PASS_THROUGH) - sizeof(SRB_IO_CONTROL);

		NVMeData.Payload.Version = 1;
		NVMeData.Payload.PathId = pathId;
		NVMeData.Payload.Cmd.CDW0.Opcode = 0xc2; // ADMIN_GET_LOG_PAGE
		NVMeData.Payload.Cmd.u.GET_LOG_PAGE.CDW10.AsDWord = 0x4000;
		NVMeData.Payload.Cmd.u.GET_LOG_PAGE.CDW12 = 0x5a;
	 
		NVMeData.Payload.ParamBufLen = sizeof(INTEL_NVME_PAYLOAD) + sizeof(SRB_IO_CONTROL); //0xA4;
		NVMeData.Payload.ReturnBufferLen = 0x1000;
		NVMeData.Payload.CplEntry[0] = 0;

		DWORD dummy;
		if (DeviceIoControl(hIoCtrl, IOCTL_SCSI_MINIPORT,
			&NVMeData,
			sizeof(NVMeData),
			&NVMeData,
			sizeof(NVMeData),
			&dummy, nullptr))
		{
			memcpy_s(data, 512, NVMeData.DataBuffer, 512);
			CloseHandle(hIoCtrl);
			return TRUE;
		}
		CloseHandle(hIoCtrl);
	}
	return FALSE;
}
BOOL  DoSmartNVMe(INT scsiPort, INT scsiTargetId, PBYTE data)
{
	CString path;
	BYTE portNumber, pathId, targetId, lun;
	CString drive;
	portNumber = scsiPort;
	pathId = scsiTargetId;
	drive.Format(L"\\\\.\\Scsi%d:", portNumber);

	HANDLE hIoCtrl = CreateFile(drive,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr,
		OPEN_EXISTING, 0, nullptr);

	if (hIoCtrl != INVALID_HANDLE_VALUE)
	{
		INTEL_NVME_PASS_THROUGH NVMeData;
		memset(&NVMeData, 0, sizeof(NVMeData));

		NVMeData.SRB.HeaderLength = sizeof(SRB_IO_CONTROL);
		memcpy(NVMeData.SRB.Signature, "IntelNvm", 8);
		NVMeData.SRB.Timeout = 10;
		NVMeData.SRB.ControlCode = IOCTL_INTEL_NVME_PASS_THROUGH;
		NVMeData.SRB.Length = sizeof(INTEL_NVME_PASS_THROUGH) - sizeof(SRB_IO_CONTROL);

		NVMeData.Payload.Version = 1;
		NVMeData.Payload.PathId = pathId;
		NVMeData.Payload.Cmd.CDW0.Opcode = 0x02; // ADMIN_GET_LOG_PAGE
		NVMeData.Payload.Cmd.NSID = 0xFFFFFFFF; //  NVME_NAMESPACE_ALL;
		NVMeData.Payload.Cmd.u.GET_LOG_PAGE.CDW10.LID = 2; // = 0x7f0002;
		NVMeData.Payload.Cmd.u.GET_LOG_PAGE.CDW10.NUMD = 0x7F;
		NVMeData.Payload.ParamBufLen = sizeof(INTEL_NVME_PAYLOAD) + sizeof(SRB_IO_CONTROL); //0xA4;
		NVMeData.Payload.ReturnBufferLen = 0x1000;
		NVMeData.Payload.CplEntry[0] = 0;

		DWORD dummy;
		if (DeviceIoControl(hIoCtrl, IOCTL_SCSI_MINIPORT,
			&NVMeData,
			sizeof(NVMeData),
			&NVMeData,
			sizeof(NVMeData),
			&dummy, nullptr))
		{
			memcpy_s(data, 512, NVMeData.DataBuffer, 512);
			CloseHandle(hIoCtrl);
			return TRUE;
		}
		CloseHandle(hIoCtrl);
	}
	return FALSE;
}




BOOL  DoIdentifyDeviceNVMe(INT scsiPort, INT scsiTargetId, PBYTE data, int *diskSize)
{
	CString path;
	BYTE portNumber, pathId, targetId, lun;
	CString drive;
 
	portNumber = scsiPort;
	pathId = scsiTargetId;
 
	drive.Format(L"\\\\.\\Scsi%d:", portNumber);

	HANDLE hIoCtrl = CreateFile(drive,
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr,
		OPEN_EXISTING, 0, nullptr);

	if (hIoCtrl != INVALID_HANDLE_VALUE)
	{
		INTEL_NVME_PASS_THROUGH NVMeData;
		memset(&NVMeData, 0, sizeof(NVMeData));

		NVMeData.SRB.HeaderLength = sizeof(SRB_IO_CONTROL);
		memcpy(NVMeData.SRB.Signature, "IntelNvm", 8);
		NVMeData.SRB.Timeout = 10;
		NVMeData.SRB.ControlCode = IOCTL_INTEL_NVME_PASS_THROUGH;
		NVMeData.SRB.Length = sizeof(INTEL_NVME_PASS_THROUGH) - sizeof(SRB_IO_CONTROL);

		NVMeData.Payload.Version = 1;
		NVMeData.Payload.PathId = pathId;
		NVMeData.Payload.Cmd.CDW0.Opcode = 0x06; // ADMIN_IDENTIFY
		NVMeData.Payload.Cmd.NSID = 1;
		NVMeData.Payload.Cmd.u.IDENTIFY.CDW10.CNS = 0;
		NVMeData.Payload.ParamBufLen = sizeof(INTEL_NVME_PAYLOAD) + sizeof(SRB_IO_CONTROL); //0xA4;
		NVMeData.Payload.ReturnBufferLen = 0x1000;
		NVMeData.Payload.CplEntry[0] = 0;

		DWORD dummy;
		 
		if (DeviceIoControl(hIoCtrl, IOCTL_SCSI_MINIPORT,
			&NVMeData,
			sizeof(NVMeData),
			&NVMeData,
			sizeof(NVMeData),
			&dummy, nullptr))
		{
			ULONG64 totalLBA =
				(((ULONG64)NVMeData.DataBuffer[7] << 56)
					+ ((ULONG64)NVMeData.DataBuffer[6] << 48)
					+ ((ULONG64)NVMeData.DataBuffer[5] << 40)
					+ ((ULONG64)NVMeData.DataBuffer[4] << 32)
					+ ((ULONG64)NVMeData.DataBuffer[3] << 24)
					+ ((ULONG64)NVMeData.DataBuffer[2] << 16)
					+ ((ULONG64)NVMeData.DataBuffer[1] << 8)
					+ ((ULONG64)NVMeData.DataBuffer[0]));
			int sectorSize = 1 << NVMeData.DataBuffer[130];

			*diskSize = totalLBA;
		}
		NVMeData.Payload.Cmd.NSID = 0;
		NVMeData.Payload.Cmd.u.IDENTIFY.CDW10.CNS = 1;
		if (DeviceIoControl(hIoCtrl, IOCTL_SCSI_MINIPORT,
			&NVMeData,
			sizeof(NVMeData),
			&NVMeData,
			sizeof(NVMeData),
			&dummy, nullptr))
		{
			memcpy_s(data, sizeof(NVME_IDENTIFY_DEVICE), NVMeData.DataBuffer, sizeof(NVME_IDENTIFY_DEVICE));

			CloseHandle(hIoCtrl);
			return TRUE;
		}

		CloseHandle(hIoCtrl);
	}
	return FALSE;
}


BOOL  CsmiIoctl(HANDLE hHandle, UINT code, SRB_IO_CONTROL *csmiBuf, UINT csmiBufSize)
{
	// Determine signature
	const CHAR *sig;
	switch (code)
	{
	case CC_CSMI_SAS_GET_DRIVER_INFO:
		sig = CSMI_ALL_SIGNATURE;
		break;
	case CC_CSMI_SAS_GET_PHY_INFO:
	case CC_CSMI_SAS_STP_PASSTHRU:
		sig = CSMI_SAS_SIGNATURE;
		break;
	case CC_CSMI_SAS_GET_RAID_INFO:
	case CC_CSMI_SAS_GET_RAID_CONFIG:
		sig = CSMI_RAID_SIGNATURE;
		break;
	default:
		return FALSE;
	}

	// Set header
	csmiBuf->HeaderLength = sizeof(IOCTL_HEADER);
	strncpy_s((char *)csmiBuf->Signature, sizeof(csmiBuf->Signature), sig, sizeof(csmiBuf->Signature));
	csmiBuf->Timeout = CSMI_SAS_TIMEOUT;
	csmiBuf->ControlCode = code;
	csmiBuf->ReturnCode = 0;
	csmiBuf->Length = csmiBufSize - sizeof(IOCTL_HEADER);

	// Call function
	DWORD num_out = 0;
	if (!DeviceIoControl(hHandle, IOCTL_SCSI_MINIPORT,
		csmiBuf, csmiBufSize, csmiBuf, csmiBufSize, &num_out, (OVERLAPPED*)0))
	{
		long err = GetLastError();

		if (err == ERROR_INVALID_FUNCTION || err == ERROR_NOT_SUPPORTED
			|| err == ERROR_DEV_NOT_EXIST)
		{
			return FALSE;
		}
	}

	// Check result
	return TRUE;
}

BOOL SendAtaCommandCsmi(INT scsiPort, int bPhyIdentifier, int bPortIdentifier, byte bSASAddress[], BYTE main, BYTE sub, BYTE param, PBYTE data, DWORD dataSize)
{
	HANDLE hIoCtrl = GetIoCtrlHandleCsmi(scsiPort);
	if (hIoCtrl == INVALID_HANDLE_VALUE)
	{
		return	FALSE;
	}

	DWORD size = sizeof(CSMI_SAS_STP_PASSTHRU_BUFFER) + dataSize;
	CSMI_SAS_STP_PASSTHRU_BUFFER* buf = (CSMI_SAS_STP_PASSTHRU_BUFFER*)VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);

	buf->Parameters.bPhyIdentifier = bPhyIdentifier;
	buf->Parameters.bPortIdentifier = bPortIdentifier;
	memcpy(&(buf->Parameters.bDestinationSASAddress), bSASAddress, sizeof(bSASAddress));
	buf->Parameters.bConnectionRate = CSMI_SAS_LINK_RATE_NEGOTIATED;

	if (main == 0xEF) // AAM/APM
	{
		buf->Parameters.uFlags = CSMI_SAS_STP_UNSPECIFIED;
	}
	else
	{
		buf->Parameters.uFlags = CSMI_SAS_STP_PIO | CSMI_SAS_STP_READ;
	}
	buf->Parameters.uDataLength = dataSize;

	buf->Parameters.bCommandFIS[0] = 0x27; // Type: host-to-device FIS
	buf->Parameters.bCommandFIS[1] = 0x80; // Bit7: Update command register

	if (main == SMART_CMD)
	{
		buf->Parameters.bCommandFIS[2] = main;
		buf->Parameters.bCommandFIS[3] = sub;
		buf->Parameters.bCommandFIS[4] = 0;
		buf->Parameters.bCommandFIS[5] = SMART_CYL_LOW;
		buf->Parameters.bCommandFIS[6] = SMART_CYL_HI;
		buf->Parameters.bCommandFIS[7] = 0xA0; // target
		buf->Parameters.bCommandFIS[8] = 0;
		buf->Parameters.bCommandFIS[9] = 0;
		buf->Parameters.bCommandFIS[10] = 0;
		buf->Parameters.bCommandFIS[11] = 0;
		buf->Parameters.bCommandFIS[12] = param;
		buf->Parameters.bCommandFIS[13] = 0;
	}
	else
	{
		buf->Parameters.bCommandFIS[2] = main;
		buf->Parameters.bCommandFIS[3] = sub;
		buf->Parameters.bCommandFIS[4] = 0;
		buf->Parameters.bCommandFIS[5] = 0;
		buf->Parameters.bCommandFIS[6] = 0;
		buf->Parameters.bCommandFIS[7] = 0xA0; // target
		buf->Parameters.bCommandFIS[8] = 0;
		buf->Parameters.bCommandFIS[9] = 0;
		buf->Parameters.bCommandFIS[10] = 0;
		buf->Parameters.bCommandFIS[11] = 0;
		buf->Parameters.bCommandFIS[12] = param;
		buf->Parameters.bCommandFIS[13] = 0;
	}

	if (!CsmiIoctl(hIoCtrl, CC_CSMI_SAS_STP_PASSTHRU, &buf->IoctlHeader, size))
	{
		CloseHandle(hIoCtrl);
		VirtualFree(buf, 0, MEM_RELEASE);
		return FALSE;
	}

	if (main != 0xEF && buf->bDataBuffer && data != NULL)
	{
		memcpy_s(data, dataSize, buf->bDataBuffer, dataSize);
	}

	CloseHandle(hIoCtrl);
	VirtualFree(buf, 0, MEM_RELEASE);

	return	TRUE;
}

bool getPhyPort(INT scsiPort, CSMI_SAS_PHY_INFO *phyInfo)
{
	bool result = true;
	HANDLE hHandle = GetIoCtrlHandleCsmi(scsiPort);
	if (hHandle == INVALID_HANDLE_VALUE)
	{
		return FALSE;
	}


	CSMI_SAS_PHY_INFO_BUFFER phyInfoBuf = { 0 };
	if (!CsmiIoctl(hHandle, CC_CSMI_SAS_GET_PHY_INFO, &phyInfoBuf.IoctlHeader, sizeof(phyInfoBuf)))
	{
		CloseHandle(hHandle);
		printf("FAILED: CC_CSMI_SAS_GET_PHY_INFO");
		return FALSE;
	}
	CloseHandle(hHandle);
	memcpy(phyInfo, &(phyInfoBuf.Information), sizeof(phyInfoBuf.Information));
 
	return result;
}

bool getPhyPortNVME(INT scsiPort, byte phyPort[16],INT *Type,INT *Status)
{
	int cnt = 0;
	for (int i = 0; i < 16; i++)
	{
		int scsiPort = i;
		HANDLE hHandle = GetIoCtrlHandleCsmi(scsiPort);
		CSMI_SAS_RAID_INFO_BUFFER raidInfoBuf = { 0 };
		if (!CsmiIoctl(hHandle, CC_CSMI_SAS_GET_RAID_INFO, &raidInfoBuf.IoctlHeader, sizeof(raidInfoBuf)))
		{
			CloseHandle(hHandle);

			return FALSE;
		}
		DWORD size = sizeof(CSMI_SAS_RAID_CONFIG_BUFFER) + sizeof(CSMI_SAS_RAID_DRIVES) * raidInfoBuf.Information.uNumRaidSets * raidInfoBuf.Information.uMaxDrivesPerSet;
		PCSMI_SAS_RAID_CONFIG_BUFFER buf = (PCSMI_SAS_RAID_CONFIG_BUFFER)VirtualAlloc(NULL, size, MEM_COMMIT, PAGE_READWRITE);
		for (UINT i = 0; i < raidInfoBuf.Information.uNumRaidSets; i++)
		{
			buf->Configuration.uRaidSetIndex = i;
			if (!CsmiIoctl(hHandle, CC_CSMI_SAS_GET_RAID_CONFIG, &(buf->IoctlHeader), size))
			{
				CloseHandle(hHandle);

				VirtualFree(buf, 0, MEM_RELEASE);
				return FALSE;
			}
			else
			{
				for (UINT j = 0; j < raidInfoBuf.Information.uMaxDrivesPerSet; j++)
				{
					if (buf->Configuration.Drives[j].bModel[0] != '\0')
					{
						//	raidDrives.Add(buf->Configuration.Drives[j]);
						//raidDrives.Add(buf->Configuration.Drives[j].bSASAddress[2]);
						phyPort[cnt] = buf->Configuration.Drives[j].bSASAddress[2];
						*Type = buf->Configuration.bRaidType;
						*Status = buf->Configuration.bStatus;
						cnt++;
					}
				}
			}
		}


		CloseHandle(hHandle);
		
	}
	return TRUE;
}

bool getIdSmart(INT scsiPort, int  Phy, int  Identify, byte address[8], byte id[512], byte smart[512])
{
	bool result = false;
	HANDLE hHandle = GetIoCtrlHandleCsmi(scsiPort);
	if (hHandle == INVALID_HANDLE_VALUE)
	{
		return FALSE;
	}
	 
	result = SendAtaCommandCsmi(scsiPort, Phy, Identify, address, 0xEC, 0x00, 0x00, (PBYTE)id, 512);
	if (result)
	{

		result = SendAtaCommandCsmi(scsiPort, Phy, Identify, address, SMART_CMD, READ_ATTRIBUTES, 0x00, (PBYTE)smart, 512);
	}
 
	return result;

}

bool getNVMEIdSmart(INT scsiPort,  int  scsiTargetId,  byte id[4096], byte smart[512],int *diskSize)
{
	bool result = false;
	HANDLE hHandle = GetIoCtrlHandleCsmi(scsiPort);
	if (hHandle == INVALID_HANDLE_VALUE)
	{
 			return FALSE;
 	}
	result=DoIdentifyDeviceNVMe(scsiPort, scsiTargetId, (PBYTE)id, diskSize);
	if (result)
	{
		result=	DoSmartNVMe(scsiPort, scsiTargetId, (PBYTE)smart);

	}
	return result;
}
