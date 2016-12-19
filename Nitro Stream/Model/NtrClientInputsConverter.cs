using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Nitro_Stream.Model
{
	class NtrClientInputsConverter
	{
		static public Key[] KeyboardInputs = { Key.Z, Key.E, Key.Q, Key.F, Key.Right, Key.Left, Key.Up, Key.Down, Key.R, Key.A, Key.S, Key.Y };
		public bool[] KeyboardState = new bool[12];
		//string[] KeyboardNames = { "A", "B", "Select", "Start", "DPad Right", "DPad Left", "DPad Up", "DPad Down", "R", "L", "X", "Y" };

		static public Key[] CPadDirInputsKey = { Key.NumPad8, Key.NumPad2, Key.NumPad4, Key.NumPad6 }; //UP DOWN RIGHT LEFT
		static public float[,] CPadDirInputsValue = { { 0.0f, -1.0f }, { 0.0f, 1.0f }, { -1.0f, 0.0f }, { 1.0f, 0.0f } };
		public float[] CPadState = new float[2];
		//string[] CPadNames = { "ThumbStick_X", "ThumbStick_Y" };

		private uint oldbuttons = 0xFFF;
		private uint newbuttons = 0xFFF;
		private uint oldtouch = 0x2000000;
		private uint newtouch = 0x2000000;
		private uint oldcpad = 0x800800;
		private uint newcpad = 0x800800;
		private uint touchclick = 0x00;
		private uint cpadclick = 0x00;

		public bool FillInput(ref uint buttons, ref uint touch, ref uint cpad)
		{
			if ((newbuttons != oldbuttons) || (newtouch != oldtouch) || (newcpad != oldcpad))
			{
				oldbuttons = newbuttons;
				oldtouch = newtouch;
				oldcpad = newcpad;
				buttons = newbuttons;
				touch = newtouch;
				cpad = newcpad;
				return true;
			}
			return false;
		}

		public void Update()
		{
			//Keys[] KeyboardInput = { Keys.A, Keys.S, Keys.N, Keys.M, Keys.H, Keys.F, Keys.T, Keys.G, Keys.W, Keys.Q, Keys.Z, Keys.X, Keys.Right, Keys.Left, Keys.Up, Keys.Down };
			uint[] GamePadInput = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x020, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800 };
			//string[] ButtonNames = { "A", "B", "Select", "Start", "DPad Right", "DPad Left", "DPad Up", "DPad Down", "R", "L", "X", "Y" };
			//Buttons.B == ButtonState.Pressed) //0
			//Buttons.A == ButtonState.Pressed) //1
			//Buttons.Back == ButtonState.Pressed) //2
			//Buttons.Start == ButtonState.Pressed) //3
			//DPad.Right == ButtonState.Pressed) //4
			//DPad.Left == ButtonState.Pressed) //5
			//DPad.Up == ButtonState.Pressed) //6
			//DPad.Down == ButtonState.Pressed) //7
			//Buttons.RightShoulder == ButtonState.Pressed) //8
			//Buttons.LeftShoulder == ButtonState.Pressed) //9
			//Buttons.Y == ButtonState.Pressed) //10
			//Buttons.X == ButtonState.Pressed) //11
			//

			if (KeyboardState.Length != GamePadInput.Length)
				return;

			//Keyboard
			newbuttons = 0x00;
			for (int i = 0; i < GamePadInput.Length; i++)
			{
				if (KeyboardState[i])
				{
					newbuttons += (uint)(0x01 << i);
				}
			}
			newbuttons ^= 0xFFF;

			//Touch
			//if (Mouse.GetState().LeftButton == ButtonState.Pressed)
			//{
			//	TouchInput(ref newtouch, ref touchclick, false);
			//}
			//else
			//{
			//	touchclick = 0x00;
			//	if (useGamePad)
			//	{
			//		if (GamePad.GetState(PlayerIndex.One).Buttons.RightStick == ButtonState.Pressed)
			//		{
			//			newtouch = (uint)Math.Round(2047.5 + (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 2047.5));
			//			newtouch += (uint)Math.Round(2047.5 + (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 2047.5)) << 0x0C;
			//			newtouch += 0x1000000;
			//		}
			//		else
			//		{
			//			newtouch = 0x2000000;
			//		}
			//	}
			//	else
			//	{
			//		newtouch = 0x2000000;
			//	}
			//}

			//Circle Pad
			//if (Mouse.GetState().RightButton == ButtonState.Pressed)
			//{
			//	TouchInput(ref newcpad, ref cpadclick, true);
			//}
			//else
			{
				cpadclick = 0x00;
				newcpad = (uint)Math.Round(2047.5 + (CPadState[0] * 2047.5));
				newcpad += (uint)Math.Round(4095 - (2047.5 + (CPadState[1] * 2047.5))) << 0x0C;
			
				//if (newcpad == 0x800800)
				//{
				//
				//	if (Keyboard.GetState().IsKeyDown(KeyboardInput[12]))
				//	{
				//		newcpad = 0xFFF + (((newcpad >> 0x0C) & 0xFFF) << 0x0C);
				//	}
				//
				//	if (Keyboard.GetState().IsKeyDown(KeyboardInput[13]))
				//	{
				//		newcpad = (((newcpad >> 0x0C) & 0xFFF) << 0x0C);
				//	}
				//
				//	if (Keyboard.GetState().IsKeyDown(KeyboardInput[15]))
				//	{
				//		newcpad = (newcpad & 0xFFF) + (0x00 << 0x0C);
				//	}
				//
				//	if (Keyboard.GetState().IsKeyDown(KeyboardInput[14]))
				//	{
				//		newcpad = (newcpad & 0xFFF) + (0xFFF << 0x0C);
				//	}
				//}
			
				if (newcpad != 0x800800)
				{
					newcpad += 0x1000000;
				}
			}
		}
	}
}
