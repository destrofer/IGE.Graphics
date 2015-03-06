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

namespace IGE.Graphics {
	/// <summary>
	/// </summary>
	public static class GraphicsUtils {
		public static void DrawRect(int x1, int y1, int x2, int y2, Color4 color) {
			GL.PushAttrib(AttribBits.EnableBit);
			GL.Disable(EnableCap.Texture2D);
			GL.Color4(ref color);
			GL.Begin(BeginMode.LineLoop);
			GL.Vertex2(x1 + 0.5f, y1 + 0.5f);
			GL.Vertex2(x2 + 0.5f, y1 + 0.5f);
			GL.Vertex2(x2 + 0.5f, y2 + 0.5f);
			GL.Vertex2(x1 + 0.5f, y2 + 0.5f);
			GL.End();
			GL.PopAttrib();
		}
		
		public static void DrawGrid(int x, int y, int width, int height, int cellWidth, int cellHeight) {
			int i, x2, y2;
			x2 = x + width * cellWidth;
			y2 = y + height * cellHeight;
			
			GL.Begin(BeginMode.Lines);
			
			for( i = x2; i >= x; i -= cellWidth ) {
				//GL.Vertex2(0.5f + i, 0.5f + (float)y);
				//GL.Vertex2(0.5f + i, 0.5f + (float)y2);
				GL.Vertex2(i, y);
				GL.Vertex2(i, y2);
			}

			for( i = y2; i >= y; i -= cellHeight ) {
				//GL.Vertex2(0.5f + (float)x, 0.5f + (float)i);
				//GL.Vertex2(0.5f + (float)x2, 0.5f + (float)i);
				GL.Vertex2(x, i);
				GL.Vertex2(x2, i);
			}
			
			GL.End();
		}
	}
}
