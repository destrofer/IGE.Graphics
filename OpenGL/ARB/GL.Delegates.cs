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
using System.Runtime.InteropServices;

namespace IGE.Graphics.OpenGL {
	public static partial class GL {
		public partial class Delegates {
			[RuntimeImport("opengl32")]
			public delegate void glActiveTextureARB(TextureUnit texture);
			public static glActiveTextureARB ActiveTextureARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glMultiTexCoord2fARB(TextureUnit target, float s, float t);
			public static glMultiTexCoord2fARB MultiTexCoord2fARB;
			
			
			
			[RuntimeImport("opengl32")]
			public delegate int glCreateProgramObjectARB();
			public static glCreateProgramObjectARB CreateProgramObjectARB;
			
			[RuntimeImport("opengl32")]
			public delegate int glCreateShaderObjectARB(ShaderObjectType shaderType); // ArbShaderObjects
			public static glCreateShaderObjectARB CreateShaderObjectARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glShaderSourceARB(int shaderObj, int lineCount, byte **lines, int *length);
			public unsafe static glShaderSourceARB ShaderSourceARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glCompileShaderARB(int shaderObj);
			public static glCompileShaderARB CompileShaderARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glCompileShaderIncludeARB(int shaderObj, int lineCount, byte **lines, int *length);
			public unsafe static glCompileShaderIncludeARB CompileShaderIncludeARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glAttachObjectARB(int attachToObj, int attachmentObj);
			public static glAttachObjectARB AttachObjectARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glLinkProgramARB(int programObj);
			public static glLinkProgramARB LinkProgramARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUseProgramObjectARB(int programObj);
			public static glUseProgramObjectARB UseProgramObjectARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glDeleteObjectARB(int obj);
			public static glDeleteObjectARB DeleteObjectARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGetInfoLogARB(int obj, int maxLength, [Out] int *length, byte *log);
			public unsafe static glGetInfoLogARB GetInfoLogARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate int glGetUniformLocationARB(int programObj, byte *name);
			public unsafe static glGetUniformLocationARB GetUniformLocationARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate int glGetAttribLocationARB(int programObj, byte *name);
			public unsafe static glGetAttribLocationARB GetAttribLocationARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glEnableVertexAttribArrayARB(int index);
			public static glEnableVertexAttribArrayARB EnableVertexAttribArrayARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glDisableVertexAttribArrayARB(int index);
			public static glDisableVertexAttribArrayARB DisableVertexAttribArrayARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform1fARB(int uniformLocation, float val);
			public static glUniform1fARB Uniform1fARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform1fvARB(int uniformLocation, int count, float *val);
			public unsafe static glUniform1fvARB Uniform1fvARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform1iARB(int uniformLocation, int val);
			public static glUniform1iARB Uniform1iARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform1ivARB(int uniformLocation, int count, int *val);
			public unsafe static glUniform1ivARB Uniform1ivARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform2fARB(int uniformLocation, float s, float t);
			public static glUniform2fARB Uniform2fARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform2fvARB(int uniformLocation, int count, float *val);
			public unsafe static glUniform2fvARB Uniform2fvARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform2iARB(int uniformLocation, int s, int t);
			public static glUniform2iARB Uniform2iARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform2ivARB(int uniformLocation, int count, int *val);
			public unsafe static glUniform2ivARB Uniform2ivARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform3fARB(int uniformLocation, float x, float y, float z);
			public static glUniform3fARB Uniform3fARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform3fvARB(int uniformLocation, int count, float *val);
			public unsafe static glUniform3fvARB Uniform3fvARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform3iARB(int uniformLocation, int x, int y, int z);
			public static glUniform3iARB Uniform3iARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform3ivARB(int uniformLocation, int count, int *val);
			public unsafe static glUniform3ivARB Uniform3ivARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform4fARB(int uniformLocation, float x, float y, float z, float w);
			public static glUniform4fARB Uniform4fARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform4fvARB(int uniformLocation, int count, float *val);
			public unsafe static glUniform4fvARB Uniform4fvARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glUniform4iARB(int uniformLocation, int x, int y, int z, int w);
			public static glUniform4iARB Uniform4iARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniform4ivARB(int uniformLocation, int count, int *val);
			public unsafe static glUniform4ivARB Uniform4ivARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniformMatrix3fvARB(int uniformLocation, int count, bool transpose, float *matrix);
			public unsafe static glUniformMatrix3fvARB UniformMatrix3fvARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glUniformMatrix4fvARB(int uniformLocation, int count, bool transpose, float *matrix);
			public unsafe static glUniformMatrix4fvARB UniformMatrix4fvARB;
			
			
						
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGenBuffersARB(int amount, [Out] int* bufIdsPtr);
			public unsafe static glGenBuffersARB GenBuffersARB;
			
			[RuntimeImport("opengl32")]
			public delegate void glBindBufferARB(BufferTarget target, int buffer);
			public static glBindBufferARB BindBufferARB;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glDeleteBuffersARB(int amount, int* bufIdsPtr);
			public unsafe static glDeleteBuffersARB DeleteBuffersARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glBufferDataARB(BufferTarget target, int size, [In,Out] void *data, BufferUsage usage);
			public unsafe static glBufferDataARB BufferDataARB;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glBufferSubDataARB(BufferTarget target, int offset, int size, [In,Out] void *data);
			public unsafe static glBufferSubDataARB BufferSubDataARB;
			
			[RuntimeImport("opengl32", "glBufferDataARB")]
			public delegate void glBufferDataARB_IP(BufferTarget target, int size, IntPtr data, BufferUsage usage);
			public static glBufferDataARB_IP BufferDataARB_IP;
			
			[RuntimeImport("opengl32", "glBufferSubDataARB")]
			public delegate void glBufferSubDataARB_IP(BufferTarget target, int offset, int size, IntPtr data);
			public static glBufferSubDataARB_IP BufferSubDataARB_IP;
		}
	}
}
