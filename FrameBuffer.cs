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
	public class FrameBuffer : IDisposable {
		protected int m_Id = 0;
		public int Id { get { return m_Id; } }
		
		public FrameBuffer() {
			m_Id = GL.GenFramebuffer();
		}
		
		~FrameBuffer() {
			Dispose(false);
		}
		
		protected virtual void Dispose(bool manual) {
			if( m_Id != 0 ) {
				Unbind();
				GL.DeleteFramebuffer(ref m_Id);
				m_Id = 0;
			}
		}
		
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		public virtual void Bind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_Id);
		}
		
		public virtual void Unbind() {
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}
		
		public void AttachTexture(Texture texture, FramebufferAttachment attachment) {
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, texture.TextureType, texture.Id, 0);
		}

		public void AttachTexture(Texture texture) {
			AttachTexture(texture, FramebufferAttachment.ColorAttachment0);
		}
		
		public void DetachTexture(Texture texture, FramebufferAttachment attachment) {
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, texture.TextureType, 0, 0);
		}
		
		public void AttachRenderBuffer(RenderBuffer renderBuffer, FramebufferAttachment attachment) {
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, renderBuffer.Id);
		}
		
		public void AttachRenderBuffer(RenderBuffer renderBuffer) {
			AttachRenderBuffer(renderBuffer, FramebufferAttachment.DepthAttachment);
		}
		
		public void DetachRenderBuffer(RenderBuffer renderBuffer, FramebufferAttachment attachment) {
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer, 0);
		}
		
		public FramebufferStatus CheckStatus() {
			Bind();
			return GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
		}
	}
}
