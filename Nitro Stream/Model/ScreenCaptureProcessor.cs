using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitro_Stream.Model
{
	class ScreenCaptureProcessor
	{
		Bitmap _Screen;

		public ScreenCaptureProcessor(Bitmap screen)
		{
			_Screen = screen;
		}

		public void DrawCube(int startx, int starty, int width, int height, Color color)
		{
			int endx = startx + width;
			int endy = starty + height;
			for (int x = startx; x <= endx; ++x)
			{
				for (int y = starty; y <= endy; ++y)
				{
					_Screen.SetPixel(x, y, color);
				}
			}
		}

		public void DrawEmptyCube(int startx, int starty, int width, int height, Color color)
		{
			int endx = startx + width;
			int endy = starty + height;
			for (int x = startx; x <= endx; ++x)
			{
				_Screen.SetPixel(x, starty, color);
				_Screen.SetPixel(x, endy, color);
			}
			for (int y = starty + 1; y < endy; ++y)
			{
				_Screen.SetPixel(startx, y, color);
				_Screen.SetPixel(endx, y, color);
			}
		}

		public bool FindMostlyRedPixel(int startx, int starty, int width, int height)
		{
			int endx = startx + width;
			int endy = starty + height;
			for (int x = startx; x <= endx; ++x)
			{
				for (int y = starty; y <= endy; ++y)
				{
					var pixel = _Screen.GetPixel(x, y);
					if (pixel.R > 240 && pixel.G < 100 && pixel.B < 100)
						return true;
				}
			}
			return false;
		}
	}
}
