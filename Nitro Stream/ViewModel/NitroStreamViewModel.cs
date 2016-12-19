using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Permissions;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Nitro_Stream.ViewModel
{
    class NitroStreamViewModel : ViewModelBase
    {

        public static string Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
		
		public Process NtrViewerProcess
		{
			get { return _NtrViewerProcess; }
		}

        Model.NtrClient _NtrClient;
		Model.NtrClientInputsConverter _NtrInputConverter;
		Process _NtrViewerProcess;
		System.Timers.Timer _DisconnectTimeout;
		System.Timers.Timer _UpdateScreenTimeout;
		System.Timers.Timer _HearthbeatTimeout;
		bool _PatchMem;
        bool _Connected;

        public string configPath { get { return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml"); } }

        public Model.Updater Updater { get; set; }

        private StringBuilder _RunningLog;
        public string runningLog
        {
            get { return _RunningLog.ToString(); }
        }

		private BitmapImage _screen3DS;

		public BitmapImage screen3DS
		{
			get { return _screen3DS; }
		}

        Model.ViewSettings _ViewSettings;
        public Model.ViewSettings ViewSettings { get { return _ViewSettings; } set { _ViewSettings = value; } }

        public NitroStreamViewModel()
        {
            _ViewSettings = new Model.ViewSettings(true);
            _NtrClient = new Model.NtrClient();
			_NtrInputConverter = new Model.NtrClientInputsConverter();
			_DisconnectTimeout = new System.Timers.Timer(10000);
            _DisconnectTimeout.Elapsed += _disconnectTimeout_Elapsed;
			_UpdateScreenTimeout = new System.Timers.Timer(100);
			_UpdateScreenTimeout.Elapsed += _UpdateScreenTimeout_Elapsed;
			_HearthbeatTimeout = new System.Timers.Timer(1000);
			_HearthbeatTimeout.Elapsed += _UpdateHearthbeat_Elapsed;
			_HearthbeatTimeout.Start();
			if (System.IO.File.Exists(configPath))
                _ViewSettings = Model.ViewSettings.Load(configPath);

            _NtrClient.onLogArrival += WriteToLog;
            _NtrClient.Connected += _ntrClient_Connected;
			_NtrClient.InfosReady += _ntrClient_InfosReady;
            AppDomain.CurrentDomain.UnhandledException += ExceptionToLog;

            _RunningLog = new StringBuilder("");

            //Updater = new Model.Updater();
        }

		private void _ntrClient_InfosReady(string Message)
		{
			if (Message.Contains("niji_loc")) // Sun/Moon
			{
				string pname = ", pname: niji_loc";
				string splitlog = Message.Substring(Message.IndexOf(pname) - 8, Message.Length - Message.IndexOf(pname));
				int pid = Convert.ToInt32("0x" + splitlog.Substring(0, 8), 16);
				byte[] command = BitConverter.GetBytes(0xE3A01000);
				_WriteToDeviceMemory(0x3DFFD0, command, pid);
			}
		}

		private void _UpdateScreenTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_UpdateScreenCapture();
		}

		private void _UpdateHearthbeat_Elapsed(object sender, EventArgs e)
		{
			try
			{
				if (_NtrClient != null && _Connected)
					_NtrClient.sendHeartbeatPacket();
			}
			catch (Exception)
			{

			}
		}

		internal void Donate()
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ASKURE99X999W");
        }

        private void _ntrClient_Connected(bool Connected)
        {
            if (Connected)
            {
				_NtrClient.sendEmptyPacket(5); //list process packet

				_Connected = true;
				//if (_PatchMem)
				//{
				//    //byte[] bytes = { 0x70, 0x47 };
				//    //_WriteToDeviceMemory(0x0105AE4, bytes, 0x1a);
				//    _PatchMem = false;
				//}
				//else
				{
				    uint pm = (uint)(_ViewSettings.PriorityMode ? 1 : 0);
				    remoteplay(pm, _ViewSettings.PriorityFactor, _ViewSettings.PictureQuality, _ViewSettings.QosValue);
				    _DisconnectTimeout.Start();
				
				    if (System.IO.File.Exists(_ViewSettings.ViewerPath))
				    {
				        StringBuilder args = new StringBuilder();
				
				        args.Append("-l ");
				        args.Append(((_ViewSettings.ViewMode == Model.Orientations.Vertical) ? "0" : "1") + " ");
				        args.Append("-t " + _ViewSettings.TopScale.ToString() + " ");
				        args.Append("-b " + _ViewSettings.BottomScale.ToString());
				
				        System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(_ViewSettings.ViewerPath);
				        p.Verb = "runas";
				        p.Arguments = args.ToString().Replace(',','.');
						_NtrViewerProcess = Process.Start(p);
					}
				    else
				        WriteToLog("NTRViewer not found, please run it manually as admin");
				}
			}
		}

        private void ExceptionToLog(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                throw ex;
            }
            WriteToLog("ERR:" + ex.Message.ToString());
        }

        public void WriteToLog(string msg)
        {
            _RunningLog.Append(msg);
            _RunningLog.Append("\n");
            OnPropertyChanged("runningLog");
        }

        private void _disconnectTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
			//Disconnect();
			_UpdateScreenTimeout.Start();
			_DisconnectTimeout.Stop();
        }

        public void InitiateRemotePlay()
        {
            Connect(_ViewSettings.IPAddress);
        }

        public void Connect(string host)
        {
            _NtrClient.setServer(host, 8000);
            _NtrClient.connectToServer();
        }

        public void MemPatch()
        {
            //_PatchMem = true; 
            //Connect(_ViewSettings.IPAddress);            
			if (_Connected)
			{
				//byte[] bytes = { 0x70, 0x47 };
				//_WriteToDeviceMemory(0x0105AE4, bytes, 0x1a);

				//string pname = ", pname: niji_loc";
				//string splitlog = log.Substring(log.IndexOf(pname) - 8, log.Length - log.IndexOf(pname));
				//pid = Convert.ToInt32("0x" + splitlog.Substring(0, 8), 16);
				//byte[] command = BitConverter.GetBytes(0xE3A01000);
				//_WriteToDeviceMemory(0x3DFFD0, bytes, 0x1a);
			}
			WriteToLog("OK: Memory patch applied");
        }

        public void Disconnect()
        {
            _NtrClient.disconnect();
        }

        private void _WriteToDeviceMemory(uint addr, byte[] buf, int pid = -1)
        {
            _NtrClient.sendWriteMemPacket(addr, (uint)pid, buf);
        }

        public void remoteplay(uint priorityMode = 0, uint priorityFactor = 5, uint quality = 90, double qosValue = 15)
        {
            uint num = 1;
            if (priorityMode == 1)
            {
                num = 0;
            }
            uint qosval = (uint)(qosValue * 1024 * 1024 / 8);
            _NtrClient.sendEmptyPacket(901, num << 8 | priorityFactor, quality, qosval);
            WriteToLog("OK: Remoteplay initiated. This client will disconnect in 10 seconds.");
        }

		public void UpdateKeyboardState(Key key, bool isKeyDown)
		{
			if (!_Connected)
				return;

			uint index = 0;
			bool keyFound = false;

			foreach (var it in Model.NtrClientInputsConverter.KeyboardInputs)
			{
				if (it == key)
				{
					_NtrInputConverter.KeyboardState[index] = isKeyDown;
					keyFound = true;
					break;
				}
				index++;
			}

			if (!keyFound)
			{
				index = 0;
				foreach (var it in Model.NtrClientInputsConverter.CPadDirInputsKey)
				{
					if (it == key)
					{
						if (isKeyDown)
						{
							_NtrInputConverter.CPadState[0] = Model.NtrClientInputsConverter.CPadDirInputsValue[index, 0];
							_NtrInputConverter.CPadState[1] = Model.NtrClientInputsConverter.CPadDirInputsValue[index, 1];
						}
						else
						{
							_NtrInputConverter.CPadState[0] = 0.0f;
							_NtrInputConverter.CPadState[1] = 0.0f;
						}
						keyFound = true;
						break;
					}
					index++;
				}
			}

			if (keyFound)
			{
				_NtrInputConverter.Update();
				_SendInputs();
			}
		}

		private void _SendInputs()
		{
			uint buttons = 0;
			uint touch = 0;
			uint cpad = 0;
			byte[] data = new byte[12];

			if (_NtrInputConverter.FillInput(ref buttons, ref touch, ref cpad))
			{
				//Buttons
				data[0x00] = (byte)(buttons & 0xFF);
				data[0x01] = (byte)((buttons >> 0x08) & 0xFF);
				data[0x02] = (byte)((buttons >> 0x10) & 0xFF);
				data[0x03] = (byte)((buttons >> 0x18) & 0xFF);
				//Touch
				data[0x04] = (byte)(touch & 0xFF);
				data[0x05] = (byte)((touch >> 0x08) & 0xFF);
				data[0x06] = (byte)((touch >> 0x10) & 0xFF);
				data[0x07] = (byte)((touch >> 0x18) & 0xFF);
				//CPad
				data[0x08] = (byte)(cpad & 0xFF);
				data[0x09] = (byte)((cpad >> 0x08) & 0xFF);
				data[0x0A] = (byte)((cpad >> 0x10) & 0xFF);
				data[0x0B] = (byte)((cpad >> 0x18) & 0xFF);

				_NtrClient.sendWriteMemPacket(0x10DF20, (uint)0x10, data);
				//System.Diagnostics.Debug.WriteLine("send mem write");
			}
		}

		private void _UpdateScreenCapture()
		{
			//_screen3DS.BeginInit();
			//_screen3DS.UriSource = new Uri(@"C:\Users\Raveh\Pictures\3dsCapture.jpg");
			////screen3DS.DecodePixelWidth = 200;
			//_screen3DS.EndInit();

			//OnPropertyChanged("screen3DS");
			//WriteToLog("_UpdateScreenCapture");
			//_screen3DS = Model.CaptureProcessScreen.PrintWindowToImageSource(_NtrViewerProcess.MainWindowHandle);
			var screenBmp = Model.CaptureProcessScreen.PrintWindow("NTRViewer");
			if (screenBmp != null)
			{
				_ProcessScreenCapture(screenBmp);
				_screen3DS = Model.CaptureProcessScreen.BitmapToImageSource(screenBmp);
				if (_screen3DS != null)
					_screen3DS.Freeze();
			}
			else
				_screen3DS = null;
			OnPropertyChanged("screen3DS");
		}

		private void _ProcessScreenCapture(System.Drawing.Bitmap screen)
		{
			// 509 x 406
			Model.ScreenCaptureProcessor processor = new Model.ScreenCaptureProcessor(screen);

			//var pixel = screen.GetPixel(353, 243);
			//WriteToLog(pixel.ToString());

			//System.Drawing.Color colorToFind = System.Drawing.Color.FromArgb(255, 98, 94);
			if (processor.FindMostlyRedPixel(340, 240, 20, 10))
			{
				processor.DrawEmptyCube(340, 240, 20, 10, System.Drawing.Color.Green);
			}
			else
				processor.DrawEmptyCube(340, 240, 20, 10, System.Drawing.Color.Red);
			//processor.DrawEmptyCube(353, 243, 1, 1, System.Drawing.Color.Green);
		}

	}
}
