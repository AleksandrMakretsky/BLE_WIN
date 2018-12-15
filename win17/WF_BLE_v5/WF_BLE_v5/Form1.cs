using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
//using Windows.System;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
//using System.Reflection;
using Windows.Foundation;
using Windows.Foundation.Metadata;
//using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;

using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

using System.Windows.Threading;
//using SDKTemplate;
using System.Windows.Forms;


namespace WF_BLE_v5
{


    public partial class Form1 : Form
    {
        #region Designer Variables
        private ColumnHeader colServerId=null;
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

        ObservableCollection<BluetoothLEDeviceDisplay> KnownDevices = null;//null new ObservableCollection<BluetoothLEDeviceDisplay>();
        List<DeviceInformation> UnknownDevices = null;// new List<DeviceInformation>();
        //List<DeviceInformation> interfaceList = null;// new List<DeviceInformation>(interfaces);
        DateTime aDateTime;
        public Form1()
        {
            InitializeComponent();

            //interfaceList = new List<DeviceInformation>(interfaces);
            UnknownDevices =  new List<DeviceInformation>();
            KnownDevices = new ObservableCollection<BluetoothLEDeviceDisplay>();

            var doubleBufferedPropInfo = listView1.GetType().GetProperty("DoubleBuffered",
                                            System.Reflection.BindingFlags.Instance | 
                                            System.Reflection.BindingFlags.NonPublic);

            doubleBufferedPropInfo.SetValue(listView1, true, null);

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
            UnknownDevices.Clear();
            count = 0;
            

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

                /*
                watcher.Added += (DeviceWatcher sender, DeviceInformation devInfo) =>
                {
                    if (UnknownDevices.FirstOrDefault(d => d.Id.Equals(devInfo.Id) || d.Name.Equals(devInfo.Name)) == null)
                    {

                        UnknownDevices.Add(devInfo);
                        DisplayDeviceInterfaceArray();

                    }
                };
                */

                watcher.Removed += watcher_Removed;
                
                /*
                watcher.Removed += (DeviceWatcher sender, DeviceInformationUpdate devInfo) =>
                {
                    UnknownDevices.Remove(FindUnknownDevices(devInfo.Id));
                };
                */
               
                watcher.Updated += watcher_Updated;
                //watcher.Updated += (_, __) => { };
                watcher.EnumerationCompleted += watcher_EnumerationCompleted;
               //watcher.EnumerationCompleted += (DeviceWatcher sender, object arg) => { sender.Stop(); };
                watcher.Stopped += watcher_Stopped;
                //watcher.Stopped += (DeviceWatcher sender, object arg) => { UnknownDevices.Clear(); sender.Start(); };



                // Start over with an empty collection.
                KnownDevices.Clear();
                watcher.Start();
                
                printToLog( "Сканирование BLE устройств... ");
            }
            catch (ArgumentException)
            {
                //The ArgumentException gets thrown by FindAllAsync when the GUID isn't formatted properly
                //The only reason we're catching it here is because the user is allowed to enter GUIDs without validation
                //In normal usage of the API, this exception handling probably wouldn't be necessary when using known-good GUIDs 
                // richTextBox1.AppendText("Caught ArgumentException. Failed to create watcher." + " \r\n");
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
                    //Debug.WriteLine(String.Format("Added {0}{1}", deviceInterface.Id, deviceInterface.Name));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        // Make sure device isn't already present in the list.
                        if (FindBluetoothLEDeviceDisplay(deviceInterface.Id) == null)
                        {
                            if (deviceInterface.Name != string.Empty)
                            {
                                // If device has a friendly name display it immediately.
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInterface));

                                string sDev = "Add device:" + deviceInterface.Name;
                                printToLog(sDev);

                                interfaces[count] = deviceInterface;
                                count += 1;

                                DisplayDeviceInterfaceArray();

                            }
                            else
                            {
                                // Add it to a list in case the name gets updated later. 
                                UnknownDevices.Add(deviceInterface);
                            }
                        }

                    }
                }
            }));

        }


        public BluetoothLEDevice FindBluetoothLEDevice(string id)
        {
            int i = 0;

            for (i=0; i < KnownDevices.Count; i++ )
            //luetoothLEDevice bleDeviceDisplay in KnownDevices)
            {
                if (KnownDevices[i].Id == id)
                {
                    //return i; (BluetoothLEDevice)(KnownDevices[i].Id);
                }
            }


            return null;
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }



        private async void watcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            //Dispatcher
            //await dispUI.BeginInvoke((Action)(() =>
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
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            // Device is already being displayed - update UX.
                            bleDeviceDisplay.Update(deviceInfoUpdate);

                            DisplayDeviceInterfaceArray();

                            return;
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            deviceInfo.Update(deviceInfoUpdate);
                            // If device has been updated with a friendly name it's no longer unknown.
                            if (deviceInfo.Name != String.Empty)
                            {
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));

                                interfaces[count] = deviceInfo;//DeviceInformation deviceInterface
                                count += 1;

                                UnknownDevices.Remove(deviceInfo);

                            }
                        }
                    }
                }
            }));

        }

        async void watcher_Removed(DeviceWatcher sender, DeviceInformationUpdate devUpdate)
        {
            int count2 = 0;
            //Convert interfaces array to a list (IList).
            List<DeviceInformation> interfaceList = new List<DeviceInformation>(interfaces);

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            await dispUI.BeginInvoke((Action)(() =>
            {
                lock (this)
                {
      


                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == watcher)
                    {
                        // Find the corresponding DeviceInformation in the collection and remove it.
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(devUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            KnownDevices.Remove(bleDeviceDisplay);

                            string sDev = "Удалено {0}{1}" + devUpdate.Id;
                            printToLog(sDev);

                            foreach (DeviceInformation deviceInterface in interfaces)
                            {
                                if (count2 < count)
                                {
                                    if (interfaces[count2].Id == devUpdate.Id)
                                    {
                                        //Remove the element.
                                        interfaceList.RemoveAt(count2);

                                    }

                                }
                                count2 += 1;
                            }
                            //Convert the list back to the interfaces array.
                            interfaces = interfaceList.ToArray();
                            if (count > 0) count -= 1;
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(devUpdate.Id);
                        if (deviceInfo != null)
                        {
                            UnknownDevices.Remove(deviceInfo);
                        }
                        //refresh display (ListView)
                        DisplayDeviceInterfaceArray();
                    }
                }
            }));
        }

        private async void watcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            await dispUI.BeginInvoke((Action)(() =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == watcher)
                {
                    //rootPage.NotifyUser($"{KnownDevices.Count} devices found. Enumeration completed.",
                    //   NotifyType.StatusMessage);

                    //refresh display (ListView)
                    //DisplayDeviceInterfaceArray();
                }
            }));
        }

        async void DisplayDeviceInterfaceArray()
        {
           
            listView1.Invoke((MethodInvoker)(() =>
                            listView1.Items.Clear()));

            int count2 = 0;
            foreach (DeviceInformation deviceInterface in interfaces)
            {
                if (count2 < count)
                {
                    //await dispUI.BeginInvoke((Action)(() =>
                    //{
                        DisplayDeviceInterface(deviceInterface);
                    //}));
                }
                count2 += 1;
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


                        // rootPage.NotifyUser($"No longer watching for devices.",
                        // sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);

                        //watcher.Stop();
                        //watcher = null;
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
        

        static int _forEachCmdCounter = 0;
        static int _forEachDeviceCounter = 0;
        static bool _forEachCollection = false;
        static bool _forEachExecution = false;
        static string _forEachDeviceMask = "";
        static int _exitCode = 0;

        #region Error Codes
        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)
        #endregion



        //#region Enumerating Services

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.
            /*
            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
            var message = $"Value at {DateTime.Now:hh:mm:ss.FFF}: {newValue}";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => CharacteristicLatestValue.Text = message);
            */
        }

        private async Task<bool> ClearBluetoothLEDeviceAsync()
        {
            if (subscribedForNotifications)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                var result = await registeredCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (result != GattCommunicationStatus.Success)
                {
                    return false;
                }
                else
                {
                    selectedCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                    subscribedForNotifications = false;
                }
            }
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;
            return true;
        }

        static TimeSpan _timeout = TimeSpan.FromSeconds(3);
        //private bool isBusy = false;
        private async void btnDevConnect_ClickAsync(object sender, EventArgs e)
        {
            string strfoundId = "";
            string sDev = "";

            btnDevConnect.Enabled = false;
            /*
            if (!await ClearBluetoothLEDeviceAsync())
            {
                rootPage.NotifyUser("Error: Unable to reset state, " +
                                    "try again.", NotifyType.ErrorMessage);
                ConnectButton.IsEnabled = false;
                return;
            }*/

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                // await 
                
                // BluetoothLEDevice.FromIdAsync(rootPage.SelectedBleDeviceId);
                strfoundId = textIDDev.Text;
                if(strfoundId == "" )
                {
                    btnDevConnect.Enabled = true;
                    return;
                }

                btnDevConnect.BackColor = Color.GreenYellow;

                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(strfoundId);//.AsTask().TimeoutAfter(_timeout);

                if (bluetoothLeDevice == null)
                {
                    //rootPage.NotifyUser("Failed to connect to device.", NotifyType.ErrorMessage);
                    sDev = "Failed to connect to device.";
                    printToLog(sDev);
                }
            }
            catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {
                //rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);
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
                    // rootPage.NotifyUser(String.Format("Found {0} services", services.Count), NotifyType.StatusMessage);
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


        /// Connect to the specific device by name or number, and make this device current
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        static int OpenDevice(string deviceName) //async 
        {
            int retVal = 0;



            /*
            if (!string.IsNullOrEmpty(deviceName))
            {

                
                var devs = _deviceList.OrderBy(d => d.Name).Where(d => !string.IsNullOrEmpty(d.Name)).ToList();
                string foundId = Utilities.GetIdByNameOrNumber(devs, deviceName);
                
                 If device is found, connect to device and enumerate all services
                if (!string.IsNullOrEmpty(foundId))
                {
                    
                    _selectedCharacteristic = null;
                    _selectedService = null;
                    _services.Clear();
                

                    try
                    {
                        // only allow for one connection to be open at a time
                        if (_selectedDevice != null) CloseDevice();

                        _selectedDevice = await BluetoothLEDevice.FromIdAsync(foundId).AsTask().TimeoutAfter(_timeout);
                        if (!Console.IsInputRedirected)
                            Console.WriteLine($"Connecting to {_selectedDevice.Name}.");

                       var result = await _selectedDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            if (!Console.IsInputRedirected)
                                Console.WriteLine($"Found {result.Services.Count} services:");

                            for (int i = 0; i < result.Services.Count; i++)
                            {
                                var serviceToDisplay = new BluetoothLEAttributeDisplay(result.Services[i]);
                                _services.Add(serviceToDisplay);
                                if (!Console.IsInputRedirected)
                                    Console.WriteLine($"#{i:00}: {_services[i].Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Device {deviceName} is unreachable.");
                            retVal += 1;
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Device {deviceName} is unreachable.");
                        retVal += 1;
                    }
                }
                else
                {
                    retVal += 1;
                }
            }
            else
            {
                Console.WriteLine("Device name can not be empty.");
                retVal += 1;
            }

            */

            return retVal;

        }//static async Task<int> OpenDevice(string deviceName)
        

 
        private void btnDevDisConn_Click(object sender, EventArgs e)
        {
            btnDevConnect.BackColor = btnDevDisConn.BackColor;
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
            RemoveValueChangedHandler();

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

        private void RemoveValueChangedHandler()
        {
            //ValueChangedSubscribeToggle.Content = "Subscribe to value changes";
            if (subscribedForNotifications)
            {
                registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                registeredCharacteristic = null;
                subscribedForNotifications = false;
            }
        }
    }
}


