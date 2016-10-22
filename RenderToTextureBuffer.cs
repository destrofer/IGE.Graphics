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
		protected Texture m_Texture = null;
		protected RenderBuffer m_RenderBuffer = null;

		public Texture Texture { get { return m_Texture; } }
		public RenderBuffer RenderBuffer { get { return m_RenderBuffer; } }
		
		public RenderToTextureBuffer(int width, int height, bool addDepthBuffer, int multisamplingSamples) : base() {
			base.Bind();
			m_Texture = new Texture();
			if( multisamplingSamples > 0 ) {
				m_Texture.TextureType = TextureTarget.Texture2DMultisample;
				m_Texture.MultisamplingSamples = multisamplingSamples;
			}
			m_Texture.Create(width, height, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			AttachTexture(m_Texture, FramebufferAttachment.ColorAttachment0);
			if( addDepthBuffer ) {
				m_RenderBuffer = new RenderBuffer(width, height, multisamplingSamples);
				AttachRenderBuffer(m_RenderBuffer, FramebufferAttachment.DepthAttachment);
				m_RenderBuffer.Unbind();
			}
			m_Texture.Unbind();
			
			base.Unbind();
		}
		
		~RenderToTextureBuffer() {
			Dispose(false);
		}
		
		protected override void Dispose(bool manual) {
			if( m_RenderBuffer != null ) {
				base.Bind();
				m_RenderBuffer.Unbind();
				DetachRenderBuffer(m_RenderBuffer, FramebufferAttachment.DepthAttachment);
				m_RenderBuffer.Dispose();
				m_RenderBuffer = null;
				base.Unbind();
			}
			if( m_Texture != null ) {
				base.Bind();
				m_Texture.Unbind();
				DetachTexture(m_Texture, FramebufferAttachment.ColorAttachment0);
				m_Texture.Dispose();
				m_Texture = null;
				base.Unbind();
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
		
		public override void Unbind() {
			base.Unbind();
			if( m_RenderBuffer != null )
				m_RenderBuffer.Unbind();
			if( m_Texture != null )
				m_Texture.Unbind();
		}

		public virtual void CopyTo(RenderToTextureBuffer to) {
			BlitFramebufferBits mask = BlitFramebufferBits.ColorBufferBit;
			if( to.m_RenderBuffer != null )
				mask |= BlitFramebufferBits.DepthBufferBit;
			CopyTo(to, mask);
		}
		
		public virtual void CopyTo(RenderToTextureBuffer to, BlitFramebufferBits mask) {
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, m_Id);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, to.m_Id);
			
			GL.BlitFramebuffer(
				0, 0, m_Texture.Width, m_Texture.Height,
				0, 0, to.m_Texture.Width, to.m_Texture.Height,
				mask,
				TextureMagFilter.Linear
			);
			
			GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
			GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
		}
	}
}
