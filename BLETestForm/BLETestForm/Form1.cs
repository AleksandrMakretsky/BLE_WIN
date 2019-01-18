
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

		private ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
		private ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();


		private async void connect(DeviceInformation deviceInfo)
		{
			bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

			if (bluetoothLeDevice == null) return;

			GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();


			if (result.Status == GattCommunicationStatus.Success)
			{
				Services.Items.Clear();
				Properties.Items.Clear();
				var services = result.Services;

				int n = 0;
				foreach (var service in services)
				{
					ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));
					Console.WriteLine("     Service:   " + ServiceCollection[n].Name);
					Services.Items.Add(ServiceCollection[n].Name);
					n++;
				}
				if (n > 0) {
					Services.SetSelected(0, true);
				}


				int stop = 0;
				// ...
			}


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


		private async void Services_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = -1;
			for (int x = 0; x < Services.Items.Count; x++)
			{
				if (Services.GetSelected(x) == true)
				{
					index = x;
					break;
				}
			}
			if (index < 0) return;

			var attributeInfoDisp = (BluetoothLEAttributeDisplay)ServiceCollection[index];

			CharacteristicCollection.Clear();
			Properties.Items.Clear();

			IReadOnlyList<GattCharacteristic> characteristics = null;

			var accessStatus = await attributeInfoDisp.service.RequestAccessAsync();
			if ((int)accessStatus == (int)DeviceAccessStatus.Allowed)
			{

				var result = await attributeInfoDisp.service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
				if (result.Status == GattCommunicationStatus.Success)
				{
					characteristics = result.Characteristics;
				}
				else
				{
					Console.WriteLine("Ошибка доступа к сервису.");
				}


				int n = 0;
				foreach (GattCharacteristic c in characteristics)
				{
					CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
					Properties.Items.Add(CharacteristicCollection[n].Name);
					n++;
				}
			}
			else
			{
				Console.WriteLine("DeviceAccessStatus.Allowed = false");
			}



		}
		////////////////////////////////////////////////////////////////////////////////////////////

	} //class Form1
} // namespace BLETestForm
