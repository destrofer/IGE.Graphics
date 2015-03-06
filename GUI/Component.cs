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
using System.Collections;
using System.Collections.Generic;

using IGE;
using IGE.Platform;
using IGE.Input;
using IGE.Graphics.OpenGL;

namespace IGE.GUI {
	public interface IUIComponent {
		int X { get; set; }
		int Y { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		Point2 Location { get; set; }
		Size2 Size { get; set; }
		Rectangle Area { get; set; }
		int Id { get; }
		bool Removed { get; }
		bool ClipChildren { get; }
		bool Add(Component child);
	}
	
	public abstract class Component : IDisposable, IUIComponent {
		public static double DoubleClickTimeout = 0.5; // two clicks within 0.5 seconds are considered to be doubleclicks
		public static int ClickDistance = 3; // moving 3 pixels away from where mouse was down cancels click event
	
		#region Static fields
		
		private static int NextComponentId = 1;
		internal static Component m_Focus = null;
		internal static Component m_MouseOver = null;
		private static bool m_UIInvalid = true;
		// private static int m_PrevMouseX = -1;
		// private static int m_PrevMouseY = -1;
		private static int m_ClickMouseX = -1;
		private static int m_ClickMouseY = -1;
		private static double m_LastClickTime = 0.0;
		private static MouseButton m_ClickButton = MouseButton.None;
		private static UIMouseEventHandler m_MouseEventReceiver = null;
		private static HitTestResult m_LastHit = HitTestResult.None;
		
		public static bool UIInvalid {
			get { return m_UIInvalid; }
			set {
				m_UIInvalid = value;
				if( m_UIInvalid )
					m_LastHit = HitTestResult.None;
			}
		}
		
		public static HitTestResult LastHit { get { return m_LastHit; } }
		
		#endregion
	
		#region Fields
	
		private int m_Id;
		protected Rectangle m_Area;
		protected Component m_Parent = null;
		protected List<Component> m_Children;
		
		private bool m_Removed = false;
		private bool m_ChildHasFocus = false;
		private bool m_MouseOverChild = false;
		private bool m_Visible = true;
		
		#endregion Fields
		
		#region Events
		public event UIMoveEventHandler OnMove;
		public event UIResizeEventHandler OnResize;
		public event UIParentChangeEventHandler OnParentChange;

		public event UIMouseMoveEventHandler OnMouseMove;
		public event UIMouseWheelEventHandler OnMouseWheel;
		public event UIMouseButtonEventHandler OnMouseDown;
		public event UIMouseButtonEventHandler OnMouseUp;
		public event UIMouseButtonEventHandler OnMouseClick;
		public event UIMouseButtonEventHandler OnMouseDblClick;
		public event UIMouseOverEventHandler OnMouseOver;
		public event UIMouseOverEventHandler OnMouseOut;

		public event UIFocusEventHendler OnFocus;
		public event UIFocusEventHendler OnBlur;
		#endregion
		
		#region Properties
		public int Id {
			get { return m_Id; }
		}
		
		public bool Removed {
			get { return m_Removed; }
		}
		
		public Point2 Location {
			get { return m_Area.Location; }
			set {
				if( !m_Area.Location.Equals(value) && CanMove(value, m_Area.Location) ) {
					Point2 old_location = m_Area.Location;
					m_Area.Location = value;
					if( OnMove != null )
						OnMove(new UIMoveEventArgs(UIEventType.Move, this, old_location, value));
					UIInvalid = true;
				}
			}
		}
		
		public Size2 Size {
			get { return m_Area.Size; }
			set {
				if( !m_Area.Size.Equals(value) && CanResize(value, m_Area.Size) ) {
					Size2 old_size = m_Area.Size;
					m_Area.Size = value;
					if( OnResize != null )
						OnResize(new UIResizeEventArgs(UIEventType.Resize, this, old_size, value));
					UIInvalid = true;
				}
			}
		}
		
		public Rectangle Area {
			get { return m_Area; }
			set {
				Location = value.Location;
				Size = value.Size;
			}
		}
		
		public int X {
			get { return m_Area.X; }
			set { Location = new Point2(value, m_Area.Y); }
		}
		
		public int Y {
			get { return m_Area.Y; }
			set { Location = new Point2(m_Area.X, value); }
		}
		
		public int Width {
			get { return m_Area.Width; }
			set { Size = new Size2(value, m_Area.Height); }
		}
		
		public int Height {
			get { return m_Area.Height; }
			set { Size = new Size2(m_Area.Width, value); }
		}
		
