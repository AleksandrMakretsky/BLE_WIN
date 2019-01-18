
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.IO;
using System.Drawing;
using System.Text;
//using System.Threading.Tasks;
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
using System.Windows.Forms;
//=============================
////////////////////////////////////////////////////////////////////////////////////////////
using System.Runtime.InteropServices; // find window


namespace BLETestForm
{

	public partial class Form1 : Form
	{



		private BluetoothLEDevice bluetoothLeDevice;
		private BleDevice btDevice;


		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern uint RegisterWindowMessage(string lpString);

		private uint _messageId = RegisterWindowMessage("BLEDeviceMessage");

		void WriteDeviceList(BleDevice _btDevice)
		{

			Console.WriteLine("     Devices");
			lock (_btDevice._locker)
			{
				DeviceList.Items.Clear();
				int count = 0;
				for (int index = 0; index < _btDevice.devices.Count; index++)
				{
					DeviceInformation deviceInterface = _btDevice.devices[index];
					Console.WriteLine("              " + deviceInterface.Name);

					DeviceList.Items.Add(deviceInterface.Name);
					count++;
				}
				int stop = 0;
			}
		}
		////////////////////////////////////////////////////////////////////////////////////////////

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == _messageId)
			{
				WriteDeviceList(btDevice);
			}
			base.WndProc(ref m);
		}
		////////////////////////////////////////////////////////////////////////////////////////////



		public Form1()
		{
			btDevice = null;
			bluetoothLeDevice = null;
			InitializeComponent();
			btDevice = new BleDevice();
			btDevice.StartDiscovery();
		}
////////////////////////////////////////////////////////////////////////////////////////////

		private async void connect(DeviceInformation deviceInfo)
		{
			bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
		}
////////////////////////////////////////////////////////////////////////////////////////////


		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if ( bluetoothLeDevice != null ) {
				bluetoothLeDevice.Dispose();
				bluetoothLeDevice = null;
			}
			btDevice.StopDiscovery();
		}
////////////////////////////////////////////////////////////////////////////////////////////


		private void Connect_Click(object sender, EventArgs e)
		{

			int index = -1;
			for (int x = 0; x < DeviceList.Items.Count; x++)
			{
				// Determine if the item is selected.
				if (DeviceList.GetSelected(x) == true)
				{
					index = x;
					break;
				}
			}

			if (index >= 0)
			{
				DeviceInformation deviceInfo = btDevice.devices[index];
				connect(deviceInfo);
				Console.WriteLine("Connect to " + deviceInfo.Name);
			}
			else
			{
				Console.WriteLine("Index out of range");
			}

		}
		////////////////////////////////////////////////////////////////////////////////////////////

		private void Disconnect_Click(object sender, EventArgs e)
		{
			if (bluetoothLeDevice != null)
			{
				bluetoothLeDevice.Dispose();
				bluetoothLeDevice = null;
				Console.WriteLine("DisConnect ");

			}

		}
		////////////////////////////////////////////////////////////////////////////////////////////

	} //class Form1
} // namespace BLETestForm
