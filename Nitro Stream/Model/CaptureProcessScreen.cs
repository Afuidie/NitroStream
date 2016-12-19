using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nitro_Stream.Model
{
	class CaptureProcessScreen
	{
		public static System.Windows.Media.Imaging.BitmapImage BitmapToImageSource(Bitmap bitmap)
		{
			if (bitmap == null)
				return null;
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				System.Windows.Media.Imaging.BitmapImage bitmapimage = new System.Windows.Media.Imaging.BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}

#if true
		public static System.Windows.Media.Imaging.BitmapImage PrintWindowToImageSource(string procName)
		{
			return BitmapToImageSource(PrintWindow(procName));
		}

		public static Bitmap PrintWindow(string procName)
		{
			Process[] processes = Process.GetProcessesByName(procName);
			if (processes.Length <= 0)
				return null;
			var proc = processes[0];
			var rect = new User32.Rect();
			User32.GetWindowRect(proc.MainWindowHandle, ref rect);

			int width = rect.right - rect.left;
			int height = rect.bottom - rect.top;

			var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(bmp);
			graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

			return bmp;
		}

		private class User32
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct Rect
			{
				public int left;
				public int top;
				public int right;
				public int bottom;
			}

			[DllImport("user32.dll")]
			public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
		}

#else
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

		public static System.Windows.Media.Imaging.BitmapImage PrintWindowToImageSource(string procName)
		{
			return BitmapToImageSource(PrintWindow(procName));
		}

		public static Bitmap PrintWindow(string procName)
		{
			Process[] processes = Process.GetProcessesByName(procName);
			if (processes.Length <= 0)
				return null;
			var proc = processes[0];

			IntPtr hwnd = proc.MainWindowHandle;

			RECT rc;
			GetWindowRect(hwnd, out rc);

			Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
			Graphics gfxBmp = Graphics.FromImage(bmp);
			IntPtr hdcBitmap = gfxBmp.GetHdc();

			PrintWindow(hwnd, hdcBitmap, 0);

			gfxBmp.ReleaseHdc(hdcBitmap);
			gfxBmp.Dispose();

			return bmp;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			private int _Left;
			private int _Top;
			private int _Right;
			private int _Bottom;

			public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
			{
			}
			public RECT(int Left, int Top, int Right, int Bottom)
			{
				_Left = Left;
				_Top = Top;
				_Right = Right;
				_Bottom = Bottom;
			}

			public int X
			{
				get { return _Left; }
				set { _Left = value; }
			}
			public int Y
			{
				get { return _Top; }
				set { _Top = value; }
			}
			public int Left
			{
				get { return _Left; }
				set { _Left = value; }
			}
			public int Top
			{
				get { return _Top; }
				set { _Top = value; }
			}
			public int Right
			{
				get { return _Right; }
				set { _Right = value; }
			}
			public int Bottom
			{
				get { return _Bottom; }
				set { _Bottom = value; }
			}
			public int Height
			{
				get { return _Bottom - _Top; }
				set { _Bottom = value + _Top; }
			}
			public int Width
			{
				get { return _Right - _Left; }
				set { _Right = value + _Left; }
			}
			public Point Location
			{
				get { return new Point(Left, Top); }
				set
				{
					_Left = value.X;
					_Top = value.Y;
				}
			}
			public Size Size
			{
				get { return new Size(Width, Height); }
				set
				{
					_Right = value.Width + _Left;
					_Bottom = value.Height + _Top;
				}
			}

			public static implicit operator Rectangle(RECT Rectangle)
			{
				return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
			}
			public static implicit operator RECT(Rectangle Rectangle)
			{
				return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
			}
			public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
			{
				return Rectangle1.Equals(Rectangle2);
			}
			public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
			{
				return !Rectangle1.Equals(Rectangle2);
			}

			public override string ToString()
			{
				return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
			}

			public override int GetHashCode()
			{
				return ToString().GetHashCode();
			}

			public bool Equals(RECT Rectangle)
			{
				return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
			}

			public override bool Equals(object Object)
			{
				if (Object is RECT)
				{
					return Equals((RECT)Object);
				}
				else if (Object is Rectangle)
				{
					return Equals(new RECT((Rectangle)Object));
				}

				return false;
			}
		}
#endif
	}
}