		public Component Parent {
			get { return m_Parent; }
			set {
				if( m_Parent != value && CanChangeParent(value, m_Parent) ) {
					Component p;
					if( m_Parent != null ) {
						if( m_Parent.m_Children.Contains(this) )
							m_Parent.m_Children.Remove(this);
						if( m_Focus == this || m_ChildHasFocus )
							for( p = m_Parent; p != null; p = p.m_Parent )
								p.m_ChildHasFocus = false;
						if( m_MouseOver == this || m_MouseOverChild )
							for( p = m_Parent; p != null; p = p.m_Parent )
								p.m_MouseOverChild = false;
					}
					
					Component old_parent = m_Parent;
					m_Parent = value;
					
					if( m_Parent != null ) {
						if( !m_Parent.m_Children.Contains(this) )
							m_Parent.m_Children.Add(this);
						if( m_Focus == this || m_ChildHasFocus )
							for( p = m_Parent; p != null; p = p.m_Parent )
								p.m_ChildHasFocus = true;
						if( m_MouseOver == this || m_MouseOverChild )
							for( p = m_Parent; p != null; p = p.m_Parent )
								p.m_MouseOverChild = true;
					}
					
					if( OnParentChange != null )
						OnParentChange(new UIParentChangeEventArgs(UIEventType.ParentChange, this, old_parent, value));
					
					UIInvalid = true;
				}
			}
		}
		
		public bool Visible {
			get { return m_Visible; }
			set {
				if( m_Visible != value ) {
					m_Visible = value;
					Component.UIInvalid = true;
				}
			}
		}
		
		public virtual bool ClipChildren { get { return true; } }
		
		public IEnumerable<Component> Children {
			get { return m_Children; }
		}

		public virtual bool CanHaveFocus {
			get { return false; }
		}
		
		public virtual bool IsFocusHolder {
			get { return m_Focus == this; }
		}
		
		public virtual bool ChildHasFocus {
			get { return m_ChildHasFocus; }
		}
		
		public virtual bool HasFocus {
			get { return m_Focus == this || m_ChildHasFocus; }
		}

		
		/// <summary>
		/// True if the component has area which can be hit by mouse or false otherwise.
		/// </summary>
		public virtual bool CanHit {
			get { return false; }
		}
		
		public virtual bool IsMouseOverHolder {
			get { return m_MouseOver == this; }
		}
		
		public virtual bool MouseOverChild {
			get { return m_MouseOverChild; }
		}
		
		public virtual bool IsMouseOver {
			get { return m_MouseOver == this || m_MouseOverChild; }
		}
		
		#endregion Properties
		
		#region Constructors
		
		public Component() {
			m_Id = NextComponentId++;
			m_Parent = null;
			m_Children = new List<Component>();
			m_Area = new Rectangle(0, 0, 0, 0);
			SetDefaultStyle();
		}
		
		public Component(Component parent) : this() {
			Parent = parent;
		}
		
		public Component(int x, int y, int width, int height) : this() {
			Area = new Rectangle(x, y, width, height);
		}

		public Component(Component parent, int x, int y, int width, int height) : this() {
			Parent = parent;
			Area = new Rectangle(x, y, width, height);
		}
		
		#endregion
		
		#region Destructors
		~Component() {
			Remove();
		}
		
		public virtual void Dispose() {
			Remove();
		}
		#endregion Destructors
		
		#region Testers
		public virtual bool CanMove(Point2 new_location, Point2 old_location) {
			return true;
		}
		
		public virtual bool CanResize(Size2 new_location, Size2 old_location) {
			return true;
		}
		
		public virtual bool CanChangeParent(Component new_parent, Component old_parent) {
			return true;
		}
		
		public virtual HitTestResult HitTest(int x, int y) {
			if( !m_Visible )
				return HitTestResult.None;
			x -= m_Area.X;
			y -= m_Area.Y;
			bool missMe = (x < 0 || y < 0 || x >= m_Area.Width || y >= m_Area.Height);
			if( ClipChildren && missMe )
				return HitTestResult.None;
			HitTestResult hit;
			for( int i = m_Children.Count - 1; i >= 0; i-- ) {
				hit = m_Children[i].HitTest(x, y);
				if( hit.Target != null )
					return hit;
			}
			if( missMe )
				return HitTestResult.None;
			return CanHit ? new HitTestResult { Target = this, X = x, Y = y } : HitTestResult.None;
		}
		
		#endregion Testers
		
		#region Control methods
		public abstract void SetDefaultStyle();
		
