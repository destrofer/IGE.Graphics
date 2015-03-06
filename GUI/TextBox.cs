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
using System.Collections.Generic;
using System.Text;

using IGE;
using IGE.Graphics;
using IGE.Graphics.OpenGL;
using IGE.IO;

namespace IGE.GUI {
	public class TextBox {
		protected string m_Text = null;
		protected Font m_Font = null;
		protected Color4 m_Color = new Color4(1f, 1f, 1f, 1f);
		protected int m_Top = 0;
		protected int m_Left = 0;
		protected int m_Width = 0;
		protected int m_Height = 0;
		protected UITextAlign m_HAlign = UITextAlign.Left;
		protected UIVerticalAlign m_VAlign = UIVerticalAlign.Top;
		protected int m_VerticalOffset = 0;
		protected TextType m_BoxType = TextType.Auto;
		protected TextType m_CurrentBoxType = TextType.Dynamic;
		private bool m_Invalid = true;
		protected int m_RenderCount = 0;
		protected int m_LowResetCount = 0;
	
		public string Text { get { return m_Text; } set { if( !m_Text.Equals(value) ) { m_Text = value; Invalidate(); } } }
		public Font Font { get { return m_Font; } set { if( m_Font != value ) { m_Font = value; Invalidate(); } } }
		public Color4 Color { get { return m_Color; } set { if( !m_Color.Equals(value) ) { m_Color = value; Invalidate(); } } }
		public int Top { get { return m_Top; } set { m_Top = value; } }
		public int Left { get { return m_Left; } set { m_Left = value; } }
		public int Width { get { return m_Width; } set { if( m_Width != value ) { m_Width = value; Invalidate(); } } }
		public int Height { get { return m_Height; } set { if( m_Height != value ) { m_Height = value; Invalidate(); } } }
		public UITextAlign HAlign { get { return m_HAlign; } set { if( m_HAlign != value ) { m_HAlign = value; Invalidate(); } } }
		public UIVerticalAlign VAlign { get { return m_VAlign; } set { if( m_VAlign != value ) { m_VAlign = value; Invalidate(); } } }
		public int VerticalOffset { get { return m_VerticalOffset; } set { m_VerticalOffset = value; } }
		public TextType BoxType { get { return m_BoxType; } set { if( m_BoxType != value ) { m_BoxType = value; Invalidate(); } } }

		public TextBox(string text, Font font, Color4 color, int left, int top, int width, int height, UITextAlign halign, UIVerticalAlign valign, int voffset)
		: this(text, font, color, left, top, width, height, halign, valign, voffset, TextType.Auto)
		{
		}
		
		public TextBox(string text, Font font, Color4 color, int left, int top, int width, int height, UITextAlign halign, UIVerticalAlign valign, int voffset, TextType type) {
			m_Text = text;
			m_Font = font;
			m_Color = color;
			m_Left = left;
			m_Top = top;
			m_Width = width;
			m_Height = height;
			m_HAlign = halign;
			m_VAlign = valign;
			m_VerticalOffset = voffset;
			m_BoxType = type;
			m_CurrentBoxType = (type == TextType.Static) ? TextType.Static : TextType.Dynamic;
		}
		
		/// <summary>
		/// Forces static text box to rebuild it's optimized data on next render
		/// </summary>
		public void Invalidate() {
			m_Invalid = true;
			if( m_CurrentBoxType != TextType.Dynamic && m_BoxType == TextType.Auto ) {
				// If text/font/color/alignment/render area changes too often (3 times
				// in a row within 5 frames) then this box is switched to dynamic mode.
				if( m_RenderCount <= 5 ) {
					if( ++m_LowResetCount >= 3 )
						m_CurrentBoxType = TextType.Dynamic;
				}
				else
					m_LowResetCount = 0;
			}
			m_RenderCount = 0;
		}
		
		internal delegate void StoreChunkDelegate();
		internal delegate void StoreLineDelegate(bool storeEmpty);
		
