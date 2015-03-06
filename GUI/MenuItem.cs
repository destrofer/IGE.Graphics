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
	public interface IMenu : IDisposable, IUIComponent {
		event UIMenuEventHandler OnItemOver;
		event UIMenuEventHandler OnItemOut;
		event UIMenuEventHandler OnItemClick;
		
		void AddItem(MenuItem item);
		bool GetItemArea(MenuItem item, out int x, out int y, out int width, out int height);
	}
	
	public interface IMenuItem {
		int Id { get; set; }
		string Name { get; set; }
		object Data { get; set; }
		int Width { get; set; }
		int Height { get; set; }
		void Invalidate();
		void RecalcSize(Font font);
		void Render(UIComponentState state, int x, int y, int width, int height, int paddingTop, int paddingRight, int paddingBottom, int paddingLeft, Font font, Color4 color, ISizedRenderable2D background, ISizedRenderable2D border);
	}
	
	/// <summary>
	/// </summary>
	public class MenuItem : IMenuItem {
		protected int m_Id;
		protected string m_Name;
		protected object m_Data;
		protected int m_Width = 0;
		protected int m_Height = 0;
		
		public UIMenuEventHandler OnOver = null;
		public UIMenuEventHandler OnOut = null;
		public UIMenuEventHandler OnClick = null;
		
		public virtual int Id {
			get { return m_Id; }
			set { m_Id = value; }
		}
		
		public virtual string Name {
			get { return m_Name; }
			set { m_Name = value; Invalidate(); }
		}
		
		public virtual object Data {
			get { return m_Data; }
			set { m_Data = value; }
		}
		
		public virtual int Width {
			get { return m_Width; }
			set { m_Width = value; }
		}
		
		public virtual int Height {
			get { return m_Height; }
			set { m_Height = value; }
		}
		
		public TextBox[] TextCache = new TextBox[(int)UIComponentState.Max + 1];
		
		public MenuItem(int id, string name, object data) : this(id, name, data, null, null, null) {
		}

		public MenuItem(int id, string name, object data, UIMenuEventHandler onClick) : this(id, name, data, onClick, null, null) {
		}

		public MenuItem(int id, string name, object data, UIMenuEventHandler onClick, UIMenuEventHandler onOver) : this(id, name, data, onClick, onOver, null) {
		}

		public MenuItem(int id, string name, object data, UIMenuEventHandler onClick, UIMenuEventHandler onOver, UIMenuEventHandler onOut) {
			Id = id;
			Name = name;
			Data = data;
			OnOver = onOver;
			OnOut = onOut;
			OnClick = onClick;
		}
		
		public virtual void OnMenuMove() {
			Invalidate();
		}
		
		public virtual void Invalidate() {
			for( int i = TextCache.Length - 1; i >= 0; i-- )
				TextCache[i] = null;
		}
		
		public virtual void RecalcSize(Font font) {
			if( font != null && m_Name != null ) {
				TextBox box = new TextBox(m_Name, font, Color4.White, 0, 0, 0, 0, UITextAlign.Left, UIVerticalAlign.Top, 0);
				box.GetTextSize(out m_Width, out m_Height);
			}
		}
		
		public virtual void Render(UIComponentState state, int x, int y, int width, int height, int paddingTop, int paddingRight, int paddingBottom, int paddingLeft, Font font, Color4 color, ISizedRenderable2D background, ISizedRenderable2D border) {
			int stt = (int)state;
			if( TextCache[stt] == null )
				TextCache[stt] = new TextBox(Name, font, color, x + paddingLeft, y + paddingTop, width - paddingLeft - paddingRight, height - paddingTop - paddingBottom, UITextAlign.Left, UIVerticalAlign.Center, 0);
			TextBox box = TextCache[stt];
			if( background != null )
				background.Render(x, y, width, height);
			if( border != null )
				border.Render(x, y, width, height);
			box.Render();
		}
	}
	
	public class MenuItemSeparator : MenuItem {
		public MenuItemSeparator()
			: base(-1, null, null)
		{
		}
	}
	
	public class MenuItemSubmenu : MenuItem {
		protected PopupMenu m_Submenu;
		protected bool m_MenuAdded = false;
		
		public MenuItemSubmenu(string name, MenuItem[] submenuItems)
			: base(-1, name, null)
		{
			m_Submenu = new PopupMenu(submenuItems);
			
			OnOver = delegate(UIMenuEventArgs args) {
				m_Submenu.Parent = (Component)args.Menu;
				int x, y, w, h;
				args.Menu.GetItemArea(this, out x, out y, out w, out h);
				m_Submenu.MoveTo(args.Menu.Width, y); // x + w
				
				if( !m_MenuAdded )
					args.Menu.Add(m_Submenu);
				
				m_Submenu.Visible = true;
			};
			
			OnOut = delegate(UIMenuEventArgs args) {
				m_Submenu.Visible = false;
			};
		}
		
		public void AddItem(MenuItem item) {
			m_Submenu.AddItem(item);
		}
		
		public override void OnMenuMove() {
			base.OnMenuMove();
			m_Submenu.Visible = false;
		}
	}
}
