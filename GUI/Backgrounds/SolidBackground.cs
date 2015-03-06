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

using IGE.Graphics.OpenGL;

namespace IGE.GUI {
	public class SolidBackground : ISizedRenderable2D {
		protected Color4 m_Color = Color4.Zero;
		public Color4 Color { get { return m_Color; } set { m_Color = value; } }
		
		public SolidBackground() {
		}

		public SolidBackground(Color4 color) {
			m_Color = color;
		}
		
		public SolidBackground(DomNode node) {
			Load(node);
		}
		
		public virtual void Load(DomNode node) {
			if( !Color4.TryParse(node["color"], out m_Color) )
				m_Color = Color4.Black;
		}
		
		public virtual void Render(int x, int y, int width, int height) {
			GL.PushAttrib(AttribBits.EnableBit);
			GL.Disable(EnableCap.Texture2D);
			GL.Color4(ref m_Color);
			GL.Begin(BeginMode.TriangleStrip);
				GL.Vertex2(x, y + height);
				GL.Vertex2(x + width, y + height);
				GL.Vertex2(x, y);
				GL.Vertex2(x + width, y);
			GL.End();
			GL.PopAttrib();
		}
	}
}
