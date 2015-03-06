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

using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using SystemBitmap = System.Drawing.Bitmap;
using SystemColor = System.Drawing.Color;

namespace IGE.IO {
	[FileFormat("png")]
	[FileFormat("jpg")]
	[FileFormat("jpeg")]
	[FileFormat("bmp")]
	[FileFormat("tiff")]
	[FileFormat("gif")]
	[FileFormat("ico")]
	[FileFormat("guid")]
	[FileFormat("emf")]
	[FileFormat("exif")]
	[FileFormat("wmf")]
	public class BitmapFile : GameFile {
		public Bitmap Bitmap = null;
		
		public BitmapFile() {
		}
		
		public BitmapFile(Stream file) {
			using( SystemBitmap bitmap = new SystemBitmap(file) ) {
				Bitmap = new Bitmap(bitmap);
			}
		}
		
		public virtual void Save(string savePath) {
			if( Bitmap == null )
				return;
			unsafe {
				byte[] inPixels;
				SystemPixelFormat pixelFormat;
				switch( Bitmap.BytesPerPixel ) {
					case 1: pixelFormat = SystemPixelFormat.Format8bppIndexed; break;
					case 2: pixelFormat = SystemPixelFormat.Format16bppGrayScale; break;
					case 3: pixelFormat = SystemPixelFormat.Format24bppRgb; break;
					case 4: pixelFormat = SystemPixelFormat.Format32bppArgb; break;
					case 6: pixelFormat = SystemPixelFormat.Format48bppRgb; break;
					case 8: pixelFormat = SystemPixelFormat.Format64bppArgb; break;
					default: pixelFormat = SystemPixelFormat.Undefined; break;
				}
				
				int stride = Bitmap.Width * Bitmap.BytesPerPixel;
				if( stride % 4 != 0 ) {
					// input pixel array for new System.Drawing.Bitmap MUST have a scan line
					// stride divisible by 4 so we will have to reformat the array
					byte[] bPixels = Bitmap.Pixels;
					stride = (stride / 4 + 1) * 4;
					int bwidth = Bitmap.Width * Bitmap.BytesPerPixel;
					int idx = Bitmap.Height * bwidth - 1;
					int idx2 = Bitmap.Height * stride - 1;
					int delta = stride - bwidth;
					int i, j;
					inPixels = new byte[Bitmap.Height * stride];
					
					for( j = Bitmap.Height; j > 0; j-- ) {
						for( i = delta; i > 0; i--, idx2-- )
							inPixels[idx2] = 0;
						for( i = bwidth; i > 0; i--, idx--, idx2-- )
							inPixels[idx2] = bPixels[idx];
					}
				}
				else
					inPixels = Bitmap.Pixels;
					
				fixed(byte *pixels = inPixels) {
					using( SystemBitmap bitmap = new SystemBitmap(Bitmap.Width, Bitmap.Height, stride, pixelFormat, new IntPtr(pixels)) ) {
						if( pixelFormat == SystemPixelFormat.Format8bppIndexed ) {
							ColorPalette palette = bitmap.Palette;
							switch(Bitmap.Format) {
								case BitmapFormat.R:
									for( int i = 0; i < 256; i++ ) palette.Entries[i] = SystemColor.FromArgb(255, i, 0, 0);
									break;
								case BitmapFormat.G:
									for( int i = 0; i < 256; i++ ) palette.Entries[i] = SystemColor.FromArgb(255, 0, i, 0);
									break;
								case BitmapFormat.B:
									for( int i = 0; i < 256; i++ ) palette.Entries[i] = SystemColor.FromArgb(255, 0, 0, i);
									break;
								case BitmapFormat.A:
									for( int i = 0; i < 256; i++ ) palette.Entries[i] = SystemColor.FromArgb(i, 255, 255, 255);
									break;
								default:
									for( int i = 0; i < 256; i++ ) palette.Entries[i] = SystemColor.FromArgb(255, i, i, i);
									break;
							}
							bitmap.Palette = palette;
						}
						bitmap.Save(savePath);
					}
				}
			}
		}
	}
}
