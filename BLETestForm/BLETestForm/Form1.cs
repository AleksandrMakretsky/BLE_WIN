
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

//			Console.WriteLine("     Devices");
			lock (_btDevice._locker)
			{
				DeviceList.Items.Clear();
				int count = 0;
				for (int index = 0; index < _btDevice.devices.Count; index++)
				{
					DeviceInformation deviceInterface = _btDevice.devices[index];
//					Console.WriteLine("              " + deviceInterface.Name);
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
		private BluetoothLEAttributeDisplay selAttributeCharacteristic;
		private GattCharacteristic selectedCharacteristic;




		private async void connect(DeviceInformation deviceInfo)
		{
			bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

			if (bluetoothLeDevice == null) return;

			GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();


			if (result.Status == GattCommunicationStatus.Success)
			{
				Services.Items.Clear();
				Characteristic.Items.Clear();
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
			Characteristic.Items.Clear();

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
					Characteristic.Items.Add(CharacteristicCollection[n].Name);
					n++;
				}
				if (n > 0)
				{
					Characteristic.SetSelected(0, true);
				}
			}
			else
			{
				Console.WriteLine("DeviceAccessStatus.Allowed = false");
			}
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		private async void Read_Click(object sender, EventArgs e)
		{
			if (selectedCharacteristic == null) {
				Console.WriteLine("read result.Status false , selectedCharacteristic == null");
				return;
			}
			
			GattReadResult result = await selectedCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
			if (result.Status == GattCommunicationStatus.Success)
			{

				byte[] data;
				CryptographicBuffer.CopyToByteArray(result.Value, out data);

				string ss = "Value: " + data[0].ToString();
				Console.WriteLine(ss);
				string ss1 = " length: " + data.Length.ToString();
				Console.WriteLine(ss1);
				textBox1.Text = ss + ss1;

				//string ttt = System.Text.Encoding.UTF8.GetString(data);
				//textBox1.Text = ttt;


				//				string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
				//				Console.WriteLine("formattedResult: " + formattedResult);
				//				int sss = 0;

				/*
				string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
				tbDevName.Text = formattedResult;
				CharacteristicReadValue.Text = $"Read result: {formattedResult}";
				strDevName = $"Read result: {formattedResult}";
				printToLog(strDevName);
				//rootPage.NotifyUser($"Read result: {formattedResult}", NotifyType.StatusMessage);
				*/
			}
			else
			{
				Console.WriteLine("read result.Status false");
				/*
				//rootPage.NotifyUser($"Read failed: {result.Status}", NotifyType.ErrorMessage);
				strDevName = $"Read failed: {result.Status}";
				printToLog(strDevName);
				*/
			}
			
		}
		////////////////////////////////////////////////////////////////////////////////////////////

		private async void Properties_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedCharacteristic = null;
			int index = -1;
			for (int x = 0; x < Characteristic.Items.Count; x++)
			{
				if (Characteristic.GetSelected(x) == true)
				{
					index = x;
					break;
				}
			}
			if (index < 0) return;

			selAttributeCharacteristic = (BluetoothLEAttributeDisplay)CharacteristicCollection[index];
			selectedCharacteristic = selAttributeCharacteristic.characteristic;

			if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
			{
				if (index != 0) return;
				// Subscribing for notifications
				GattCommunicationStatus status = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
							GattClientCharacteristicConfigurationDescriptorValue.Notify);
				if (status == GattCommunicationStatus.Success)
				{
					// Server has been informed of clients interest.
					Console.WriteLine("Subscribing OK");
				}
				else
				{
					Console.WriteLine("Subscribing FALSE");
				}

				selectedCharacteristic.ValueChanged += Characteristic_ValueChanged;

			}

		}
		////////////////////////////////////////////////////////////////////////////////////////////


		private GattCharacteristic registeredCharacteristic;
		private GattPresentationFormat presentationFormat;

		private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
		{
			// BT_Code: For the purpose of this sample, this function converts only UInt32 and
			// UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
			byte[] data;
			CryptographicBuffer.CopyToByteArray(buffer, out data);

			string str;
			str = "data = " + data[0].ToString() + " length = " + data.Length.ToString();

			return str;

			if (format != null)
			{
				if (format.FormatType == GattPresentationFormatTypes.UInt32 && data.Length >= 4)
				{
					return BitConverter.ToInt32(data, 0).ToString();
				}
				else if (format.FormatType == GattPresentationFormatTypes.Utf8)
				{
					try
					{
						return Encoding.UTF8.GetString(data);
					}
					catch (ArgumentException)
					{
						return "(error: Invalid UTF-8 string)";
					}
				}
				else
				{
					// Add support for other format types as needed.
					return "Unsupported format: " + CryptographicBuffer.EncodeToHexString(buffer);
				}
			}
			else if (data != null)
			{
				// We don't know what format to use. Let's try some well-known profiles, or default back to UTF-8.
				if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.HeartRateMeasurement))
				{
					try
					{
						return "Heart Rate: " + ParseHeartRateValue(data).ToString();
					}
					catch (ArgumentException)
					{
						return "Heart Rate: (unable to parse)";
					}
				}
				else if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.BatteryLevel))
				{
					try
					{
						// battery level is encoded as a percentage value in the first byte according to
						// https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.battery_level.xml
						return "Battery Level: " + data[0].ToString() + "%";
					}
					catch (ArgumentException)
					{
						return "Battery Level: (unable to parse)";
					}
				}
				// This is our custom calc service Result UUID. Format it like an Int
				else if (selectedCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
				{
					return BitConverter.ToInt32(data, 0).ToString();
				}
				// No guarantees on if a characteristic is registered for notifications.
				else if (registeredCharacteristic != null)
				{
					// This is our custom calc service Result UUID. Format it like an Int
					if (registeredCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
					{
						return BitConverter.ToInt32(data, 0).ToString();
					}
				}
				else
				{
					try
					{
						return Encoding.UTF8.GetString(data);
					}
					catch (ArgumentException)
					{
						return "Unknown format";
					}
				}
			}
			else
			{
				return "Empty data received";
			}
			return "Unknown format";
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		private static ushort ParseHeartRateValue(byte[] data)
		{
			// Heart Rate profile defined flag values
			const byte heartRateValueFormat = 0x01;

			byte flags = data[0];
			bool isHeartRateValueSizeLong = ((flags & heartRateValueFormat) != 0);

			if (isHeartRateValueSizeLong)
			{
				return BitConverter.ToUInt16(data, 1);
			}
			else
			{
				return data[1];
			}
		}
		////////////////////////////////////////////////////////////////////////////////////////////


		private async void Write_Click(object sender, EventArgs e)
		{


			//var writeBuffer =  CryptographicBuffer.ConvertStringToBinary(textBox1.Text, BinaryStringEncoding.Utf8);

			byte[] cbuf = { 0 };
			cbuf[0] = (byte)Convert.ToInt32(textBox1.Text);
			IBuffer ibuf = CryptographicBuffer.CreateFromByteArray(cbuf);

			try
			{
				// BT_Code: Writes the value from the buffer to the characteristic.
				var result = await selectedCharacteristic.WriteValueWithResultAsync(ibuf);
				Console.WriteLine("cbuf[0] = " + cbuf[0].ToString());

				if (result.Status == GattCommunicationStatus.Success)
				{
					Console.WriteLine("OK");
					return;
				}
				else
				{
					Console.WriteLine("ERROR");
					return;
				}
			}
			catch (Exception ex)
			{
				//rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
				return ;
			}

		}
		////////////////////////////////////////////////////////////////////////////////////////////


		private async void WriteBufferToSelectedCharacteristicAsync(IBuffer buffer)
		{
			try
			{
				// BT_Code: Writes the value from the buffer to the characteristic.
				var result = await selectedCharacteristic.WriteValueWithResultAsync(buffer);

				if (result.Status == GattCommunicationStatus.Success)
				{

					return ;
				}
				else
				{
					return ;
				}
			}
			catch (Exception ex)
			{
				//rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
				return ;
			}

		}
		///////////////////////////////////////////////////////////////////////////////////////////

/*
		private void AddValueChangedHandler()
		{
			ValueChangedSubscribeToggle.Text = "Unsubscribe from value changes";
			if (!subscribedForNotifications)
			{
				registeredCharacteristic = selectedCharacteristic;
				registeredCharacteristic.ValueChanged += Characteristic_ValueChanged;
				subscribedForNotifications = true;
			}
		}
*/
		private  void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			// BT_Code: An Indicate or Notify reported that the value has changed.
			// Display the new value with a timestamp.
			var reader = DataReader.FromBuffer(args.CharacteristicValue);

			var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
			//var message = $"Value at {DateTime.Now:hh:mm:ss.FFF}: {newValue}";
			//printToLog(newValue);
			Console.WriteLine("Got Notification");
			Console.WriteLine(newValue);
		}
		/// ///////////////////////////////////////////////////////////////////////////////////////////



	} //class Form1
} // namespace BLETestForm
