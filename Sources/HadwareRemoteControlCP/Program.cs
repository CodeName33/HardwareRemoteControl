using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using Gtk;
using OpenCvSharp;
using OpenCvSharp.Flann;
//using GLib;
using HadwareRemoteControlCP;
using System.Xml;
using Gtk;
using System.Runtime.InteropServices;
using Cairo;
using Gdk;
using System.Net;
using System.Text;
using System.Collections.Concurrent;
using System.Linq.Expressions;


namespace hardware_remote_control_cp
{
    static class Program
    {
        static SerialPort serialPort;

        static List<string> GetAvailableWebcams(bool ffmpegMode)
        {
            var availableIndices = new List<string>();

            if (ffmpegMode && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-list_devices true -f dshow -i dummy -hide_banner",
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();

                while (true)
                {
                    var line = process.StandardError.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    if (line.Contains("(video)"))
                    {
                        line = line.CutLeft("(video)");
                        line = line.CutRight("]").Trim();
                        availableIndices.Add(line);
                    }
                }
            }
            else
            {
                for (int index = 0; index < 16; index++)
                {
                    using (var capture = new VideoCapture(index))
                    {
                        if (capture.IsOpened())
                        {
                            availableIndices.Add($"{index}");
                            capture.Release();
                        }
                    }
                }
            }

            return availableIndices;
        }

        public static int STR2INT_EX(string s)
		{
			int value = 0;
			int len = s.Length;
			if (s != null)
			{
				if (len > 0)
				{
					if (s[0] == '-')
					{
						for (int i = 1; i < len; i++)
						{
							if (s[i] < '0' || s[i] > '9')
							{
								break;
							}
							value = value * 10 + (s[i] - '0');
						}
						return -value;
					}
					else
					{
						for (int i = 0; i < len; i++)
						{
							if (s[i] < '0' || s[i] > '9')
							{
								break;
							}
							value = value * 10 + (s[i] - '0');
						}
						return value;
					}
				}
			}
			return 0;
		}

        static string _AppExeName = null;
        public static string GetAppExeName()
        {
            if (_AppExeName == null)
            {
                _AppExeName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            }
            return _AppExeName;
        }

        public static string GetAppPath()
        {
#if DEBUG
            return System.AppDomain.CurrentDomain.BaseDirectory;
#else
            return System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
#endif
            //System.AppDomain.CurrentDomain.BaseDirectory; //Doesn't work with PublishSingleFile
        }

