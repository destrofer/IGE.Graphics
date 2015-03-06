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
		public static OpenGLError GetError() {
			return Delegates.GetError();
		}
		
        public static void Viewport(Rectangle rect) {
            Delegates.Viewport(rect.X, rect.Y, rect.Width, rect.Height);
        }
    
        public static void ClearColor(ref Color4 color) {
            Delegates.ClearColor(color.R, color.G, color.B, color.A);
        }
    
        public static void ClearColor(float red, float green, float blue, float alpha) {
            Delegates.ClearColor(red, green, blue, alpha);
        }
        
		public static void ClearDepth(double d) {
            Delegates.ClearDepth(d);
        }
        
        public static void DepthFunc(DepthFunction func) {
            Delegates.DepthFunc(func);
        }

        public static void FrontFace(FrontFaceDirection dir) {
            Delegates.FrontFace(dir);
        }
        
        public static void Hint(HintTarget target, HintMode mode) {
            Delegates.Hint(target, mode);
        }
        
        public static void ShadeModel(ShadeModel mode) {
            Delegates.ShadeModel(mode);
        }
        
        public static void BlendFunc(BlendFuncSrc src, BlendFuncDst dst) {
            Delegates.BlendFunc(src, dst);
        }
        
        public static void Enable(EnableCap capability) {
            Delegates.Enable(capability);
        }
        
        public static void Disable(EnableCap capability) {
            Delegates.Disable(capability);
        }
        
        public static void MatrixMode(MatrixMode mode) {
            Delegates.MatrixMode(mode);
        }
        
        public static void LoadIdentity() {
            Delegates.LoadIdentity();
        }
        
		public static void LoadMatrix(ref Matrix4 m) {
			unsafe { fixed (float* ptr = &m.M11) { Delegates.LoadMatrixf(ptr); } }
		}
		
        public static void Ortho(float left, float right, float bottom, float top, float zNear, float zFar) {
            Delegates.Ortho(left, right, bottom, top, zNear, zFar);
        }
        
		public static void Clear(ClearBufferBits bits) {
			Delegates.Clear(bits);
		}
		
		public static void Begin(BeginMode mode) {
			Delegates.Begin(mode);
		}
		
		public static void End() {
			Delegates.End();
		}
		
		public static void Color4(byte r, byte g, byte b, byte a) {
			Delegates.Color4ub(r, g, b, a);
		}
		
		public static void Color4(float r, float g, float b, float a) {
			Delegates.Color4f(r, g, b, a);
		}
		
		public static void Color4(double r, double g, double b, double a) {
			Delegates.Color4d(r, g, b, a);
		}
		
		public static void Color4(ref Color4 color) {
			unsafe { fixed(float *ptr = &color.R) { Delegates.Color4fv(ptr); } }
		}
		
		public static void Color4(Color4 color) {
			unsafe { Delegates.Color4fv(&color.R); }
		}
		
		public static void Color4(ref Color color) {
			unsafe { fixed(byte *ptr = &color.R) { Delegates.Color4ubv(ptr); } }
		}
		
		public static void Color4(Color color) {
			unsafe { Delegates.Color4ubv(&color.R); }
		}
		
		public static void Color3(float r, float g, float b) {
			Delegates.Color3f(r, g, b);
		}
		
		public static void Color3(ref Color4 color) {
			unsafe { fixed(float *ptr = &color.R) { Delegates.Color3fv(ptr); } }
		}
		
		public static void Vertex2(int x, int y) {
			Delegates.Vertex2i(x, y);
		}
		
		public static void Vertex2(float x, float y) {
			Delegates.Vertex2f(x, y);
		}
		
		public static void Vertex2(ref Vector2 v) {
			Delegates.Vertex2f(v.X, v.Y);
		}
		
		public static void Vertex2(Vector2 v) {
			Delegates.Vertex2f(v.X, v.Y);
		}
		
		public static void Vertex3(int x, int y, int z) {
			Delegates.Vertex3i(x, y, z);
		}
		
		public static void Vertex3(float x, float y, float z) {
			Delegates.Vertex3f(x, y, z);
		}
		
		public static void Vertex3(double x, double y, double z) {
			Delegates.Vertex3d(x, y, z);
		}
		
		public static void Vertex3(ref Vector3 v) {
			// TODO: check wether using unsafe+fixed+Vertex3fv would be faster than this
			Delegates.Vertex3f(v.X, v.Y, v.Z);
		}
		
		public static void Vertex3(Vector3 v) {
			Delegates.Vertex3f(v.X, v.Y, v.Z);
		}
		
		public static void TexCoord2(float s, float t) {
			Delegates.TexCoord2f(s, t);
		}
		
		public static void TexCoord2(ref Vector2 v) {
			Delegates.TexCoord2f(v.X, v.Y);
		}
		
		public static void TexCoord2(Vector2 v) {
			Delegates.TexCoord2f(v.X, v.Y);
		}
		
		public static int GenTexture() {
			int texId;
            unsafe { Delegates.GenTextures(1, &texId); }
            return texId;
		}

		public static void GenTextures(int amount, ref int[] texIds) {
            unsafe { fixed (int *ptr = texIds) { Delegates.GenTextures(amount, ptr); } }
        }

		public static void DeleteTexture(ref int texId) {
			unsafe { fixed (int *ptr = &texId) { Delegates.DeleteTextures(1, ptr); } }
		}

		public static void DeleteTextures(int amount, ref int[] texIds) {
			unsafe { fixed (int *ptr = texIds) { Delegates.DeleteTextures(amount, ptr); } }
		}

		public static void GetInteger(ParamName param, out int val) {
			unsafe { fixed(int *ptr = &val) { Delegates.GetIntegerv(param, ptr); } }
		}
        
        public static void BindTexture(TextureTarget target, int texId) {
            Delegates.BindTexture(target, texId);
        }
		
        public static void TexParameter(TextureTarget target, TextureParamName pname, float param) {
            Delegates.TexParameterf(target, pname, param);
        }
		
        public static void TexParameter(TextureTarget target, TextureParamName pname, int param) {
            Delegates.TexParameteri(target, pname, param);
        }
		
		public static void TexParameter(TextureTarget target, TextureParamName pname, float[] param) {
			unsafe { fixed (float *ptr = param) { Delegates.TexParameterfv(target, pname, ptr); } }
        }
		
		public static void TexParameter(TextureTarget target, TextureParamName pname, int[] param) {
			unsafe { fixed (int *ptr = param) { Delegates.TexParameteriv(target, pname, ptr); } }
        }
        
        public unsafe static void TexImage2D(TextureTarget target, int level, InternalPixelFormat internalFormat, int width, int height, int border, PixelFormatEnum format, PixelType type, byte *pixelPtr) {
            Delegates.TexImage2D(target, level, internalFormat, width, height, border, format, type, (IntPtr)pixelPtr);
        }
        
        public static void TexImage2D(TextureTarget target, int level, InternalPixelFormat internalFormat, int width, int height, int border, PixelFormatEnum format, PixelType type, IntPtr pixelPtr) {
            Delegates.TexImage2D(target, level, internalFormat, width, height, border, format, type, pixelPtr);
        }
        
		public static void PushAttrib(AttribBits bits) {
			Delegates.PushAttrib(bits);
		}
        
		public static void PopAttrib() {
			Delegates.PopAttrib();
		}
        
		public static void Scale(float scale) {
			Delegates.Scalef(scale, scale, scale);
		}
        
		public static void Scale(float x, float y) {
			Delegates.Scalef(x, y, 1f);
		}
        
		public static void Scale(double x, double y) {
			Delegates.Scaled(x, y, 1.0);
		}
        
		public static void Scale(float x, float y, float z) {
			Delegates.Scalef(x, y, z);
		}
        
		public static void Scale(double x, double y, double z) {
			Delegates.Scaled(x, y, z);
		}
        
		public static void Translate(int x, int y) {
			Delegates.Translatef((float)x, (float)y, 0f);
		}
        
		public static void Translate(float x, float y) {
			Delegates.Translatef(x, y, 0f);
		}
        
		public static void Translate(double x, double y) {
			Delegates.Translated(x, y, 0f);
		}

		public static void Translate(Vector2 v) {
			Delegates.Translatef(v.X, v.Y, 0f);
		}

		public static void Translate(ref Vector2 v) {
			Delegates.Translatef(v.X, v.Y, 0f);
		}
        
		public static void Translate(float x, float y, float z) {
			Delegates.Translatef(x, y, z);
		}
        
		public static void Translate(double x, double y, double z) {
			Delegates.Translated(x, y, z);
		}
        
		public static void Translate(Vector3 v) {
			Delegates.Translatef(v.X, v.Y, v.Z);
		}

		public static void Translate(ref Vector3 v) {
			Delegates.Translatef(v.X, v.Y, v.Z);
		}
		
		public static void Rotate(float angle, float x, float y, float z) {
			Delegates.Rotatef(angle, x, y, z);
		}
        
		public static void Rotate(double angle, double x, double y, double z) {
			Delegates.Rotated(angle, x, y, z);
		}
		
		public static void PushMatrix() {
			Delegates.PushMatrix();
		}
		
		public static void PopMatrix() {
			Delegates.PopMatrix();
		}
		
		public static void ColorMaterial(FaceName face, ColorMaterialMode mode) {
			Delegates.ColorMaterial(face, mode);
		}
		
        public static void Material(FaceName face, MaterialParamName pname, float param) {
            Delegates.Materialf(face, pname, param);
        }
		
        public static void Material(FaceName face, MaterialParamName pname, int param) {
            Delegates.Materiali(face, pname, param);
        }
		
		public static void Material(FaceName face, MaterialParamName pname, float[] param) {
			unsafe { fixed (float *ptr = param) { Delegates.Materialfv(face, pname, ptr); } }
        }
		
		public static void Material(FaceName face, MaterialParamName pname, int[] param) {
			unsafe { fixed (int *ptr = param) { Delegates.Materialiv(face, pname, ptr); } }
        }
		
		public static void Material(FaceName face, MaterialParamName pname, ref Color4 color) {
			unsafe { fixed (float *ptr = &color.R) { Delegates.Materialfv(face, pname, ptr); } }
        }
		
		public static void Material(FaceName face, MaterialParamName pname, Color4 color) {
			unsafe { Delegates.Materialfv(face, pname, &color.R); }
        }
		
		public static string GetString(GlStringName stringName) {
			byte[] strBytes;
			int i, len = 0;
			unsafe {
				byte *strPtr = Delegates.GetString(stringName);
				byte *tmpPtr = strPtr;
				if( strPtr == null )
					return null;
				while( len < 65536 && *tmpPtr++ != 0 )
					len++;
				strBytes = new byte[len];
				for( i = 0; i < len; i++ )
					strBytes[i] = *strPtr++;
			}
			return Encoding.ASCII.GetString(strBytes, 0, len);
		}
		
		public static string GetString(GlStringName stringName, int index) {
			byte[] strBytes;
			int i, len = 0;
			unsafe {
				byte *strPtr = Delegates.GetStringi(stringName, index);
				byte *tmpPtr = strPtr;
				if( strPtr == null )
					return null;
				while( len < 65536 && *tmpPtr++ != 0 )
					len++;
				strBytes = new byte[len];
				for( i = 0; i < len; i++ )
					strBytes[i] = *strPtr++;
			}
			return Encoding.ASCII.GetString(strBytes, 0, len);
		}
		
        public static void EnableClientState(ClientStateCap cap) {
            Delegates.EnableClientState(cap);
        }
        
        public static void DisableClientState(ClientStateCap cap) {
            Delegates.DisableClientState(cap);
        }
  		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="structElementCount">2, 3 or 4</param>
		/// <param name="type"></param>
		/// <param name="stride"></param>
		/// <param name="offset"></param>
		public static void VertexPointer(int structElementCount, PointerToType type, int stride, int offset) {
			Delegates.VertexPointer(structElementCount, type, stride, offset);
		}
  		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="structElementCount">3 or 4</param>
		/// <param name="type"></param>
		/// <param name="stride"></param>
		/// <param name="offset"></param>
		public static void ColorPointer(int structElementCount, PointerToType type, int stride, int offset) {
			Delegates.ColorPointer(structElementCount, type, stride, offset);
		}
  		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="structElementCount">3 or 4</param>
		/// <param name="type"></param>
		/// <param name="stride"></param>
		/// <param name="offset"></param>
		public static void SecondaryColorPointer(int structElementCount, PointerToType type, int stride, int offset) {
			Delegates.SecondaryColorPointer(structElementCount, type, stride, offset);
		}
  		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="structElementCount">1, 2, 3 or 4</param>
		/// <param name="type"></param>
		/// <param name="stride"></param>
		/// <param name="offset"></param>
		public static void TexCoordPointer(int structElementCount, PointerToType type, int stride, int offset) {
			Delegates.TexCoordPointer(structElementCount, type, stride, offset);
		}
  		
		public static void FogCoordPointer(PointerToType type, int stride, int offset) {
			Delegates.FogCoordPointer(type, stride, offset);
		}
  		
		public static void IndexPointer(PointerToType type, int stride, int offset) {
			Delegates.IndexPointer(type, stride, offset);
		}
  		
		public static void NormalPointer(PointerToType type, int stride, int offset) {
			Delegates.NormalPointer(type, stride, offset);
		}
  		
		public static void EdgeFlagPointer(int stride, int offset) {
			Delegates.EdgeFlagPointer(stride, offset);
		}
  		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="structElementCount">1, 2, 3 or 4</param>
		/// <param name="type"></param>
		/// <param name="stride"></param>
		/// <param name="offset"></param>
		public static void VertexAttribPointer(int attribIndex, int structElementCount, PointerToType type, byte normalized, int stride, int offset) {
			Delegates.VertexAttribPointer(attribIndex, structElementCount, type, normalized, stride, offset);
		}
		
		public static void DrawArrays(BeginMode mode, int first, int count) {
			Delegates.DrawArrays(mode, first, count);
		}
		
		public static void DrawElements(BeginMode mode, int count, PointerToType type, int offset) {
			Delegates.DrawElements(mode, count, type, offset);
		}
		
		public static void DrawRangeElements(BeginMode mode, int start, int end, int count, PointerToType type, int offset) {
			Delegates.DrawRangeElements(mode, start, end, count, type, offset);
		}
	}
}