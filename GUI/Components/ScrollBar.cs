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

using IGE;
using IGE.Graphics;

namespace IGE.GUI {
	public abstract class ScrollBar : Component, IUIBackground {
		protected int m_Value = 0;
		protected int m_MinValue = 0;
		protected int m_MaxValue = 100;
		
		public abstract UIEventType ScrollEventType { get; }
		
		public int Value {
			get { return m_Value; }
			set {
				int val = value;
				if( val < m_MinValue ) val = m_MinValue;
				else if( val > m_MaxValue ) val = m_MaxValue;
				if( m_Value != val ) {
					int delta = val - m_Value;
					m_Value = val;
					Update();
					if( OnScroll != null )
						OnScroll(new UIScrollEventArgs(ScrollEventType, this, val, delta));
				}
			}
		}
		
		public int MinValue {
			get { return m_MinValue; }
			set {
				if( m_MinValue != value ) {
					if( value > m_MaxValue )
						throw new Exception("MinValue cannot be greater than MaxValue");
					m_MinValue = value;
					if( m_Value < m_MinValue ) {
						int delta = m_MinValue - m_Value;
						m_Value = m_MinValue;
						Update();
						if( OnScroll != null )
							OnScroll(new UIScrollEventArgs(ScrollEventType, this, m_Value, delta));
					}
					else
						Update();
				}
			}
		}
		
		public int MaxValue {
			get { return m_MaxValue; }
			set {
				if( m_MaxValue != value ) {
					if( value < m_MinValue )
						throw new Exception("MaxValue cannot be smaller than MinValue");
					m_MaxValue = value;
					if( m_Value > m_MaxValue ) {
						int delta = m_MaxValue - m_Value;
						m_Value = m_MaxValue;
						Update();
						if( OnScroll != null )
							OnScroll(new UIScrollEventArgs(ScrollEventType, this, m_Value, delta));
					}
					else
						Update();
				}
			}
		}
		
		public event UIScrollEventHandler OnScroll;
		
		protected ISizedRenderable2D m_Background = null;
		public ISizedRenderable2D Background { get { return m_Background; } set { m_Background = value; Component.UIInvalid = true; } }
		
		private ScrollBox m_ScrollBox = null;
		public ScrollBox ScrollBox { get { return m_ScrollBox; } }
		
		public override bool CanHit { get { return true; } }

		public ScrollBar(int x, int y, int width, int height, int min, int max) : base(x, y, width, height) {
			OnMouseWheel += OnMouseWheelEvent;
			OnResize += OnResizeEvent;
		
			m_MinValue = min;
			m_MaxValue = max;
			
			m_ScrollBox = new ScrollBox();
			m_ScrollBox.Area = new Rectangle(0, 0, m_Area.Width, m_Area.Height);
			Add(m_ScrollBox);
			
			Update();
		}
		
		public ScrollBar(int x, int y, int width, int height) : this(x, y, width, height, 0, 100) {
		}
		
		/// <summary>
		/// This method is called to update ScrollBox position when values were changed by code
		/// </summary>
		public virtual void Update() {
			UIEventType type = ScrollEventType;
			int fullSize = (type == UIEventType.VScroll) ? m_Area.Height : m_Area.Width;
			int valueDelta = m_MaxValue - m_MinValue + 1;
			
			int blockSize = fullSize / valueDelta;
			if( blockSize < 4 )
				blockSize = 4;
			if( blockSize > fullSize ) // in case when full size is less than 4
				blockSize = fullSize;
			int leftover = fullSize - blockSize;
			int pos = (valueDelta == 1) ? 0 : (int)(((long)leftover * (long)(m_Value - m_MinValue)) / (long)(valueDelta - 1));
			
			Rectangle rect = m_ScrollBox.Area;
			if( type == UIEventType.VScroll ) {
				rect.X = 0;
				rect.Y = pos;
				rect.Width = m_Area.Width;
				rect.Height = blockSize;
			}
			else {
				rect.X = pos;
				rect.Y = 0;
				rect.Width = blockSize;
				rect.Height = m_Area.Height;
			}
			m_ScrollBox.Area = rect;
		}
		
		public virtual void OnResizeEvent(UIResizeEventArgs args) {
			Update();
		}
		
		public virtual void OnScrollBoxMove() {
		
		}
		
		public virtual void OnMouseWheelEvent(UIMouseWheelEventArgs args) {
			Value -= args.Wheel; // inverse because wheel up is positive while positive values are down
		}
	
		public override void Render(int x, int y) {
			if( m_Background != null )
				m_Background.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			base.Render(x, y);
		}
	}
}
