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

using IGE.Graphics;
using IGE.Graphics.OpenGL;


namespace IGE.Graphics {
	public class RenderBuffer : IDisposable {
		protected int m_Id = 0;
		protected int m_Width = 0;
		protected int m_Height = 0;
		protected int m_MultisamplingSamples = 0;
		
		public int Id { get { return m_Id; } }
		public int Width { get { return m_Width; } }
		public int Height { get { return m_Height; } }
		public int MultisamplingSamples { get { return m_MultisamplingSamples; } }

		public RenderBuffer(int width, int height, int multisamplingSamples)
			: this(width, height, multisamplingSamples, InternalPixelFormat.DepthComponent24)
		{
		}

		public RenderBuffer(int width, int height, int multisamplingSamples, InternalPixelFormat internalPixelFormat) {
			m_Width = width;
			m_Height = height;
			m_MultisamplingSamples = multisamplingSamples;
			m_Id = GL.GenRenderbuffer();
			Bind();
			if( m_MultisamplingSamples > 0 )
				GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, m_MultisamplingSamples, internalPixelFormat, (uint)width, (uint)height);
			else
				GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, internalPixelFormat, (uint)width, (uint)height);
			Unbind();
		}
		
		~RenderBuffer() {
			Dispose(false);
		}
		
		protected void Dispose(bool manual) {
			if( m_Id != 0 ) {
				GL.DeleteRenderbuffer(ref m_Id);
				m_Id = 0;
			}
		}
		
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		public void Bind() {
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, m_Id);
		}
		
		public void Unbind() {
			GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
		}
	}
}
