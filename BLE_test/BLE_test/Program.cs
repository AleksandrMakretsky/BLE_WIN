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
//using System.Threading;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;




namespace BLE_test
{
	class Program {

		////////////////////////////////////////////////////////////////////////////////////////////

		static void WriteDeviceList(BleDevice _btd) {

			Console.WriteLine("     Devices");
			lock ( _btd._locker )
			{
				for (int index = 0; index < _btd.devices.Count; index++)
				{
					DeviceInformation deviceInterface = _btd.devices[index];
					Console.WriteLine("              " + deviceInterface.Name);
				}
			}
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		static void Main(string[] args) {

			BleDevice btd = new BleDevice();
            Console.WriteLine("Hello BLE!");

			btd.change = false;
			btd.WatchDevices();

			//			while ( !btd.enumerationCompleted ) {
			//				Thread.Sleep(200);
			//			}

			ConsoleKeyInfo key;
			ConsoleKeyInfo cki = new ConsoleKeyInfo();

			bool go = true;
			while ( go ) {

//				if ( btd.change ) {
					WriteDeviceList(btd);
					btd.change = false;
//				}
				Thread.Sleep(500);
				if (Console.KeyAvailable == true)
				{
					cki = Console.ReadKey(true);
					if (cki.Key == ConsoleKey.X) go = false;
				}

				//			key = Console.ReadKey();
				//				if (key.Key == ConsoleKey.Escape) go = false;
			}


            Console.ReadKey();
        } // main
		////////////////////////////////////////////////////////////////////////////////////////////

	} // class Program
} // namespace BLE_test
