using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BLE_test
{

	class BleDevice {

		public bool change;

		List<DeviceInformation> devices = null;// new List<DeviceInformation>();

		//		static List<DeviceInformation> _deviceList;// = new List<DeviceInformation>();

		public BleDevice() {
			devices =  new List<DeviceInformation>();
		}

		public void WatchDevices() {
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

		async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInterface) {
			Console.WriteLine("DeviceWatcher_Added." + deviceInterface.Name);
			lock (this)
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

		async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate) {
			Console.WriteLine("DeviceWatcher_Updated.");

		}

		async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate devUpdate) {
			Console.WriteLine("DeviceWatcher_Removed.");

			lock (this)
			{
				Console.WriteLine("    before Device");
				int count = 0;
				foreach (DeviceInformation deviceInterface in devices)
				{
					Console.WriteLine("               Device " + deviceInterface.Name);
					if (count++ == devices.Count)
					{
						break;
					}

				}


				count = 0;
				foreach (DeviceInformation deviceInterface in devices)
				{

					if (deviceInterface.Id == devUpdate.Id)
					{
						devices.Remove(deviceInterface);
					}
					if (count++ == devices.Count)
					{
						break;
					}
				}

				count = 0;
				Console.WriteLine("    after Device");
				foreach (DeviceInformation deviceInterface in devices)
				{
					Console.WriteLine("               Device " + deviceInterface.Name);
					if (count++ == devices.Count)
					{
						break;
					}
				}

			}
		}

		async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e) {
			Console.WriteLine("DeviceWatcher_EnumerationCompleted.");

		}

		async void DeviceWatcher_Stopped(DeviceWatcher sender, object args) {
			Console.WriteLine("DeviceWatcher_Stopped.");

		}


	}
}




