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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using IGE;
using IGE.Graphics.OpenGL;
using IGE.IO;

namespace IGE.Graphics {
	public class Texture : CacheableObject<Texture> {
		public static TextureMinFilter DefaultMinFilter = TextureMinFilter.Linear;
		public static TextureMagFilter DefaultMagFilter = TextureMagFilter.Linear;
	
		protected int m_Id;
		public int Id { get { return m_Id; } }

		protected string m_FileName;
		public string FileName { get { return m_FileName; } }

		protected TextureLoadError m_LastError;
		public TextureLoadError LastError { get { return m_LastError; } }
		
		protected static int m_MaxUnitsAvailable;
		public static int MaxUnitsAvailable { get {
				if( m_MaxUnitsAvailable > 0 )
					return m_MaxUnitsAvailable;
				GL.GetInteger(ParamName.MaxTextureUnits, out m_MaxUnitsAvailable);
				if( m_MaxUnitsAvailable < 1 )
					m_MaxUnitsAvailable = 1;
				return m_MaxUnitsAvailable;
			}
		}
		
		protected static int m_ActiveUnit = 0;
		public static int ActiveUnit { get { return m_ActiveUnit; } set { if( m_ActiveUnit != value ) SetActiveUnit(value); } }

		protected int m_Width;
		public int Width { get { return m_Width; } }

		protected int m_Height;
		public int Height { get { return m_Height; } }
		
		protected float m_PixelSizeS;
		public float PixelSizeS { get { return m_PixelSizeS; } }
		
		protected float m_PixelSizeT;
		public float PixelSizeT { get { return m_PixelSizeT; } }
		
		protected TextureTarget m_TextureType = TextureTarget.Texture2D;
		public TextureTarget TextureType {
			get { return m_TextureType; }
			set { m_TextureType = value; }
		}
		
		protected int m_MultisamplingSamples = 8;
		public int MultisamplingSamples {
			get { return m_MultisamplingSamples; }
			set { m_MultisamplingSamples = value; }
		}
		
		protected TextureMinFilter m_MinFilter = DefaultMinFilter;
		public TextureMinFilter MinFilter {
			get { return m_MinFilter; }
			set { SetFilter(value); }
		}

		protected TextureMagFilter m_MagFilter = DefaultMagFilter;
		public TextureMagFilter MagFilter {
			get { return m_MagFilter; }
			set { SetFilter(value); }
		}
				
		public bool Loaded { get { return m_Id != 0; } }
		
		#region Constructors /destructors and loaders
		
		public Texture() {
			m_Id = 0; // an OpenGL texture unit id
			m_FileName = "";
			m_Width = 0;
			m_Height = 0;
			m_PixelSizeS = 0.0f;
			m_PixelSizeT = 0.0f;
		}
		
		public Texture(string filename) : this() {
			Load(filename);
		}
		
		public Texture(string filename, TextureMinFilter min_filter, TextureMagFilter mag_filter) : this() {
			Load(filename, min_filter, mag_filter);
		}

		/// <summary>
		/// Creates a texture with memory allocated, but undefined pixels.
		/// Useful when texture is intended to be used as framebuffer render target.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public Texture(int width, int height) : this(width, height, TextureMinFilter.Nearest, TextureMagFilter.Nearest) {
		}

		/// <summary>
		/// Creates a texture with memory allocated, but undefined pixels.
		/// Useful when texture is intended to be used as framebuffer render target.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="min_filter"></param>
		/// <param name="mag_filter"></param>
		public Texture(int width, int height, TextureMinFilter min_filter, TextureMagFilter mag_filter) : this() {
			Create(width, height);
		}
		
		public Texture(int width, int height, byte[] pixels) : this() {
			Load(width, height, pixels);
		}
		
		public Texture(int width, int height, byte[] pixels, TextureMinFilter min_filter, TextureMagFilter mag_filter) : this() {
			Load(width, height, pixels, min_filter, mag_filter);
		}
		
		public bool GenTexId() {
			if( m_Id != 0 )
				return true;
			m_Id = GL.GenTexture();
			if( m_Id == 0 ) {
				m_LastError = TextureLoadError.OutOfTextureHandles;
				return false;
			}
			
			Bind();
			GL.TexParameter(m_TextureType, TextureParamName.TextureMinFilter, (int)m_MinFilter);
			GL.TexParameter(m_TextureType, TextureParamName.TextureMagFilter, (int)m_MagFilter);
			m_LastError = TextureLoadError.NoError;
			return true;
		}

		public bool Create(int width, int height) {
			return Load(width, height, null);
		}

		public bool Create(int width, int height, TextureMinFilter min_filter, TextureMagFilter mag_filter) {
			return Load(width, height, null, min_filter, mag_filter);
		}
		
		public bool Load(int width, int height, byte[] pixels) {
			if( !GenTexId() )
				return false;
				
			m_LastError = TextureLoadError.NoError;

			m_Width = width;
			m_Height = height;
			m_PixelSizeS = 1.0f / (float)m_Width;
			m_PixelSizeT = 1.0f / (float)m_Height;

			Bind();
			
			if( pixels == null ) {
				if( m_TextureType == TextureTarget.Texture2DMultisample ) {
					GL.TexImage2DMultisample(
						m_TextureType,
						m_MultisamplingSamples, InternalPixelFormat.Rgba8,
						width, height,
						true
					);
				}
				else {
					GL.TexImage2D(
						m_TextureType,
						0, InternalPixelFormat.Rgba8,
						width, height,
						0, PixelFormatEnum.Bgra,
						IGE.Graphics.OpenGL.PixelType.UnsignedByte, IntPtr.Zero
					);
				}
			}
			else {
				lock(pixels.SyncRoot) {
					unsafe {
						fixed(byte *pix = &pixels[0]) {
							GL.TexImage2D(
								m_TextureType,
								0, InternalPixelFormat.Rgba8,
								width, height,
								0, PixelFormatEnum.Bgra,
								IGE.Graphics.OpenGL.PixelType.UnsignedByte, pix
							);
						}
					}
				}
			}
			
			if( m_MinFilter != TextureMinFilter.Linear && m_MinFilter != TextureMinFilter.Nearest )
				GL.GenerateMipmap(m_TextureType);

			return true;
		}
		
