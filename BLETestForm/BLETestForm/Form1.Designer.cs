namespace BLETestForm
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Connect = new System.Windows.Forms.Button();
			this.DeviceList = new System.Windows.Forms.ListBox();
			this.Disconnect = new System.Windows.Forms.Button();
			this.Services = new System.Windows.Forms.ListBox();
			this.Properties = new System.Windows.Forms.ListBox();
			this.Read = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.Write = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Connect
			// 
			this.Connect.Location = new System.Drawing.Point(26, 31);
			this.Connect.Name = "Connect";
			this.Connect.Size = new System.Drawing.Size(139, 23);
			this.Connect.TabIndex = 0;
			this.Connect.Text = "Connect";
			this.Connect.UseVisualStyleBackColor = true;
			this.Connect.Click += new System.EventHandler(this.Connect_Click);
			// 
			// DeviceList
			// 
			this.DeviceList.FormattingEnabled = true;
			this.DeviceList.Location = new System.Drawing.Point(212, 31);
			this.DeviceList.Name = "DeviceList";
			this.DeviceList.ScrollAlwaysVisible = true;
			this.DeviceList.Size = new System.Drawing.Size(307, 69);
			this.DeviceList.TabIndex = 1;
			// 
			// Disconnect
			// 
			this.Disconnect.Location = new System.Drawing.Point(26, 61);
			this.Disconnect.Name = "Disconnect";
			this.Disconnect.Size = new System.Drawing.Size(139, 23);
			this.Disconnect.TabIndex = 2;
			this.Disconnect.Text = "Disconnect";
			this.Disconnect.UseVisualStyleBackColor = true;
			this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
			// 
			// Services
			// 
			this.Services.FormattingEnabled = true;
			this.Services.Location = new System.Drawing.Point(212, 106);
			this.Services.Name = "Services";
			this.Services.ScrollAlwaysVisible = true;
			this.Services.Size = new System.Drawing.Size(307, 69);
			this.Services.TabIndex = 3;
			this.Services.SelectedIndexChanged += new System.EventHandler(this.Services_SelectedIndexChanged);
			// 
			// Properties
			// 
			this.Properties.FormattingEnabled = true;
			this.Properties.Location = new System.Drawing.Point(212, 181);
			this.Properties.Name = "Properties";
			this.Properties.ScrollAlwaysVisible = true;
			this.Properties.Size = new System.Drawing.Size(307, 82);
			this.Properties.TabIndex = 4;
			// 
			// Read
			// 
			this.Read.Location = new System.Drawing.Point(26, 178);
			this.Read.Name = "Read";
			this.Read.Size = new System.Drawing.Size(139, 23);
			this.Read.TabIndex = 5;
			this.Read.Text = "Read";
			this.Read.UseVisualStyleBackColor = true;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(26, 207);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(139, 20);
			this.textBox1.TabIndex = 6;
			// 
			// Write
			// 
			this.Write.Location = new System.Drawing.Point(26, 237);
			this.Write.Name = "Write";
			this.Write.Size = new System.Drawing.Size(139, 23);
			this.Write.TabIndex = 7;
			this.Write.Text = "Write";
			this.Write.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(540, 348);
			this.Controls.Add(this.Write);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.Read);
			this.Controls.Add(this.Properties);
			this.Controls.Add(this.Services);
			this.Controls.Add(this.Disconnect);
			this.Controls.Add(this.DeviceList);
			this.Controls.Add(this.Connect);
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Connect;
		private System.Windows.Forms.ListBox DeviceList;
		private System.Windows.Forms.Button Disconnect;
		private System.Windows.Forms.ListBox Services;
		private System.Windows.Forms.ListBox Properties;
		private System.Windows.Forms.Button Read;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button Write;
	}
}

