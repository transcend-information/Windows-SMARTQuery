# Windows-SMARTQuery
Get S.M.A.R.T Information for the SATA/NVMe/USB drive.

-------------------------

This sample code demonstrates how to get the disk information of the solid state drives (SSD) in Windows, include identify information(model name, firmeare version and serial number, etc.) and S.M.A.R.T information(temperature, erase count and power-on hours, etc.). Supports SATA, USB and NVMe interface devices.

Note: This project needs to be executed as administrator 

Environment 
-------------------------

- .NET framework 4.0 or higher

Development Tool
-------------------------

- Microsoft Visual Studio 2017 above

Usage
-------------------------

1.Select the interface of the device which you want to get information
![SelectInterface](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/SelectInterface.png)

2.The combobox will show all the storage device which match selected interface
![ComboboxList](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/ComboboxList.png)

![ComboboxList](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/ComboboxList_RAID.png)
(Also support RAID0/RAID1 devices)

3.Select the storage device you want to get information
![SelectDevice](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/SelectDevice.png)

4.Use tab control to switch identify information and S.M.A.R.T information
![ID](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/ID.png)
![SMART](https://github.com/transcend-information/Windows-SMARTQuery/blob/master/SMART.png)
