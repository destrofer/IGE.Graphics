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
	public class Label : Component, IUIText {
		protected Color4 m_Color = Color4.White;
		public Color4 TextColor { get { return m_Color; } set { m_Color = value; RecompileText(); } } // changing color does not affect render area so UI it not marked as invalid
		
		protected Font m_Font = null;
		public Font Font { get { return m_Font; } set { m_Font = value; RecompileText(); Component.UIInvalid = true; } }
		
		protected string m_Text = null;
		public string Text { get { return m_Text; } set { m_Text = value; RecompileText(); Component.UIInvalid = true; } }
		
		protected UITextAlign m_TextAlign = UITextAlign.Left;
		public UITextAlign TextAlign { get { return m_TextAlign; } set { m_TextAlign = value; RecompileText(); Component.UIInvalid = true; } }

		protected string m_CompiledText = null;
		protected TextInfo m_CompiledInfo = null;
		
		public Label() : base() {
		}
		
		public Label(int x, int y, string text) : base(x, y, 0, 0) {
			Text = text;
		}

		public override void SetDefaultStyle() {
			m_Color = UI.Defaults.Label.Color;
			m_Font = UI.Defaults.Label.Font;
		}
		
		public virtual void RecompileText() {
			if( m_Font == null || m_Text == null || m_Text.Length == 0 ) {
				m_CompiledText = null;
				m_CompiledInfo = null;
			}
			else {
				m_CompiledText = String.Format("⌂{0}⌂{1}{2}", (m_TextAlign == UITextAlign.Right) ? '>' : ((m_TextAlign == UITextAlign.Center) ? '|' : '<'), m_Color.ToString("X"), m_Text);
				m_CompiledInfo = Font.GetTextInfo(m_CompiledText);
			}
		}
		
		public override void Render(int x, int y) {
			if( Font != null && m_CompiledText != null )
				Font.Print(x + m_Area.X, y + m_Area.Y, "{0}", m_CompiledText);
			base.Render(x, y);
		}
	}
}
