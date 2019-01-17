/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.IO;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
//using Windows.System;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
//using System.Reflection;
//using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using System.Windows.Threading;
//using SDKTemplate;
using System.IO.Ports;
*/
//using System.Windows.Forms;
//=============================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;


namespace BLE_test
{
	//	static List<DeviceInformation> _deviceList;// = new List<DeviceInformation>();



	class Program
    {
		bool change = false;

		//static
		List<DeviceInformation> _deviceList = new List<DeviceInformation>();


		static void Main(string[] args)
        {
			BleDevice btd = new BleDevice();
            Console.WriteLine("Hello World!");

            btd.WatchDevices();
			btd.change = false;

			// Keep the console window open in debug mode.
			Console.WriteLine("Press any key to exit.");
			bool go = true;
			while ( go ) {

				if ( btd.change ) {
					// wtite 
					btd.change = false;
				}
				Thread.Sleep(20);
			}


            Console.ReadKey();
        }
    }
}
