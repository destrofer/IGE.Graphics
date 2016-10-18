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

using IGE;

namespace IGE.Graphics.OpenGL {
	public static partial class GL {
		public partial class Delegates {
			[RuntimeImport("opengl32")]
			public delegate OpenGLError glGetError();
			public static glGetError GetError;
			
			
			[RuntimeImport("opengl32")]
			public delegate void glViewport(int x, int y, int width, int height);
			public static glViewport Viewport;
			
			[RuntimeImport("opengl32")]
			public delegate void glOrtho(double left, double right, double bottom, double top, double zNear, double zFar);
			public static glOrtho Ortho;
			
			[RuntimeImport("opengl32")]
			public delegate void glEnable(EnableCap cap);
			public static glEnable Enable;
			
			[RuntimeImport("opengl32")]
			public delegate void glDisable(EnableCap cap);
			public static glDisable Disable;
			
			[RuntimeImport("opengl32")]
			public delegate void glClearColor(float r, float g, float b, float a);
			public static glClearColor ClearColor;
			
			[RuntimeImport("opengl32")]
			public delegate void glClearDepth(double depth);
			public static glClearDepth ClearDepth;
			
			[RuntimeImport("opengl32")]
			public delegate void glClear(ClearBufferBits bits);
			public static glClear Clear;
			
			[RuntimeImport("opengl32")]
			public delegate void glDepthFunc(DepthFunction func);
			public static glDepthFunc DepthFunc;
			
			[RuntimeImport("opengl32")]
			public delegate void glFrontFace(FrontFaceDirection dir);
			public static glFrontFace FrontFace;
			
			[RuntimeImport("opengl32")]
			public delegate void glHint(HintTarget target, HintMode mode);
			public static glHint Hint;
			
			[RuntimeImport("opengl32")]
			public delegate void glShadeModel(ShadeModel mode);
			public static glShadeModel ShadeModel;
			
			[RuntimeImport("opengl32")]
			public delegate void glBlendFunc(BlendFuncSrc src, BlendFuncDst dst);
			public static glBlendFunc BlendFunc;
			
			[RuntimeImport("opengl32")]
			public delegate void glMatrixMode(MatrixMode mode);
			public static glMatrixMode MatrixMode;
			
			[RuntimeImport("opengl32")]
			public delegate void glLoadIdentity();
			public static glLoadIdentity LoadIdentity;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glLoadMatrixf(float* m);
			public unsafe static glLoadMatrixf LoadMatrixf;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glMultMatrixf(float* m);
			public unsafe static glMultMatrixf MultMatrixf;
			
			[RuntimeImport("opengl32")]
			public delegate void glBegin(BeginMode mode);
			public static glBegin Begin;
			
			[RuntimeImport("opengl32")]
			public delegate void glEnd();
			public static glEnd End;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor3ub(byte red, byte green, byte blue);
			public static glColor3ub Color3ub;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor3ubv(byte* ptr);
			public unsafe static glColor3ubv Color3ubv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor3b(sbyte red, sbyte green, sbyte blue);
			public static glColor3b Color3b;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor3bv(sbyte* ptr);
			public unsafe static glColor3bv Color3bv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor3f(float r, float g, float b);
			public static glColor3f Color3f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor3fv(float* ptr);
			public unsafe static glColor3fv Color3fv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor4ub(byte red, byte green, byte blue, byte alpha);
			public static glColor4ub Color4ub;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor4ubv(byte* ptr);
			public unsafe static glColor4ubv Color4ubv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor4b(sbyte red, sbyte green, sbyte blue, sbyte alpha);
			public static glColor4b Color4b;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor4bv(sbyte* ptr);
			public unsafe static glColor4bv Color4bv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor4f(float r, float g, float b, float a);
			public static glColor4f Color4f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor4fv(float* ptr);
			public unsafe static glColor4fv Color4fv;
			
			[RuntimeImport("opengl32")]
			public delegate void glColor4d(double r, double g, double b, double a);
			public static glColor4d Color4d;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glColor4dv(double* ptr);
			public unsafe static glColor4dv Color4dv;
			
			[RuntimeImport("opengl32")]
			public delegate void glVertex2f(float x, float y);
			public static glVertex2f Vertex2f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glVertex2fv(float* ptr);
			public unsafe static glVertex2fv Vertex2fv;
			
