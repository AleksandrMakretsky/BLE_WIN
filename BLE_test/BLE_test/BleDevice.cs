using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;


namespace BLE_test
{

	class BleDevice {

		public bool change;
		public bool enumerationCompleted;
		public Dispatcher dispUI = null;// Dispatcher.CurrentDispatcher;
		public object _locker;// = new object();

		public List<DeviceInformation> devices = null;// new List<DeviceInformation>();


		public BleDevice() {
			devices =  new List<DeviceInformation>();
			enumerationCompleted = false;

			_locker = new object();

		}
		////////////////////////////////////////////////////////////////////////////////////////////

		public void WatchDevices() {
			// from https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/gatt-client

			// Query for extra properties you want returned
			string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

			DeviceWatcher deviceWatcher =
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

		}
		////////////////////////////////////////////////////////////////////////////////////////////


		async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInterface) {
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
			change = true;
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate) {

			Console.WriteLine("DeviceWatcher_Updated.");

		}
		////////////////////////////////////////////////////////////////////////////////////////////


		async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate devUpdate) {

			Console.WriteLine("DeviceWatcher_Removed.");
			lock ( _locker )
			{
				for (int index = 0; index < devices.Count; index++)
				{
					DeviceInformation deviceInterface = devices[index];
					if (deviceInterface.Id == devUpdate.Id)
					{
						devices.Remove(deviceInterface);
					}
				}
			}
			change = true;
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e) {

			Console.WriteLine("DeviceWatcher_EnumerationCompleted.");
			enumerationCompleted = true;



		}
		////////////////////////////////////////////////////////////////////////////////////////////

		async void DeviceWatcher_Stopped(DeviceWatcher sender, object args) {
			Console.WriteLine("DeviceWatcher_Stopped.");

		}
		////////////////////////////////////////////////////////////////////////////////////////////


	} // end of class
} // end of namespace BLE_test




