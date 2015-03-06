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

using IGE.Input;
 
namespace IGE.GUI {
	public class UIEventArgs : EventArgs {
		protected Component m_Target;
		public Component Target { get { return m_Target; } }
		
		protected UIEventType m_Type;
		public UIEventType Type { get { return m_Type; } }
		
		public bool StopPropagation = false;
		public bool PreventDefault = false;
		
		public UIEventArgs(UIEventType type, Component target) {
			m_Type = type;
			m_Target = target;
		}
		
		public override string ToString() {
			return String.Format("{0}", m_Type);
		} 
	}
	public delegate void UIEventHandler(UIEventArgs args);
	
	public class UIFocusEventArgs : UIEventArgs {
		protected Component m_FocusFrom;
		public Component FocusFrom { get { return m_FocusFrom; } }
		
		protected Component m_FocusTo;
		public Component FocusTo { get { return m_FocusTo; } }
		
		public UIFocusEventArgs(UIEventType type, Component target, Component focusFrom, Component focusTo) : base(type, target) {
			m_FocusFrom = focusFrom;
			m_FocusTo = focusTo;
		}
	}
	public delegate void UIFocusEventHendler(UIEventArgs args);

	public abstract class UIMouseEventArgs : UIEventArgs {
		protected Point2 m_Cursor;
		public Point2 Cursor { get { return m_Cursor; } }

		public UIMouseEventArgs(UIEventType type, Component target, Point2 cursor) : base(type, target) {
			m_Cursor = cursor;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" ({0}, {1})", m_Cursor.X, m_Cursor.Y);
		} 
	}
	public delegate void UIMouseEventHandler(UIMouseEventArgs args);
	
	public class UIMouseWheelEventArgs : UIMouseEventArgs {
		protected int m_Wheel;
		public int Wheel { get { return m_Wheel; } }
		
		public UIMouseWheelEventArgs(UIEventType type, Component target, Point2 cursor, int wheel) : base(type, target, cursor) {
			m_Wheel = wheel;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" Wheel:{0}", m_Wheel);
		} 
	}
	public delegate void UIMouseWheelEventHandler(UIMouseWheelEventArgs args);
	
	public class UIMouseButtonEventArgs : UIMouseEventArgs {
		protected MouseButton m_Button;
		public MouseButton Button { get { return m_Button; } }
		
		public UIMouseButtonEventArgs(UIEventType type, Component target, Point2 cursor, MouseButton button) : base(type, target, cursor) {
			m_Button = button;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" Button:{0}", m_Button);
		} 
	}
	public delegate void UIMouseButtonEventHandler(UIMouseButtonEventArgs args);
	
	public class UIMouseMoveEventArgs : UIMouseEventArgs {
		protected Point2 m_CursorDelta;
		public Point2 CursorDelta { get { return m_CursorDelta; } }
		
		public UIMouseMoveEventArgs(UIEventType type, Component target, Point2 cursor, Point2 cursorDelta) : base(type, target, cursor) {
			m_CursorDelta = cursorDelta;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" Delta:({0}, {1})", m_CursorDelta.X, m_CursorDelta.Y);
		} 
	}
	public delegate void UIMouseMoveEventHandler(UIMouseMoveEventArgs args);

	public class UIMouseOverEventArgs : UIMouseEventArgs {
		protected Component m_OverFrom;
		public Component OverFrom { get { return m_OverFrom; } }
		
		protected Component m_OverTo;
		public Component OverTo { get { return m_OverTo; } }
		
		public UIMouseOverEventArgs(UIEventType type, Component target, Point2 cursor, Component overFrom, Component overTo) : base(type, target, cursor) {
			m_OverFrom = overFrom;
			m_OverTo = overTo;
		}
	}
	public delegate void UIMouseOverEventHandler(UIMouseOverEventArgs args);

	public class UIMoveEventArgs : UIEventArgs {
		protected Point2 m_OldLocation;
		public Point2 OldLocation { get { return m_OldLocation; } }
		
		protected Point2 m_NewLocation;
		public Point2 NewLocation { get { return m_NewLocation; } }
		
		public UIMoveEventArgs(UIEventType type, Component target, Point2 old_location, Point2 new_location) : base(type, target) {
			m_OldLocation = old_location;
			m_NewLocation = new_location;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" ({0}, {1})-({2}, {3})", m_OldLocation.X, m_OldLocation.Y, m_NewLocation.X, m_NewLocation.Y);
		} 
	}
	public delegate void UIMoveEventHandler(UIMoveEventArgs args);
	
	
	public class UIResizeEventArgs : UIEventArgs {
		protected Size2 m_OldSize;
		public Size2 OldSize { get { return m_OldSize; } }
		
		protected Size2 m_NewSize;
		public Size2 NewSize { get { return m_NewSize; } }
		
		public UIResizeEventArgs(UIEventType type, Component target, Size2 old_size, Size2 new_size) : base(type, target) {
			m_OldSize = old_size;
			m_NewSize = new_size;
		}
		
		public override string ToString() {
			return base.ToString() + String.Format(" ({0}, {1})-({2}, {3})", m_OldSize.Width, m_OldSize.Height, m_NewSize.Width, m_NewSize.Height);
		} 
	}
	public delegate void UIResizeEventHandler(UIResizeEventArgs args);
	
	public class UIParentChangeEventArgs : UIEventArgs {
		protected Component m_OldParent;
		public Component OldParent { get { return m_OldParent; } }
		
		protected Component m_NewParent;
		public Component NewParent { get { return m_NewParent; } }
		
		public UIParentChangeEventArgs(UIEventType type, Component target, Component old_parent, Component new_parent) : base(type, target) {
			m_OldParent = old_parent;
			m_NewParent = new_parent;
		}
	}
	public delegate void UIParentChangeEventHandler(UIParentChangeEventArgs args);
	
	public class UIScrollEventArgs : UIEventArgs {
		protected int m_ScrollValue;
		public int ScrollValue { get { return m_ScrollValue; } }
		
		protected int m_ScrollDelta;
		public int ScrollDelta { get { return m_ScrollDelta; } }
		
		public UIScrollEventArgs(UIEventType type, Component target, int scrollValue, int scrollDelta) : base(type, target) {
			m_ScrollValue = scrollValue;
			m_ScrollDelta = scrollDelta;
		}
	}
	public delegate void UIScrollEventHandler(UIScrollEventArgs args);


	public class UIMenuEventArgs : EventArgs {
		protected IMenu m_Menu;
		public IMenu Menu { get { return m_Menu; } }
		
		protected IMenuItem m_MenuItem;
		public IMenuItem MenuItem { get { return m_MenuItem; } }
		
		public UIMenuEventArgs(IMenu menu, IMenuItem item) {
			m_Menu = menu;
			m_MenuItem = item;
		}
	}
	public delegate void UIMenuEventHandler(UIMenuEventArgs args);
	
	/*public class UIMenuEventArgs : UIEventArgs {
		public MenuItem
	}*/	
}