		public TextLine[] GetLines() {
			List<TextLine> lines = new List<TextLine>();
			if( m_Font != null ) {
				int curLeftWidth = 0, curCenterWidth = 0, curRightWidth = 0, curLineWidth = 0, curChunkWidth = 0;
				List<TextLine.Chunk> curLeftChunks = new List<TextLine.Chunk>();
				List<TextLine.Chunk> curCenterChunks = new List<TextLine.Chunk>();
				List<TextLine.Chunk> curRightChunks = new List<TextLine.Chunk>();
				UITextAlign curAlign = m_HAlign;
				Color4 curColor = m_Color;
				StringBuilder curText = new StringBuilder();
				StringBuilder curEscape = new StringBuilder();
				byte state = 0;
				int pos = 0, len = m_Text.Length, spaceWidth = m_Font[' '].Width;
				char chr;
				
				StoreChunkDelegate storeChunk = () => {
					if( curText.Length == 0 )
						return;
					TextLine.Chunk chunk = new TextLine.Chunk {
						Width = curChunkWidth,
						Color = curColor,
						Value = curText.ToString()
					};
					if( curAlign == UITextAlign.Left ) {
						curLeftChunks.Add(chunk);
						curLeftWidth += curChunkWidth;
					}
					else if( curAlign == UITextAlign.Center ) {
						curCenterChunks.Add(chunk);
						curCenterWidth += curChunkWidth;
					}
					else {
						curRightChunks.Add(chunk);
						curRightWidth += curChunkWidth;
					}
					
					curLineWidth += curChunkWidth;
					curChunkWidth = 0;
					curText.Clear();
				};
				
				StoreLineDelegate storeLine = (storeEmpty) => {
					storeChunk();
					if( !storeEmpty && curLineWidth == 0 )
						return;
					lines.Add(new TextLine {
						LineWidth = curLineWidth,
						LeftWidth = curLeftWidth,
						CenterWidth = curCenterWidth,
						RightWidth = curRightWidth,
						LeftChunks = curLeftChunks.ToArray(),
						CenterChunks = curCenterChunks.ToArray(),
						RightChunks = curRightChunks.ToArray()
					});
					curLineWidth = curLeftWidth = curCenterWidth = curRightWidth = 0;
					curLeftChunks.Clear();
					curCenterChunks.Clear();
					curRightChunks.Clear();
				};
				
				for( pos = 0; pos < len; pos++ ) {
					chr = m_Text[pos];
					if( state == 0 ) {
						if( chr == '⌂' )
							state = 1;
						else {
							if( chr == '\t' ) {
								curText.Append("    ");
								curChunkWidth += spaceWidth * 4;
							}
							else if( chr == '\n' )
								storeLine(true);
							else if( chr != '\r' ) {
								curText.Append(chr);
								curChunkWidth += m_Font[chr].Width;
							}
						}
					}
					else {
						if( chr == '⌂' ) {
							state = 0;
							if( curEscape.Length == 0 ) {
								curText.Append('⌂');
								curChunkWidth += m_Font['⌂'].Width;
							}
							else {
								string escapeStr = curEscape.ToString();
								if( escapeStr.Equals("<") ) {
									if( curAlign != UITextAlign.Left ) {
										storeChunk();
										curAlign = UITextAlign.Left;
									}
								}
								else if( escapeStr.Equals("|") ) {
									if( curAlign != UITextAlign.Center ) {
										storeChunk();
										curAlign = UITextAlign.Center;
									}
								}
								else if( escapeStr.Equals(">") ) {
									if( curAlign != UITextAlign.Right ) {
										storeChunk();
										curAlign = UITextAlign.Right;
									}
								}
								else {
									Color4 col;
									if( Color4.TryParse(escapeStr, out col) && !col.Equals(curColor) ) {
										storeChunk();
										curColor = col;
									}
								}
							}
							curEscape.Clear();
						}
						else
							curEscape.Append(chr);
					}
				}
				storeLine(false);
			}
			return lines.ToArray();
		}
		
		protected void Compile() {
			m_Invalid = false;
			
			
		}
		
		public void GetTextSize(out int width, out int height) {
			TextLine[] lines = GetLines();
			width = 0;
			int j;
			for( int i = lines.Length - 1; i >= 0; i-- ) {
				j = lines[i].LineWidth;
				if( width < j )
					width = j;
			}
			height = lines.Length * m_Font.LineHeight;
		}
		
		public void Render() {
			Render(m_Left, m_Top, m_Width, m_Height);
		}
		
		public void Render(int x, int y) {
			Render(x, y, m_Width, m_Height);
		}
		
		public void Render(int x, int y, int width, int height) {
			m_RenderCount++;

			// If no changes to text/font/color/alignment/render area were
			// made during last 25 frames then this box is switched to static mode
			if( m_CurrentBoxType == TextType.Dynamic && m_BoxType == TextType.Auto && m_RenderCount > 25 )
				m_CurrentBoxType = TextType.Static;
			
			if( m_CurrentBoxType == TextType.Dynamic ) {
				// TODO: Render text without compiling it
			}
			else {
				if( m_Invalid )
					Compile();
				// TODO: Render text according to compiled data
			}
			
			int i, j, xx = x, maxWidth = 0, maxHeight;
			
			TextLine[] lines = GetLines();
			if( width == 0 ) {
				for( i = lines.Length - 1; i >= 0; i-- ) {
					j = lines[i].LineWidth;
					if( maxWidth < j )
						maxWidth = j;
				}
			}
			else {
				maxWidth = width;
			}
			
			maxHeight = lines.Length * m_Font.LineHeight;
			
			if( m_VAlign == UIVerticalAlign.Center )
				y += (height - maxHeight + m_Font.LineHeight) / 2 - m_Font.CenterLine;
			else if( m_VAlign == UIVerticalAlign.Bottom )
				y += height - maxHeight;
			y += m_Font.BaseLine;
			
			m_Font.StartTextOutput();
			
			foreach(TextLine line in lines) {
				if( line.LeftWidth > 0 ) {
					x = xx;
					foreach (TextLine.Chunk chunk in line.LeftChunks) {
						GL.Color4(chunk.Color);
						x = m_Font.OutText(x, y, chunk.Value);
					}
				}
				
				if( line.CenterWidth > 0 ) {
					x = xx + (maxWidth - line.CenterWidth) / 2;
					foreach (TextLine.Chunk chunk in line.CenterChunks) {
						GL.Color4(chunk.Color);
						x = m_Font.OutText(x, y, chunk.Value);
					}
				}
				
				if( line.RightWidth > 0 ) {
					x = xx + maxWidth - line.RightWidth;
					foreach (TextLine.Chunk chunk in line.RightChunks) {
						GL.Color4(chunk.Color);
						x = m_Font.OutText(x, y, chunk.Value);
					}
				}
				y += m_Font.LineHeight;
			}
			
			m_Font.EndTextOutput();
		}
		
		public enum TextType {
			Auto,
			Static,
			Dynamic
		}
		
		public enum TextChunkType {
			Text,
			NewLine,
			Color,
			LeftAlign,
			CenterAlign,
			RightAlign
		}
		
		public struct TextLine {
			public int LineWidth;
			public int LeftWidth;
			public int CenterWidth;
			public int RightWidth;
			public Chunk[] LeftChunks;
			public Chunk[] CenterChunks;
			public Chunk[] RightChunks;
			
			public struct Chunk {
				public int Width;
				public Color4 Color;
				public string Value;
			}
		}
	}
}