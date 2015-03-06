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

// Specs used:
// http://www.opengl.org/registry/specs/EXT/framebuffer_object.txt

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace IGE.Graphics.OpenGL {
	public static partial class GL {
		public partial class Delegates {
			// GLboolean = byte

			#region Render buffer

			[RuntimeImport("opengl32")]
			public delegate byte glIsRenderbufferEXT(int renderBufferId);
			public static glIsRenderbufferEXT IsRenderbufferEXT;

			[RuntimeImport("opengl32")]
			public delegate void glBindRenderbufferEXT(RenderbufferTarget target, int renderBufferId);
			public static glBindRenderbufferEXT BindRenderbufferEXT;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGenRenderbuffersEXT(int amount, [Out] int* rbIdsPtr);
			public unsafe static glGenRenderbuffersEXT GenRenderbuffersEXT;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glDeleteRenderbuffersEXT(int amount, int* rbIdsPtr);
			public unsafe static glDeleteRenderbuffersEXT DeleteRenderbuffersEXT;

			[RuntimeImport("opengl32")]
			public delegate void glRenderbufferStorageEXT(RenderbufferTarget target, InternalPixelFormat format, uint width, uint height);
			public static glRenderbufferStorageEXT RenderbufferStorageEXT;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGetRenderbufferParameterivEXT(RenderbufferTarget target, RenderbufferParam param, [Out] int* paramsPtr);
			public unsafe static glGetRenderbufferParameterivEXT GetRenderbufferParameterivEXT;
			
			#endregion

			#region Frame buffer

			[RuntimeImport("opengl32")]
			public delegate byte glIsFramebufferEXT(int frameBufferId);
			public static glIsFramebufferEXT IsFramebufferEXT;
			
			[RuntimeImport("opengl32")]
			public delegate void glBindFramebufferEXT(FramebufferTarget target, int frameBufferId);
			public static glBindFramebufferEXT BindFramebufferEXT;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGenFramebuffersEXT(int amount, [Out] int* fbIdsPtr);
			public unsafe static glGenFramebuffersEXT GenFramebuffersEXT;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glDeleteFramebuffersEXT(int amount, int* fbIdsPtr);
			public unsafe static glDeleteFramebuffersEXT DeleteFramebuffersEXT;

			[RuntimeImport("opengl32")]
			public delegate FramebufferStatus glCheckFramebufferStatusEXT(FramebufferTarget target);
			public static glCheckFramebufferStatusEXT CheckFramebufferStatusEXT;

			[RuntimeImport("opengl32")]
			public delegate void glFramebufferTexture1DEXT(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level);
			public static glFramebufferTexture1DEXT FramebufferTexture1DEXT;

			[RuntimeImport("opengl32")]
			public delegate void glFramebufferTexture2DEXT(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level);
			public static glFramebufferTexture2DEXT FramebufferTexture2DEXT;

			[RuntimeImport("opengl32")]
			public delegate void glFramebufferTexture3DEXT(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level, int zOffset);
			public static glFramebufferTexture3DEXT FramebufferTexture3DEXT;

			[RuntimeImport("opengl32")]
			public delegate void glFramebufferRenderbufferEXT(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget rbTarget, int renderBufferId);
			public static glFramebufferRenderbufferEXT FramebufferRenderbufferEXT;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGetFramebufferAttachmentParameterivEXT(FramebufferTarget target, FramebufferAttachment attachment, FramebufferAttachmentParam param, [Out] int* paramsPtr);
			public unsafe static glGetFramebufferAttachmentParameterivEXT GetFramebufferAttachmentParameterivEXT;
			
			[RuntimeImport("opengl32")]
			public delegate void glGenerateMipmapEXT(TextureTarget texTarget);
			public static glGenerateMipmapEXT GenerateMipmapEXT;
			
			#endregion
/*			
			
			
			GLboolean APIENTRY glIsFramebufferEXT(GLuint framebuffer);
			void APIENTRY glBindFramebufferEXT(GLenum target, GLuint framebuffer);
			void APIENTRY glDeleteFramebuffersEXT(GLsizei n, const GLuint *framebuffers);
			void APIENTRY glGenFramebuffersEXT(GLsizei n, GLuint *framebuffers);
			
			GLenum APIENTRY glCheckFramebufferStatusEXT(GLenum target);
			
			void APIENTRY glFramebufferTexture1DEXT(GLenum target, GLenum attachment,
			                                        GLenum textarget, GLuint texture,
			                                        GLint level);
			void APIENTRY glFramebufferTexture2DEXT(GLenum target, GLenum attachment,
			                                        GLenum textarget, GLuint texture,
			                                        GLint level);
			void APIENTRY glFramebufferTexture3DEXT(GLenum target, GLenum attachment,
			                                        GLenum textarget, GLuint texture,
			                                        GLint level, GLint zoffset);
			
			void APIENTRY glFramebufferRenderbufferEXT(GLenum target, GLenum attachment,
			                                           GLenum renderbuffertarget,
			                                           GLuint renderbuffer);
			
			void APIENTRY glGetFramebufferAttachmentParameterivEXT(GLenum target,
			                                                       GLenum attachment,
			                                                       GLenum pname,
			                                                       GLint *params);
			
			void APIENTRY glGenerateMipmapEXT(GLenum target);			
			
			#endregion
			
			*/
		}
	}
}