		public bool Load(int width, int height, byte[] pixels, TextureMinFilter min_filter, TextureMagFilter mag_filter) {
			SetFilter(min_filter, mag_filter);
			return Load(width, height, pixels);
		}
		
		public bool Load(Bitmap bmp) {
			return Load(bmp.Width, bmp.Height, bmp.Pixels);
		}
		
		public bool Load(Bitmap bmp, TextureMinFilter min_filter, TextureMagFilter mag_filter) {
			SetFilter(min_filter, mag_filter);
			return Load(bmp);
		}
		
		public override bool Load(string filename) {
			m_LastError = TextureLoadError.NoError;
			if(File.Exists(filename)) {
				BitmapFile bmpf;
				try {
					bmpf = GameFile.LoadFile<BitmapFile>(filename);
				}
				catch(OutOfMemoryException) {
					m_LastError = TextureLoadError.OutOfMemory;
					return false;
				}
				catch(Exception ex) {
					throw new UserFriendlyException(String.Format("There was an error while trying to load a texture bitmap file: {0}", filename), "Error occured while trying to load a texture", ex);
				}
				return Load(bmpf.Bitmap);
			}
			else
				m_LastError = TextureLoadError.FileNotFound;
			return false;
		}
		
		public bool Load(string filename, TextureMinFilter min_filter, TextureMagFilter mag_filter) {
			SetFilter(min_filter, mag_filter);
			return Load(filename);
		}
		
		public override void OnDispose(bool manual_dispose)
		{
			if( m_Id != 0 ) {
				if( manual_dispose ) {
					// This code executes only when disposal is called by code, not by a garbage collector
					GL.DeleteTexture(ref m_Id);
				}
				m_FileName = "";
				m_Id = 0;
			}
		}

		#endregion
		
		public void Bind() {
			GL.BindTexture(m_TextureType, m_Id);
		}
		
		public void Bind(int textureUnit) {
			if( SetActiveUnit(textureUnit) )
				Bind();
		}
		
		public void Unbind() {
			GL.BindTexture(m_TextureType, 0);
		}

		public static void UnbindAny() {
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public static void Enable() {
			GL.Enable(EnableCap.Texture2D);
		}
		
		public static void Disable() {
			GL.Disable(EnableCap.Texture2D);
		}
		
		public static bool SetActiveUnit(int unit_index) {
			if( unit_index < 0 )
				unit_index = 0;
			if( unit_index >= MaxUnitsAvailable )
				return false;
			if( GL.Delegates.ActiveTextureARB == null )
				return false;
			GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + unit_index));
			m_ActiveUnit = unit_index;
			return true;
		}

		public void SetFilter(TextureMinFilter min, TextureMagFilter mag) {
			m_MinFilter = min;
			m_MagFilter = mag;
			if( m_Id != 0 ) {
				Bind();
				GL.TexParameter(m_TextureType, TextureParamName.TextureMinFilter, (int)min);
				GL.TexParameter(m_TextureType, TextureParamName.TextureMagFilter, (int)mag);
			}
		}
		
		public void SetFilter(TextureMinFilter min) {
			m_MinFilter = min;
			if( m_Id != 0 ) {
				Bind();
				GL.TexParameter(m_TextureType, TextureParamName.TextureMinFilter, (int)min);
			}
		}
		
		public void SetFilter(TextureMagFilter mag) {
			m_MagFilter = mag;
			if( m_Id != 0 ) {
				Bind();
				GL.TexParameter(m_TextureType, TextureParamName.TextureMagFilter, (int)mag);
			}
		}
		

		public void SetWrapModeS(TextureWrapMode mode) {
			GL.TexParameter(m_TextureType, TextureParamName.TextureWrapS, (int)mode);
		}
		
		public void SetWrapModeT(TextureWrapMode mode) {
			GL.TexParameter(m_TextureType, TextureParamName.TextureWrapT, (int)mode);
		}
		
		public static void SetMTCoord(int unit_index, float s, float t) {
			GL.MultiTexCoord2((TextureUnit)((int)TextureUnit.Texture0 + unit_index), s, t);
		}
		
		public static int ConvertToSuitableDimension(int size) {
			int curSize = 4; // minimum texture size
			while( size > curSize )
				curSize *= 2;
			return curSize;
		}
		
		public static float GetMaxAnisotropyMaximum() {
			float max = 0f;
			GL.GetFloat(ParamName.MaxTextureMaxAnisotropy, out max);
			return max;
		}
		
		public void SetMaxAnisotropy(float maxAnisotropy) {
			Bind();
			GL.TexParameter(m_TextureType, TextureParamName.TextureMaxAnisotropy, maxAnisotropy);
		}
	}
	
	public enum TextureLoadError {
		NoError,
		FileNotFound,
		OutOfMemory,
		OutOfVideoMemory,
		OutOfTextureHandles
	}
}
