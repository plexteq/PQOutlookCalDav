namespace OutlookCalDAVSyncronizer.UI
{
    partial class ConfigWindow
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
            this.components = new System.ComponentModel.Container();
            this.authButton = new System.Windows.Forms.Button();
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.syncTimeTextBox = new System.Windows.Forms.TextBox();
            this.lblSync = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.cmbCallendars = new System.Windows.Forms.ComboBox();
            this.calidLabel = new System.Windows.Forms.Label();
            this.pswdLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.urlCombo = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.connectionGroupBox.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // authButton
            // 
            this.authButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.authButton.Location = new System.Drawing.Point(365, 134);
            this.authButton.Name = "authButton";
            this.authButton.Size = new System.Drawing.Size(104, 23);
            this.authButton.TabIndex = 0;
            this.authButton.Text = "Connect";
            this.authButton.UseVisualStyleBackColor = true;
            this.authButton.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionGroupBox.Controls.Add(this.syncTimeTextBox);
            this.connectionGroupBox.Controls.Add(this.lblSync);
            this.connectionGroupBox.Controls.Add(this.btnSync);
            this.connectionGroupBox.Controls.Add(this.cmbCallendars);
            this.connectionGroupBox.Controls.Add(this.calidLabel);
            this.connectionGroupBox.Controls.Add(this.pswdLabel);
            this.connectionGroupBox.Controls.Add(this.passwordTextBox);
            this.connectionGroupBox.Controls.Add(this.authButton);
            this.connectionGroupBox.Controls.Add(this.usernameLabel);
            this.connectionGroupBox.Controls.Add(this.usernameTextBox);
            this.connectionGroupBox.Controls.Add(this.label1);
            this.connectionGroupBox.Controls.Add(this.urlCombo);
            this.connectionGroupBox.Location = new System.Drawing.Point(12, 12);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(560, 174);
            this.connectionGroupBox.TabIndex = 11;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Connection";
            // 
            // syncTimeTextBox
            // 
            this.syncTimeTextBox.Location = new System.Drawing.Point(64, 135);
            this.syncTimeTextBox.Name = "syncTimeTextBox";
            this.syncTimeTextBox.Size = new System.Drawing.Size(295, 20);
            this.syncTimeTextBox.TabIndex = 27;
            this.syncTimeTextBox.Text = "10";
            this.toolTip.SetToolTip(this.syncTimeTextBox, "Syncronization interval in minutes");
            // 
            // lblSync
            // 
            this.lblSync.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSync.AutoSize = true;
            this.lblSync.Location = new System.Drawing.Point(-3, 139);
            this.lblSync.Name = "lblSync";
            this.lblSync.Size = new System.Drawing.Size(64, 13);
            this.lblSync.TabIndex = 26;
            this.lblSync.Text = "Sync time, s";
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSync.Enabled = false;
            this.btnSync.Location = new System.Drawing.Point(475, 134);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(76, 23);
            this.btnSync.TabIndex = 25;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // cmbCallendars
            // 
            this.cmbCallendars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCallendars.Enabled = false;
            this.cmbCallendars.FormattingEnabled = true;
            this.cmbCallendars.Location = new System.Drawing.Point(64, 55);
            this.cmbCallendars.Name = "cmbCallendars";
            this.cmbCallendars.Size = new System.Drawing.Size(487, 21);
            this.cmbCallendars.TabIndex = 22;
            // 
            // calidLabel
            // 
            this.calidLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.calidLabel.AutoSize = true;
            this.calidLabel.Location = new System.Drawing.Point(12, 58);
            this.calidLabel.Name = "calidLabel";
            this.calidLabel.Size = new System.Drawing.Size(49, 13);
            this.calidLabel.TabIndex = 19;
            this.calidLabel.Text = "Calendar";
            // 
            // pswdLabel
            // 
            this.pswdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pswdLabel.AutoSize = true;
            this.pswdLabel.Location = new System.Drawing.Point(8, 111);
            this.pswdLabel.Name = "pswdLabel";
            this.pswdLabel.Size = new System.Drawing.Size(53, 13);
            this.pswdLabel.TabIndex = 17;
            this.pswdLabel.Text = "Password";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.Location = new System.Drawing.Point(64, 108);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(487, 20);
            this.passwordTextBox.TabIndex = 16;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // usernameLabel
            // 
            this.usernameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(6, 85);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 15;
            this.usernameLabel.Text = "Username";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameTextBox.Location = new System.Drawing.Point(64, 82);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(487, 20);
            this.usernameTextBox.TabIndex = 14;
            this.usernameTextBox.Text = "ozakernychna@plexteq.com";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Url";
            // 
            // urlCombo
            // 
            this.urlCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlCombo.FormattingEnabled = true;
            this.urlCombo.Items.AddRange(new object[] {
            "https://caldav.icloud.com/",
            "https://apidata.googleusercontent.com/caldav/v2/",
            "https://caldav.calendar.yahoo.com/dav/",
            "Outlook"});
            this.urlCombo.Location = new System.Drawing.Point(64, 29);
            this.urlCombo.Name = "urlCombo";
            this.urlCombo.Size = new System.Drawing.Size(487, 21);
            this.urlCombo.TabIndex = 12;
            this.urlCombo.Text = "https://mx.plexteq.com/dav/";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 176);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(581, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // ConfigWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 198);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.connectionGroupBox);
            this.Name = "ConfigWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CalCli Demo";
            this.Load += new System.EventHandler(this.Main_Load);
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button authButton;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.Label pswdLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox urlCombo;
        private System.Windows.Forms.Label calidLabel;
        private System.Windows.Forms.ComboBox cmbCallendars;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox syncTimeTextBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblSync;
    }
}

