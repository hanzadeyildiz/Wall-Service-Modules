using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using VLC_Module.Utils;

namespace VLC_Module
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibVLC _libVLC;
        LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

        public Options.CommandLineOptions cmd;
        public MainWindow(Options.CommandLineOptions cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
        }
        public void GetAudioOutputs(ref IEnumerable<AudioOutputDescription> audioOutputDescription)
        {
            LibVLC libvlc = new LibVLC();
            audioOutputDescription = libvlc.AudioOutputs;
            libvlc.Dispose();
        }
        public void GetAudioDevices(ref IEnumerable<AudioOutputDevice> audioOutputDevices, string name)
        {
            LibVLC libvlc = new LibVLC();
            audioOutputDevices = libvlc.AudioOutputDevices(name);
            libvlc.Dispose();
        }
        public void InitVLC()
        {
            IEnumerable<AudioOutputDescription> audioOutputDescription = new List<AudioOutputDescription>();
            GetAudioOutputs(ref audioOutputDescription);
            Console.WriteLine(string.Join(",", audioOutputDescription.Select(x => x.Name)));
            Core.Initialize();
            List<string> args = new List<string>
            {
                "--aout=mmdevice",
            };
            if ((bool)cmd.Loop)
            {
                args.Add("--input-repeat=65535");
            }
            string filter = "--video-filter=";
            if (cmd.Angle != null)
            {
                filter += $":rotate{{angle={cmd.Angle}}}";
            }
            if (cmd.Rotate != null)
            {
                filter += $":transform{{type={cmd.Rotate}}}";
            }
            args.Add(filter);
            string[] extraParams = cmd.ExtraParameters.Split('|');
            foreach (var item in extraParams)
            {
                args.Add($"--{item.Trim()}");
            }

            _libVLC = new LibVLC(args.ToArray());
            /*
            foreach (var audioOutput in _libVLC.AudioOutputs)
            {
                Console.WriteLine($"AudioOutput: {audioOutput.Name} - {audioOutput.Description}");
                foreach (var outputDevice in _libVLC.AudioOutputDevices(audioOutput.Name))
                {
                    Console.WriteLine($"OutputDevice: {outputDevice.DeviceIdentifier} - {outputDevice.Description}");
                }
            }
            */
            _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC);

            vlcPlayer.MediaPlayer = _mediaPlayer;
            var media = new Media(_libVLC, new Uri(cmd.Path));
            SetSize(cmd.Width, cmd.Height);
            SetLocation(cmd.Left, cmd.Top);
            _mediaPlayer.Volume = cmd.Volume;
            _mediaPlayer.AspectRatio = cmd.Width + ":" + cmd.Height;
            /*
            _mediaPlayer.SetAudioOutput("mmdevice");
            _mediaPlayer.SetOutputDevice("{0.0.0.00000000}.{6427d743-1e72-416a-8345-102e5dcdc4bf}", "mmdevice");

            Console.WriteLine($"OutputDevice: {_mediaPlayer.OutputDevice}");

            _mediaPlayer.AudioDevice += (s, e) =>
            {
                Console.WriteLine($"AudioDevice: {e.AudioDevice}");
            };
            */
            if (cmd.Crop != null)
            {
                _mediaPlayer.CropGeometry = cmd.Crop;
            }

            _mediaPlayer.Play(media);

        }

        private void vlcPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            InitVLC();
        }
        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

        }

        public void SetLocation(int left, int top)
        {
            this.Left = left;
            this.Top = top;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
    }
}
