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
		#region Renderbuffers
		
        public static bool IsRenderbuffer(int bufferId) {
            return Delegates.IsRenderbufferEXT(bufferId) != 0;
        }

		public static int GenRenderbuffer() {
			int bufId;
            unsafe { Delegates.GenRenderbuffersEXT(1, &bufId); }
            return bufId;
		}

		public static void GenRenderbuffers(int amount, ref int[] bufIds) {
            unsafe { fixed (int *ptr = bufIds) { Delegates.GenRenderbuffersEXT(amount, ptr); } }
        }

		public static void DeleteRenderbuffer(ref int bufId) {
			unsafe { fixed (int *ptr = &bufId) { Delegates.DeleteRenderbuffersEXT(1, ptr); } }
		}

		public static void DeleteRenderbuffers(int amount, ref int[] bufIds) {
			unsafe { fixed (int *ptr = bufIds) { Delegates.DeleteRenderbuffersEXT(amount, ptr); } }
		}
        
        public static void BindRenderbuffer(RenderbufferTarget target, int buffer) {
            Delegates.BindRenderbufferEXT(target, buffer);
        }
        
        public static void RenderbufferStorage(RenderbufferTarget target, InternalPixelFormat format, uint width, uint height) {
            Delegates.RenderbufferStorageEXT(target, format, width, height);
        }

		public static void GetRenderbufferParameteriv(RenderbufferTarget target, RenderbufferParam param, out int val) {
			unsafe { fixed(int *ptr = &val) { Delegates.GetRenderbufferParameterivEXT(target, param, ptr); } }
		}
   		
		#endregion
		
		#region Framebuffers
		
        public static bool IsFramebuffer(int bufferId) {
            return Delegates.IsFramebufferEXT(bufferId) != 0;
        }

		public static int GenFramebuffer() {
			int bufId;
            unsafe { Delegates.GenFramebuffersEXT(1, &bufId); }
            return bufId;
		}

		public static void GenFramebuffers(int amount, ref int[] bufIds) {
            unsafe { fixed (int *ptr = bufIds) { Delegates.GenFramebuffersEXT(amount, ptr); } }
        }

		public static void DeleteFramebuffer(ref int bufId) {
			unsafe { fixed (int *ptr = &bufId) { Delegates.DeleteFramebuffersEXT(1, ptr); } }
		}

		public static void DeleteFramebuffers(int amount, ref int[] bufIds) {
			unsafe { fixed (int *ptr = bufIds) { Delegates.DeleteFramebuffersEXT(amount, ptr); } }
		}
        
        public static void BindFramebuffer(FramebufferTarget target, int buffer) {
            Delegates.BindFramebufferEXT(target, buffer);
        }

		public static FramebufferStatus CheckFramebufferStatus(FramebufferTarget target) {
			return Delegates.CheckFramebufferStatusEXT(target);
		}

        public static void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level) {
            Delegates.FramebufferTexture1DEXT(target, attachment, texTarget, textureId, level);
        }

        public static void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level) {
            Delegates.FramebufferTexture2DEXT(target, attachment, texTarget, textureId, level);
        }

        public static void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget texTarget, int textureId, int level, int zOffset) {
            Delegates.FramebufferTexture3DEXT(target, attachment, texTarget, textureId, level, zOffset);
        }

        public static void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget rbTarget, int renderBufferId) {
            Delegates.FramebufferRenderbufferEXT(target, attachment, rbTarget, renderBufferId);
        }

		public static void GetFramebufferAttachmentParameteriv(FramebufferTarget target, FramebufferAttachment attachment, FramebufferAttachmentParam param, out int val) {
			unsafe { fixed(int *ptr = &val) { Delegates.GetFramebufferAttachmentParameterivEXT(target, attachment, param, ptr); } }
		}
		
		public static void GenerateMipmap(TextureTarget texTarget) {
			Delegates.GenerateMipmapEXT(texTarget);
		}
		
		#endregion
		
		
		/*
        public static void ActiveTexture(TextureUnit unit) {
            Delegates.ActiveTextureARB(unit);
        }
        
		public static void MultiTexCoord2(TextureUnit unit, float s, float t) {
			Delegates.MultiTexCoord2fARB(unit, s, t);
		}
		
		public static int CreateProgramObject() {
			return Delegates.CreateProgramObjectARB();
		}
		
		public static int CreateShaderObject(ShaderObjectType shaderType) {
			return Delegates.CreateShaderObjectARB(shaderType);
		}
		
		public static void ShaderSource(int shaderObj, string source) {
			byte[] srcBytes = source.ToASCIIZByteArray();
			unsafe {
				fixed(byte *ptr = srcBytes) {
					IntPtr ptrVal = new IntPtr(ptr);
					byte **arr = (byte **)&ptrVal;
					Delegates.ShaderSourceARB(shaderObj, 1, arr, null);
				}
			}
		}
		
		public static void CompileShader(int shaderObj) {
			Delegates.CompileShaderARB(shaderObj);
		}

		public static void AttachObject(int attachToObj, int attachmentObj) {
			Delegates.AttachObjectARB(attachToObj, attachmentObj);
		}
		
		public static void LinkProgram(int programObj) {
			Delegates.LinkProgramARB(programObj);
		}
		
		public static void UseProgramObject(int programObj) {
			Delegates.UseProgramObjectARB(programObj);
		}
		
		public static void DeleteObject(int obj) {
			Delegates.DeleteObjectARB(obj);
		}
		
		public static string GetInfoLog(int obj) {
			byte[] logBytes = new byte[65536];
			int logLen = 0;
			unsafe {
				fixed(byte *ptr = logBytes) {
					Delegates.GetInfoLogARB(obj, 65536, &logLen, ptr);
				}
			}
			return Encoding.ASCII.GetString(logBytes, 0, logLen);
		}
		
		public static int GetUniformLocation(int programObj, string uniformName) {
			byte[] nameBytes = uniformName.ToASCIIZByteArray();
			unsafe {
				fixed(byte *ptr = nameBytes) {
					return Delegates.GetUniformLocationARB(programObj, ptr);
				}
			}
		}
		
		public static int GetAttribLocation(int programObj, string attribName) {
			byte[] nameBytes = attribName.ToASCIIZByteArray();
			unsafe {
				fixed(byte *ptr = nameBytes) {
					return Delegates.GetAttribLocationARB(programObj, ptr);
				}
			}
		}
		
		public static void EnableVertexAttribArray(int index) {
			Delegates.EnableVertexAttribArrayARB(index);
		}
		
		public static void DisableVertexAttribArray(int index) {
			Delegates.DisableVertexAttribArrayARB(index);
		}
		
		
		
		public static void Uniform1(int uniformLocation, float val) {
			Delegates.Uniform1fARB(uniformLocation, val);
		}
		
		public static void Uniform1(int uniformLocation, int val) {
			Delegates.Uniform1iARB(uniformLocation, val);
		}

		public static void Uniform1(int uniformLocation, float[] values) {
			unsafe { fixed(float *ptr = values) { Delegates.Uniform1fvARB(uniformLocation, values.Length, ptr); } }
		}
		
		public static void Uniform1(int uniformLocation, int[] values) {
			unsafe { fixed(int *ptr = values) { Delegates.Uniform1ivARB(uniformLocation, values.Length, ptr); } }
		}

		public unsafe static void Uniform1(int uniformLocation, int count, float *values) {
			Delegates.Uniform1fvARB(uniformLocation, count, values);
		}
		
		public unsafe static void Uniform1(int uniformLocation, int count, int *values) {
			Delegates.Uniform1ivARB(uniformLocation, count, values);
		}

		
		
		public static void Uniform2(int uniformLocation, float s, float t) {
			Delegates.Uniform2fARB(uniformLocation, s, t);
		}
		
		public static void Uniform2(int uniformLocation, int s, int t) {
			Delegates.Uniform2iARB(uniformLocation, s, t);
		}
		
		public static void Uniform2(int uniformLocation, ref Vector2 texCoords) {
			unsafe { fixed(float *ptr = &texCoords.X) { Delegates.Uniform2fvARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform2(int uniformLocation, Vector2 texCoords) {
			unsafe { Delegates.Uniform2fvARB(uniformLocation, 1, &texCoords.X); }
		}		
		
		public static void Uniform2(int uniformLocation, ref Point2 texCoords) {
			unsafe { fixed(int *ptr = &texCoords.X) { Delegates.Uniform2ivARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform2(int uniformLocation, Point2 texCoords) {
			unsafe { Delegates.Uniform2ivARB(uniformLocation, 1, &texCoords.X); }
		}
		
		public static void Uniform2(int uniformLocation, float[] values) {
			unsafe { fixed(float *ptr = values) { Delegates.Uniform2fvARB(uniformLocation, values.Length / 2, ptr); } }
		}

		public static void Uniform2(int uniformLocation, int[] values) {
			unsafe { fixed(int *ptr = values) { Delegates.Uniform2ivARB(uniformLocation, values.Length / 2, ptr); } }
		}

		public static void Uniform2(int uniformLocation, Vector2[] values) {
			unsafe { fixed(float *ptr = &values[0].X) { Delegates.Uniform2fvARB(uniformLocation, values.Length, ptr); } }
		}

		public static void Uniform2(int uniformLocation, Point2[] values) {
			unsafe { fixed(int *ptr = &values[0].X) { Delegates.Uniform2ivARB(uniformLocation, values.Length, ptr); } }
		}

		public unsafe static void Uniform2(int uniformLocation, int count, float *values) {
			Delegates.Uniform2fvARB(uniformLocation, count, values);
		}
		
		public unsafe static void Uniform2(int uniformLocation, int count, int *values) {
			Delegates.Uniform2ivARB(uniformLocation, count, values);
		}

		public unsafe static void Uniform2(int uniformLocation, int count, Vector2 *values) {
			Delegates.Uniform2fvARB(uniformLocation, count, (float *)values);
		}

		public unsafe static void Uniform2(int uniformLocation, int count, Point2 *values) {
			Delegates.Uniform2ivARB(uniformLocation, count, (int *)values);
		}

		
		
		public static void Uniform3(int uniformLocation, float x, float y, float z) {
			Delegates.Uniform3fARB(uniformLocation, x, y, z);
		}
		
		public static void Uniform3(int uniformLocation, int x, int y, int z) {
			Delegates.Uniform3iARB(uniformLocation, x, y, z);
		}
		
		public static void Uniform3(int uniformLocation, ref Vector3 coords) {
			unsafe { fixed(float *ptr = &coords.X) { Delegates.Uniform3fvARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform3(int uniformLocation, Vector3 coords) {
			unsafe { Delegates.Uniform3fvARB(uniformLocation, 1, &coords.X); }
		}		
		
		public static void Uniform3(int uniformLocation, ref Point3 coords) {
			unsafe { fixed(int *ptr = &coords.X) { Delegates.Uniform3ivARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform3(int uniformLocation, Point3 coords) {
			unsafe { Delegates.Uniform3ivARB(uniformLocation, 1, &coords.X); }
		}
		
		public static void Uniform3(int uniformLocation, float[] values) {
			unsafe { fixed(float *ptr = values) { Delegates.Uniform3fvARB(uniformLocation, values.Length / 3, ptr); } }
		}

		public static void Uniform3(int uniformLocation, int[] values) {
			unsafe { fixed(int *ptr = values) { Delegates.Uniform3ivARB(uniformLocation, values.Length / 3, ptr); } }
		}

		public static void Uniform3(int uniformLocation, Vector3[] values) {
			unsafe { fixed(float *ptr = &values[0].X) { Delegates.Uniform3fvARB(uniformLocation, values.Length, ptr); } }
		}

		public static void Uniform3(int uniformLocation, Point3[] values) {
			unsafe { fixed(int *ptr = &values[0].X) { Delegates.Uniform3ivARB(uniformLocation, values.Length, ptr); } }
		}

		public unsafe static void Uniform3(int uniformLocation, int count, float *values) {
			Delegates.Uniform3fvARB(uniformLocation, count, values);
		}
		
		public unsafe static void Uniform3(int uniformLocation, int count, int *values) {
			Delegates.Uniform3ivARB(uniformLocation, count, values);
		}

		public unsafe static void Uniform3(int uniformLocation, int count, Vector3 *values) {
			Delegates.Uniform3fvARB(uniformLocation, count, (float *)values);
		}

		public unsafe static void Uniform3(int uniformLocation, int count, Point3 *values) {
			Delegates.Uniform3ivARB(uniformLocation, count, (int *)values);
		}

		
		
		public static void Uniform4(int uniformLocation, float x, float y, float z, float w) {
			Delegates.Uniform4fARB(uniformLocation, x, y, z, w);
		}
		
		public static void Uniform4(int uniformLocation, int x, int y, int z, int w) {
			Delegates.Uniform4iARB(uniformLocation, x, y, z, w);
		}
		
		public static void Uniform4(int uniformLocation, ref Vector4 coords) {
			unsafe { fixed(float *ptr = &coords.X) { Delegates.Uniform4fvARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform4(int uniformLocation, Vector4 coords) {
			unsafe { Delegates.Uniform4fvARB(uniformLocation, 1, &coords.X); }
		}		
		
		public static void Uniform4(int uniformLocation, ref Point4 coords) {
			unsafe { fixed(int *ptr = &coords.X) { Delegates.Uniform4ivARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform4(int uniformLocation, Point4 coords) {
			unsafe { Delegates.Uniform4ivARB(uniformLocation, 1, &coords.X); }
		}
		
		public static void Uniform4(int uniformLocation, ref Color4 color) {
			unsafe { fixed(float *ptr = &color.R) { Delegates.Uniform4fvARB(uniformLocation, 1, ptr); } }
		}
		
		public static void Uniform4(int uniformLocation, Color4 color) {
			unsafe { Delegates.Uniform4fvARB(uniformLocation, 1, &color.R); }
		}
		
		public static void Uniform4(int uniformLocation, float[] values) {
			unsafe { fixed(float *ptr = values) { Delegates.Uniform4fvARB(uniformLocation, values.Length / 4, ptr); } }
		}

		public static void Uniform4(int uniformLocation, int[] values) {
			unsafe { fixed(int *ptr = values) { Delegates.Uniform4ivARB(uniformLocation, values.Length / 4, ptr); } }
		}

		public static void Uniform4(int uniformLocation, Vector4[] values) {
			unsafe { fixed(float *ptr = &values[0].X) { Delegates.Uniform4fvARB(uniformLocation, values.Length, ptr); } }
		}

		public static void Uniform4(int uniformLocation, Point4[] values) {
			unsafe { fixed(int *ptr = &values[0].X) { Delegates.Uniform4ivARB(uniformLocation, values.Length, ptr); } }
		}

		public static void Uniform4(int uniformLocation, Color4[] values) {
			unsafe { fixed(float *ptr = &values[0].R) { Delegates.Uniform4fvARB(uniformLocation, values.Length, ptr); } }
		}

		public unsafe static void Uniform4(int uniformLocation, int count, float *values) {
			Delegates.Uniform4fvARB(uniformLocation, count, values);
		}
		
		public unsafe static void Uniform4(int uniformLocation, int count, int *values) {
			Delegates.Uniform4ivARB(uniformLocation, count, values);
		}

		public unsafe static void Uniform4(int uniformLocation, int count, Vector4 *values) {
			Delegates.Uniform4fvARB(uniformLocation, count, (float *)values);
		}

		public unsafe static void Uniform4(int uniformLocation, int count, Point4 *values) {
			Delegates.Uniform4ivARB(uniformLocation, count, (int *)values);
		}
		
		public static int GenBuffer() {
			int bufId;
            unsafe { Delegates.GenBuffersARB(1, &bufId); }
            return bufId;
		}

		public static void GenBuffers(int amount, ref int[] bufIds) {
            unsafe { fixed (int *ptr = bufIds) { Delegates.GenBuffersARB(amount, ptr); } }
        }
        
        public static void BindBuffer(BufferTarget target, int buffer) {
            Delegates.BindBufferARB(target, buffer);
        }

		public static void DeleteBuffer(ref int bufId) {
			unsafe { fixed (int *ptr = &bufId) { Delegates.DeleteBuffersARB(1, ptr); } }
		}

		public static void DeleteBuffers(int amount, ref int[] bufIds) {
			unsafe { fixed (int *ptr = bufIds) { Delegates.DeleteBuffersARB(amount, ptr); } }
		}
		
		/// <summary>
		/// Creates a buffer data storage without the actual data
		/// </summary>
		/// <param name="target"></param>
		/// <param name="size"></param>
		/// <param name="usage"></param>
		public static void BufferData(BufferTarget target, int size, BufferUsage usage) {
			Delegates.BufferDataARB_IP(target, size, IntPtr.Zero, usage);
		}

		/*
		/// <summary>
		/// Creates a buffer data storage and copies data into it
		/// </summary>
		/// <param name="target"></param>
		/// <param name="size"></param>
		/// <param name="ptr"></param>
		/// <param name="usage"></param>
		public static void BufferData(BufferTarget target, int size, IntPtr ptr, BufferUsage usage) {
			Delegates.BufferDataARB_IP(target, size, ptr, usage);
		}

		public static void BufferSubData(BufferTarget target, int offset, int size, IntPtr ptr) {
			Delegates.BufferSubDataARB_IP(target, offset, size, ptr);
		}
		*/
