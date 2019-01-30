namespace WF_BLE_v5
{

    partial class Form1
    {


        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.ValueChangedSubscribeToggle = new System.Windows.Forms.Button();
            this.btnCharacteristicWriteData1 = new System.Windows.Forms.Button();
            this.CharacteristicWriteValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnReadData = new System.Windows.Forms.Button();
            this.CharacteristicReadValue = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CharacteristicList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ServiceList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textIDDev = new System.Windows.Forms.TextBox();
            this.textSelDev = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnScanDev = new System.Windows.Forms.Button();
            this.btnStopScan = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDevConnect = new System.Windows.Forms.Button();
            this.btnDevDisConn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageDev = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCharacteristicReadButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tbDevName = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colServerId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRssi = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTxPower = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConnectable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServiceUuids = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colServicesWithData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colManufacturerCompanyId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colManufacturerData = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSolicitedServiceUuids = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBoxNum = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labelErr = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageDev.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelErr);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.textBoxNum);
            this.panel1.Controls.Add(this.ValueChangedSubscribeToggle);
            this.panel1.Controls.Add(this.btnCharacteristicWriteData1);
            this.panel1.Controls.Add(this.CharacteristicWriteValue);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.btnReadData);
            this.panel1.Controls.Add(this.CharacteristicReadValue);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.CharacteristicList);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ServiceList);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textIDDev);
            this.panel1.Controls.Add(this.textSelDev);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(903, 220);
            this.panel1.TabIndex = 1;
            // 
            // ValueChangedSubscribeToggle
            // 
            this.ValueChangedSubscribeToggle.Location = new System.Drawing.Point(241, 175);
            this.ValueChangedSubscribeToggle.Name = "ValueChangedSubscribeToggle";
            this.ValueChangedSubscribeToggle.Size = new System.Drawing.Size(115, 28);
            this.ValueChangedSubscribeToggle.TabIndex = 28;
            this.ValueChangedSubscribeToggle.Text = "Set Notify";
            this.ValueChangedSubscribeToggle.UseVisualStyleBackColor = true;
            this.ValueChangedSubscribeToggle.Click += new System.EventHandler(this.ValueChangedSubscribeToggle_Click);
            // 
            // btnCharacteristicWriteData1
            // 
            this.btnCharacteristicWriteData1.Location = new System.Drawing.Point(475, 176);
            this.btnCharacteristicWriteData1.Name = "btnCharacteristicWriteData1";
            this.btnCharacteristicWriteData1.Size = new System.Drawing.Size(118, 27);
            this.btnCharacteristicWriteData1.TabIndex = 27;
            this.btnCharacteristicWriteData1.Text = "WriteByte";
            this.btnCharacteristicWriteData1.UseVisualStyleBackColor = true;
            this.btnCharacteristicWriteData1.Click += new System.EventHandler(this.btnCharacteristicWriteData1_Click);
            // 
            // CharacteristicWriteValue
            // 
            this.CharacteristicWriteValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CharacteristicWriteValue.Location = new System.Drawing.Point(475, 147);
            this.CharacteristicWriteValue.Name = "CharacteristicWriteValue";
            this.CharacteristicWriteValue.Size = new System.Drawing.Size(421, 22);
            this.CharacteristicWriteValue.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(700, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "CharacteristicWriteValue";
            // 
            // btnReadData
            // 
            this.btnReadData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnReadData.Location = new System.Drawing.Point(72, 175);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(117, 28);
            this.btnReadData.TabIndex = 23;
            this.btnReadData.Text = "ReadData";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // CharacteristicReadValue
            // 
            this.CharacteristicReadValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CharacteristicReadValue.Location = new System.Drawing.Point(17, 147);
            this.CharacteristicReadValue.Name = "CharacteristicReadValue";
            this.CharacteristicReadValue.Size = new System.Drawing.Size(388, 22);
            this.CharacteristicReadValue.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(146, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(124, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "CharacteristicReadValue";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(289, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 18);
            this.label4.TabIndex = 13;
            this.label4.Text = "Выбрать хар-ку";
            // 
            // CharacteristicList
            // 
            this.CharacteristicList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CharacteristicList.FormattingEnabled = true;
            this.CharacteristicList.Location = new System.Drawing.Point(410, 94);
            this.CharacteristicList.Name = "CharacteristicList";
            this.CharacteristicList.Size = new System.Drawing.Size(486, 23);
            this.CharacteristicList.TabIndex = 12;
            this.CharacteristicList.SelectedIndexChanged += new System.EventHandler(this.CharacteristicList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(335, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Сервисы";
            // 
            // ServiceList
            // 
            this.ServiceList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ServiceList.FormattingEnabled = true;
            this.ServiceList.Location = new System.Drawing.Point(410, 65);
            this.ServiceList.Name = "ServiceList";
            this.ServiceList.Size = new System.Drawing.Size(486, 24);
            this.ServiceList.TabIndex = 10;
            this.ServiceList.SelectedIndexChanged += new System.EventHandler(this.ServiceList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(357, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(383, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 18);
            this.label1.TabIndex = 8;
            this.label1.Text = "ID";
            // 
            // textIDDev
            // 
            this.textIDDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textIDDev.Location = new System.Drawing.Point(410, 12);
            this.textIDDev.Name = "textIDDev";
            this.textIDDev.Size = new System.Drawing.Size(486, 22);
            this.textIDDev.TabIndex = 7;
            // 
            // textSelDev
            // 
            this.textSelDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textSelDev.Location = new System.Drawing.Point(410, 38);
            this.textSelDev.Name = "textSelDev";
            this.textSelDev.Size = new System.Drawing.Size(486, 22);
            this.textSelDev.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnScanDev);
            this.groupBox1.Controls.Add(this.btnStopScan);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 91);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сканер BLE";
            // 
            // btnScanDev
            // 
            this.btnScanDev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnScanDev.Location = new System.Drawing.Point(8, 18);
            this.btnScanDev.Name = "btnScanDev";
            this.btnScanDev.Size = new System.Drawing.Size(117, 28);
            this.btnScanDev.TabIndex = 0;
            this.btnScanDev.Text = "Сканировать";
            this.btnScanDev.UseVisualStyleBackColor = true;
            this.btnScanDev.Click += new System.EventHandler(this.btnScanDev_Click);
            // 
            // btnStopScan
            // 
            this.btnStopScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnStopScan.Location = new System.Drawing.Point(8, 54);
            this.btnStopScan.Name = "btnStopScan";
            this.btnStopScan.Size = new System.Drawing.Size(117, 28);
            this.btnStopScan.TabIndex = 1;
            this.btnStopScan.Text = "Стоп";
            this.btnStopScan.UseVisualStyleBackColor = true;
            this.btnStopScan.Click += new System.EventHandler(this.btnStopScan_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDevConnect);
            this.groupBox2.Controls.Add(this.btnDevDisConn);
            this.groupBox2.Location = new System.Drawing.Point(145, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(134, 91);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Подключение BLE";
            // 
            // btnDevConnect
            // 
            this.btnDevConnect.BackColor = System.Drawing.SystemColors.Control;
            this.btnDevConnect.Enabled = false;
            this.btnDevConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDevConnect.Location = new System.Drawing.Point(8, 19);
            this.btnDevConnect.Name = "btnDevConnect";
            this.btnDevConnect.Size = new System.Drawing.Size(117, 28);
            this.btnDevConnect.TabIndex = 2;
            this.btnDevConnect.Text = "Соединить";
            this.btnDevConnect.UseVisualStyleBackColor = false;
            this.btnDevConnect.Click += new System.EventHandler(this.btnDevConnect_ClickAsync);
            // 
            // btnDevDisConn
            // 
            this.btnDevDisConn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDevDisConn.Location = new System.Drawing.Point(8, 55);
            this.btnDevDisConn.Name = "btnDevDisConn";
            this.btnDevDisConn.Size = new System.Drawing.Size(117, 28);
            this.btnDevDisConn.TabIndex = 3;
            this.btnDevDisConn.Text = "Разъединить";
            this.btnDevDisConn.UseVisualStyleBackColor = true;
            this.btnDevDisConn.Click += new System.EventHandler(this.btnDevDisConn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageDev);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 220);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(903, 309);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPageDev
            // 
            this.tabPageDev.Controls.Add(this.splitContainer1);
            this.tabPageDev.Location = new System.Drawing.Point(4, 22);
            this.tabPageDev.Name = "tabPageDev";
            this.tabPageDev.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDev.Size = new System.Drawing.Size(895, 283);
            this.tabPageDev.TabIndex = 0;
            this.tabPageDev.Text = "BLE устройства";
            this.tabPageDev.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Silver;
            this.splitContainer1.Panel1.Controls.Add(this.btnCharacteristicReadButton);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.tbDevName);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(889, 277);
            this.splitContainer1.SplitterDistance = 296;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnCharacteristicReadButton
            // 
            this.btnCharacteristicReadButton.BackColor = System.Drawing.Color.LavenderBlush;
            this.btnCharacteristicReadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCharacteristicReadButton.Location = new System.Drawing.Point(40, 18);
            this.btnCharacteristicReadButton.Name = "btnCharacteristicReadButton";
            this.btnCharacteristicReadButton.Size = new System.Drawing.Size(223, 28);
            this.btnCharacteristicReadButton.TabIndex = 15;
            this.btnCharacteristicReadButton.Text = "Information about the device";
            this.btnCharacteristicReadButton.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(23, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 16);
            this.label5.TabIndex = 2;
            this.label5.Text = "Device";
            // 
            // tbDevName
            // 
            this.tbDevName.BackColor = System.Drawing.SystemColors.Menu;
            this.tbDevName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbDevName.Location = new System.Drawing.Point(80, 68);
            this.tbDevName.Name = "tbDevName";
            this.tbDevName.ReadOnly = true;
            this.tbDevName.Size = new System.Drawing.Size(202, 22);
            this.tbDevName.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer2.Size = new System.Drawing.Size(589, 277);
            this.splitContainer2.SplitterDistance = 132;
            this.splitContainer2.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colServerId,
            this.colName,
            this.colRssi,
            this.colTxPower,
            this.colConnectable,
            this.colServiceUuids,
            this.colServicesWithData,
            this.colManufacturerCompanyId,
            this.colManufacturerData,
            this.colSolicitedServiceUuids});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(589, 132);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // colServerId
            // 
            this.colServerId.Text = "Server ID";
            this.colServerId.Width = 120;
            // 
            // colName
            // 
            this.colName.Text = "Local Name";
            this.colName.Width = 240;
            // 
            // colRssi
            // 
            this.colRssi.Text = "RSSI";
            this.colRssi.Width = 90;
            // 
            // colTxPower
            // 
            this.colTxPower.Text = "TxPwr";
            this.colTxPower.Width = 45;
            // 
            // colConnectable
            // 
            this.colConnectable.Text = "Connectable";
            this.colConnectable.Width = 120;
            // 
            // colServiceUuids
            // 
            this.colServiceUuids.Text = "Service UUIDs";
            this.colServiceUuids.Width = 300;
            // 
            // colServicesWithData
            // 
            this.colServicesWithData.Text = "Services With Data";
            this.colServicesWithData.Width = 105;
            // 
            // colManufacturerCompanyId
            // 
            this.colManufacturerCompanyId.Text = "Mfr ID";
            // 
            // colManufacturerData
            // 
            this.colManufacturerData.Text = "Manufacturer Data";
            this.colManufacturerData.Width = 300;
            // 
            // colSolicitedServiceUuids
            // 
            this.colSolicitedServiceUuids.Text = "Solicited Service UUIDs";
            this.colSolicitedServiceUuids.Width = 130;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(589, 141);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(895, 283);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBoxNum
            // 
            this.textBoxNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxNum.Location = new System.Drawing.Point(757, 175);
            this.textBoxNum.Name = "textBoxNum";
            this.textBoxNum.Size = new System.Drawing.Size(139, 22);
            this.textBoxNum.TabIndex = 29;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(693, 183);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 30;
            this.label8.Text = "Num Cycle";
            // 
            // labelErr
            // 
            this.labelErr.AutoSize = true;
            this.labelErr.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelErr.Location = new System.Drawing.Point(625, 196);
            this.labelErr.Name = "labelErr";
            this.labelErr.Size = new System.Drawing.Size(274, 18);
            this.labelErr.TabIndex = 31;
            this.labelErr.Text = "ОШИБКА СРАВНЕНИЯ БУФФЕРА";
            this.labelErr.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 529);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "DSBle";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageDev.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDevDisConn;
        private System.Windows.Forms.Button btnDevConnect;
        private System.Windows.Forms.Button btnStopScan;
        private System.Windows.Forms.Button btnScanDev;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageDev;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textSelDev;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textIDDev;
        private System.Windows.Forms.ComboBox ServiceList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox CharacteristicList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCharacteristicWriteData1;
        private System.Windows.Forms.TextBox CharacteristicWriteValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.TextBox CharacteristicReadValue;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCharacteristicReadButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDevName;
        private System.Windows.Forms.Button ValueChangedSubscribeToggle;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxNum;
        private System.Windows.Forms.Label labelErr;
    }
}

