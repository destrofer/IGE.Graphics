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
using System.Text;

using IGE.IO;
using IGE.Graphics;
using IGE.Graphics.OpenGL;

namespace IGE.GUI {
	public class CharInfo {
		public char Symbol;
		public Texture Texture;
		public float U1;
		public float V1;
		public float U2;
		public float V2;
		public int TexWidth;
		public int TexHeight;
		public int Width;
		public int Height;
		public int XOffset;
		public int BaseLine;
		
		public CharInfo(DomNode node, Texture defaultTexture, int defaultBaseline, float defaultU, float defaultV, int defaultWidth, int defaultHeight, int defaultTexWidth, int defaultTexHeight) {
			if( node["code"].Equals("") ) {
				if( node["sym"].Equals("") )
					throw new UserFriendlyException("A 'char' node must contain a 'code' or 'sym' attribute.", "An XML containing character information seems to be damaged or with mistakes.");
				Symbol = node["sym"][0];
			}
			else {
				int code = node["code"].ToInt32(-1);
				if( code < 0 )
					throw new UserFriendlyException("Character code is invalid.", "An XML containing character information seems to be damaged or with mistakes.");
				char[] syms = Encoding.Unicode.GetChars(new byte[] { (byte)((code >> 8) & 0xFF), (byte)(code & 0xFF) });
				Symbol = syms[0];
			}
			
			Texture = node["tex"].Equals("") ? defaultTexture : Texture.Cache(node["tex"].ToPath());
			
			BaseLine = node["baseline"].ToInt32(-10000);
			if( BaseLine <= -10000 )
				BaseLine = defaultBaseline;
				
			XOffset = node["xoffset"].ToInt32(0);
			
			Width = node["width"].ToInt32(-1);
			if( Width < 0 )
				Width = defaultWidth;
			
			Height = node["height"].ToInt32(-1);
			if( Height < 0 )
				Height = defaultHeight;

			TexWidth = node["texWidth"].ToInt32(-1);
			if( TexWidth < 0 )
				TexWidth = defaultTexWidth;
			if( TexWidth < 0 )
				TexWidth = Width;

			TexHeight = node["texHeight"].ToInt32(-1);
			if( TexHeight < 0 )
				TexHeight = defaultTexHeight;
			if( TexHeight < 0 )
				TexHeight = Height;
			
			U1 = (float)node["u"].ToInt32(-1);
			if( U1 < 0.0f )
				U1 = defaultU;
			
			V1 = (float)node["v"].ToInt32(-1);
			if( V1 < 0.0f )
				V1 = defaultV;
			
			U1 /= (float)Texture.Width;
			V1 /= (float)Texture.Height;
			U2 = U1 + (float)TexWidth / (float)Texture.Width;
			V2 = V1 + (float)TexHeight / (float)Texture.Height;
		}

		public void Render(int x, int y) {
			int x2, y2;
			x += XOffset;
			y -= BaseLine;
			x2 = x + TexWidth;
			if( View.YAxisIsUp )
				y2 = y - TexHeight;
			else
				y2 = y + TexHeight;
			Texture.Bind();
			GL.Begin(BeginMode.TriangleStrip);
			GL.TexCoord2(U1, V2); GL.Vertex2(x, y2);
			GL.TexCoord2(U2, V2); GL.Vertex2(x2, y2);
			GL.TexCoord2(U1, V1); GL.Vertex2(x, y);
			GL.TexCoord2(U2, V1); GL.Vertex2(x2, y);
			GL.End();
		}
		
		public void Render(int x, int y, float scaleX, float scaleY) {
			int x2, y2;
			x += XOffset;
			y -= BaseLine;
			x2 = x + (int)(TexWidth * scaleX);
			if( View.YAxisIsUp )
				y2 = y - (int)(TexHeight * scaleY);
			else
				y2 = y + (int)(TexHeight * scaleY);
			Texture.Bind();
			GL.Begin(BeginMode.TriangleStrip);
			GL.TexCoord2(U1, V2); GL.Vertex2(x, y2);
			GL.TexCoord2(U2, V2); GL.Vertex2(x2, y2);
			GL.TexCoord2(U1, V1); GL.Vertex2(x, y);
			GL.TexCoord2(U2, V1); GL.Vertex2(x2, y);
			GL.End();
		}
	}
}