/*
		public static void BufferData<TArray>(BufferTarget target, int size, ref TArray[] data, BufferUsage usage) where TArray : struct {
			// TODO: if possible, find a way to do it without a garbage collector handle creation ...
			// commented code now generates error "Cannot implicitly convert type 'System.Array' to 'void*' (CS0029)"
			
			// unsafe { fixed (void *ptr = data) { Delegates.BufferDataARB(target, size, ptr, usage); } }
			
			GCHandle hData = GCHandle.Alloc(data, GCHandleType.Pinned);
			try { Delegates.BufferDataARB_IP(target, size, hData.AddrOfPinnedObject(), usage); }
			finally { hData.Free(); }
		}

		public static void BufferSubData<TArray>(BufferTarget target, int offset, int size, ref TArray[] data) where TArray : struct {
			// unsafe { fixed (void *ptr = data) { Delegates.BufferSubDataARB(target, offset, size, ptr); } }
			
			GCHandle hData = GCHandle.Alloc(data, GCHandleType.Pinned);
			try { Delegates.BufferSubDataARB_IP(target, offset, size, hData.AddrOfPinnedObject()); }
			finally { hData.Free(); }
		}

		public static void BufferData(BufferTarget target, int size, ref object dataArray, BufferUsage usage) {
			GCHandle hData = GCHandle.Alloc(dataArray, GCHandleType.Pinned);
			try { Delegates.BufferDataARB_IP(target, size, hData.AddrOfPinnedObject(), usage); }
			finally { hData.Free(); }
		}

		public static void BufferSubData(BufferTarget target, int offset, int size, ref object dataArray) {
			GCHandle hData = GCHandle.Alloc(dataArray, GCHandleType.Pinned);
			try { Delegates.BufferSubDataARB_IP(target, offset, size, hData.AddrOfPinnedObject()); }
			finally { hData.Free(); }
		}
		*/
	}
}