		public static void SetFocus(Component target) {
			if( target != null )
				target.SetFocus();
			else if( m_Focus != null ) {
				if( m_Focus.OnBlur != null )
					m_Focus.OnBlur(new UIFocusEventArgs(UIEventType.Blur, m_Focus, m_Focus, null));
				for( Component p = m_Focus.m_Parent; p != null; p = p.m_Parent )
					p.m_ChildHasFocus = false;
				m_Focus = null;
			}
		}

		public void SetFocus() {
			if( m_Focus == this || !CanHaveFocus )
				return;
			Component p, from = m_Focus;
			if( m_Focus != null ) {
				if( m_Focus.OnBlur != null )
					m_Focus.OnBlur(new UIFocusEventArgs(UIEventType.Blur, m_Focus, from, this));
				for( p = m_Focus.m_Parent; p != null; p = p.m_Parent )
					p.m_ChildHasFocus = false;
			}
			m_Focus = this;
			if( OnFocus != null )
				OnFocus(new UIFocusEventArgs(UIEventType.Focus, this, from, this));
			for( p = m_Focus.m_Parent; p != null; p = p.m_Parent )
				p.m_ChildHasFocus = true;
		}
		
		protected bool BeginCapture(UIMouseEventHandler handler) {
			if( m_MouseEventReceiver != null || handler == null )
				return false;
			m_MouseEventReceiver = handler;
			return true;
		}
		
		protected void EndCapture() {
			m_MouseEventReceiver = null;
		}
		
		#endregion Control methods


		#region Component relation methods
		public bool Add(Component child) {
			if( m_Children.Contains(child) )
				return false;
			child.Parent = this;
			return child.m_Parent == this;
		}
		
		public bool Remove() {
			if( m_Removed ) return false;
			m_Removed = true;
			if( m_Parent != null ) {
				if( m_Parent.m_Children.Contains(this) )
					m_Parent.m_Children.Remove(this);
				m_Parent = null;
			}
			RemoveAllChildren();
			return true;
		}
		
		public void RemoveAllChildren() {
			foreach( Component child in m_Children ) {
				child.m_Parent = null;
				child.Remove();
			}
			m_Children.Clear();
		}		
		#endregion Component relation methods
		
		#region Render methods
		
		public virtual void Render(int x, int y) {
			foreach( Component comp in m_Children )
				if( comp.m_Visible )
					comp.Render(x + m_Area.X, y + m_Area.Y);
		}
		
		#endregion Render methods
		
		#region Other methods
		
		public override int GetHashCode() {
			return m_Id;
		}
		
		#endregion Other methods
		
		#region Internals
		delegate void MouseButtonCheckDelegate(MouseButton btn);
		
