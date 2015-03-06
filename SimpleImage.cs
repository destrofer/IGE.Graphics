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
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public class SimpleImage : IRenderable2D {
		protected Texture m_Texture;
		protected float m_U1;
		protected float m_V1;
		protected float m_U2;
		protected float m_V2;
		protected int m_Width;
		protected int m_Height;
		
		public virtual Texture Texture { get { return m_Texture; } }
		
		/// <summary>
		/// Constructs an Image object using whole passed texture with center at top left corner
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		public SimpleImage(Texture texture) : this( texture, 0, 0, texture.Width, texture.Height ) {}
		
		/// <summary>
		/// Constructs an Image object using part (defined by u,v,width and height) of passed texture and passed center coordinates
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		/// <param name="u">U coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="v">V coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="width">Width of cutout (in pixels)</param>
		/// <param name="height">Height of cutout (in pixels)</param>
		public SimpleImage(Texture texture, int u, int v, int width, int height) {
			m_Texture = texture;
			m_U1 = (float)u / (float)texture.Width;
			m_V1 = (float)v / (float)texture.Height;
			m_Width = (width <= 0) ? texture.Width : width;
			m_Height = (height <= 0) ? texture.Height : height;
			m_U2 = m_U1 + (float)m_Width / (float)texture.Width;
			m_V2 = m_V1 + (float)m_Height / (float)texture.Height;
		}
		
		public virtual void Render() {
			m_Texture.Bind();
			GL.Begin(BeginMode.TriangleStrip);
			if( View.YAxisIsUp ) {
				GL.TexCoord2(m_U2, m_V1); GL.Vertex2((float)m_Width, 0f);
				GL.TexCoord2(m_U1, m_V1); GL.Vertex2(0f, 0f);
				GL.TexCoord2(m_U2, m_V2); GL.Vertex2((float)m_Width, -(float)m_Height);
				GL.TexCoord2(m_U1, m_V2); GL.Vertex2(0f, -(float)m_Height);
			}
			else {
				GL.TexCoord2(m_U2, m_V1); GL.Vertex2((float)m_Width, 0f);
				GL.TexCoord2(m_U1, m_V1); GL.Vertex2(0f, 0f);
				GL.TexCoord2(m_U2, m_V2); GL.Vertex2((float)m_Width, (float)m_Height);
				GL.TexCoord2(m_U1, m_V2); GL.Vertex2(0f, (float)m_Height);
			}
			GL.End();
		}
		
		public virtual void Render(int x, int y) {
			float x1 = (float)x;
			float y1 = (float)y;
			float x2 = (float)(x + m_Width);
			float y2 = (float)(y + (View.YAxisIsUp ? -m_Height : m_Height));
			m_Texture.Bind();
			GL.Begin(BeginMode.TriangleStrip);
			GL.TexCoord2(m_U2, m_V1); GL.Vertex2(x2, y1);
			GL.TexCoord2(m_U1, m_V1); GL.Vertex2(x1, y1);
			GL.TexCoord2(m_U2, m_V2); GL.Vertex2(x2, y2);
			GL.TexCoord2(m_U1, m_V2); GL.Vertex2(x1, y2);
			GL.End();
		}
		
		public virtual void RenderMasked(int x, int y, SimpleImage mask) {
			float x1 = (float)x;
			float y1 = (float)y;
			float x2 = (float)(x + m_Width);
			float y2 = (float)(y + (View.YAxisIsUp ? -m_Height : m_Height));
			m_Texture.Bind(0);
			// GL.Enable(EnableCap.Texture2D);
			mask.Texture.Bind(1);
			GL.Begin(BeginMode.TriangleStrip);
				SetMTCoord(0, 3); mask.SetMTCoord(1, 3); GL.Vertex2(x2, y1);
				SetMTCoord(0, 2); mask.SetMTCoord(1, 2); GL.Vertex2(x1, y1);
				SetMTCoord(0, 1); mask.SetMTCoord(1, 1); GL.Vertex2(x2, y2);
				SetMTCoord(0, 0); mask.SetMTCoord(1, 0); GL.Vertex2(x1, y2);
			GL.End();
			// GL.Disable(EnableCap.Texture2D);
			// Texture.SetActiveUnit(0);
		}
		
		public virtual void SetMTCoord(int texture_unit_index, int vertex_index) {
			switch(vertex_index) {
				case 0: Texture.SetMTCoord(texture_unit_index, m_U2, m_V1); break;
				case 1: Texture.SetMTCoord(texture_unit_index, m_U1, m_V1); break;
				case 2: Texture.SetMTCoord(texture_unit_index, m_U2, m_V2); break;
				case 3: Texture.SetMTCoord(texture_unit_index, m_U1, m_V2); break;
			}
		}
	}
}
