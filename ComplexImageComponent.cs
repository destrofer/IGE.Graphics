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

using IGE.IO;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public struct ComplexImageComponent {
		private int m_X1;
		private int m_Y1;
		private int m_X2;
		private int m_Y2;
		private int m_U1;
		private int m_V1;
		private int m_U2;
		private int m_V2;
		private ComplexImageFlags m_Flags;
		private Texture m_Texture;
		
		public ComplexImageComponent(Texture tex, int x1, int y1, int x2, int y2, int u, int v, int texWidth, int texHeight, ComplexImageFlags flags) {
			ComplexImageFlags f;
			f = flags & (ComplexImageFlags.UsingLeft | ComplexImageFlags.UsingRight | ComplexImageFlags.UsingWidth);
			if( f == (ComplexImageFlags.UsingLeft | ComplexImageFlags.UsingRight | ComplexImageFlags.UsingWidth) )
				throw new UserFriendlyException("ComplexImageComponent may not have 'left', 'right' and 'width' attributes at the same time");
			if( f == ComplexImageFlags.None )
				throw new UserFriendlyException("ComplexImageComponent must have a combination of 'left'-'right', 'left'-'width' or 'right'-'width' attributes to define it's positioning");
			if( f == ComplexImageFlags.UsingLeft )
				throw new UserFriendlyException("ComplexImageComponent 'right' or 'width' attribute is missing");
			if( f == ComplexImageFlags.UsingRight )
				throw new UserFriendlyException("ComplexImageComponent 'left' or 'width' attribute is missing");
			if( f == ComplexImageFlags.UsingWidth )
				throw new UserFriendlyException("ComplexImageComponent 'left' or 'right' attribute is missing");
			
			f = flags & (ComplexImageFlags.UsingBottom | ComplexImageFlags.UsingTop | ComplexImageFlags.UsingHeight);
			if( f == (ComplexImageFlags.UsingBottom | ComplexImageFlags.UsingTop | ComplexImageFlags.UsingHeight) )
				throw new UserFriendlyException("ComplexImageComponent may not have 'bottom', 'top' and 'height' attributes at the same time");
			if( f == ComplexImageFlags.None )
				throw new UserFriendlyException("ComplexImageComponent must have a combination of 'bottom'-'top', 'bottom'-'height' or 'top'-'height' attributes to define it's positioning");
			if( f == ComplexImageFlags.UsingBottom )
				throw new UserFriendlyException("ComplexImageComponent 'top' or 'height' attribute is missing");
			if( f == ComplexImageFlags.UsingTop )
				throw new UserFriendlyException("ComplexImageComponent 'bottom' or 'height' attribute is missing");
			if( f == ComplexImageFlags.UsingHeight )
				throw new UserFriendlyException("ComplexImageComponent 'top' or 'bottom' attribute is missing");
							
			if( tex == null )
				throw new UserFriendlyException("ComplexImageComponent is useless without a texture");
			
			m_X1 = x1;
			m_Y1 = y1;
			m_X2 = x2;
			m_Y2 = y2;
			m_Flags = flags;
			
			m_U1 = u;
			m_V1 = v;
			m_U2 = texWidth;
			m_V2 = texHeight;
			
			m_Texture = tex;
			
			// GameDebugger.Log("{0},{1}-{2},{3} [{4}]", m_X1, m_Y1, m_X2, m_Y2, m_Flags);
		}
		
		public static ComplexImageComponent Load(string fileName, DomNode node) {
			return Load(fileName, node, "");
		}
		
		public static ComplexImageComponent Load(string fileName, DomNode node, string defaultTex) {
			Texture tex = null;
			int x1 = 0, y1 = 0, x2 = 0, y2 = 0, u, v, texWidth, texHeight;
			ComplexImageFlags flags = ComplexImageFlags.None;
			
			if( !node["relTex"].Equals("") ) {
				tex = Texture.Cache(node["relTex"].RelativeTo(fileName));
				if( tex == null )
					throw new UserFriendlyException(String.Format("Could not load the texture '{0}' for ComplexImageComponent", node["relTex"].RelativeTo(fileName)));
			}
			else if( !node["tex"].Equals("") ) {
				tex = Texture.Cache(node["tex"].ToPath());
				if( tex == null )
					throw new UserFriendlyException(String.Format("Could not load the texture '{0}' for ComplexImageComponent", node["tex"].ToPath()));
			}
			else {
				tex = Texture.Cache(defaultTex);
				if( tex == null )
					throw new UserFriendlyException(String.Format("Could not load the texture '{0}' for ComplexImageComponent", defaultTex));
			}
			
			tex.Bind();
			Texture.SetFilter(TextureMinFilter.Nearest, TextureMagFilter.Nearest);

			if( !node["width"].Equals("") ) {
				flags |= ComplexImageFlags.UsingWidth;
				x2 = node["width"].ToInt32(0);
			}
			if( !node["left"].Equals("") ) {
				flags |= ComplexImageFlags.UsingLeft;
				x1 = node["left"].ToInt32(0);
			}
			if( !node["right"].Equals("") ) {
				flags |= ComplexImageFlags.UsingRight;
				if( (flags & ComplexImageFlags.UsingWidth) == ComplexImageFlags.UsingWidth )
					x1 = node["right"].ToInt32(0);
				else
					x2 = node["right"].ToInt32(0);
			}
			if( node["tilex"].Equals("true")
			|| node["tilex"].Equals("yes")
			|| node["tilex"].Equals("on")
			|| node["tilex"].Equals("1") )
				flags |= ComplexImageFlags.TileHorizontal;

			if( !node["height"].Equals("") ) {
				flags |= ComplexImageFlags.UsingHeight;
				y2 = node["height"].ToInt32(0);
			}
			if( !node["bottom"].Equals("") ) {
				flags |= ComplexImageFlags.UsingBottom;
				y1 = node["bottom"].ToInt32(0);
			}
			if( !node["top"].Equals("") ) {
				flags |= ComplexImageFlags.UsingTop;
				if( (flags & ComplexImageFlags.UsingHeight) == ComplexImageFlags.UsingHeight )
					y1 = node["top"].ToInt32(0);
				else
					y2 = node["top"].ToInt32(0);
			}
			if( node["tiley"].Equals("true")
			|| node["tiley"].Equals("yes")
			|| node["tiley"].Equals("on")
			|| node["tiley"].Equals("1") )
				flags |= ComplexImageFlags.TileVertical;

			u = node["u"].ToInt32(0);
			v = node["v"].ToInt32(0);
			texWidth = node["texwidth"].ToInt32(tex.Width);
			texHeight = node["texheight"].ToInt32(tex.Height);

			return new ComplexImageComponent(tex, x1, y1, x2, y2, u, v, texWidth, texHeight, flags);
		}

		public void Render(int x, int y, int width, int height) {
			int x1, y1, x2, y2, xt1, xt2, yt2;
			float u1, v1, u2, v2, ut, vt;
			
			if( (m_Flags & ComplexImageFlags.UsingWidth) != ComplexImageFlags.None ) {
				if( (m_Flags & ComplexImageFlags.UsingLeft) != ComplexImageFlags.None ) {
					x1 = x + m_X1;
					x2 = x1 + m_X2;
				}
				else {
					x2 = x + width - m_X1;
					x1 = x2 - m_X2;
				}
			}
			else {
				x1 = x + m_X1;
				x2 = x + width - m_X2;
			}
			
			if( x2 < x1 ) return;
			
			if( (m_Flags & ComplexImageFlags.UsingHeight) != ComplexImageFlags.None ) {
				if( (m_Flags & ComplexImageFlags.UsingBottom) != ComplexImageFlags.None ) {
					y1 = y + m_Y1;
					y2 = y1 + m_Y2;
				}
				else {
					y2 = y + height - m_Y1;
					y1 = y2 - m_Y2;
				}
			}
			else {
				y1 = y + m_Y1;
				y2 = y + height - m_Y2;
			}
			
			if( y2 < y1 ) return;
			
			m_Texture.Bind();
			
			if( (m_Flags & ComplexImageFlags.TileHorizontal) != ComplexImageFlags.None ) {
				if( (m_Flags & ComplexImageFlags.TileVertical) != ComplexImageFlags.None ) {
					// tile on both
					u1 = m_U1 * m_Texture.PixelSizeS;
					v1 = m_V1 * m_Texture.PixelSizeT;
					u2 = u1 + m_U2 * m_Texture.PixelSizeS;
					v2 = v1 + m_V2 * m_Texture.PixelSizeT;
					
					GL.Begin(BeginMode.Quads);
					
					for( yt2 = y1 + m_V2; yt2 < y2; yt2 += m_V2, y1 += m_V2 ) {
						for( xt1 = x1, xt2 = x1 + m_U2; xt2 < x2; xt2 += m_U2, xt1 += m_U2 ) {
							GL.TexCoord2(u1, v1); GL.Vertex2(xt1, yt2);
							GL.TexCoord2(u2, v1); GL.Vertex2(xt2, yt2);
							GL.TexCoord2(u2, v2); GL.Vertex2(xt2, y1);
							GL.TexCoord2(u1, v2); GL.Vertex2(xt1, y1);
						}
						
						ut = u2 - (xt2 - x2) * m_Texture.PixelSizeS;
						
						GL.TexCoord2(u1, v1); GL.Vertex2(xt1, yt2);
						GL.TexCoord2(ut, v1); GL.Vertex2(x2, yt2);
						GL.TexCoord2(ut, v2); GL.Vertex2(x2, y1);
						GL.TexCoord2(u1, v2); GL.Vertex2(xt1, y1);
					}
					
					vt = v1 + (yt2 - y2) * m_Texture.PixelSizeT;
					
					for( xt2 = x1 + m_U2; xt2 < x2; xt2 += m_U2, x1 += m_U2 ) {
						GL.TexCoord2(u1, vt); GL.Vertex2(x1, y2);
						GL.TexCoord2(u2, vt); GL.Vertex2(xt2, y2);
						GL.TexCoord2(u2, v2); GL.Vertex2(xt2, y1);
						GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					}
					
					ut = u2 - (xt2 - x2) * m_Texture.PixelSizeS;
					
					GL.TexCoord2(u1, vt); GL.Vertex2(x1, y2);
					GL.TexCoord2(ut, vt); GL.Vertex2(x2, y2);
					GL.TexCoord2(ut, v2); GL.Vertex2(x2, y1);
					GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					
					GL.End();
					
				}
				else {
					// tile on x
					u1 = m_U1 * m_Texture.PixelSizeS;
					v1 = m_V1 * m_Texture.PixelSizeT;
					u2 = u1 + m_U2 * m_Texture.PixelSizeS;
					v2 = v1 + m_V2 * m_Texture.PixelSizeT;
					
					GL.Begin(BeginMode.Quads);
					
					for( xt2 = x1 + m_U2; xt2 < x2; xt2 += m_U2, x1 += m_U2 ) {
						GL.TexCoord2(u1, v1); GL.Vertex2(x1, y2);
						GL.TexCoord2(u2, v1); GL.Vertex2(xt2, y2);
						GL.TexCoord2(u2, v2); GL.Vertex2(xt2, y1);
						GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					}
					
					ut = u2 - (xt2 - x2) * m_Texture.PixelSizeS;
					
					GL.TexCoord2(u1, v1); GL.Vertex2(x1, y2);
					GL.TexCoord2(ut, v1); GL.Vertex2(x2, y2);
					GL.TexCoord2(ut, v2); GL.Vertex2(x2, y1);
					GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					
					GL.End();
				}
			}
			else {
				if( (m_Flags & ComplexImageFlags.TileVertical) != ComplexImageFlags.None ) {
					// tile on y
					u1 = m_U1 * m_Texture.PixelSizeS;
					v1 = m_V1 * m_Texture.PixelSizeT;
					u2 = u1 + m_U2 * m_Texture.PixelSizeS;
					v2 = v1 + m_V2 * m_Texture.PixelSizeT;
					
					GL.Begin(BeginMode.Quads);
					
					for( yt2 = y1 + m_V2; yt2 < y2; yt2 += m_V2, y1 += m_V2 ) {
						GL.TexCoord2(u1, v1); GL.Vertex2(x1, yt2);
						GL.TexCoord2(u2, v1); GL.Vertex2(x2, yt2);
						GL.TexCoord2(u2, v2); GL.Vertex2(x2, y1);
						GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					}
					
					vt = v1 + (yt2 - y2) * m_Texture.PixelSizeT;
					
					GL.TexCoord2(u1, vt); GL.Vertex2(x1, y2);
					GL.TexCoord2(u2, vt); GL.Vertex2(x2, y2);
					GL.TexCoord2(u2, v2); GL.Vertex2(x2, y1);
					GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					
					GL.End();
				}
				else {
					// don't tile
					u1 = m_U1 * m_Texture.PixelSizeS;
					v1 = m_V1 * m_Texture.PixelSizeT;
					u2 = u1 + m_U2 * m_Texture.PixelSizeS;
					v2 = v1 + m_V2 * m_Texture.PixelSizeT;
					
					GL.Begin(BeginMode.TriangleStrip);
					GL.TexCoord2(u1, v1); GL.Vertex2(x1, y2);
					GL.TexCoord2(u2, v1); GL.Vertex2(x2, y2);
					GL.TexCoord2(u1, v2); GL.Vertex2(x1, y1);
					GL.TexCoord2(u2, v2); GL.Vertex2(x2, y1);
					GL.End();
				}
			}
		}
	}

	[Flags]
	public enum ComplexImageFlags : byte {
		None = 0x00,
	
		UsingLeft = 0x01,
		UsingRight = 0x02,
		UsingWidth = 0x04,
		TileHorizontal = 0x08,
		
		UsingBottom = 0x10,
		UsingTop = 0x20,
		UsingHeight = 0x40,
		TileVertical = 0x80
	}
}
