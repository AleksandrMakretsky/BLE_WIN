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
using System.Windows.Forms;


namespace WF_BLE_v5
{


    public partial class Form1 : Form
    {
        #region Designer Variables
        private ColumnHeader colServerId = null;
        private ColumnHeader colName;
        private ColumnHeader colRssi;
        private ColumnHeader colTxPower;
        private ColumnHeader colServiceUuids;
        private ColumnHeader colServicesWithData;
        private ColumnHeader colSolicitedServiceUuids;
        private ColumnHeader colManufacturerCompanyId;
        private ColumnHeader colManufacturerData;
        private ColumnHeader colConnectable;
        #endregion

       
        List<DeviceInformation> interfaceList = null;

        DateTime aDateTime;

        bool PrintScreanEn = false;
        public Form1()
        {
            InitializeComponent();

            interfaceList = new List<DeviceInformation>();//interfaces

            var doubleBufferedPropInfo = listView1.GetType().GetProperty("DoubleBuffered",
                                            System.Reflection.BindingFlags.Instance |
                                            System.Reflection.BindingFlags.NonPublic);

            doubleBufferedPropInfo.SetValue(listView1, true, null);

            //SerialPort serialPort5 = new SerialPort();
            //serialPort5.DtrEnable = true;
            //serialPort5.Open();

            dispUI = Dispatcher.CurrentDispatcher;

            aDateTime = new DateTime();
        }


        public Dispatcher dispUI = null;// Dispatcher.CurrentDispatcher;

        public static DeviceWatcher watcher = null;
        public static int count = 0;
        public static DeviceInformation[] interfaces = new DeviceInformation[1000];
        public static bool isEnumerationComplete = false;
        public static string StopStatus = null;


