
namespace HadwareRemoteControl
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pbScreen = new System.Windows.Forms.PictureBox();
            tsMain = new System.Windows.Forms.ToolStrip();
            btnConnect = new System.Windows.Forms.ToolStripButton();
            btnDisconnect = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            tsbCameras = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            tsbFormat = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            tsCom = new System.Windows.Forms.ToolStripDropDownButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tsbOther = new System.Windows.Forms.ToolStripDropDownButton();
            powerOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tsbReboot = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            pasteClipboardTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            sendAltCtrlDelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sencCtrlEscItem = new System.Windows.Forms.ToolStripMenuItem();
            sendWinLButton = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            tsDisplayType = new System.Windows.Forms.ToolStripDropDownButton();
            scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            scrollToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            transformImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tsDebug = new System.Windows.Forms.ToolStripLabel();
            tsDebugFormat = new System.Windows.Forms.ToolStripLabel();
            tsbDebugSerial = new System.Windows.Forms.ToolStripLabel();
            tsbFullScreen = new System.Windows.Forms.ToolStripButton();
            bwEnterText = new System.ComponentModel.BackgroundWorker();
            bgFrameProcessing = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)pbScreen).BeginInit();
            tsMain.SuspendLayout();
            SuspendLayout();
            // 
            // pbScreen
            // 
            pbScreen.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pbScreen.BackColor = System.Drawing.Color.Black;
            pbScreen.Location = new System.Drawing.Point(4, 29);
            pbScreen.Name = "pbScreen";
            pbScreen.Size = new System.Drawing.Size(1031, 662);
            pbScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbScreen.TabIndex = 1;
            pbScreen.TabStop = false;
            pbScreen.Paint += pbScreen_Paint;
            pbScreen.MouseDown += pbScreen_MouseDown;
            pbScreen.MouseMove += pbScreen_MouseMove;
            pbScreen.MouseUp += pbScreen_MouseUp;
            // 
            // tsMain
            // 
            tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnConnect, btnDisconnect, toolStripSeparator4, toolStripLabel1, tsbCameras, toolStripLabel3, tsbFormat, toolStripSeparator1, toolStripLabel2, tsCom, toolStripSeparator2, tsbOther, toolStripSeparator3, toolStripLabel5, tsDisplayType, tsDebug, tsDebugFormat, tsbDebugSerial, tsbFullScreen });
            tsMain.Location = new System.Drawing.Point(0, 0);
            tsMain.Name = "tsMain";
            tsMain.Size = new System.Drawing.Size(1039, 25);
            tsMain.TabIndex = 2;
            tsMain.Text = "toolStrip1";
            // 
            // btnConnect
            // 
            btnConnect.Image = (System.Drawing.Image)resources.GetObject("btnConnect.Image");
            btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(71, 22);
            btnConnect.Text = "Connect";
            btnConnect.Click += btnConnect_Click;
            // 
            // btnDisconnect
            // 
            btnDisconnect.Enabled = false;
            btnDisconnect.Image = (System.Drawing.Image)resources.GetObject("btnDisconnect.Image");
            btnDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new System.Drawing.Size(87, 22);
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new System.Drawing.Size(50, 22);
            toolStripLabel1.Text = "Camera:";
            // 
            // tsbCameras
            // 
            tsbCameras.Image = (System.Drawing.Image)resources.GetObject("tsbCameras.Image");
            tsbCameras.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbCameras.Name = "tsbCameras";
            tsbCameras.Size = new System.Drawing.Size(84, 22);
            tsbCameras.Tag = "0";
            tsbCameras.Text = "Camera 1";
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new System.Drawing.Size(82, 22);
            toolStripLabel3.Text = "Prefer Format:";
            // 
            // tsbFormat
            // 
            tsbFormat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            tsbFormat.Image = (System.Drawing.Image)resources.GetObject("tsbFormat.Image");
            tsbFormat.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbFormat.Name = "tsbFormat";
            tsbFormat.Size = new System.Drawing.Size(59, 22);
            tsbFormat.Text = "Default";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new System.Drawing.Size(60, 22);
            toolStripLabel2.Text = "Keyboard:";
            // 
            // tsCom
            // 
            tsCom.Image = (System.Drawing.Image)resources.GetObject("tsCom.Image");
            tsCom.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsCom.Name = "tsCom";
            tsCom.Size = new System.Drawing.Size(63, 22);
            tsCom.Text = "COM";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbOther
            // 
            tsbOther.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { powerOffToolStripMenuItem, tsbReboot, toolStripMenuItem3, pasteClipboardTextToolStripMenuItem, toolStripMenuItem2, sendAltCtrlDelToolStripMenuItem, sencCtrlEscItem, sendWinLButton });
            tsbOther.Enabled = false;
            tsbOther.Image = (System.Drawing.Image)resources.GetObject("tsbOther.Image");
            tsbOther.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbOther.Name = "tsbOther";
            tsbOther.Size = new System.Drawing.Size(66, 22);
            tsbOther.Text = "Other";
            tsbOther.ToolTipText = " ";
            // 
            // powerOffToolStripMenuItem
            // 
            powerOffToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("powerOffToolStripMenuItem.Image");
            powerOffToolStripMenuItem.Name = "powerOffToolStripMenuItem";
            powerOffToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            powerOffToolStripMenuItem.Text = "Power On/Off";
            powerOffToolStripMenuItem.Click += powerOffToolStripMenuItem_Click;
            // 
            // tsbReboot
            // 
            tsbReboot.Image = (System.Drawing.Image)resources.GetObject("tsbReboot.Image");
            tsbReboot.Name = "tsbReboot";
            tsbReboot.Size = new System.Drawing.Size(181, 22);
            tsbReboot.Text = "Reboot";
            tsbReboot.Click += tsbReboot_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(178, 6);
            // 
            // pasteClipboardTextToolStripMenuItem
            // 
            pasteClipboardTextToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("pasteClipboardTextToolStripMenuItem.Image");
            pasteClipboardTextToolStripMenuItem.Name = "pasteClipboardTextToolStripMenuItem";
            pasteClipboardTextToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            pasteClipboardTextToolStripMenuItem.Text = "Enter Clipboard Text";
            pasteClipboardTextToolStripMenuItem.Click += pasteClipboardTextToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(178, 6);
            // 
            // sendAltCtrlDelToolStripMenuItem
            // 
            sendAltCtrlDelToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("sendAltCtrlDelToolStripMenuItem.Image");
            sendAltCtrlDelToolStripMenuItem.Name = "sendAltCtrlDelToolStripMenuItem";
            sendAltCtrlDelToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            sendAltCtrlDelToolStripMenuItem.Text = "Send Alt+Ctrl+Del";
            sendAltCtrlDelToolStripMenuItem.Click += sendAltCtrlDelToolStripMenuItem_Click;
            // 
            // sencCtrlEscItem
            // 
            sencCtrlEscItem.Image = (System.Drawing.Image)resources.GetObject("sencCtrlEscItem.Image");
            sencCtrlEscItem.Name = "sencCtrlEscItem";
            sencCtrlEscItem.Size = new System.Drawing.Size(181, 22);
            sencCtrlEscItem.Text = "Send Ctrl+Shift+Esc";
            sencCtrlEscItem.Click += sencCtrlEscItem_Click;
            // 
            // sendWinLButton
            // 
            sendWinLButton.Image = (System.Drawing.Image)resources.GetObject("sendWinLButton.Image");
            sendWinLButton.Name = "sendWinLButton";
            sendWinLButton.Size = new System.Drawing.Size(181, 22);
            sendWinLButton.Text = "Send Win+L";
            sendWinLButton.Click += sendWinLButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel5
            // 
            toolStripLabel5.Name = "toolStripLabel5";
            toolStripLabel5.Size = new System.Drawing.Size(49, 22);
            toolStripLabel5.Text = "Screen: ";
            // 
            // tsDisplayType
            // 
            tsDisplayType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { scaleToolStripMenuItem, scrollToolStripMenuItem, toolStripMenuItem1, transformImageToolStripMenuItem });
            tsDisplayType.Enabled = false;
            tsDisplayType.Image = (System.Drawing.Image)resources.GetObject("tsDisplayType.Image");
            tsDisplayType.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsDisplayType.Name = "tsDisplayType";
            tsDisplayType.Size = new System.Drawing.Size(64, 22);
            tsDisplayType.Tag = "0";
            tsDisplayType.Text = "Scale";
            // 
            // scaleToolStripMenuItem
            // 
            scaleToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("scaleToolStripMenuItem.Image");
            scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            scaleToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            scaleToolStripMenuItem.Tag = "0";
            scaleToolStripMenuItem.Text = "Scale";
            scaleToolStripMenuItem.Click += screenTypeToolStripMenuItem_Click;
            // 
            // scrollToolStripMenuItem
            // 
            scrollToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("scrollToolStripMenuItem.Image");
            scrollToolStripMenuItem.Name = "scrollToolStripMenuItem";
            scrollToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            scrollToolStripMenuItem.Tag = "1";
            scrollToolStripMenuItem.Text = "Original";
            scrollToolStripMenuItem.Click += screenTypeToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(172, 6);
            // 
            // transformImageToolStripMenuItem
            // 
            transformImageToolStripMenuItem.Image = (System.Drawing.Image)resources.GetObject("transformImageToolStripMenuItem.Image");
            transformImageToolStripMenuItem.Name = "transformImageToolStripMenuItem";
            transformImageToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            transformImageToolStripMenuItem.Text = "Transform Image...";
            transformImageToolStripMenuItem.Click += transformImageToolStripMenuItem_Click;
            // 
            // tsDebug
            // 
            tsDebug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            tsDebug.Name = "tsDebug";
            tsDebug.Size = new System.Drawing.Size(13, 22);
            tsDebug.Text = "::";
            // 
            // tsDebugFormat
            // 
            tsDebugFormat.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            tsDebugFormat.Name = "tsDebugFormat";
            tsDebugFormat.Size = new System.Drawing.Size(0, 22);
            // 
            // tsbDebugSerial
            // 
            tsbDebugSerial.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            tsbDebugSerial.Name = "tsbDebugSerial";
            tsbDebugSerial.Size = new System.Drawing.Size(0, 22);
            // 
            // tsbFullScreen
            // 
            tsbFullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            tsbFullScreen.Image = (System.Drawing.Image)resources.GetObject("tsbFullScreen.Image");
            tsbFullScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbFullScreen.Name = "tsbFullScreen";
            tsbFullScreen.Size = new System.Drawing.Size(23, 22);
            tsbFullScreen.Text = "FullScreen";
            tsbFullScreen.Click += tsbFullScreen_Click;
            // 
            // bwEnterText
            // 
            bwEnterText.DoWork += bwEnterText_DoWork;
            // 
            // bgFrameProcessing
            // 
            bgFrameProcessing.DoWork += bgFrameProcessing_DoWork;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1039, 695);
            Controls.Add(tsMain);
            Controls.Add(pbScreen);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "HardwareRemoteControl";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            Shown += Form1_Shown;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)pbScreen).EndInit();
            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripDropDownButton tsbCameras;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnConnect;
        private System.Windows.Forms.ToolStripButton btnDisconnect;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton tsCom;
        private System.Windows.Forms.ToolStripLabel tsDebug;
        private System.Windows.Forms.ToolStripDropDownButton tsbOther;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem tsbReboot;
        private System.ComponentModel.BackgroundWorker bwEnterText;
        private System.Windows.Forms.ToolStripMenuItem pasteClipboardTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendAltCtrlDelToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripDropDownButton tsDisplayType;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrollToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem transformImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel tsDebugFormat;
        private System.Windows.Forms.ToolStripLabel tsbDebugSerial;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sencCtrlEscItem;
        private System.Windows.Forms.ToolStripMenuItem powerOffToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem sendWinLButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripDropDownButton tsbFormat;
        private System.ComponentModel.BackgroundWorker bgFrameProcessing;
        private System.Windows.Forms.ToolStripButton tsbFullScreen;
    }
}

