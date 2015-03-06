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
using System.Collections.Generic;

using IGE.Platform;

namespace IGE.GUI {
	public class PopupMenu : Component, IUIBackground, IUIBorder, IMenu {
		protected ISizedRenderable2D m_Background = null;
		public ISizedRenderable2D Background { get { return m_Background; } set { m_Background = value; Component.UIInvalid = true; } }

		protected ISizedRenderable2D m_Border = null;
		public ISizedRenderable2D Border { get { return m_Border; } set { m_Border = value; Component.UIInvalid = true; } }

		protected Font m_Font = null;
		protected UIDefaults.UIOffsetDefaults m_MenuPadding;
		protected UIDefaults.UIOffsetDefaults m_ItemPadding;
		protected UIDefaults.UISpacingDefaults m_ItemSpacing;
		protected UIDefaults.UIComponentStateDefaultsCollection m_ItemStates;

		protected ISizedRenderable2D m_Separator = null;
		public ISizedRenderable2D Separator { get { return m_Separator; } set { m_Separator = value; Component.UIInvalid = true; } }
		
		public override bool CanHit { get { return true; } }
		public override bool CanHaveFocus { get { return true; } }
		public override bool ClipChildren { get { return false; } }
		
		protected List<MenuItem> m_Items = null;
		
		protected int m_HoverItemIndex = -1;
		
		public event UIMenuEventHandler OnItemOver;
		public event UIMenuEventHandler OnItemOut;
		public event UIMenuEventHandler OnItemClick;
		public event UIMenuEventHandler OnMenuOut;
		
		public PopupMenu() : this(0, 0, null) {
		}

		public PopupMenu(MenuItem[] items) : this(0, 0, items) {
		}

		public PopupMenu(int x, int y) : this(x, y, null) {
		}

		public PopupMenu(int x, int y, MenuItem[] items) : base(x, y, 0, 0) {
			m_Items = (items != null) ? new List<MenuItem>(items) : new List<MenuItem>();
			
			foreach( MenuItem item in m_Items )
				item.RecalcSize(m_Font);
			Invalidate();
			
			OnMouseMove += delegate(UIMouseMoveEventArgs args) {
				if( args.Target != this )
					return;

				int over = m_HoverItemIndex;
				MenuItem item;
				
				m_HoverItemIndex = ItemHitTest(args.Cursor);
				if( over != m_HoverItemIndex ) {
					UIMenuEventArgs margs;
					if( over >= 0 && over < m_Items.Count ) {
						item = m_Items[over];
						margs = new UIMenuEventArgs(this, item);
						if( item.OnOut != null )
							item.OnOut(margs);
						if( OnItemOut != null )
							OnItemOut(margs);
					}

					if( m_HoverItemIndex >= 0 && m_HoverItemIndex < m_Items.Count ) {
						item = m_Items[m_HoverItemIndex];
						margs = new UIMenuEventArgs(this, item);
						if( item.OnOver != null )
							item.OnOver(margs);
						if( OnItemOver != null )
							OnItemOver(margs);
					}
				}
			};
			
			OnMouseOut += delegate(UIMouseOverEventArgs args) {
				if( args.Target != this )
					return;
				if( args.OverTo is PopupMenu ) {
					if( args.OverTo.Parent == this )
						return;
				}

				int over = m_HoverItemIndex;
				MenuItem item;
				m_HoverItemIndex = -1;
				if( over >= 0 && over < m_Items.Count ) {
					item = m_Items[over];
					UIMenuEventArgs margs = new UIMenuEventArgs(this, item);
					if( item.OnOut != null )
						item.OnOut(margs);
					if( OnItemOut != null )
						OnItemOut(margs);
				}
				
				if( !(args.OverTo is PopupMenu) ) {
					PopupMenu rootMenu = this;
					while( rootMenu.Parent is PopupMenu ) {
						rootMenu.m_HoverItemIndex = -1;
						if( rootMenu.OnMenuOut != null )
							rootMenu.OnMenuOut(new UIMenuEventArgs(rootMenu, null));
						rootMenu = (PopupMenu)rootMenu.Parent;
					}
					rootMenu.m_HoverItemIndex = -1;
					if( rootMenu.OnMenuOut != null )
						rootMenu.OnMenuOut(new UIMenuEventArgs(rootMenu, null));
				}
			};
			
			OnMouseOver += delegate(UIMouseOverEventArgs args) {
				if( args.Target != this )
					return;
			};
			
			OnMouseClick += delegate(UIMouseButtonEventArgs args) {
				if( args.Target != this )
					return;
				
				MenuItem item;
				if( m_HoverItemIndex >= 0 && m_HoverItemIndex < m_Items.Count ) {
					item = m_Items[m_HoverItemIndex];
					UIMenuEventArgs margs = new UIMenuEventArgs(this, item);
					if( item.OnClick != null )
						item.OnClick(margs);
					if( OnItemClick != null )
						OnItemClick(margs);
				}
			};
		}
		
