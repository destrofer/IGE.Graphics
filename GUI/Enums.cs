/*
 * Author: Viacheslav Soroka
 * 
 * This file is part of IGE <https://github.com/destrofer/IGE>.
 * 
 * IGE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * IGE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with IGE.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace IGE.GUI {
	public enum UIEventType {
		Move = 0x00,
		Resize,
		ParentChange, ChangeParent = ParentChange,
		
		MouseMove = 0x100,
		MouseWheel,
		MouseDown,
		MouseUp,
		MouseClick,
		MouseDblClick,
		MouseOver,
		MouseOut,
		
		Focus = 0x200,
		Blur,
		
		VScroll = 0x1000,
		HScroll,
	}
	
	public enum UIComponentState : byte {
		None = 0,
		Focus = 1,
		Hover = 2,
		MouseDown = 3,
		Max = 3
	}
	
	public enum UITextAlign : byte {
		Left,
		Center,
		Right
	}

	public enum UIVerticalAlign {
		Top,
		Center,
		Bottom
	}
	
	[Flags]
	public enum InputHandlingFlags : byte {
		None = 0x00,
		Keyboard = 0x01,
		Mouse = 0x02,
		Joystick = 0x04,
		All = Keyboard | Mouse | Joystick,		
	}
}