        public static void InitDynamic<
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)] 
            T>()
        {
            typeof(T).GetFields();
            typeof(T).GetMethods();
        }

        public static void InitDynamicGtk()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string GTKDir = System.IO.Path.Combine(GetAppPath(), "gtk");
                if (Directory.Exists(GTKDir))
                {
                    Console.WriteLine($"GTK: {GTKDir}");
                    Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + GTKDir);
                }
            }
            InitDynamic<Gdk.Screen>();
            InitDynamic<GLib.Value>();
            InitDynamic<GLib.GType>();
            InitDynamic<GLib.Object>();
            InitDynamic<PolicyType>();
            InitDynamic<ButtonPressEventArgs>();
            InitDynamic<ButtonReleaseEventArgs>();
            InitDynamic<DeleteEventArgs>();
            InitDynamic<RowExpandedArgs>();
            InitDynamic<RowCollapsedArgs>();
            InitDynamic<Gdk.Pixbuf>();
            InitDynamic<CellRendererPixbuf>();
            InitDynamic<CellRendererText>();
            InitDynamic<Gdk.Event>();
            InitDynamic<Gdk.Atom>(); 
            InitDynamic<Clipboard>();
            InitDynamic<XmlElement>();
            InitDynamic<TreePath>();
            InitDynamic<TreeIter>();
            InitDynamic<TreeStore>();
            InitDynamic<CellRenderer>();
            InitDynamic<TreeSelection>();
            InitDynamic<KeyReleaseEventArgs>();
            InitDynamic<ListStore>();
            InitDynamic<Box>();
            InitDynamic<Entry>();
            InitDynamic<ComboBox>();
            InitDynamic<Button>();
            InitDynamic<FocusOutEventArgs>();
            InitDynamic<TextBuffer>();
            InitDynamic<DrawingArea>();
            InitDynamic<Cairo.Surface>();
            InitDynamic<Cairo.Context>();
            InitDynamic<MotionNotifyEventArgs>();
            InitDynamic<KeyPressEventArgs>();
            InitDynamic<DeleteEventArgs>();
        }

        public static int ToInt32(this string Text)
        {
            if (Text != null)
            { 
                return STR2INT_EX(Text);
            }
            return 0;
        }

        public static void SplitTwo(this string Text, string Delimiter, out string Left, out string Right)
        {
            var parts = Text.Split(Delimiter, 2);
            Left = parts[0];
            if (parts.Length > 1)
            {
                Right = parts[1];
            }
            else
            {
                Right = "";
            }
        }

        public static void SplitTwoTrimed(this string Text, string Delimiter, out string Left, out string Right)
        {
            var parts = Text.Split(Delimiter, 2);
            Left = parts[0].Trim();
            if (parts.Length > 1)
            {
                Right = parts[1].Trim();
            }
            else
            {
                Right = "";
            }
        }

        public static string CutLeft(this string Text, string Delimiter)
        {
            return Text.Split(Delimiter, 2)[0];
        }

        public static string CutRight(this string Text, string Delimiter)
        {
            return Text.Split(Delimiter, 2).Last();
        }

        private static void SendSerial(string Value, int Delay)
        {
#if DEBUG
            //return;
#endif
            if (serialPort != null)
            {
                lock (serialPort)
                {
                    try
                    {
                        System.DateTime start = System.DateTime.Now;
                        serialPort.WriteLine($"{Value}");
                        //Console.WriteLine($"SendSerial: {Value}");
                        //tsDebug.Text = $"Send: {(DateTime.Now - start).TotalMilliseconds}";
                        if (Delay > 0)
                        {
                            System.Threading.Thread.Sleep(Delay);
                        }
                    }
                    catch { }
                }
            }
        }

        static void OnMouseCallback(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData)
        {
            int button = 0;
            if ((flags & MouseEventFlags.LButton) == MouseEventFlags.LButton)
            {
                button = 1;
            }
            else if ((flags & MouseEventFlags.RButton) == MouseEventFlags.RButton)
            {
                button = 2;
            }
            else if ((flags & MouseEventFlags.MButton) == MouseEventFlags.MButton)
            {
                button = 4;
            }

            x = (int)((x / 1920.0) * 32768);
            y = (int)((y / 1080.0) * 32768);

            switch (@event)
            {
                case MouseEventTypes.MouseMove:
                    SendSerial($"MM:{x},{y}", 0);
                    return;
                case MouseEventTypes.LButtonDoubleClick:
                    SendSerial($"MD:1,{x},{y}", 50);
                    SendSerial($"MU:1,{x},{y}", 50);
                    SendSerial($"MD:1,{x},{y}", 50);
                    SendSerial($"MU:1,{x},{y}", 50);
                    return;
                case MouseEventTypes.RButtonDoubleClick:
                    SendSerial($"MD:2,{x},{y}", 50);
                    SendSerial($"MU:2,{x},{y}", 50);
                    SendSerial($"MD:2,{x},{y}", 50);
                    SendSerial($"MU:2,{x},{y}", 50);
                    return;
                case MouseEventTypes.MButtonDoubleClick:
                    SendSerial($"MD:4,{x},{y}", 50);
                    SendSerial($"MU:4,{x},{y}", 50);
                    SendSerial($"MD:4,{x},{y}", 50);
                    SendSerial($"MU:4,{x},{y}", 50);
                    return;
                case MouseEventTypes.LButtonDown:
                    SendSerial($"MD:1,{x},{y}", 0);
                    return;
                case MouseEventTypes.MButtonDown:
                    SendSerial($"MD:4,{x},{y}", 0);
                    return;
                case MouseEventTypes.RButtonDown:
                    SendSerial($"MD:2,{x},{y}", 0);
                    return;
                case MouseEventTypes.LButtonUp:
                    SendSerial($"MU:1,{x},{y}", 0);
                    return;
                case MouseEventTypes.MButtonUp:
                    SendSerial($"MU:4,{x},{y}", 0);
                    return;
                case MouseEventTypes.RButtonUp:
                    SendSerial($"MU:2,{x},{y}", 0);
                    return;
                case MouseEventTypes.MouseWheel:
                    var delta = Cv2.GetMouseWheelDelta(flags);
                    SendSerial($"MW:{delta / Math.Abs(delta)}", 0);
                    return;
            }
            //Console.WriteLine($"{@event} : {x}x{y} {flags},{userData}");
        }

        static void OnKeyPress(int Key)
        {
            var HKeys = HID.GetHIDKeys(Key);
            foreach (var HKey in HKeys)
            {
                SendSerial($"KD:{(int)HKey}", 0);
            }
            foreach (var HKey in HKeys.Reverse<HID.HIDKeys>())
            {
                SendSerial($"KU:{(int)HKey}", 0);
            }
        }

        class FrameBuffMat
        {
            object locker = new object();
            Mat? data = null;
            long counter = 0;
            ManualResetEvent pushEvent = new ManualResetEvent(false);
            object getLocker = new object();

            public void addFrame(Mat frame)
            {
                lock (locker)
                {
                    if (data != null)
                    {
                        data.Dispose();
                    }
                    data = frame;
                    counter++;
                    pushEvent.Set();
                }
            }

            public Mat? getFrameWait(ref long c)
            {
                lock (locker)
                {
                    if (c != counter)
                    {
                        c = counter;
                        return data?.Clone();
                    }
                }

                lock (getLocker)
                {
                    lock (locker)
                    {
                        if (c != counter)
                        {
                            c = counter;
                            return data?.Clone();
                        }
                    }

                    while (pushEvent.WaitOne())
                    {
                        lock (locker)
                        {
                            pushEvent.Reset();
                            if (c != counter)
                            {
                                c = counter;
                                return data?.Clone();
                            }
                        }
                    }
                }
                return null;
            }
        }

        class FrameBuff<T> where T : class
        {
            object locker = new object();
            T? data = null;
            long counter = 0;
            ManualResetEvent pushEvent = new ManualResetEvent(false);
            object getLocker = new object();

            public void addFrame(T frame)
            {
                lock (locker)
                {
                    data = frame;
                    counter++;
                    pushEvent.Set();
                }
            }

            public T? getFrameWait(ref long c)
            {
                lock (locker)
                {
                    if (c != counter)
                    {
                        c = counter;
                        return data;
                    }
                }

                lock (getLocker)
                {
                    lock (locker)
                    {
                        if (c != counter)
                        {
                            c = counter;
                            return data;
                        }
                    }

                    while (pushEvent.WaitOne())
                    {
                        lock (locker)
                        {
                            pushEvent.Reset();
                            if (c != counter)
                            {
                                c = counter;
                                return data;
                            }
                        }
                    }
                }
                return default;
            }
        }

        class FrameEnc
        {
            public Mat frame;
            public byte[] result = { };
        }

        class VideoParameters
        {
            public int Width = 1920;
            public int Height = 1080;
            public int outputFPS = -1;
            public int FPS = 30;
            public bool Recode = false;
            public string FourCC = "";
            public int BufferSize = -1;
            public int Quality = 60;

            public int OutputFPS => (outputFPS == -1 ? FPS : outputFPS );

        }

        static class Status
        {
            static DateTime lastUpdate = default;
            static object locker = new object();
            static string[] Data = {  "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };


            public static void ColorPrint(object text, ConsoleColor color)
            {
                var c = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write($"{text}");
                Console.ForegroundColor = c;
            }

            public static void SetValue(int index, object value)
            {
                lock (locker)
                {
                    Data[index] = $"{value}";
                }
            }

            public static void Update()
            {
                lock (locker)
                {
                    if ((DateTime.Now - lastUpdate).TotalSeconds > 1)
                    {
                        lastUpdate = DateTime.Now;
                        Console.Write($"\r{"".PadLeft(Console.BufferWidth - 1, ' ')}\r");
                        ColorPrint("Status: ", ConsoleColor.Cyan);

                        var sb = new StringBuilder();
                        bool added = false;
                        foreach (var item in Data)
                        {
                            if (item.Length > 0)
                            {
                                if (added)
                                {
                                    sb.Append(", ");
                                }
                                sb.Append(item.ToString());
                                added = true;
                            }
                        }

                        ColorPrint(sb.ToString(), ConsoleColor.Magenta);
                    }
                }
            }
        }



        static void WebServerMode(string camIndex, int webPort, VideoParameters videoParameters, bool ffmpegMode)
        {
            
            VideoCapture? capture = null;

            if (ffmpegMode)
            {
                Console.WriteLine($"FFMpegMode ({camIndex})");
            }
            else
            {
                Console.Write($"Video Device Init ({camIndex})...");
                capture = new VideoCapture(camIndex);
                if (videoParameters.Width > 0)
                {
                    capture.FrameWidth = videoParameters.Width;
                }
                if (videoParameters.Height > 0)
                {
                    capture.FrameHeight = videoParameters.Height;
                }
                if (videoParameters.FourCC.Length > 0)
                {
                    capture.FourCC = videoParameters.FourCC;
                }
                if (videoParameters.FPS > 0)
                {
                    capture.Fps = videoParameters.FPS;
                }
                if (videoParameters.BufferSize >= 0)
                {
                    capture.BufferSize = videoParameters.BufferSize;
                }
                Console.WriteLine(" OK");
            }
            var running = true;

            string ListenAddress = $"http://*:{webPort}/";

            var listener = new HttpListener();
            listener.Prefixes.Add(ListenAddress);
            listener.Start();

            Console.WriteLine($"WebServer at http://*:{webPort}/");
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => { running = false; };

            object clientLocker = new object();
            int clientCount = 0;
            FrameBuffMat matBuff = new FrameBuffMat();
            FrameBuff<byte[]> rawBuff = new FrameBuff<byte[]>();


            if (ffmpegMode)
            {
                Console.WriteLine($"Capture properties:\n\tSourceFPS = {videoParameters.FPS}");
            }
            else
            {
                Console.WriteLine($"Capture properties:\n\tSourceFPS = {capture.Fps}\n\tBufferSize = {capture.BufferSize}\n\tFourCC = {capture.FourCC}");
            }
            ImageEncodingParam[] ep = { new ImageEncodingParam(ImwriteFlags.JpegQuality, videoParameters.Quality) };

            if (ffmpegMode)
            {
                new Thread(() =>
                {
                    var recodeArgs = "-c:v copy";
                    if (videoParameters.Recode)
                    {
                        recodeArgs = $"-vcodec mjpeg -q:v {31 - (int)(videoParameters.Quality * 0.31)}";
                    }
                    var arguments = $"-f v4l2 { (videoParameters.Recode ? "" : "-input_format mjpeg") } -video_size {videoParameters.Width}x{videoParameters.Height} -framerate {videoParameters.FPS} -i /dev/video{camIndex} -tune zerolatency {recodeArgs} -r {videoParameters.OutputFPS} -f mjpeg -";
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        arguments = $"-f dshow {(videoParameters.Recode ? "" : "-vcodec mjpeg") } -video_size {videoParameters.Width}x{videoParameters.Height} -framerate {videoParameters.FPS} -i \"video={camIndex}\" -tune zerolatency {recodeArgs} -r {videoParameters.OutputFPS} -f mjpeg -";

                        File.WriteAllText(System.IO.Path.Combine(GetAppPath(), "ffmpeg.run.txt"), $"ffmpeg {arguments}");
                    }

                    while (true)
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "ffmpeg",
                                Arguments = arguments,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };

                        process.Start();
                        Console.WriteLine($"FFMpeg started");

                        var ebuffer = new byte[4096];
                        var buffer = new byte[8192];
                        var ms = new MemoryStream();
                        bool isRecording = false;
                        byte lb = 0;
                        long FrameNumber = 0;
                        new Thread(() =>
                        {
                            try
                            {
                                while (true)
                                {
                                    var bytesRead = process.StandardError.BaseStream.Read(ebuffer, 0, ebuffer.Length);
                                    if (bytesRead == 0)
                                    {
                                        break;
                                    }
                                    //Thread.Sleep(100);
                                }
                            }
                            catch { }
                        }).Start();

                        //*
                        while (true)
                        {
                            var bytesRead = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                            //*
                            if (bytesRead == 0)
                            {
                                break;
                            }

                            //foreach (var b in buffer)
                            for (int i = 0; i < bytesRead; i++)
                            {
                                var b = buffer[i];
                                if (b == 0xD8)
                                {
                                    if (lb == 0xFF)
                                    {
                                        ms.WriteByte(lb);
                                        isRecording = true;
                                    }
                                }
                                else if (b == 0xD9)
                                {
                                    if (lb == 0xFF)
                                    {
                                        ms.WriteByte(b);
                                        isRecording = false;
                                        byte[] imageBytes = ms.ToArray();
                                        ms.SetLength(0);

                                        if (imageBytes.Length > 4)
                                        {
                                            rawBuff.addFrame(imageBytes);
                                            FrameNumber++;
                                            Status.SetValue(0, $"Source Frames = {FrameNumber}");
                                            Status.Update();
                                        }
                                        //Console.WriteLine($"Frame: {imageBytes.Length} bytes");
                                    }
                                }

                                if (isRecording)
                                {
                                    ms.WriteByte(b);
                                }
                                lb = b;
                            }//*/
                        }//*/

                        process.WaitForExit();
                        Console.WriteLine();
                        Console.WriteLine($"FFMpeg exited. Waiting 1 sec...");
                        Thread.Sleep(1000);
                        Console.WriteLine($"Restarting FFMpeg");
                    }
                }).Start();
            }
            else
            {
                new Thread(() =>
                {

                    long[] Measures = new long[10];
                    long MeasuresCount = 0;

                    var updateBegin = DateTime.Now;


                    while (running)
                    {
                        if (clientCount > 0)
                        {
                            capture.Fps = 30;
                            var frame = new Mat();
                            if (capture.Read(frame))
                            {
                                matBuff.addFrame(frame);

                                MeasuresCount++;
                            }

                            Status.SetValue(0, $"Source Frames = {MeasuresCount}");
                            Status.Update();
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }

                    }
                }).Start();
            }

            while (running)
            {
                var context = listener.GetContext();

                if (context == null || context.Request.Url == null)
                {
                    break;
                }

                var thread = new Thread(() =>
                {
                switch (context.Request.Url.LocalPath)
                {
                    case "/input":
                        {
                            try
                            {
                                using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
                                var lines = reader.ReadToEnd().Split('&');
                                foreach (var line in lines)
                                {
                                    SendSerial(line, 0);
                                }

                                context.Response.ContentType = "text/html";
                                context.Response.OutputStream.Write(Encoding.UTF8.GetBytes("OK"));
                                context.Response.Close();
                            }
                            catch { }
                            break;
                        }
                    case "/changes":
                        {
                            context.Response.Headers.Add("Content-Type", "application/octet-stream");
                            context.Response.Headers.Add("Transfer-Encoding", "chunked");
                            var responseStream = context.Response.OutputStream;
                            var writer = new StreamWriter(responseStream);
                            Mat? lImage = null;

                            lock (clientLocker)
                            {
                                clientCount++;
                            }

                            var lastSend = DateTime.Now;
                            long c = 0;
                            while (true)
                            {
                                MemoryStream partsStream = new MemoryStream();
                                BinaryWriter partsWriter = new BinaryWriter(partsStream);
                                try
                                {
                                    var image = matBuff.getFrameWait(ref c);
                                    if (image != null)
                                    {
                                        int ChangesCount = 0;
                                        var timeBegin = DateTime.Now;
                                        DateTime timeGet = DateTime.Now;
                                        if (lImage != null)
                                        {
                                            int partSize = 128;
                                            Vec3b[] partArray = new Vec3b[partSize * partSize];
                                            //using var partImage = new Mat(new OpenCvSharp.Size(partSize, partSize), MatType.CV_8UC3);


                                            int MaxChanges = (image.Cols / partSize) * (image.Rows / partSize);

                                            image.GetArray(out Vec3b[] pixels);
                                            lImage.GetArray(out Vec3b[] lPixels);
                                            timeGet = DateTime.Now;

                                            List<Tuple<int, int, Vec3b[]>> parts = new List<Tuple<int, int, Vec3b[]>>();

                                            unsafe
                                                {

                                                    fixed (Vec3b* pixelsPtr = pixels)
                                                    {
                                                        fixed (Vec3b* lPixelsPtr = lPixels)
                                                        {
                                                            int pos = 0;
                                                            for (int y = 0; y < image.Rows; y += partSize)
                                                            {
                                                                for (int x = 0; x < image.Cols; x += partSize)
                                                                {

                                                                    int xSize = x + partSize > image.Cols ? image.Cols : x + partSize;
                                                                    int ySize = y + partSize > image.Rows ? image.Rows : y + partSize;
                                                                    bool diff = false;
                                                                    int xW = xSize - x;
                                                                    fixed (Vec3b* partArrayPtr = partArray)
                                                                    {

                                                                        for (int yy = y, yn = 0; yy < ySize; yy++, yn++)
                                                                        {
                                                                            pos = yy * image.Cols + x;

                                                                            Vec3b* partArrayLinePtr = partArrayPtr + yn * partSize;
                                                                            Vec3b* pixelsLinePtr = pixelsPtr + pos;
                                                                            Vec3b* pixelsLineMaxPtr = pixelsLinePtr + xW;
                                                                            Vec3b* lPixelsLinePtr = lPixelsPtr + pos;

                                                                            if (!diff)
                                                                            {
                                                                                while (pixelsLinePtr < pixelsLineMaxPtr)
                                                                                {
                                                                                    Vec3b pixel = *pixelsLinePtr++;
                                                                                    Vec3b lPixel = *lPixelsLinePtr++;

                                                                                    *partArrayLinePtr++ = pixel;

                                                                                    if (!pixel.Equals(lPixel))
                                                                                    {
                                                                                        diff = true;
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                            while (pixelsLinePtr < pixelsLineMaxPtr)
                                                                            {
                                                                                *partArrayLinePtr++ = *pixelsLinePtr++;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (diff)
                                                                    {
                                                                        //var part = new Mat(new OpenCvSharp.Size(partSize, partSize), MatType.CV_8UC3);
                                                                        //part.SetArray(partArray);
                                                                        parts.Add(new Tuple<int, int, Vec3b[]>(x, y, partArray));
                                                                        partArray = new Vec3b[partSize * partSize];

                                                                        /*
                                                                        var imageBytes = partImage.ToBytes(".jpg", ep);

                                                                        partsWriter.Write((int)1);
                                                                        partsWriter.Write(x);
                                                                        partsWriter.Write(y);
                                                                        partsWriter.Write(partSize);
                                                                        partsWriter.Write(partSize);
                                                                        //var imageBytes = partImage.ToBytes(".bmp");
                                                                        partsWriter.Write((int)imageBytes.Length);
                                                                        partsWriter.Write(imageBytes);
                                                                        */
                                                                        ChangesCount++;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (ChangesCount > MaxChanges * 0.5)
                                                {
                                                    partsStream.Dispose();
                                                    partsStream = new MemoryStream();
                                                    partsWriter = new BinaryWriter(partsStream);
                                                    partsWriter.Write((int)1);
                                                    partsWriter.Write(0);
                                                    partsWriter.Write(0);
                                                    partsWriter.Write(image.Cols);
                                                    partsWriter.Write(image.Rows);
                                                    var imageBytes = image.ToBytes(".jpg", ep);
                                                    //var imageBytes = image.ToBytes(".bmp");
                                                    partsWriter.Write((int)imageBytes.Length);
                                                    partsWriter.Write(imageBytes);
                                                }
                                                else
                                                {
                                                    foreach (var p in parts)
                                                    {
                                                        using var part = new Mat(new OpenCvSharp.Size(partSize, partSize), MatType.CV_8UC3);
                                                        part.SetArray(p.Item3);
                                                        var imageBytes = part.ToBytes(".jpg", ep);

                                                        partsWriter.Write((int)1);
                                                        partsWriter.Write(p.Item1);
                                                        partsWriter.Write(p.Item2);
                                                        partsWriter.Write(partSize);
                                                        partsWriter.Write(partSize);
                                                        //var imageBytes = partImage.ToBytes(".bmp");
                                                        partsWriter.Write((int)imageBytes.Length);
                                                        partsWriter.Write(imageBytes);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                partsWriter.Write((int)0);
                                                partsWriter.Write(image.Cols);
                                                partsWriter.Write(image.Rows);
                                                var imageBytes = image.ToBytes(".jpg", ep);
                                                //var imageBytes = image.ToBytes(".bmp");
                                                partsWriter.Write((int)imageBytes.Length);
                                                partsWriter.Write(imageBytes);
                                            }


                                            Status.SetValue(6, $"Frame Process Time = {(int)(DateTime.Now - timeBegin).TotalMilliseconds}ms");
                                            Status.Update();

                                            lImage?.Dispose();
                                            lImage = image;
                                            //byte[] buffer = { 1, 2, 3, 4, 255 };
                                            //context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                                            if (partsStream.Length == 0)
                                            {
                                                if ((DateTime.Now - lastSend).TotalSeconds < 1)
                                                {
                                                    continue;
                                                }
                                                partsWriter.Write((int)255);
                                            }
                                            //Console.WriteLine($"Done: {(DateTime.Now - timeBegin).TotalMilliseconds}");
                                            partsStream.Position = 0;
                                            partsStream.CopyTo(context.Response.OutputStream);
                                            partsStream.Dispose();
                                            context.Response.OutputStream.Flush();
                                            lastSend = DateTime.Now;
                                            if (ChangesCount > 10)
                                            {
                                                int sleepTime = 40 - (int)(DateTime.Now - timeBegin).TotalMilliseconds;
                                                if (sleepTime > 0)
                                                {
                                                    Thread.Sleep(40);
                                                }
                                            }
                                        }
                                        //Thread.Sleep(2000);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                        break;
                                    }
                                    //writer.Wr
                                    //writer.Flush();
                                }

                                lock (clientLocker)
                                {
                                    clientCount++;
                                }
                                lImage?.Dispose();
                            }
                            break;
                        case "/stream":
                            {
                                try
                                {
                                    context.Response.ContentType = "multipart/x-mixed-replace; boundary=image-boundary";

                                    if (!ffmpegMode)
                                    {
                                        context.Response.StatusCode = 404;
                                        context.Response.Close();
                                        break;
                                    }

                                    long c = 0;
                                    while (true)
                                    {
                                        byte[]? jb = rawBuff.getFrameWait(ref c);

                                        if (jb != null && jb.Length > 0)
                                        {
                                            var boundary = Encoding.ASCII.GetBytes("\r\n--image-boundary\r\n");
                                            context.Response.OutputStream.Write(boundary);
                                            context.Response.OutputStream.Write(Encoding.ASCII.GetBytes("Content-Type: image/jpeg\r\n\r\n"));
                                            context.Response.OutputStream.Write(jb);
                                        }
                                        else
                                        {
                                            Thread.Sleep(10);
                                        }
                                    }
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    context.Response.Close();
                                }

                                break;
                            }
                        case "/":
                            {
                                string indexFileName = System.IO.Path.Combine(GetAppPath(), System.IO.Path.GetFileNameWithoutExtension(GetAppExeName()) + ".htm");
#if DEBUG
                                //indexFileName = @"c:\Projects\Utils\HadwareRemoteControlCP\HadwareRemoteControlCP.htm";
                                indexFileName = @"e:\Projects\Utils\HadwareRemoteControlCP\HadwareRemoteControlCP.htm";
#endif
                                if (File.Exists(indexFileName))
                                {
                                    var htmlBytes = File.ReadAllBytes(indexFileName);

                                    if (ffmpegMode)
                                    {
                                        var html = Encoding.UTF8.GetString(htmlBytes);
                                        html = html.Replace("<canvas ", "<img src=\"/stream\" ");
                                        html = html.Replace("</canvas>", "");
                                        html = html.Replace("ffmpegMode = false", "ffmpegMode = true");
                                        htmlBytes = Encoding.UTF8.GetBytes(html);
                                    }

                                    context.Response.ContentType = "text/html";
                                    context.Response.OutputStream.Write(htmlBytes);
                                    context.Response.Close();
                                    break;
                                }
                                //var htmlBytes = Encoding.UTF8.GetBytes(html);
                                context.Response.ContentType = "text/html";
                                //context.Response.OutputStream.Write(htmlBytes);
                                context.Response.Close();
                                break;
                            }

                        default:
                            context.Response.StatusCode = 404;
                            context.Response.Close();
                            break;
                    }
                });

                thread.Start();
            }

            capture.Release();
        }

        static void GUIMode(string camIndex, VideoParameters videoParameters)
        {
            try
            {
                InitDynamicGtk();
                Gtk.Application.Init();

                Console.Write($"Video Device Init ({camIndex})...");
                using (var capture = new VideoCapture(camIndex))
                {
                    if (videoParameters.Width > 0)
                    {
                        capture.FrameWidth = videoParameters.Width;
                    }
                    if (videoParameters.Height > 0)
                    {
                        capture.FrameHeight = videoParameters.Height;
                    }
                    if (videoParameters.FourCC.Length > 0)
                    {
                        capture.FourCC = videoParameters.FourCC;//OpenCvSharp.FourCC.MJPG;
                    }
                    if (videoParameters.FPS > 0)
                    {
                        capture.Fps = videoParameters.FPS;
                    }
                    if (videoParameters.BufferSize >= 0)
                    {
                        capture.BufferSize = videoParameters.BufferSize;
                    }
                    Console.WriteLine(" OK");

                    // Check if the webcam is available
                    if (!capture.IsOpened())
                    {
                        Console.WriteLine("Webcam could not be opened.");
                        return;
                    }
                    capture.FrameWidth = 1920;
                    capture.FrameHeight = 1080;

                    //using (var window = new OpenCvSharp.Window("Webcam", WindowFlags.KeepRatio | WindowFlags.Normal))
                    var window = new Gtk.Window($"HardwareRemoteControl");
                    {
                        window.Events = window.Events | EventMask.PointerMotionMask;
                        var da = new DrawingArea();
                        uint eventTime = 0;
                        bool inLoop = true;

                        window.DeleteEvent += (s, e) =>
                        {
                            inLoop = false;
                            Gtk.Application.Quit();
                        };
                        window.KeyPressEvent += [GLib.ConnectBefore] (s, e) => {
                            Console.WriteLine($"KeyPress: {e.Event.KeyValue}");

                            var HKeys = HID.GetHIDKeys((int)e.Event.KeyValue);
                            foreach (var HKey in HKeys)
                            {
                                SendSerial($"KD:{(int)HKey}", 0);
                            }
                        };
                        window.KeyReleaseEvent += (s, e) => {
                            //Console.WriteLine($"KeyRelease: {e.Event.KeyValue}");
                            var HKeys = HID.GetHIDKeys((int)e.Event.KeyValue);
                            foreach (var HKey in HKeys.Reverse<HID.HIDKeys>())
                            {
                                SendSerial($"KU:{(int)HKey}", 0);
                            }
                        };


                        window.MotionNotifyEvent += (s, e) => {
                            window.GetSize(out var windowWidth, out var windowHeight);
                            int x = (int)((e.Event.X / (double)windowWidth) * 32768);
                            int y = (int)((e.Event.Y / (double)windowHeight) * 32768);

                            SendSerial($"MM:{x},{y}", 0);
                            //Console.WriteLine($"MM: {x}:{y}");
                        };

                        window.ButtonPressEvent += (s, e) => {
                            window.GetSize(out var windowWidth, out var windowHeight);
                            int x = (int)((e.Event.X / (double)windowWidth) * 32768);
                            int y = (int)((e.Event.Y / (double)windowHeight) * 32768);
                            if (e.Event.Button != 0)
                            {
                                if (e.Event.Time != eventTime)
                                {
                                    SendSerial($"MD:{e.Event.Button},{x},{y}", 0);
                                    //Console.WriteLine($"MD:{e.Event.Button},{x}:{y}");
                                    eventTime = e.Event.Time;
                                }
                            }

                        };
                        window.ButtonReleaseEvent += (s, e) => {
                            window.GetSize(out var windowWidth, out var windowHeight);
                            int x = (int)((e.Event.X / (double)windowWidth) * 32768);
                            int y = (int)((e.Event.Y / (double)windowHeight) * 32768);
                            if (e.Event.Button != 0)
                            {
                                if (e.Event.Time != eventTime)
                                {
                                    SendSerial($"MU:{e.Event.Button},{x},{y}", 0);
                                    //Console.WriteLine($"MU:{e.Event.Button},{x}:{y}");
                                    eventTime = e.Event.Time;
                                }
                            }
                        };

                        window.Add(da);
                        object buffLock = new object();
                        Gdk.Pixbuf pixbuf = null;


                        int color = 0;
                        da.Drawn += (s, e) =>
                        {
                            var cr = e.Cr;

                            Gdk.Pixbuf buf = null;
                            lock (buffLock)
                            {
                                buf = pixbuf;
                                pixbuf = null;
                            }
                            if (buf != null)
                            {
                                window.GetSize(out var windowWidth, out var windowHeight);
                                double scaleW = (double)windowWidth / buf.Width;
                                double scaleH = (double)windowHeight / buf.Height;
                                //double scale = Math.Min(scaleW, scaleH);
                                using var surface = Gdk.CairoHelper.SurfaceCreateFromPixbuf(buf, 0, null);
                                cr.Scale(scaleW, scaleH);
                                cr.SetSourceSurface(surface, 0, 0);
                                cr.Paint();
                                buf.Dispose();
                            }
                        };

                        window.SetPosition(WindowPosition.CenterOnParent);
                        window.SetSizeRequest(640, 480);
                        window.ShowAll();
                        window.Show();

                        new Thread(() =>
                        {
                            using (var image = new Mat())
                            {
                                while (inLoop)
                                {
                                    try
                                    {
                                        capture.Read(image);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error (capture.Read): {ex}");
                                        return;
                                    }

                                    if (image.Empty())
                                    {
                                        Console.WriteLine("Empty frame.");
                                        break;
                                    }

                                    bool needUpdate = false;
                                    lock (buffLock)
                                    {
                                        needUpdate = (pixbuf == null);
                                    }
                                    if (needUpdate)
                                    {
                                        using MemoryStream stream = new MemoryStream();
                                        try
                                        {
                                            image.WriteToStream(stream, ".bmp");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error (capture.Read): {ex}");
                                            return;
                                        }
                                        stream.Position = 0;

                                        bool draw = false;
                                        lock (buffLock)
                                        {
                                            if (pixbuf == null)
                                            {
                                                pixbuf = new Gdk.Pixbuf(stream);
                                                draw = true;
                                            }
                                        }

                                        if (draw)
                                        {
                                            Gtk.Application.Invoke((s, e) =>
                                            {
                                                window.GetSize(out var windowWidth, out var windowHeight);
                                                da.QueueDrawArea(0, 0, windowWidth, windowHeight);
                                            });
                                        }
                                    }
                                    Thread.Sleep(10);
                                }
                            }
                        }).Start();
                        Gtk.Application.Run();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {
            Cv2.SetLogLevel(LogLevel.SILENT);

            int webPort = 0;
            string camIndex = "";
            string portName = "";
            VideoParameters videoParameters = new VideoParameters();
            bool isGUI = false;
            bool testMode = false;
            bool ffmpegMode = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith('-'))
                {
                    arg.SplitTwo("=", out var key, out var value);
                    switch (key)
                    {
                        case "-c":
                            camIndex = value;
                            break;
                        case "-p":
                            portName = value;
                            break;
                        case "-w":
                            webPort = value.ToInt32();
                            break;
                        case "-gui":
                            isGUI = true;
                            break;
                        case "-q":
                            videoParameters.Quality = value.ToInt32();
                            break;
                        case "-v:w":
                            videoParameters.Width = value.ToInt32();
                            break;
                        case "-v:h":
                            videoParameters.Height = value.ToInt32();
                            break;
                        case "-v:c":
                            videoParameters.FourCC = value;
                            break;
                        case "-v:f":
                            videoParameters.FPS = value.ToInt32();
                            break;
                        case "-v:of":
                            videoParameters.outputFPS = value.ToInt32();
                            break;
                        case "-v:b":
                            videoParameters.BufferSize = value.ToInt32();
                            break;
                        case "-t":
                            testMode = true;
                            break;
                        case "-recode":
                            videoParameters.Recode = true;
                            break;
                        case "-ffmpeg":
                            ffmpegMode = true;
                            break;
                    }
                }
                else
                {
                    if (arg == "listcams")
                    {
                        var deviceIndices = GetAvailableWebcams(ffmpegMode);
                        Console.WriteLine($"Webcams:");
                        foreach (var index in deviceIndices)
                        {
                            Console.WriteLine($"\tWebcam: {index}");
                        }
                        return;
                    }
                    if (arg == "listports")
                    {
                        var deviceIndices = SerialPort.GetPortNames();
                        Console.WriteLine($"Ports:");
                        foreach (var index in deviceIndices)
                        {
                            Console.WriteLine($"\tPort: {index}");
                        }
                        return;
                    }
                    if (arg == "/?" || arg == "help")
                    {
                        var deviceIndices = SerialPort.GetPortNames();
                        Console.WriteLine($"HadwareRemoteControlCP -c=[CAPRURE_DEVICE_INDEX] -p=[SERIAL_PORT_NAME] -w=[WEB_SERVER_PORT] -q=[JPEG_QUALITY_FOR_WEB] -v:w=[PREFERRED_IMAGE_WIDTH] -v:h=[PREFERRED_IMAGE_HEIGHT] -v:c=[PREFERRED_FORMAT like MJPG etc] -v:f=[PREFERRED_FPS] -v:b=[BUFFER_FRAMES_COUNT]");
                        return;
                    }
                }
            }

            if (portName.Length == 0)
            {
                var deviceNames = SerialPort.GetPortNames();
                Console.WriteLine($"Enter serial port number from list (or use command line -p=NAME):");
                int n = 1;
                foreach (var name in deviceNames)
                {
                    Console.WriteLine($"{n}. {name}");
                    n++;
                }
                Console.Write("Port Number: ");
                portName = deviceNames[Console.ReadLine().ToInt32() - 1];
            }

            if (camIndex.Length == 0)
            {
                
                var deviceIndices = GetAvailableWebcams(ffmpegMode);
                Console.WriteLine($"Enter capture device number from list (or use command line -c=INDEX):");
                int n = 1;
                foreach (var index in deviceIndices)
                {
                    Console.WriteLine($"{n}. Capture Device '{index}'");
                    n++;
                }
                Console.Write("Index: ");
                camIndex = deviceIndices[Console.ReadLine().ToInt32() - 1];
            }

            if (!isGUI && webPort <= 0)
            {
                Console.WriteLine($"Select mode:");
                Console.WriteLine($"1. GUI Mode");
                Console.WriteLine($"2. WebServer Mode");
                var index = Console.ReadLine().ToInt32();
                if (index == 1)
                {
                    isGUI = true;
                }
                if (index == 2)
                {
                    Console.Write($"Enter port number for web server: ");
                    webPort = Console.ReadLine().ToInt32();
                }
            }

            if (testMode)
            {
                //var rc = new RawCapture();
                //rc.CaptureRawMJPEGFrames(camIndex);
                //MjpgCapture.StartCapture($"/dev/video{camIndex}");
                //var cap = new MjpegStreamProcessor();
                //cap.ProcessMjpegStreamAsync("ffmpeg", $"-f v4l2 -input_format mjpeg -i /dev/video{camIndex} -video_size {videoParameters.Width}x{videoParameters.Height} -framerate {videoParameters.FPS} -c:v copy -vframes 50 -f mjpeg -").Wait();
                Console.WriteLine("Test Done");
                return;
            }


            Console.Write($"Serial Port ({portName})...");
            //*
            serialPort = new SerialPort();

            serialPort.PortName = portName;
            serialPort.BaudRate = 115200;
            serialPort.Handshake = Handshake.None;
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;

            serialPort.Open();//*/
            Console.WriteLine($" OK");

            if (webPort > 0)
            {
                WebServerMode(camIndex, webPort, videoParameters, ffmpegMode);
                return;
            }
            else if (isGUI)
            {
                GUIMode(camIndex, videoParameters);
            }

            

            //Console.WriteLine("Ended");
            //Console.ReadKey();
        }
    }
} 
