using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;

namespace BLETestForm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			BleDevice btDevice = new BleDevice();
			btDevice.StartDiscovery();


		}


		async void connect() {
			DeviceInformation deviceInfo;
			BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{

		}
	}
}