        private void btnScanDev_Click(object sender, EventArgs e)
        {

            btnScanDev.BackColor = Color.GreenYellow;

            listView1.Items.Clear();
            PrintScreanEn = false;



            interfaceList.Clear(); //for listView;

            ServiceList.Items.Clear();
            CharacteristicList.Items.Clear();

            ServiceList.Text="";
            CharacteristicList.Text="";

            textIDDev.Clear();
            CharacteristicReadValue.Clear();
            textSelDev.Clear();

            count = 0;
            btnDevConnect.Enabled = false;

            WatchDevices();


        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// filtr BLE
        /// </summary>
        /// 
        //#region Device discovery

        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress",
                                         "System.Devices.Aep.IsConnected",
                                "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
        // BT_Code: Example showing paired and non-paired in a single query.
        string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

        /*
        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
        // BT_Code: Example showing paired and non-paired in a single query.
        string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
        */
        //===============================================================================================================

        private bool subscribedForNotifications = false;

        private async void printToLog(string args)
        {

            aDateTime = DateTime.Now;

            String strDataTime = aDateTime.ToString() + " - ";

            await dispUI.BeginInvoke((Action)(() =>
            {
                richTextBox1.Invoke((MethodInvoker)(() =>
                            richTextBox1.AppendText(strDataTime + args + " \r\n")));

                richTextBox1.ScrollToCaret();
            }));
        }


        private void WatchDevices()
        {
            try
            {

                watcher = DeviceInformation.CreateWatcher(
                         aqsAllBluetoothLEDevices,
                         requestedProperties,
                         DeviceInformationKind.AssociationEndpoint);

                // Add event handlers
                watcher.Added += watcher_Added;
                watcher.Removed += watcher_Removed;
                watcher.Updated += watcher_Updated;
                watcher.EnumerationCompleted += watcher_EnumerationCompleted;
                watcher.Stopped += watcher_Stopped;

                // Start over with an empty collection.

                watcher.Start();

                printToLog("Сканирование BLE устройств... ");
            }
            catch (ArgumentException)
            {
                printToLog("BLE устройства не найдены. ");
            }
        }

        async void watcher_Added(DeviceWatcher sender, DeviceInformation deviceInterface)
        {
            //await System.Windows.Threading.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            await dispUI.BeginInvoke((Action)(() =>
            {
                lock (this)
                {
                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        // Make sure device isn't already present in the list.
                        if (FindInterfaceList(deviceInterface.Id) == null)
                        {
                            if (deviceInterface.Name != string.Empty)
                            {
                                // If device has a friendly name display it immediately.
                                
                                interfaceList.Add(deviceInterface);

                                string sDev = "Add device:" + deviceInterface.Name;
                                printToLog(sDev);

                                DisplayDeviceInterfaceArray();

                            }

                        }

                    }
                }
            }));

        }


        private async void watcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await dispUI.BeginInvoke((Action)(() =>
            {
                lock (this)
                {

                    string sDev = "Updated {0}{1}" + deviceInfoUpdate.Id;
                    printToLog(sDev);

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        DeviceInformation deviceInfo = null;
                        deviceInfo = FindInterfaceList(deviceInfoUpdate.Id);

                        if (deviceInfo != null)
                        {
                            // Device is already being displayed - update UX.
                            //bleDeviceDisplay.Update(deviceInfoUpdate);
                            deviceInfo.Update(deviceInfoUpdate);
                            
                            DisplayDeviceInterfaceArray();

                            return;
                        }

 
                    }
                }
            }));

        }


        private DeviceInformation FindInterfaceList(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in interfaceList)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }
        async void watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate devUpdate)
        {


            await dispUI.BeginInvoke((Action)(() =>
            {
                lock (this)
                {

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        // Find the corresponding DeviceInformation in the collection and remove it.

                        DeviceInformation deviceInfo = null;
                        deviceInfo = FindInterfaceList(devUpdate.Id);

                        if (deviceInfo != null)
                        {
                         
                            interfaceList.Remove(deviceInfo);

                            string sDev = "Удалено {0}{1}" + devUpdate.Id;
                            printToLog(sDev);
                        }



                        DisplayDeviceInterfaceArray();
                    }
                }
            }));
        }

        private async void watcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element

                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == watcher)
                {
                    PrintScreanEn = true;// disp enable
                    DisplayDeviceInterfaceArray();
                    string sDev = "watcher_EnumerationCompleted";
                    printToLog(sDev);
                    
                }

        }

        async void DisplayDeviceInterfaceArray()
        {

            listView1.Invoke((MethodInvoker)(() =>
                            listView1.Items.Clear()));

            if (PrintScreanEn == false) return;

            btnDevConnect.Invoke((MethodInvoker)(() => btnDevConnect.Enabled = true));

            //foreach (DeviceInformation deviceInterface in interfaces)
            for ( int i=0; i < interfaceList.Count; i++ )
            {
                
                    DisplayDeviceInterface(interfaceList[i]);

              
            }

        }

        void DisplayDeviceInterface(DeviceInformation deviceInterface)
        {
            var id = deviceInterface.Id;
            var name = deviceInterface.Name;
            var isEnabled = deviceInterface.IsEnabled.ToString();


            string[] row1 = {
                name,
                isEnabled,
                "s3",
                "s4",
                "s5",
                "s6",
                "s7",
                "s8",
                "s9" };



            listView1.Invoke((MethodInvoker)(() =>
                              listView1.Items.Add(id).SubItems.AddRange(row1)));
        }

        private void btnStopScan_Click(object sender, EventArgs e)
        {

            btnScanDev.BackColor = btnStopScan.BackColor;

            //watcher_Stopped;
            if (watcher != null)
            {

                // Unregister the event handlers.
                watcher.Added -= watcher_Added;
                //watcher.Updated -= watcher_Updated;
                // watcher.Removed -= watcher_Removed;
                //watcher.EnumerationCompleted -= watcher_EnumerationCompleted;
                // watcher.Stopped -= watcher_Stopped;

                // Stop the watcher.
                watcher.Stop();
                watcher = null;


            }
        }

        async void watcher_Stopped(DeviceWatcher sender, object args)
        {

            if (watcher != null)
            {

                // We must update the collection on the UI thread because the collection is databound to a UI element.
                //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                await dispUI.BeginInvoke((Action)(() =>
                {
                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        richTextBox1.Invoke((MethodInvoker)(() =>
                            richTextBox1.AppendText("Stopped \r\n")));
                    }
                }));


            }

        }
        //====================================================================
        //
        //
        //====================================================================
        private ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        private ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattCharacteristic selectedCharacteristic;

        // Only one registered characteristic at a time.
        private GattCharacteristic registeredCharacteristic;
        private GattPresentationFormat presentationFormat;


        // Variables for "foreach" loop implementation
        static List<string> _forEachCommands = new List<string>();
        static List<string> _forEachDeviceNames = new List<string>();
        static List<DeviceInformation> _deviceList = new List<DeviceInformation>();

        //#region Enumerating Services


        static TimeSpan _timeout = TimeSpan.FromSeconds(3);
        //private bool isBusy = false;
        private async void btnDevConnect_ClickAsync(object sender, EventArgs e)
        {
            string strfoundId = "";
            string sDev = "";

            btnDevConnect.Enabled = false;


            try
            {

                strfoundId = textIDDev.Text;
                if (strfoundId == "")
                {
                    btnDevConnect.Enabled = true;
                    return;
                }

                btnDevConnect.BackColor = Color.GreenYellow;

                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(strfoundId);//.AsTask().TimeoutAfter(_timeout);

                if (bluetoothLeDevice == null)
                {
                    sDev = "Failed to connect to device.";
                    printToLog(sDev);
                }
            }
            catch (Exception ex) 
            {
                sDev = "Bluetooth radio is not on";
                printToLog(sDev);
            }

            if (bluetoothLeDevice != null)
            {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                //await 
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;

                    sDev = "Найдено " + services.Count.ToString() + "сервис(а).";
                    printToLog(sDev);

                    int n = 0;

                    btnDevConnect.BackColor = Color.GreenYellow;

                    ServiceList.BeginUpdate();
                    ServiceList.Items.Clear();

                    foreach (var service in services)
                    {
                        ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));

                        ServiceList.Items.Add(ServiceCollection[n++].Name);

                    }

                    ServiceList.Text = "Выбрать сервис:";
                    //ServiceList.SelectedIndex = 0;
                    ServiceList.EndUpdate();

                }
                else
                {
                    //rootPage.NotifyUser("Device unreachable", NotifyType.ErrorMessage);
                }
            }

            //           _exitCode = OpenDevice(strDevName);

            btnDevConnect.Enabled = true;
        }

        string strDevName = "";
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            textIDDev.Text = listView1.SelectedItems[0].Text;
            textSelDev.Text = listView1.SelectedItems[0].SubItems[1].Text;

        }

        static BluetoothLEDevice _selectedDevice = null;

        static async void PairBluetooth(string param)
        {
            DevicePairingResult result = null;
            DeviceInformationPairing pairingInformation = _selectedDevice.DeviceInformation.Pairing;

            await _selectedDevice.DeviceInformation.Pairing.UnpairAsync();

            if (pairingInformation.CanPair)
            {
                result = await _selectedDevice.DeviceInformation.Pairing.PairAsync(pairingInformation.ProtectionLevel);
            }
        }

        private void btnDevDisConn_Click(object sender, EventArgs e)
        {
            btnDevConnect.BackColor = btnDevDisConn.BackColor;
            
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;

        }

        #region Enumerating Characteristics
        private async void ServiceList_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ServiceList.Items.Count == 0) return;

            string sDev = "";

            //ServiceCollection[ServiceList.SelectedItem]
            int num = ServiceList.SelectedIndex;
            var attributeInfoDisp = (BluetoothLEAttributeDisplay)ServiceCollection[num];

            CharacteristicList.Items.Clear();
            CharacteristicCollection.Clear();
            

            IReadOnlyList<GattCharacteristic> characteristics = null;
            try
            {
                // Ensure we have access to the device.
                var accessStatus = await attributeInfoDisp.service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                    var result = await attributeInfoDisp.service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        characteristics = result.Characteristics;
                    }
                    else
                    {
                        //rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);
                        sDev = "Ошибка доступа к сервису.";
                        printToLog(sDev);
                        // On error, act as if there are no characteristics.
                        characteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    // Not granted access
                    //rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);
                    sDev = "Ошибка доступа к сервису.";
                    printToLog(sDev);
                    // On error, act as if there are no characteristics.
                    characteristics = new List<GattCharacteristic>();

                }
            }
            catch (Exception ex)
            {
                // rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message,
                //    NotifyType.ErrorMessage);

                sDev = "Ограниченный сервис. Не могу прочитать характеристики.";
                printToLog(sDev);
                // On error, act as if there are no characteristics.
                characteristics = new List<GattCharacteristic>();
            }

            int n = 0;
            CharacteristicList.BeginUpdate();

            foreach (GattCharacteristic c in characteristics)
            {
                CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
                CharacteristicList.Items.Add(CharacteristicCollection[n++].Name);
            }

            CharacteristicList.EndUpdate();
            CharacteristicList.Visible = true;
        }
        #endregion


        private async void CharacteristicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharacteristicList.Items.Count == 0) return;


            selectedCharacteristic = null;

            int num = CharacteristicList.SelectedIndex;
            var attributeInfoDisp = (BluetoothLEAttributeDisplay)CharacteristicCollection[num];


            if (attributeInfoDisp == null)
            {
                //EnableCharacteristicPanels(GattCharacteristicProperties.None);
                return;
            }

            selectedCharacteristic = attributeInfoDisp.characteristic;
            if (selectedCharacteristic == null)
            {

                strDevName = "No characteristic selected";
                printToLog(strDevName);
                return;
            }

            // Get all the child descriptors of a characteristics. Use the cache mode to specify uncached descriptors only 
            // and the new Async functions to get the descriptors of unpaired devices as well. 
            var result = await selectedCharacteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
            if (result.Status != GattCommunicationStatus.Success)
            {
                
                strDevName = "Descriptor read failure: " + result.Status.ToString();
                printToLog(strDevName);
            }

            // BT_Code: There's no need to access presentation format unless there's at least one. 
            presentationFormat = null;
            if (selectedCharacteristic.PresentationFormats.Count > 0)
            {

                if (selectedCharacteristic.PresentationFormats.Count.Equals(1))
                {
                    // Get the presentation format since there's only one way of presenting it
                    presentationFormat = selectedCharacteristic.PresentationFormats[0];
                }
                else
                {
                    // It's difficult to figure out how to split up a characteristic and encode its different parts properly.
                    // In this case, we'll just encode the whole thing to a string to make it easy to print out.
                }
            }

            // Enable/disable operations based on the GattCharacteristicProperties.
            //EnableCharacteristicPanels(selectedCharacteristic.CharacteristicProperties);
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            if(selectedCharacteristic == null)
            {
                strDevName = "selectedCharacteristic == null";
                printToLog(strDevName);
                return;
            }
            GattReadResult result = null;
            result = await selectedCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result == null)
            {
                strDevName = "btnReadData_Click result == null";
                printToLog(strDevName);
                return;
            }
            if (result.Status == GattCommunicationStatus.Success)
            {
                string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
                tbDevName.Text = formattedResult;
                CharacteristicReadValue.Text = $"Read result: {formattedResult}";
                strDevName = $"Read result: {formattedResult}";
                printToLog(strDevName);
            }
            else
            {
                strDevName = $"Read failed: {result.Status}";
                printToLog(strDevName);
            }
        }

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


        string str_cmp = "";
        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {

            string sd = "";
            // BT_Code: For the purpose of this sample, this function converts only UInt32 and
            // UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);

            if (data == null) return sd; 

            if (data.Length == 1)
            { 
                sd = "data: " + data[0].ToString() + " [len: " + data.Length.ToString()+"]";
             }
            else if (data.Length > 1)
            {
                sd = " [len: " + data.Length.ToString() + "] data: " + Encoding.UTF8.GetString(data);// CryptographicBuffer.ConvertBinaryToString( ,buffer );// EncodeToHexString(buffer);
                if( cnt_wr == 1 )
                {
                    str_cmp = sd;
                }
                else
                {
                    if (str_cmp != sd)
                    {
                        strDevName = "ERROR WRITE SEND: " + sd;
                        printToLog(strDevName);
                        
                        labelErr.Invoke((MethodInvoker)(() =>
                                labelErr.Visible = true));
                    }
                }

                label8.Invoke((MethodInvoker)(() =>
                                label8.Text = cnt_wr.ToString()));

        

                /*
                sd = "[len: " + data.Length.ToString() + "] data: " + data[0].ToString() + " ";
                for (int i = 1; i < data.Length; i++)
                {
                    sd += data[i].ToString() + " ";
                }
                */

            }
            return sd;

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
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>

    ////////////////////////////////////////////////////////////////////////////////////////////

    private async void btnCharacteristicWriteData1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(CharacteristicWriteValue.Text))
            {

                num_wr = (int)Convert.ToInt32(textBoxNum.Text); 
                WriteCycleData();
                cnt_wr = 0;


            }
            else
            {              
                strDevName = "No data to write to device";
                printToLog(strDevName);
            }
        }

        /// ///////////////////////////////////////////////////////////////////////////////////////////
        int cnt_wr = 0;
        int num_wr = 10;
        private async void WriteCycleData()
        {
            byte[] bw = { 0 };


            bw[0] = (byte)Convert.ToInt32(CharacteristicWriteValue.Text); 

            IBuffer writeBuffer = CryptographicBuffer.CreateFromByteArray(bw);

            var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);

        }
        private async void btnCharacteristicWriteButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(CharacteristicWriteValue.Text))
            {
                  
                    byte[] readValue = new byte[240];
                    string sd = CharacteristicWriteValue.Text;
                    char[] chb = new char[240];
                    chb = sd.ToCharArray();

                    for (int i=0; i < sd.Length; i++  )
                    {
                        readValue[i] = Convert.ToByte(chb[i]);
                    }

                    //readValue = StrToByteArray(CharacteristicWriteValue.Text);

                    IBuffer writeBuffer = CryptographicBuffer.CreateFromByteArray(readValue);
                    var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);

                 if (writeSuccessful)
                 {

                //var writer = new DataWriter();

                // writer.ByteOrder = ByteOrder.LittleEndian;
                // writer.WriteBytes(readValue);

                // var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);


                /*
                byte[240] bw = { 0 };
                bw[] = (byte)Convert.ToInt32(CharacteristicWriteValue.Text); ;
                IBuffer writeBuffer = CryptographicBuffer.CreateFromByteArray(bw);
                var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);
                */
                
                    }
                    else
                    {
                        strDevName = "btnCharacteristicWriteButton_Click error 1";
                        printToLog(strDevName);
                    }
            }
            else
            {
                strDevName = "btnCharacteristicWriteButton_Click error 2";
                printToLog(strDevName);
            }
        }

        /// <summary>
     /////////////////////////////////////////////////////////////////////////////////////////////
           private async Task<bool> WriteBufferToSelectedCharacteristicAsync(IBuffer buffer)
            {
                try
                {
                        // BT_Code: Writes the value from the buffer to the characteristic.
                     var result = await selectedCharacteristic.WriteValueWithResultAsync(buffer);
                   if (result.Status == GattCommunicationStatus.Success)
                        {                  
                            strDevName = "Successfully wrote value to device";
                            printToLog(strDevName);
                            return true;
                        }
                        else
                        {
                            strDevName = "Write failed: {result.Status}";
                            printToLog(strDevName);                  
                            return false;
                        }
                }
                catch (Exception ex)
                        {
                            //rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                            return false;
                        }
            
            }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////////

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            //////////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            
            if (cnt_wr < num_wr)
            {
                WriteCycleData();
                cnt_wr++;
            }
            ////////////////////////////////!@!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
            //var message = $"Value at {DateTime.Now:hh:mm:ss.FFF}: {newValue}";
            printToLog(newValue);

 
        }
        /// ///////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       // private bool subscribedForNotifications = false;
        private async void ValueChangedSubscribeToggle_Click(object sender, EventArgs e)
        {
            if (!subscribedForNotifications)
            {
                // initialize status
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;

                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;

                if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }
                else if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

               

                try
                {
                    // BT_Code: Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);
           

                    if (status == GattCommunicationStatus.Success)
                    {
                        AddValueChangedHandler();
                        strDevName = "Successfully subscribed for value changes: ";
                        printToLog(strDevName);

                    }
                    else
                    {
                        strDevName = "Error registering for value changes: {status}";
                        printToLog(strDevName);
                      
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                    strDevName = "catch (UnauthorizedAccessException ex)";
                    printToLog(strDevName);
                }
            }
            else
            {
                try
                {
                    // BT_Code: Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // Note that this sample configures either Indicate or Notify, but not both.
                    var result = await
                            selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.None);

                    if (result == GattCommunicationStatus.Success)
                    {
                        subscribedForNotifications = false;
                        
                        strDevName = "Successfully un-registered for notifications";
                        printToLog(strDevName);
                       
                    }
                    else
                    {
                        strDevName = "Error un-registering for notifications: {result}";
                        printToLog(strDevName);

                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                    strDevName = "reports that it support notify, but it actually doesn't";
                    printToLog(strDevName);
                }
            }
        }


    }
}


