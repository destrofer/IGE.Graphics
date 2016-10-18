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
	public class RenderToTextureBuffer : FrameBuffer {
		protected Texture m_Texture;
		protected RenderBuffer m_RenderBuffer;

		public Texture Texture { get { return m_Texture; } }
		public RenderBuffer RenderBuffer { get { return m_RenderBuffer; } }
		
		public RenderToTextureBuffer(int width, int height) : base() {
			Bind();
			m_Texture = new Texture(width, height, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			AttachTexture(m_Texture, FramebufferAttachment.ColorAttachment0);
			m_RenderBuffer = new RenderBuffer(width, height);
			AttachRenderBuffer(m_RenderBuffer, FramebufferAttachment.DepthAttachment);
			RenderToTextureBuffer.Unbind();
		}
		
		~RenderToTextureBuffer() {
			Dispose(false);
		}
		
		protected override void Dispose(bool manual) {
			if( m_RenderBuffer != null ) {
				Bind();
				RenderBuffer.Unbind();
				DetachRenderBuffer(FramebufferAttachment.DepthAttachment);
				m_RenderBuffer.Dispose();
				m_RenderBuffer = null;
			}
			if( m_Texture != null ) {
				Bind();
				Texture.Unbind();
				DetachTexture(FramebufferAttachment.ColorAttachment0);
				m_Texture.Dispose();
				m_Texture = null;
			}
			base.Dispose(manual);
		}
		
		public override void Bind() {
			if( m_Texture != null )
				m_Texture.Bind();
			if( m_RenderBuffer != null )
				m_RenderBuffer.Bind();
			base.Bind();
		}
		
		public new static void Unbind() {
			FrameBuffer.Unbind();
			RenderBuffer.Unbind();
			Texture.Unbind();
		}
	}
}
