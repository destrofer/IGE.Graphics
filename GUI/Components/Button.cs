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
	public class Button : Component {
		public Font Font = null;
		public UIDefaults.UIComponentStateDefaultsCollection StateStyles = UIDefaults.UIComponentStateDefaultsCollection.Default;
		
		protected string m_Text = null;
		public string Text { get { return m_Text; } set { m_Text = value; RecompileText(); Component.UIInvalid = true; } }
		
		protected UITextAlign m_TextAlign = UITextAlign.Center;
		public UITextAlign TextAlign { get { return m_TextAlign; } set { m_TextAlign = value; RecompileText(); Component.UIInvalid = true; } }

		protected TextBox[] m_CompiledText = null;
		
		protected UIComponentState m_State = UIComponentState.None;
		
		public override bool CanHit { get { return true; } }
		public override bool CanHaveFocus { get { return true; } }
		
		public Button() : base() {
		}
		
		public Button(int x, int y, int width, int height, string text) : base(x, y, width, height) {
			Text = text;
		}

		public override void SetDefaultStyle() {
			Font = UI.Defaults.Button.Font;
			StateStyles = UI.Defaults.Button.States;
			
			OnMouseOver += MouseOver;
			OnMouseOut += MouseOut;
			OnMouseDown += MouseDown;
			OnMouseUp += MouseUp;
		}
		
		protected virtual void MouseOver(UIMouseOverEventArgs args) {
			m_State = UIComponentState.Hover;
		}
		
		protected virtual void MouseOut(UIMouseOverEventArgs args) {
			m_State = UIComponentState.None;
		}
		
		protected virtual void MouseDown(UIMouseButtonEventArgs args) {
			m_State = UIComponentState.MouseDown;
		}
		
		protected virtual void MouseUp(UIMouseButtonEventArgs args) {
			m_State = UIComponentState.Hover;
		}
		
		public virtual void RecompileText() {
			if( m_Text == null || m_Text.Length == 0 )
				m_CompiledText = null;
			else {
				m_CompiledText = new TextBox[] {
					new TextBox(m_Text, Font, StateStyles.Normal.Color, 0, 0, 0, 0, m_TextAlign, UIVerticalAlign.Center, 0),
					new TextBox(m_Text, Font, StateStyles.Focus.Color, 0, 0, 0, 0, m_TextAlign, UIVerticalAlign.Center, 0),
					new TextBox(m_Text, Font, StateStyles.Hover.Color, 0, 0, 0, 0, m_TextAlign, UIVerticalAlign.Center, 0),
					new TextBox(m_Text, Font, StateStyles.MouseDown.Color, 0, 0, 0, 0, m_TextAlign, UIVerticalAlign.Center, 0)
				};
			}
		}
		
		public override void Render(int x, int y) {
			UIDefaults.UIComponentStateDefaults style = StateStyles[m_State];
			if( style.Background != null )
				style.Background.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			if( style.Border != null )
				style.Border.Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
			base.Render(x, y);
			
			if( m_CompiledText != null )
				m_CompiledText[(int)m_State].Render(x + m_Area.X, y + m_Area.Y, m_Area.Width, m_Area.Height);
		}
	}
}