		internal static void HandleInput(UI ui, ref InputHandlingFlags handlingFlags) { // It has to be done here because at this point we have access to private members but. We don't want to attach every UI component to game engine's events, right?
			Component tmp;
			UIEventArgs args;
			
			bool invalid = UIInvalid;
			UIInvalid = false; // reset invalid flag so that it could be set again by events if needed

			if( (handlingFlags & InputHandlingFlags.Mouse) != InputHandlingFlags.None ) {
				
				IMouseDevice mouse = IL.FirstMouseDevice;
				
				/*
				if( m_PrevMouseX < 0 ) { // first initialization of mouse coords
					m_PrevMouseX = mouse.X;
					m_PrevMouseY = mouse.Y;
				}
				int mouseDeltaX = mouse.X - m_PrevMouseX;
				int mouseDeltaY = mouse.Y - m_PrevMouseY;
				MouseButton mouseButtonDelta = mouse.Buttons ^ mouse.PrevButtons;
				*/
				
				bool mouseMoved = mouse.DeltaX != 0 || mouse.DeltaY != 0;
				bool wheelMoved = mouse.DeltaWheel != 0;
				bool buttonChanged = mouse.ChangedButtons != MouseButton.None;
	
				//m_PrevMouseX = mouse.X;
				//m_PrevMouseY = mouse.Y;
			
				if( mouseMoved && m_ClickButton != MouseButton.None ) {
					if( mouse.X < m_ClickMouseX - ClickDistance
					|| mouse.X > m_ClickMouseX + ClickDistance
					|| mouse.Y < m_ClickMouseY - ClickDistance
					|| mouse.Y > m_ClickMouseY + ClickDistance ) {
						// moved outside the max click distance
						m_ClickButton = MouseButton.None;
						m_ClickMouseX = -1;
						m_ClickMouseY = -1;
					}
				}
				
				if( mouseMoved || wheelMoved || buttonChanged || invalid ) {
					HitTestResult hit = m_LastHit = ui.HitTest(mouse.X, mouse.Y);
					// GameDebugger.Log("Hit: {0} ({1}, {2})", hit.Target, hit.X, hit.Y);

					if( m_MouseEventReceiver != null ) {
						// There is a function that has to capture absolutely all mouse events!
						// Captured events are not propagated to parents!
						
						if( mouseMoved && m_MouseEventReceiver != null ) {
							args = new UIMouseMoveEventArgs(UIEventType.MouseMove, hit.Target, new Point2(hit.X, hit.Y), new Point2(mouse.DeltaX, mouse.DeltaY));
							m_MouseEventReceiver((UIMouseMoveEventArgs)args);
						}
						
						if( wheelMoved && m_MouseEventReceiver != null ) {
							args = new UIMouseWheelEventArgs(UIEventType.MouseWheel, hit.Target, new Point2(hit.X, hit.Y), mouse.DeltaWheel);
							m_MouseEventReceiver((UIMouseWheelEventArgs)args);
							// mousewheel will not affect scrollbars
						}
						
						if( buttonChanged && m_MouseEventReceiver != null ) {
							MouseButtonCheckDelegate checkMouseButton = btn => {
								if( (mouse.ChangedButtons & btn) != MouseButton.None && m_MouseEventReceiver != null ) {
									if( (mouse.Buttons & btn) == MouseButton.None ) {
										// button went up
										args = new UIMouseButtonEventArgs(UIEventType.MouseUp, hit.Target, new Point2(hit.X, hit.Y), btn);
										m_MouseEventReceiver((UIMouseButtonEventArgs)args);
										
										if( !args.PreventDefault && m_ClickButton == btn && m_MouseEventReceiver != null ) {
											m_ClickButton = MouseButton.None;
											m_ClickMouseX = -1;
											m_ClickMouseY = -1;
											args = new UIMouseButtonEventArgs(UIEventType.MouseClick, hit.Target, new Point2(hit.X, hit.Y), btn);
											m_MouseEventReceiver((UIMouseButtonEventArgs)args);
											if( !args.PreventDefault && m_MouseEventReceiver != null ) {
												if( m_LastClickTime + DoubleClickTimeout >= Application.InternalTimer.Time ) {
													args = new UIMouseButtonEventArgs(UIEventType.MouseDblClick, hit.Target, new Point2(hit.X, hit.Y), btn);
													m_MouseEventReceiver((UIMouseButtonEventArgs)args);
													m_LastClickTime = 0.0;
												}
												else
													m_LastClickTime = Application.InternalTimer.Time;
											}
										}
									}
									else {
										// mouse went down
										args = new UIMouseButtonEventArgs(UIEventType.MouseDown, hit.Target, new Point2(hit.X, hit.Y), btn);
										m_MouseEventReceiver((UIMouseButtonEventArgs)args);
										if( !args.PreventDefault ) {
											m_ClickButton = btn;
											m_ClickMouseX = mouse.X;
											m_ClickMouseY = mouse.Y;
										}
									}
								}
							};
							
							checkMouseButton(MouseButton.Left);
							checkMouseButton(MouseButton.Right);
							checkMouseButton(MouseButton.Middle);
						}
						
						handlingFlags = handlingFlags & ~InputHandlingFlags.Mouse;
					}
					else {
						SetMouseOver(hit.Target, hit.X, hit.Y);
	
						if( hit.Target != null ) {
							handlingFlags = handlingFlags & ~InputHandlingFlags.Mouse;
							
							if( mouseMoved ) {
								args = new UIMouseMoveEventArgs(UIEventType.MouseMove, hit.Target, new Point2(hit.X, hit.Y), new Point2(mouse.DeltaX, mouse.DeltaY));
								for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
									if( tmp.OnMouseMove != null )
										tmp.OnMouseMove((UIMouseMoveEventArgs)args);
							}
							
							if( wheelMoved ) {
								args = new UIMouseWheelEventArgs(UIEventType.MouseWheel, hit.Target, new Point2(hit.X, hit.Y), mouse.DeltaWheel);
								for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
									if( tmp.OnMouseWheel != null )
										tmp.OnMouseWheel((UIMouseWheelEventArgs)args);
										
								if( !args.PreventDefault ) {
									// Check if there is a scroll bar as a direct child and invoke mouse wheel event on it if there is one.
									Component vscb = null, hscb = null;
									foreach( Component child in hit.Target.Children ) {
										if( child is VScrollBar ) {
											vscb = child;
											if( hscb != null )
												break;
										}
										else if( child is HScrollBar ) {
											hscb = child;
											if( vscb != null )
												break;
										}
									}
									if( vscb != null )
										hscb = vscb;
									if( hscb != null && hscb.OnMouseWheel != null )
										hscb.OnMouseWheel((UIMouseWheelEventArgs)args);
								}
							}
							
							if( buttonChanged ) {
								MouseButtonCheckDelegate checkMouseButton = btn => {
									if( (mouse.ChangedButtons & btn) != MouseButton.None ) {
										if( (mouse.Buttons & btn) == MouseButton.None ) {
											// button went up
											args = new UIMouseButtonEventArgs(UIEventType.MouseUp, hit.Target, new Point2(hit.X, hit.Y), btn);
											for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
												if( tmp.OnMouseUp != null )
													tmp.OnMouseUp((UIMouseButtonEventArgs)args);
											
											if( !args.PreventDefault && m_ClickButton == btn ) {
												m_ClickButton = MouseButton.None;
												m_ClickMouseX = -1;
												m_ClickMouseY = -1;
												
												args = new UIMouseButtonEventArgs(UIEventType.MouseClick, hit.Target, new Point2(hit.X, hit.Y), btn);
												for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
													if( tmp.OnMouseClick != null )
														tmp.OnMouseClick((UIMouseButtonEventArgs)args);
												
												if( !args.PreventDefault ) {
													if( m_LastClickTime + DoubleClickTimeout >= Application.InternalTimer.Time ) {
														args = new UIMouseButtonEventArgs(UIEventType.MouseDblClick, hit.Target, new Point2(hit.X, hit.Y), btn);
														for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
															if( tmp.OnMouseDblClick != null )
																tmp.OnMouseDblClick((UIMouseButtonEventArgs)args);
														m_LastClickTime = 0.0;
													}
													else
														m_LastClickTime = Application.InternalTimer.Time;
												}
											}
										}
										else {
											// mouse went down
											args = new UIMouseButtonEventArgs(UIEventType.MouseDown, hit.Target, new Point2(hit.X, hit.Y), btn);
											for( tmp = hit.Target; tmp != null && !args.StopPropagation; tmp = tmp.m_Parent )
												if( tmp.OnMouseDown != null )
													tmp.OnMouseDown((UIMouseButtonEventArgs)args);
											if( !args.PreventDefault ) {
												m_ClickButton = btn;
												m_ClickMouseX = mouse.X;
												m_ClickMouseY = mouse.Y;
											}
										}
									}
								};
								
								checkMouseButton(MouseButton.Left);
								checkMouseButton(MouseButton.Right);
								checkMouseButton(MouseButton.Middle);
							}
						}
					}
				}
				else {
					// mouse state is same as in previous frame
					if( m_MouseEventReceiver != null || m_LastHit.Target != null ) {
						handlingFlags = handlingFlags & ~InputHandlingFlags.Mouse;
					}
				}
			}
		}

