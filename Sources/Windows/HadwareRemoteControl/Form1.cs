using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using OpenCvSharp;
//using OpenCvSharp.Extensions;
using AForge.Video.DirectShow;
using System.Management;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Concurrent;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using AForge.Video.DirectShow.Internals;
using Emgu.CV.Dai;

namespace HadwareRemoteControl
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        private const uint MAPVK_VK_TO_VSC = 0;

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "MapVirtualKey",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);


        //VideoCapture capture;
        //Mat frame;
        Bitmap image;
        bool Connected = false;
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoDevice;
        SerialPort serialPort;
        BlockingCollection<string> TextToEnter = new BlockingCollection<string>();
        int KeySleepTime = 20;
        Bitmap btScreen = null;
        Bitmap btSource = null;
        Point btScreenLocation = new Point();
        bool MouseCaptured = false;
        int[] Keymap = InitKeymap();
        Bitmap FrameInQueue = null;
        AutoResetEvent FrameWait = new AutoResetEvent(false);
        DateTime FrameTime = default;

        TransformationData transformationData = new();

        static XmlDocument Settings = new XmlDocument();

        static string GetAppPathCustom = "";
        public static string GetAppPath()
        {
            if (GetAppPathCustom.Length > 0)
            {
                return GetAppPathCustom;
            }
#if DEBUG
            return System.AppDomain.CurrentDomain.BaseDirectory;
#else
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
#endif
            //System.AppDomain.CurrentDomain.BaseDirectory; //Doesn't work with PublishSingleFile
        }

        static string _AppExeName = null;
        public static string GetAppExeName()
        {
            if (_AppExeName == null)
            {
                _AppExeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            }
            return _AppExeName;
        }




        class SettingsValue
        {
            string FileName = Path.Combine(GetAppPath(), Path.GetFileNameWithoutExtension(GetAppExeName()) + "_settings.xml");

            public void LoadSettings()
            {

                if (File.Exists(FileName))
                {
                    Settings.Load(FileName);
                }
                else
                {
                    Settings.AppendChild(Settings.CreateElement("root"));
                }
            }


            public void SaveSettings()
            {
                Settings.Save(FileName);
            }
            public string this[string name, string def = ""]
            {
                get { var root = ((XmlElement)Settings.SelectSingleNode("root")); if (root.HasAttribute(name)) return root.GetAttribute(name); return def; }
                set { ((XmlElement)Settings.SelectSingleNode("root")).SetAttribute(name, value); SaveSettings(); }
            }
        }

        SettingsValue settingsValue = new SettingsValue();

        public Form1()
        {
            InitializeComponent();
            pbScreen.MouseWheel += pbScreen_MouseWheel;
            this.MouseWheel += pbScreen_MouseWheel;
        }

        private void pbScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            SendSerial($"MW:{e.Delta / Math.Abs(e.Delta)}", 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            settingsValue.LoadSettings();
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count != 0)
            {
                string SelectedDevice = settingsValue["device"];
                // add all devices to combo
                foreach (FilterInfo device in videoDevices)
                {
                    //comboBox1.Items.Add(device.Name);       
                    ToolStripMenuItem btn = new ToolStripMenuItem($"{device.Name}");
                    btn.Image = tsbCameras.Image;
                    btn.Tag = device;
                    btn.Click += (sender, e) =>
                    {
                        if (sender is ToolStripMenuItem item)
                        {
                            tsbCameras.Tag = item.Tag;
                            tsbCameras.Text = item.Text;
                            settingsValue["device"] = item.Text;
                        }
                    };
                    if (tsbCameras.DropDownItems.Count == 0 || (SelectedDevice.Length > 0 && btn.Text == SelectedDevice))
                    {
                        tsbCameras.Text = btn.Text;
                        tsbCameras.Tag = btn.Tag;
                    }
                    tsbCameras.DropDownItems.Add(btn);
                }
            }

            string[] ports = SerialPort.GetPortNames();
            string SelectedPort = settingsValue["port"];
            foreach (var PortName in ports)
            {
                ToolStripMenuItem btn = new ToolStripMenuItem($"{PortName}");
                btn.Image = tsCom.Image;
                btn.Click += (sender, e) =>
                {
                    if (sender is ToolStripMenuItem item)
                    {
                        tsCom.Text = item.Text;
                        settingsValue["port"] = item.Text;
                    }
                };
                if (tsCom.DropDownItems.Count == 0 || (SelectedPort.Length > 0 && SelectedPort == btn.Text))
                {
                    tsCom.Text = btn.Text;
                }
                tsCom.DropDownItems.Add(btn);
            }

            Dictionary<string, Guid> Formats = new Dictionary<string, Guid> {
                { "Default", Guid.Empty },
                { "RGB24", MediaSubType.RGB24 },
                { "MJpeg", MediaSubType.MJpeg },
                { "NV12", MediaSubType.NV12 },
                { "YUY2", MediaSubType.YUY2 },
                { "YUYV", MediaSubType.YUYV },
                { "I420", MediaSubType.I420 },
            };
            string SelectedFormat = settingsValue["format"];
            foreach (var FormatPair in Formats)
            {
                ToolStripMenuItem btn = new ToolStripMenuItem($"{FormatPair.Key}");
                btn.Tag = FormatPair.Value;
                btn.Click += (sender, e) =>
                {
                    if (sender is ToolStripMenuItem item)
                    {
                        tsbFormat.Text = item.Text;
                        tsbFormat.Tag = item.Tag;
                        settingsValue["format"] = item.Text;
                    }
                };

                if (SelectedFormat == btn.Text)
                {
                    tsbFormat.Text = btn.Text;
                    tsbFormat.Tag = btn.Tag;
                }
                tsbFormat.DropDownItems.Add(btn);
            }


            if (settingsValue["screen"].ToInt32() == 1)
            {
                screenTypeToolStripMenuItem_Click(scrollToolStripMenuItem, new EventArgs());
            }

            transformationData.UseTransform = settingsValue["transform"].ToBool();
            for (int i = 0; i < 4; i++)
            {
                transformationData.destPoints[i].X = settingsValue[$"transformPoint{i}X"].ToFloat();
                transformationData.destPoints[i].Y = settingsValue[$"transformPoint{i}Y"].ToFloat();
            }

            bgFrameProcessing.RunWorkerAsync();
            bwEnterText.RunWorkerAsync();
        }

        private VideoCapabilities GetBestVideo(VideoCapabilities[] list)
        {
            int Value = 0;
            VideoCapabilities selected = list.Last();
            foreach (var cap in list)
            {
                int NewVal = cap.FrameSize.Width * cap.FrameSize.Height * cap.AverageFrameRate;
                if (NewVal > Value)
                {
                    selected = cap;
                    Value = NewVal;
                }
            }
            return selected;
        }

        private void DumpVideoCapabilities(VideoCapabilities[] list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var cap in list)
            {
                stringBuilder.AppendLine($"{cap.FrameSize.Width}x{cap.FrameSize.Height}r{cap.AverageFrameRate}");
            }
            File.WriteAllText("./videocap.txt", stringBuilder.ToString());
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (tsbCameras.Tag is FilterInfo fi)
            {
                videoDevice = new VideoCaptureDevice(fi.MonikerString);
                var videoCapabilities = videoDevice.VideoCapabilities;
                var snapshotCapabilities = videoDevice.SnapshotCapabilities;
                DateTime LastUpdate = DateTime.Now;
                videoDevice.NewFrame += (sender, e) =>
                {
                    lock (FrameWait)
                    {
                        FrameInQueue = (Bitmap)e.Frame.Clone();
                        FrameWait.Set();
                        FrameTime = DateTime.Now;
                    }
                    e.Frame.Dispose();
                    //Invoke(new Action(() =>
                    //{
                        
                        /*
                        //pbScreen.Image = 
                        if ((DateTime.Now - LastUpdate).TotalMilliseconds > 50 || (!transformationData.UseTransform && (DateTime.Now - LastUpdate).TotalMilliseconds > 25))
                        {
                            LastUpdate = DateTime.Now;
                            btSource = (Bitmap)e.Frame.Clone();
                            btScreen = transformationData.DoTransformation(btSource);
                            pbScreen.Refresh();
                        }
                        */
                    //}));
                    //tsDebug .Text = $"{pbScreen.Image.Width}";
                };
                videoDevice.VideoSourceError += (sender, e) =>
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show(this, e.Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                };
                videoDevice.VideoSourceMessage += (sender, e) =>
                {
                    Invoke(new Action(() =>
                    {
                        if (e.Description.StartsWith("Format:"))
                        {
                            tsDebugFormat.Text = e.Description.CutRight("Format: ");
                        }
                    }));
                };
                if (tsbFormat.Tag is Guid g)
                {
                    videoDevice.PreferEncoding = g;
                }
                else
                {
                    videoDevice.PreferEncoding = Guid.Empty;
                }
                tsDebugFormat.Text = "";
                //DumpVideoCapabilities(videoCapabilities);
                videoDevice.VideoResolution = GetBestVideo(videoCapabilities);
                videoDevice.Start();

                serialPort = new SerialPort();

                serialPort.PortName = tsCom.Text;
                serialPort.BaudRate = 115200;
                //serialPort.Parity = Parity.None;
                //serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
                //serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
                serialPort.Handshake = Handshake.None; //SetPortHandshake(_serialPort.Handshake);
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;

                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;

                serialPort.Open();

                /*
                new Thread((e) => {

                    serialPort.ReadTimeout = int.MaxValue;
                    try
                    {
                        while (true)
                        {


                            var line = serialPort.ReadLine();
                            Invoke(new Action(() =>
                            {
                                tsbDebugSerial.Text = line;
                            }));

                        }
                    }
                    catch (Exception ex)
                    {
                        int KKK = 0;
                    }

                }).Start();*/

                Connected = true;

                EnableButtons();
            }
        }

        void EnableButtons()
        {
            btnConnect.Enabled = !Connected;
            tsbCameras.Enabled = !Connected;
            tsbFormat.Enabled = !Connected;
            tsCom.Enabled = !Connected;
            btnDisconnect.Enabled = Connected;
            tsbOther.Enabled = Connected;
            tsDisplayType.Enabled = Connected;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (videoDevice != null)
            {
                videoDevice.SignalToStop();
                videoDevice.WaitForStop();
                videoDevice = null;
            }
            if (serialPort != null)
            {
                try
                {
                    serialPort.Close();
                }
                catch { }
                serialPort.Dispose();
                serialPort = null;
            }

            btScreen = null;
            pbScreen.Refresh();
            MouseCaptured = false;
            ShowCursor(true);
            Connected = false;

            EnableButtons();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                settingsValue["windowMaximized"] = WindowState == FormWindowState.Maximized ? "1" : "0";
                settingsValue["windowX"] = this.Left.ToString();
                settingsValue["windowY"] = this.Top.ToString();
                settingsValue["windowWidth"] = this.Width.ToString();
                settingsValue["windowHeight"] = this.Height.ToString();
            }
            Process.GetCurrentProcess().Kill();
        }

        private void SendSerial(string Value, int Delay)
        {
            if (Connected && serialPort != null)
            {
                lock (serialPort)
                {
                    try
                    {
                        DateTime start = DateTime.Now;
                        serialPort.WriteLine($"{Value}");
                        //tsDebug.Text = $"Send: {(DateTime.Now - start).TotalMilliseconds}";
                        if (Delay > 0)
                        {
                            Thread.Sleep(Delay);
                        }
                    }
                    catch { }
                }
            }
        }

        private static int[] InitKeymap()
        {
            int[] keysMap = new int[256];

            for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
            {
                keysMap[i] = i - (int)Keys.A + 0x04;
            }
            for (int i = '1'; i <= '9'; i++)
            {
                keysMap[i] = i - '1' + 0x1E;
            }
            keysMap['0'] = 0x27;

            for (int i = 112; i <= 123; i++)
            {
                keysMap[i] = i - 112 + 0x3A;
            }

            //KP 1-9
            for (int i = 97; i <= 105; i++)
            {
                keysMap[i] = i - 97 + 0x59;
            }
            keysMap[96] = 0x62; //KP 0
            /*
            if (key >= (int)Keys.A && key <= (int)Keys.Z)
            {
                return key - (int)Keys.A + 0x04;
            }
            if (key >= '1' && key <= '9')
            {
                return (key - '1' + 0x1E);
            }
            if (key >= 112 && key <= 123)
            {
                return key - 112 + 0x3A;
            }

            if (key >= 97 && key <= 105)
            {
                return key - 97 + 0x59;
            }
            */

            keysMap[8] = 0x2A;
            keysMap[9] = 0x2B;
            keysMap[13] = 0x28;
            keysMap[16] = 0xE1;
            keysMap[17] = 0xE0;
            keysMap[18] = 0xE2;
            keysMap[19] = 0x48; //Pause
            keysMap[20] = 0x2C;
            keysMap[27] = 0x29;
            keysMap[32] = 0x2C;

            keysMap[91] = 0xE3; //Win ???

            keysMap[106] = 0x55; //KP *
            keysMap[107] = 0x57; //KP +
            keysMap[109] = 0x56; //KP -
            keysMap[110] = 0x63; //KP .
            keysMap[111] = 0x54; //KP /

            keysMap[186] = 0x33; //;:
            keysMap[188] = 0x36; //,<
            keysMap[190] = 0x37; //.>
            keysMap[191] = 0x38; // /
            keysMap[192] = 0x35; //`

            keysMap[187] = 0x2E; //Equal
            keysMap[189] = 0x2D; //Minus
            keysMap[219] = 0x2F; //[
            keysMap[220] = 0x31; //\
            keysMap[221] = 0x30; //]
            keysMap[222] = 0x34; //'

            //Arrows
            keysMap[37] = 0x50;
            keysMap[38] = 0x52;
            keysMap[39] = 0x4F;
            keysMap[40] = 0x51;

            keysMap[33] = 0x4B; //PageUp
            keysMap[34] = 0x4E; //PageDown
            keysMap[35] = 0x4D; //End
            keysMap[36] = 0x4A; //Home
            keysMap[45] = 0x49; //Insert
            keysMap[46] = 0x4C; //Delete

            keysMap[144] = (int)HIDKeys.HID_KEY_NUM_LOCK; //Delete
            return keysMap;
        }

        private int TranslateKeyHID(int key)
        {
            return Keymap[key];
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            SendSerial($"KD:{TranslateKeyHID(e.KeyValue)}", 0);
            //tsDebug .Text = $"{e.KeyValue}";
            if (e.KeyCode != Keys.NumLock)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            SendSerial($"KU:{TranslateKeyHID(e.KeyValue)}", 0);
            if (e.KeyCode != Keys.NumLock)
            {
                e.SuppressKeyPress = true;
            }
        }


        private bool TranslateXY(MouseEventArgs e, out int X, out int Y)
        {
            X = 0;
            Y = 0;

            if (btScreen != null)
            {
                int ResWidth = 32768;
                int ResHeight = 32768;

                int Type = tsDisplayType.Tag.ToString().ToInt32();
                if (Type == 1)
                {
                    int FormX = e.X;
                    int FormY = e.Y;
                    int ScreenHeight = pbScreen.ClientSize.Height;
                    int ScreenWidth = pbScreen.ClientSize.Width;
                    if (btScreen.Width < ScreenWidth)
                    {
                        btScreenLocation.X = (ScreenWidth - btScreen.Width) / 2;
                    }
                    else
                    {
                        var ScrollSize = btScreen.Width - ScreenWidth;
                        var ScrollPos = FormX / (double)ScreenWidth;
                        btScreenLocation.X = (int)(-ScrollSize * ScrollPos);
                    }

                    if (btScreen.Height < ScreenHeight)
                    {
                        btScreenLocation.Y = (ScreenHeight - btScreen.Height) / 2;
                    }
                    else
                    {
                        var ScrollSize = btScreen.Height - ScreenHeight;
                        var ScrollPos = FormY / (double)ScreenHeight;
                        btScreenLocation.Y = (int)(-ScrollSize * ScrollPos);
                    }
                    pbScreen.Refresh();
                    X = (int)((e.X - btScreenLocation.X) * (ResWidth / (double)btScreen.Width));
                    Y = (int)((e.Y - btScreenLocation.Y) * (ResHeight / (double)btScreen.Height));
                    return true;
                }

                int ImageX = 0;
                int ImageY = 0;
                int ImageWidth = pbScreen.ClientSize.Width;
                int ImageHeight = pbScreen.ClientSize.Height;
                int PropHeight = (int)(btScreen.Height / (btScreen.Width / (double)pbScreen.ClientSize.Width));
                if (PropHeight > pbScreen.ClientSize.Height)
                {
                    int PropWidth = (int)(btScreen.Width / (btScreen.Height / (double)pbScreen.ClientSize.Height));
                    ImageX = (pbScreen.ClientSize.Width - PropWidth) / 2;
                    ImageWidth = PropWidth;
                }
                else
                {
                    ImageY = (pbScreen.ClientSize.Height - PropHeight) / 2;
                    ImageHeight = PropHeight;
                }


                if (ResWidth > 0 && ResHeight > 0 && e.X >= ImageX && e.Y >= ImageY)
                {
                    X = (int)((e.X - ImageX) * (ResWidth / (double)ImageWidth));
                    Y = (int)((e.Y - ImageY) * (ResHeight / (double)ImageHeight));
                    if (X <= ResWidth && Y <= ResHeight)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void pbScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (Connected)
            {
                if (TranslateXY(e, out var X, out var Y))
                {
                    SendSerial($"MM:{X},{Y}", 0);
                }
            }
        }

        int ConvertMouseButton(MouseButtons Button)
        {
            switch (Button)
            {
                case MouseButtons.Left:
                    return 1;
                case MouseButtons.Right:
                    return 2;
                case MouseButtons.Middle:
                    return 4;
                default:
                    return 0;
            }
        }

        private void pbScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (Connected)
            {
                if (TranslateXY(e, out var X, out var Y))
                {
                    SendSerial($"MD:{ConvertMouseButton(e.Button)},{X},{Y}", 0);
                }
            }
        }

        private void pbScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (Connected)
            {
                if (TranslateXY(e, out var X, out var Y))
                {
                    SendSerial($"MU:{ConvertMouseButton(e.Button)},{X},{Y}", 0);
                }
            }
        }

        private void tsbReboot_Click(object sender, EventArgs e)
        {
            SendSerial($"RB:", 0);
        }

        private void ConvertCharToVirtualKey(char ch, out byte key, out bool shift)
        {
            short vkey = VkKeyScan(ch);
            key = (byte)(vkey & 0xff);
            int modifiers = vkey >> 8;

            shift = ((modifiers & 1) != 0);
        }

        private void bwEnterText_DoWork(object sender, DoWorkEventArgs e)
        {

            while (true)
            {
                try
                {
                    bool Shift = false;
                    bool SecondaryLang = false;
                    string Text = TextToEnter.Take();
                    var HChars = HIDKeysTranslate.TextToHIDChars(Text);
                    foreach (var hchar in HChars)
                    {
                        if (hchar.lang && !SecondaryLang || !hchar.lang && SecondaryLang)
                        {
                            if (Shift)
                            {
                                SendSerial($"KU:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", KeySleepTime);
                                Shift = false;
                            }

                            //Switch lang
                            SendSerial($"KD:{(int)HIDKeys.HID_KEY_GUI_LEFT}", KeySleepTime);
                            SendSerial($"KD:{(int)HIDKeys.HID_KEY_SPACE}", KeySleepTime);
                            SendSerial($"KU:{(int)HIDKeys.HID_KEY_SPACE}", KeySleepTime);
                            SendSerial($"KU:{(int)HIDKeys.HID_KEY_GUI_LEFT}", KeySleepTime);
                            SecondaryLang = hchar.lang;
                        }

                        if (hchar.shift && !Shift)
                        {
                            SendSerial($"KD:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", KeySleepTime);
                            Shift = true;
                        }
                        else if (!hchar.shift && Shift)
                        {
                            SendSerial($"KU:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", KeySleepTime);
                            Shift = false;
                        }

                        SendSerial($"KP:{(int)hchar.key},{KeySleepTime}", KeySleepTime);
                    }

                    if (Shift)
                    {
                        SendSerial($"KU:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", KeySleepTime);
                        Shift = false;
                    }
                    if (SecondaryLang)
                    {

                        SendSerial($"KD:{(int)HIDKeys.HID_KEY_GUI_LEFT}", KeySleepTime);
                        SendSerial($"KD:{(int)HIDKeys.HID_KEY_SPACE}", KeySleepTime);
                        SendSerial($"KU:{(int)HIDKeys.HID_KEY_SPACE}", KeySleepTime);
                        SendSerial($"KU:{(int)HIDKeys.HID_KEY_GUI_LEFT}", KeySleepTime);
                        SecondaryLang = false;
                    }
                }
                catch { }
            }
        }

        private void pasteClipboardTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var Text = Clipboard.GetText();

                TextToEnter.Add(Text);
                //TextToEnter.CompleteAdding();
            }
            catch { }
        }

        private void sendAltCtrlDelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendSerial($"KD:{TranslateKeyHID(18)}", 0);
            SendSerial($"KD:{TranslateKeyHID(17)}", 0);
            SendSerial($"KD:{TranslateKeyHID(46)}", KeySleepTime);
            SendSerial($"KU:{TranslateKeyHID(46)}", 0);
            SendSerial($"KU:{TranslateKeyHID(17)}", 0);
            SendSerial($"KU:{TranslateKeyHID(18)}", KeySleepTime);
        }

        private void screenTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                if (tsDisplayType.Tag != item.Tag)
                {
                    tsDisplayType.Tag = item.Tag;
                    tsDisplayType.Text = item.Text;
                    tsDisplayType.Image = item.Image;

                    settingsValue["screen"] = item.Tag.ToString();

                    pbScreen.Refresh();
                }
            }
        }

        private void pbScreen_Paint(object sender, PaintEventArgs e)
        {
            if (btScreen != null)
            {
                int Type = tsDisplayType.Tag.ToString().ToInt32();
                if (Type == 0)
                {
                    int ImageX = 0;
                    int ImageY = 0;
                    int ImageWidth = pbScreen.ClientSize.Width;
                    int ImageHeight = pbScreen.ClientSize.Height;
                    int PropHeight = (int)(btScreen.Height / (btScreen.Width / (double)pbScreen.ClientSize.Width));
                    if (PropHeight > pbScreen.ClientSize.Height)
                    {
                        int PropWidth = (int)(btScreen.Width / (btScreen.Height / (double)pbScreen.ClientSize.Height));
                        ImageX = (pbScreen.ClientSize.Width - PropWidth) / 2;
                        ImageWidth = PropWidth;
                    }
                    else
                    {
                        ImageY = (pbScreen.ClientSize.Height - PropHeight) / 2;
                        ImageHeight = PropHeight;
                    }
                    e.Graphics.DrawImage(btScreen, new Rectangle(ImageX, ImageY, ImageWidth, ImageHeight));
                }
                if (Type == 1)
                {
                    e.Graphics.DrawImage(btScreen, btScreenLocation);
                }
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(pbScreen.BackColor), pbScreen.ClientRectangle);
            }
        }

        private void transformImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImageTransform.ShowForm(this, transformationData, (Bitmap)btSource.Clone());
            settingsValue["transform"] = transformationData.UseTransform ? "1" : "0";
            for (int i = 0; i < 4; i++)
            {
                settingsValue[$"transformPoint{i}X"] = transformationData.destPoints[i].X.ToString();
                settingsValue[$"transformPoint{i}Y"] = transformationData.destPoints[i].Y.ToString();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Left = settingsValue["windowX", this.Left.ToString()].ToInt32();
            this.Top = settingsValue["windowY", this.Top.ToString()].ToInt32();
            this.Width = settingsValue["windowWidth", this.Width.ToString()].ToInt32();
            this.Height = settingsValue["windowHeight", this.Height.ToString()].ToInt32();

            if (settingsValue["windowMaximized"].ToBool())
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void sencCtrlEscItem_Click(object sender, EventArgs e)
        {
            SendSerial($"KD:{(int)HIDKeys.HID_KEY_CONTROL_LEFT}", 0);
            SendSerial($"KD:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", 0);
            SendSerial($"KD:{(int)HIDKeys.HID_KEY_ESCAPE}", KeySleepTime);
            SendSerial($"KU:{(int)HIDKeys.HID_KEY_ESCAPE}", 0);
            SendSerial($"KU:{(int)HIDKeys.HID_KEY_SHIFT_LEFT}", 0);
            SendSerial($"KU:{(int)HIDKeys.HID_KEY_CONTROL_LEFT}", KeySleepTime);
        }

        private void powerOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendSerial($"PW:", 0);
        }

        private void sendWinLButton_Click(object sender, EventArgs e)
        {
            SendSerial($"KD:{(int)HIDKeys.HID_KEY_GUI_LEFT}", 0);
            SendSerial($"KD:{(int)HIDKeys.HID_KEY_L}", KeySleepTime);
            SendSerial($"KU:{(int)HIDKeys.HID_KEY_L}", 0);
            SendSerial($"KU:{(int)HIDKeys.HID_KEY_GUI_LEFT}", KeySleepTime);
        }

        private void bgFrameProcessing_DoWork(object sender, DoWorkEventArgs e)
        {
            while (FrameWait.WaitOne())
            {
                Bitmap inFrame = null;
                lock (FrameWait)
                {
                    inFrame = FrameInQueue;
                    FrameInQueue = null;
                }

                Invoke(new Action(() =>
                {
                    if (btSource != null)
                    {
                        btSource.Dispose();
                        btSource = null;
                    }
                    btSource = inFrame;
                    if (btScreen != null)
                    {
                        btScreen.Dispose();
                        btScreen = null;
                    }
                    btScreen = transformationData.DoTransformation(btSource);
                    pbScreen.Refresh();
                }));
                //Debug.Print($"FrameTime: {(DateTime.Now - FrameTime).TotalMilliseconds}ms\r\n");
            }
        }
    }
}
