using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
//using System.Activities.Core;

using System.Runtime.InteropServices; // find window

 
namespace BLETestForm
{
//	const string winName = "9999";

	class BleDevice {

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern uint RegisterWindowMessage(string lpString);

		private uint _messageId;

		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, String lpWindowName);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

		public bool change;
		public bool enumerationCompleted;
		public Dispatcher dispUI = null;
		public object _locker;
		public List<DeviceInformation> devices = null;

		private DeviceWatcher deviceWatcher;
		public BluetoothLEDevice bluetoothLeDevice = null;

		
		public BleDevice() {
			devices =  new List<DeviceInformation>();
			enumerationCompleted = false;

			_locker = new object();
			Console.SetOut(Console.Out);
			Console.WriteLine("Hello World");
			_messageId = RegisterWindowMessage("BLEDeviceMessage");


	}
	////////////////////////////////////////////////////////////////////////////////////////////

	void ConnectDevice(DeviceInformation deviceInfo)
		{

//			BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
			// Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
			//			BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
			// ...
		}
		////////////////////////////////////////////////////////////////////////////////////////////

		public void StartDiscovery()
		{
			WatchDevices();
		}
		////////////////////////////////////////////////////////////////////////////////////////////

		public void StopDiscovery()
		{
			deviceWatcher.Stop();
		}
		////////////////////////////////////////////////////////////////////////////////////////////

		private void WatchDevices() {
			// from https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/gatt-client

			// Query for extra properties you want returned
			string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

			deviceWatcher =
						DeviceInformation.CreateWatcher(
						BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
						requestedProperties,
						DeviceInformationKind.AssociationEndpoint);

			// Register event handlers before starting the watcher.
			// Added, Updated and Removed are required to get all nearby devices
			deviceWatcher.Added += DeviceWatcher_Added;
			deviceWatcher.Updated += DeviceWatcher_Updated;
			deviceWatcher.Removed += DeviceWatcher_Removed;

			// EnumerationCompleted and Stopped are optional to implement.
			deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
			deviceWatcher.Stopped += DeviceWatcher_Stopped;

			// Start the watcher.
			deviceWatcher.Start();
			change = false;
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInterface) {

			Console.WriteLine("DeviceWatcher_Added:  " + deviceInterface.Name);
			lock ( _locker )
			{
				if (deviceInterface.Name != "")
				{
					devices.Add(deviceInterface);
				}
				else
				{
					Console.WriteLine("Name = NULL");
				}
			}

			System.IntPtr hwnd = (System.IntPtr)FindWindow("BLETestForm", null);
			SendMessage(hwnd, (int)_messageId, (IntPtr)55, (IntPtr)66);

			change = true;
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate) {


		}
		////////////////////////////////////////////////////////////////////////////////////////////


		void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate devUpdate) {

			Console.WriteLine("DeviceWatcher_Removed.");
			lock ( _locker )
			{
				for (int index = 0; index < devices.Count; index++)
				{
					DeviceInformation deviceInterface = devices[index];
					if (deviceInterface.Id == devUpdate.Id)
					{
						devices.Remove(deviceInterface);
						Console.WriteLine("   delete " + deviceInterface.Name);
					}
				}
			}
			change = true;
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e) {

			Console.WriteLine("DeviceWatcher_EnumerationCompleted.");
			enumerationCompleted = true;

			System.IntPtr hwnd = (System.IntPtr)FindWindow("WindowsForms10.Window.8.app.0.141b42a_r10_ad1", null);
			SendMessage(hwnd, (int)_messageId, (IntPtr)55, (IntPtr)66);

			//			::SendMessage()
		}
	////////////////////////////////////////////////////////////////////////////////////////////

	void DeviceWatcher_Stopped(DeviceWatcher sender, object args) {

			Console.WriteLine("DeviceWatcher_Stopped.");

		}
		////////////////////////////////////////////////////////////////////////////////////////////


	} // end of class
} // end of namespace BLE_test