			[RuntimeImport("opengl32")]
			public delegate void glVertex2i(int x, int y);
			public static glVertex2i Vertex2i;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glVertex2iv(int* ptr);
			public unsafe static glVertex2iv Vertex2iv;
			
			[RuntimeImport("opengl32")]
			public delegate void glVertex3f(float x, float y, float z);
			public static glVertex3f Vertex3f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glVertex3fv(float* ptr);
			public unsafe static glVertex3fv Vertex3fv;
			
			[RuntimeImport("opengl32")]
			public delegate void glVertex3d(double x, double y, double z);
			public static glVertex3d Vertex3d;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glVertex3dv(double* ptr);
			public unsafe static glVertex3dv Vertex3dv;
			
			[RuntimeImport("opengl32")]
			public delegate void glVertex3i(int x, int y, int z);
			public static glVertex3i Vertex3i;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glVertex3iv(int* ptr);
			public unsafe static glVertex3iv Vertex3iv;
			
			[RuntimeImport("opengl32")]
			public delegate void glTexCoord2f(float s, float t);
			public static glTexCoord2f TexCoord2f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGenTextures(int amount, [Out] int* texIdsPtr);
			public unsafe static glGenTextures GenTextures;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glDeleteTextures(int amount, int* texIdsPtr);
			public unsafe static glDeleteTextures DeleteTextures;
			
			[RuntimeImport("opengl32")]
			public delegate void glBindTexture(TextureTarget target, int texture);
			public static glBindTexture BindTexture;
			
			[RuntimeImport("opengl32")]
			public delegate void glTexParameterf(TextureTarget target, TextureParamName pname, float param);
			public static glTexParameterf TexParameterf;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glTexParameterfv(TextureTarget target, TextureParamName pname, float* paramsPtr);
			public unsafe static glTexParameterfv TexParameterfv;
			
			[RuntimeImport("opengl32")]
			public delegate void glTexParameteri(TextureTarget target, TextureParamName pname, int param);
			public static glTexParameteri TexParameteri;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glTexParameteriv(TextureTarget target, TextureParamName pname, int* paramsPtr);
			public unsafe static glTexParameteriv TexParameteriv;
			
			[RuntimeImport("opengl32")]
			public delegate void glTexImage2D(TextureTarget target, int level, InternalPixelFormat internalFormat, int width, int height, int border, PixelFormatEnum format, PixelType type, IntPtr pixels);
			public static glTexImage2D TexImage2D;
			
			[RuntimeImport("opengl32")]
			public delegate void glPushAttrib(AttribBits bits);
			public static glPushAttrib PushAttrib;
			
			[RuntimeImport("opengl32")]
			public delegate void glPopAttrib();
			public static glPopAttrib PopAttrib;
			
			[RuntimeImport("opengl32")]
			public delegate void glScaled(double x, double y, double z);
			public static glScaled Scaled;
			
			[RuntimeImport("opengl32")]
			public delegate void glScalef(float x, float y, float z);
			public static glScalef Scalef;
			
			[RuntimeImport("opengl32")]
			public delegate void glTranslated(double x, double y, double z);
			public static glTranslated Translated;
			
			[RuntimeImport("opengl32")]
			public delegate void glTranslatef(float x, float y, float z);
			public static glTranslatef Translatef;
			
			[RuntimeImport("opengl32")]
			public delegate void glRotated(double angle, double x, double y, double z);
			public static glRotated Rotated;
			
			[RuntimeImport("opengl32")]
			public delegate void glRotatef(float angle, float x, float y, float z);
			public static glRotatef Rotatef;
			
			[RuntimeImport("opengl32")]
			public delegate void glPushMatrix();
			public static glPushMatrix PushMatrix;
			
			[RuntimeImport("opengl32")]
			public delegate void glPopMatrix();
			public static glPopMatrix PopMatrix;
			
			
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glGetIntegerv(ParamName pname, [Out] int* paramsPtr);
			public unsafe static glGetIntegerv GetIntegerv;
			


			[RuntimeImport("opengl32")]
			public delegate void glActiveTexture(TextureUnit texture);
			public static glActiveTexture ActiveTexture;
			
