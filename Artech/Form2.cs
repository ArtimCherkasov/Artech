using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Artech
{
    public partial class Form2 : Form
    {        
        public Form2()
        {
            InitializeComponent();
        }
       
        private void button1_Click_1(object sender, EventArgs e)
        {
            if(comboBox1.Text == "Output of HD or CCD information")
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    try
                    {
                        listBox1.Items.Add("Имя диска: " + drive.Name);
                        listBox1.Items.Add("Файловая система: " + drive.DriveFormat);
                        listBox1.Items.Add("Тип диска: " + drive.DriveType);
                        listBox1.Items.Add("Объем доступного свободного места (в байтах): " + drive.AvailableFreeSpace);
                        listBox1.Items.Add("Готов ли диск: " + drive.IsReady);
                        listBox1.Items.Add("Корневой каталог диска: " + drive.RootDirectory);
                        listBox1.Items.Add("Общий объем свободного места, доступного на диске (в байтах): " + drive.TotalFreeSpace);
                        listBox1.Items.Add("Размер диска (в байтах): " + drive.TotalSize);
                        listBox1.Items.Add("Метка тома диска: " + drive.VolumeLabel);
                    }
                    catch { }
                    listBox1.Items.Add("");
                }

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                foreach (ManagementObject hdd in searcher.Get())
                {
                    listBox1.Items.Add(hdd["SerialNumber"]);
                }
            }           
               
            if (comboBox1.Text == "USB information output")
            {
                string diskName = string.Empty;

                //предварительно очищаем список
                listBox1.Items.Clear();

                //Получение списка накопителей подключенных через интерфейс USB
                foreach (System.Management.ManagementObject drive in
                new System.Management.ManagementObjectSearcher(
                "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
                {
                    //Получаем букву накопителя
                    foreach (System.Management.ManagementObject partition in
                    new System.Management.ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
                    + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                    {
                        foreach (System.Management.ManagementObject disk in
                        new System.Management.ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                        + partition["DeviceID"]
                        + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                        {
                            //Получение буквы устройства
                            diskName = disk["Name"].ToString().Trim();
                            listBox1.Items.Add("Буква накопителя=" + diskName);
                        }
                    }

                    //Получение модели устройства
                    listBox1.Items.Add("Модель=" + drive["Model"]);

                    //Получение Ven устройства
                    listBox1.Items.Add("Ven=" +
                    parseVenFromDeviceID(drive["PNPDeviceID"].ToString().Trim()));

                    //Получение Prod устройства
                    listBox1.Items.Add("Prod=" +
                    parseProdFromDeviceID(drive["PNPDeviceID"].ToString().Trim()));

                    //Получение Rev устройства
                    listBox1.Items.Add("Rev=" +
                    parseRevFromDeviceID(drive["PNPDeviceID"].ToString().Trim()));

                    //Получение серийного номера устройства
                    string serial = drive["SerialNumber"].ToString().Trim();
                    //WMI не всегда может вернуть серийный номер накопителя через данный класс
                    if (serial.Length > 1)
                        listBox1.Items.Add("Серийный номер=" + serial);
                    else
                        //Если серийный не получен стандартным путем,
                        //Парсим информацию Plug and Play Device ID
                        listBox1.Items.Add("Серийный номер=" +
                        parseSerialFromDeviceID(drive["PNPDeviceID"].ToString().Trim()));

                    //Получение объема устройства в гигабайтах
                    decimal dSize = Math.Round((Convert.ToDecimal(
                    new System.Management.ManagementObject("Win32_LogicalDisk.DeviceID='"
                    + diskName + "'")["Size"]) / 1073741824), 2);
                    listBox1.Items.Add("Полный объем=" + dSize + " gb");

                    //Получение свободного места на устройстве в гигабайтах
                    decimal dFree = Math.Round((Convert.ToDecimal(
                    new System.Management.ManagementObject("Win32_LogicalDisk.DeviceID='"
                    + diskName + "'")["FreeSpace"]) / 1073741824), 2);
                    listBox1.Items.Add("Свободный объем=" + dFree + " gb");

                    //Получение использованного места на устройстве
                    decimal dUsed = dSize - dFree;
                    listBox1.Items.Add("Используемый объем=" + dUsed + " gb");

                    listBox1.Items.Add("");


                }
            }            
                
            if (comboBox1.Text == "Output general information about connected devices")
            {
                ManagementObjectCollection collection;
                using (var finddevice = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))
                using (var finddevice1 = new ManagementObjectSearcher(@"select * from win32_pnpentity where PNPClass = 'Keyboard'"))
                using (var finddevice2 = new ManagementObjectSearcher(@"select * from win32_pnpentity where PNPClass = 'Mouse'"))
                    collection = finddevice.Get();
                foreach (var device in collection)
                {
                    listBox1.Items.Add(device.GetPropertyValue("DeviceID"));
                    listBox1.Items.Add(device.GetPropertyValue("Description"));
                }
            }                                                        
        }
        // Относится к второму if о USB
        private string parseSerialFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('/');
            string[] serialArray;
            string serial;
            int arrayLen = splitDeviceId.Length - 1;

            serialArray = splitDeviceId[arrayLen].Split('&');
            serial = serialArray[0];

            return serial;
        }
        private string parseVenFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('/');
            string Ven;
            //Разбиваем строку на несколько частей.
            //Каждая чаcть отделяется по символу &
            string[] splitVen = splitDeviceId[0].Split('&');

            Ven = splitVen[1].Replace("VEN_", "");
            Ven = Ven.Replace("_", " ");
            return Ven;
        }
        private string parseProdFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('/');
            string Prod;
            //Разбиваем строку на несколько частей.
            //Каждая чаcть отделяется по символу &
            string[] splitProd = splitDeviceId[0].Split('&');

            Prod = splitProd[2].Replace("PROD_", ""); ;
            Prod = Prod.Replace("_", " ");
            return Prod;
        }
        private string parseRevFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('/');
            string Rev;
            //Разбиваем строку на несколько частей.
            //Каждая чаcть отделяется по символу &
            string[] splitRev = splitDeviceId[0].Split('&');

            Rev = splitRev[3].Replace("REV_", ""); ;
            Rev = Rev.Replace("_", " ");
            return Rev;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            listBox1.Items.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form3 frm3 = new Form3();
            frm3.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
