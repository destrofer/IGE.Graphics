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
	public class GradientBackground : ISizedRenderable2D {
		protected bool m_Horizontal = false;
		protected Color4 m_ColorFrom = Color4.Zero;
		protected Color4 m_ColorTo = Color4.Zero;
		
		public Color4 ColorFrom { get { return m_ColorFrom; } set { m_ColorFrom = value; } }
		public Color4 ColorTo { get { return m_ColorTo; } set { m_ColorTo = value; } }
		
		public GradientBackground() {
		}

		public GradientBackground(bool horizontal, Color4 from, Color4 to) {
			m_Horizontal = horizontal;
			m_ColorFrom = from;
			m_ColorTo = to;
		}
		
		public GradientBackground(DomNode node) {
			Load(node);
		}
		
		public virtual void Load(DomNode node) {
			if( !Color4.TryParse(node["from"], out m_ColorFrom) )
				m_ColorFrom = Color4.Black;
			if( !Color4.TryParse(node["to"], out m_ColorTo) )
				m_ColorTo = Color4.Black;
			m_Horizontal = node["horizontal"].ToBoolean(false);
		}
		
		public virtual void Render(int x, int y, int width, int height) {
			GL.PushAttrib(AttribBits.EnableBit);
			GL.Disable(EnableCap.Texture2D);
			if( m_Horizontal ) {
				GL.Begin(BeginMode.TriangleStrip);
					GL.Color4(ref m_ColorFrom);
					GL.Vertex2(x, y);
					GL.Vertex2(x, y + height);
					GL.Color4(ref m_ColorTo);
					GL.Vertex2(x + width, y);
					GL.Vertex2(x + width, y + height);
				GL.End();
			}
			else {
				GL.Begin(BeginMode.TriangleStrip);
					GL.Color4(ref m_ColorFrom);
					GL.Vertex2(x, y + height);
					GL.Vertex2(x + width, y + height);
					GL.Color4(ref m_ColorTo);
					GL.Vertex2(x, y);
					GL.Vertex2(x + width, y);
				GL.End();
			}
			GL.PopAttrib();
		}		
	}
}