			[RuntimeImport("opengl32")]
			public delegate void glMultiTexCoord2f(TextureUnit target, float s, float t);
			public static glMultiTexCoord2f MultiTexCoord2f;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glMultiTexCoord2fv(TextureUnit target, float* v);
			public unsafe static glMultiTexCoord2fv MultiTexCoord2fv;
			
			
			
			[RuntimeImport("opengl32")]
			public delegate void glColorMaterial(FaceName face, ColorMaterialMode mode);
			public static glColorMaterial ColorMaterial;
			
			[RuntimeImport("opengl32")]
			public delegate void glMaterialf(FaceName face, MaterialParamName pname, float param);
			public static glMaterialf Materialf;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glMaterialfv(FaceName face, MaterialParamName pname, float* paramsPtr);
			public unsafe static glMaterialfv Materialfv;
			
			[RuntimeImport("opengl32")]
			public delegate void glMateriali(FaceName face, MaterialParamName pname, int param);
			public static glMateriali Materiali;
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate void glMaterialiv(FaceName face, MaterialParamName pname, int* paramsPtr);
			public unsafe static glMaterialiv Materialiv;

			
			
			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate byte *glGetString(GlStringName stringName);
			public unsafe static glGetString GetString;

			[RuntimeImport("opengl32")]
			[System.Security.SuppressUnmanagedCodeSecurity()]
			public unsafe delegate byte *glGetStringi(GlStringName obj, int index);
			public unsafe static glGetStringi GetStringi;			
			
			
			
			[RuntimeImport("opengl32")]
			public delegate void glEnableClientState(ClientStateCap cap);
			public static glEnableClientState EnableClientState;
			
			[RuntimeImport("opengl32")]
			public delegate void glDisableClientState(ClientStateCap cap);
			public static glDisableClientState DisableClientState;
			
			[RuntimeImport("opengl32", "glVertexPointer")]
			public delegate void glVertexPointer(int structElementCount, PointerToType type, int stride, int offset);
			public static glVertexPointer VertexPointer;
			
			[RuntimeImport("opengl32", "glColorPointer")]
			public delegate void glColorPointer(int structElementCount, PointerToType type, int stride, int offset);
			public static glColorPointer ColorPointer;
			
			[RuntimeImport("opengl32", "glSecondaryColorPointer")]
			public delegate void glSecondaryColorPointer(int structElementCount, PointerToType type, int stride, int offset);
			public static glSecondaryColorPointer SecondaryColorPointer;
			
			[RuntimeImport("opengl32", "glTexCoordPointer")]
			public delegate void glTexCoordPointer(int structElementCount, PointerToType type, int stride, int offset);
			public static glTexCoordPointer TexCoordPointer;
			
			[RuntimeImport("opengl32", "glFogCoordPointer")]
			public delegate void glFogCoordPointer(PointerToType type, int stride, int offset);
			public static glFogCoordPointer FogCoordPointer;
			
			[RuntimeImport("opengl32", "glIndexPointer")]
			public delegate void glIndexPointer(PointerToType type, int stride, int offset);
			public static glIndexPointer IndexPointer;
			
			[RuntimeImport("opengl32", "glNormalPointer")]
			public delegate void glNormalPointer(PointerToType type, int stride, int offset);
			public static glNormalPointer NormalPointer;
			
			[RuntimeImport("opengl32", "glEdgeFlagPointer")]
			public delegate void glEdgeFlagPointer(int stride, int offset);
			public static glEdgeFlagPointer EdgeFlagPointer;
			
			[RuntimeImport("opengl32", "glVertexAttribPointer")]
			public delegate void glVertexAttribPointer(int attribIndex, int structElementCount, PointerToType type, byte normalized, int stride, int offset);
			public static glVertexAttribPointer VertexAttribPointer;
			
			[RuntimeImport("opengl32")]
			public delegate void glDrawArrays(BeginMode mode, int first, int count);
			public static glDrawArrays DrawArrays;
			
			[RuntimeImport("opengl32")]
			public delegate void glDrawElements(BeginMode mode, int count, PointerToType type, int offset);
			public static glDrawElements DrawElements;
			
			[RuntimeImport("opengl32")]
			public delegate void glDrawRangeElements(BeginMode mode, int start, int end, int count, PointerToType type, int offset);
			public static glDrawRangeElements DrawRangeElements;
		}
	}
}