		private static void SetMouseOver(Component target, int x, int y) {
			if( target != null )
				target.SetMouseOver(x, y);
			else if( m_MouseOver != null ) {
				if( m_MouseOver.OnMouseOut != null )
					m_MouseOver.OnMouseOut(new UIMouseOverEventArgs(UIEventType.MouseOut, m_MouseOver, new Point2(x, y), m_MouseOver, null));
				for( Component p = m_MouseOver.m_Parent; p != null; p = p.m_Parent )
					p.m_MouseOverChild = false;
				m_MouseOver = null;
			}
		}

		private void SetMouseOver(int x, int y) {
			if( m_MouseOver == this || !CanHit )
				return;
			Component p, from = m_MouseOver;
			if( from != null ) {
				if( from.OnMouseOut != null )
					from.OnMouseOut(new UIMouseOverEventArgs(UIEventType.MouseOut, from, new Point2(x, y), from, this));
				for( p = from.m_Parent; p != null; p = p.m_Parent )
					p.m_MouseOverChild = false;
			}
			m_MouseOver = this;
			if( OnMouseOver != null )
				OnMouseOver(new UIMouseOverEventArgs(UIEventType.MouseOver, this, new Point2(x, y), from, this));
			for( p = m_MouseOver.m_Parent; p != null; p = p.m_Parent )
				p.m_MouseOverChild = true;
		}
		#endregion Internals
	}
}
