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
	public class ScrollBox : Component, IUIBackground, IUIBorder {
		protected ISizedRenderable2D m_Background = null;
		public ISizedRenderable2D Background { get { return m_Background; } set { m_Background = value; Component.UIInvalid = true; } }

		protected ISizedRenderable2D m_Border = null;
		public ISizedRenderable2D Border { get { return m_Border; } set { m_Border = value; Component.UIInvalid = true; } }

		public ScrollBox() {
		}
	
		public override void SetDefaultStyle() {
			if( m_Parent != null && m_Parent is HScrollBar ) {
				m_Background = UI.Defaults.HScroll.BoxBackground;
				m_Border = UI.Defaults.HScroll.BoxBorder;
			}
			else {
				m_Background = UI.Defaults.VScroll.BoxBackground;
				m_Border = UI.Defaults.VScroll.BoxBorder;
			}
		}
		
		public override void Render(int x, int y) {
			if( m_Background != null )
				m_Background.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			if( m_Border != null )
				m_Border.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			base.Render(x, y);
		}
	}
}