		public override void SetDefaultStyle() {
			m_Background = UI.Defaults.PopupMenu.Background;
			m_Border = UI.Defaults.PopupMenu.Border;
			m_Font = UI.Defaults.PopupMenu.Font;
			m_MenuPadding = UI.Defaults.PopupMenu.MenuPadding;
			m_ItemPadding = UI.Defaults.PopupMenu.ItemPadding;
			m_ItemSpacing =  UI.Defaults.PopupMenu.ItemSpacing;
			m_Separator = UI.Defaults.PopupMenu.Separator;
			m_ItemStates = UI.Defaults.PopupMenu.ItemStates;
		}
		
		public int ItemHitTest(Point2 point) {
			return ItemHitTest(point.X, point.Y);
		}
		
		public virtual int ItemHitTest(int x, int y) {
			int i, h;
			MenuItem item;
			
			y -= m_MenuPadding.Top;
			
			for( i = 0; i < m_Items.Count; i++ ) {
				item = m_Items[i];
				if( item == null || item is MenuItemSeparator )
					h = m_ItemSpacing.Size;
				else {
					h = item.Height + m_ItemPadding.Top + m_ItemPadding.Bottom;
					if( x >= 0 && x <= m_Area.Width && y >= 0 && y <= h )
						return i;
				}
				y -= h + m_ItemSpacing.Space;
			}
			
			return -1;
		}
		
		public bool GetItemArea(MenuItem searchItem, out int x, out int y, out int width, out int height) {
			x = m_MenuPadding.Left;
			y = m_MenuPadding.Top;
			width = m_Area.Width - m_MenuPadding.Left - m_MenuPadding.Right;
			height = 0;
			
			foreach(MenuItem item in m_Items) {
				if( searchItem == item )
					return true;
				if( item == null || item is MenuItemSeparator )
					height = m_ItemSpacing.Size;
				else
					height = item.Height + m_ItemPadding.Top + m_ItemPadding.Bottom;
				y += height + m_ItemSpacing.Space;
			}

			height = 0;
			return false;
		}
		
		public override void Render(int x, int y) {
			int h;
			
			if( m_Background != null )
				m_Background.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			if( m_Border != null )
				m_Border.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			base.Render(x, y);
			
			x += m_Area.X + m_MenuPadding.Left;
			y += m_Area.Y + m_MenuPadding.Top;
			
			int idx = 0;
			foreach(MenuItem item in m_Items) {
				if( item == null || item is MenuItemSeparator ) {
					if( m_Separator != null && m_ItemSpacing.Size > 0 )
						m_Separator.Render(x, y, m_Area.Width - m_MenuPadding.Left - m_MenuPadding.Right, m_ItemSpacing.Size);
					y += m_ItemSpacing.Size;
				}
				else {
					h = item.Height + m_ItemPadding.Top + m_ItemPadding.Bottom;
					if( idx == m_HoverItemIndex ) {
						item.Render(
							UIComponentState.Hover,
							x, y, m_Area.Width - m_MenuPadding.Left - m_MenuPadding.Right, h,
							m_ItemPadding.Top, m_ItemPadding.Right, m_ItemPadding.Bottom, m_ItemPadding.Left,
							m_Font, m_ItemStates.Hover.Color, m_ItemStates.Hover.Background, m_ItemStates.Hover.Border
						);
					}
					else {
						item.Render(
							UIComponentState.None,
							x, y, m_Area.Width - m_MenuPadding.Left - m_MenuPadding.Right, h,
							m_ItemPadding.Top, m_ItemPadding.Right, m_ItemPadding.Bottom, m_ItemPadding.Left,
							m_Font, m_ItemStates.Normal.Color, m_ItemStates.Normal.Background, m_ItemStates.Normal.Border
						);
					}
					y += h;
				}
				y += m_ItemSpacing.Space;
				idx++;
			}
		}
		
		public virtual void Invalidate() {
			int width = 0, height = 0;
			foreach(MenuItem item in m_Items) {
				if( item == null || item is MenuItemSeparator ) {
					height += m_ItemSpacing.Size;
				}
				else {
					if( item.Width > width )
						width = item.Width;
					height += item.Height + m_ItemPadding.Top + m_ItemPadding.Bottom;
				}
				height += m_ItemSpacing.Space;
			}

			width += m_MenuPadding.Left + m_MenuPadding.Right + m_ItemPadding.Left + m_ItemPadding.Right;
			height += m_MenuPadding.Top + m_MenuPadding.Bottom;
			
			Width = width;
			Height = height;
		}
		
		public void MoveTo(int x, int y) {
			MoveTo(new Point2(x, y));
		}
		
		public virtual void MoveTo(Point2 newPosition) {
			Location = newPosition;
			foreach(MenuItem item in m_Items) {
				if( item != null )
					item.OnMenuMove();
			}
		}
		
		public virtual void AddItem(MenuItem item) {
			if( item != null )
				item.RecalcSize(m_Font);
			m_Items.Add(item);
			Invalidate();
		}
	}
}
