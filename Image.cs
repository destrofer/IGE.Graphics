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
	public class Image : SimpleImage {
		protected int m_CenterX;
		protected int m_CenterY;
		
		/// <summary>
		/// Constructs an Image object using whole passed texture with center at top left corner
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		public Image(Texture texture) : this( texture, 0, 0, texture.Width, texture.Height, 0, 0 ) {}
		
		/// <summary>
		/// Constructs an Image object using whole passed texture and passed center coordinates
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		/// <param name="center_x">X coordinate of image center on resulting cutout (in pixels)</param>
		/// <param name="center_y">Y coordinate of image center on resulting cutout (in pixels)</param>
		public Image(Texture texture, int center_x, int center_y) : this( texture, 0, 0, texture.Width, texture.Height, center_x, center_y ) {}
		
		/// <summary>
		/// Constructs an Image object using part (defined by u,v,width and height) of passed texture
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		/// <param name="u">U coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="v">V coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="width">Width of cutout (in pixels)</param>
		/// <param name="height">Height of cutout (in pixels)</param>
		public Image(Texture texture, int u, int v, int width, int height) : this( texture, u, v, width, height, 0, 0 ) {}
		
		/// <summary>
		/// Constructs an Image object using part (defined by u,v,width and height) of passed texture and passed center coordinates
		/// </summary>
		/// <param name="texture">Reference to a loaded texture</param>
		/// <param name="center_x">X coordinate of image center on resulting cutout (in pixels)</param>
		/// <param name="center_y">Y coordinate of image center on resulting cutout (in pixels)</param>
		/// <param name="u">U coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="v">V coordinate on texture of the cutout's left top corner (in pixels)</param>
		/// <param name="width">Width of cutout (in pixels)</param>
		/// <param name="height">Height of cutout (in pixels)</param>
		public Image(Texture texture, int u, int v, int width, int height, int center_x, int center_y) : base(texture, u, v, width, height) {
			m_CenterX = center_x;
			m_CenterY = center_y;
		}

		public override void Render() { Render(0, 0, false, false); }
		public virtual void Render(bool xflip) { Render(0, 0, xflip, false); }
		public virtual void Render(bool xflip, bool yflip) { Render(0, 0, xflip, yflip); }
		public override void Render(int x, int y) { Render(x, y, false, false); }
		public virtual void Render(int x, int y, bool xflip) { Render(x, y, xflip, false); }
		public virtual void Render(int x, int y, bool xflip, bool yflip) {
			// Rewriting this method because transforming and calling Render()
			// of the base class would be slow.
			float x1, y1, x2, y2;
			
			if( yflip ) {
				if( View.YAxisIsUp ) {
					y2 = (float)(y - m_CenterY);
					y1 = (float)(y2 + m_Height);
				}
				else {
					y2 = (float)(y + m_CenterY);
					y1 = (float)(y2 - m_Height);
				}
				if( xflip ) {
					x2 = (float)(x + m_CenterX);
					x1 = (float)(x2 - m_Width);
					
					m_Texture.Bind();
					GL.Begin(BeginMode.TriangleStrip);
					GL.TexCoord2(m_U2, m_V2); GL.Vertex2(x1, y1);
					GL.TexCoord2(m_U2, m_V1); GL.Vertex2(x1, y2);
					GL.TexCoord2(m_U1, m_V2); GL.Vertex2(x2, y1);
					GL.TexCoord2(m_U1, m_V1); GL.Vertex2(x2, y2);
					GL.End();
				}
				else {
					x1 = (float)(x - m_CenterX);
					x2 = (float)(x1 + m_Width);
					
					m_Texture.Bind();
					GL.Begin(BeginMode.TriangleStrip);
					GL.TexCoord2(m_U1, m_V2); GL.Vertex2(x1, y1);
					GL.TexCoord2(m_U1, m_V1); GL.Vertex2(x1, y2);
					GL.TexCoord2(m_U2, m_V2); GL.Vertex2(x2, y1);
					GL.TexCoord2(m_U2, m_V1); GL.Vertex2(x2, y2);
					GL.End();
				}
			}
			else {
				if( View.YAxisIsUp ) {
					y1 = (float)(y + m_CenterY);
					y2 = (float)(y1 - m_Height);
				}
				else {
					y1 = (float)(y - m_CenterY);
					y2 = (float)(y1 + m_Height);
				}
				if( xflip ) {
					x2 = (float)(x + m_CenterX);
					x1 = (float)(x2 - m_Width);
					
					m_Texture.Bind();
					GL.Begin(BeginMode.TriangleStrip);
					GL.TexCoord2(m_U2, m_V1); GL.Vertex2(x1, y1);
					GL.TexCoord2(m_U2, m_V2); GL.Vertex2(x1, y2);
					GL.TexCoord2(m_U1, m_V1); GL.Vertex2(x2, y1);
					GL.TexCoord2(m_U1, m_V2); GL.Vertex2(x2, y2);
					GL.End();
				}
				else {
					x1 = (float)(x - m_CenterX);
					x2 = (float)(x1 + m_Width);
					
					m_Texture.Bind();
					GL.Begin(BeginMode.TriangleStrip);
					GL.TexCoord2(m_U1, m_V1); GL.Vertex2(x1, y1);
					GL.TexCoord2(m_U1, m_V2); GL.Vertex2(x1, y2);
					GL.TexCoord2(m_U2, m_V1); GL.Vertex2(x2, y1);
					GL.TexCoord2(m_U2, m_V2); GL.Vertex2(x2, y2);
					GL.End();
				}
			}
			GL.Color4(1f, 1f, 1f, 1f);
			GL.Enable(EnableCap.Texture2D);
		}
	}
}
