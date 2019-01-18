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
			this.DeviceList.Size = new System.Drawing.Size(224, 173);
			this.DeviceList.TabIndex = 1;
			// 
			// Disconnect
			// 
			this.Disconnect.Location = new System.Drawing.Point(26, 76);
			this.Disconnect.Name = "Disconnect";
			this.Disconnect.Size = new System.Drawing.Size(139, 23);
			this.Disconnect.TabIndex = 2;
			this.Disconnect.Text = "Disconnect";
			this.Disconnect.UseVisualStyleBackColor = true;
			this.Disconnect.Click += new System.EventHandler(this.Disconnect_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 280);
			this.Controls.Add(this.Disconnect);
			this.Controls.Add(this.DeviceList);
			this.Controls.Add(this.Connect);
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button Connect;
		private System.Windows.Forms.ListBox DeviceList;
		private System.Windows.Forms.Button Disconnect;
	}
}

