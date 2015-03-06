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
	public class Font : CacheableObject<Font> {
		protected Dictionary<char, CharInfo> m_Chars = new Dictionary<char, CharInfo>();
		protected WeakDictionary<string, TextInfo> m_TextInfo = new WeakDictionary<string, TextInfo>();
		
		public int TextInfoCacheSize { get { return m_TextInfo.Count; } }
		
		public CharInfo DefaultCharacter = null;
		
		public CharInfo this[char chr] {
			get { return m_Chars.ContainsKey(chr) ? m_Chars[chr] : DefaultCharacter; }
			set { m_Chars[chr] = value; }
		}

		protected bool m_Loaded = false;
		public bool Loaded { get { return m_Loaded; } }
		
		protected Vector2 m_Scale;
		protected int m_LineHeight;
		protected int m_BaseLine;
		protected int m_CenterLine;
		
		public Vector2 Scale { get { return m_Scale; } }
		public float ScaleX { get { return m_Scale.X; } set { m_Scale.X = value; m_TextInfo.Clear(); } }
		public float ScaleY { get { return m_Scale.Y; } set { m_Scale.Y = value; m_TextInfo.Clear(); } }
		public int LineHeight { get { return m_LineHeight; } set { m_LineHeight = value; m_TextInfo.Clear(); } }
		public int BaseLine { get { return m_BaseLine; } set { m_BaseLine = value; m_TextInfo.Clear(); } }
		public int CenterLine { get { return m_CenterLine; } set { m_CenterLine = value; m_TextInfo.Clear(); } }

		#region Loading and constructors
		public Font(string filename, float scaleX, float scaleY, int lineHeight, int baseLine, int centerLine) : this(filename) {
			m_Scale = new Vector2(scaleX, scaleY);
			m_LineHeight = lineHeight;
			m_BaseLine = baseLine;
			m_CenterLine = centerLine;
		}
		
		public Font(string filename) : this() {
			Load(filename);
		}
		
		public Font() {
			m_Scale = new Vector2(1.0f, 1.0f);
			m_LineHeight = 16; // defaults for dos font
			m_BaseLine = 14; // defaults for dos font
			m_CenterLine = m_BaseLine / 2;
		}
		
		public override bool Load(String fileName) {
			try {
				using(StructuredTextFile xml = GameFile.LoadFile<StructuredTextFile>(fileName)) {
					return Load(fileName, xml.Root);
				}
			}
			catch(Exception ex) {
				GameDebugger.Log(LogLevel.Error, "Exception occured while trying to load the font from '{0}':", fileName);
				GameDebugger.Log(LogLevel.Error, ex);
				return false;
			}
		}
		
		public bool Load(string fileName, DomNode node) {
			DefaultCharacter = null;
			
			if( !node["relSrc"].Equals("") )
				return Load(node["relSrc"].RelativeTo(fileName));
			else if( !node["src"].Equals("") )
				return Load(node["src"].ToPath());
			
			LineHeight = node["lineheight"].ToInt32(16);
			if( !LoadCharacters(fileName, node, null, 14, 0.0f, 0.0f, 16, 16, -1, -1) )
				return false;
			
			if( m_Chars.ContainsKey('?') )
				DefaultCharacter = m_Chars['?'];
			else if( m_Chars.ContainsKey(' ') )
				DefaultCharacter = m_Chars[' '];
			else {
				// just take the first character as a default one :/
				IEnumerator<CharInfo> en = m_Chars.Values.GetEnumerator();
				en.Reset();
				DefaultCharacter = en.Current;
			}
			
			// after loading set base line to maximum available in the font or to the value in the 'font' node 'baseline' attribute
			m_BaseLine = 0;
			foreach(CharInfo chr in m_Chars.Values) {
				if( chr.BaseLine > m_BaseLine )
					m_BaseLine = chr.BaseLine;
			}
			m_BaseLine = node["baseline"].ToInt32(m_BaseLine);
			m_CenterLine = node["centerline"].ToInt32(m_BaseLine / 2);
			
			m_Loaded = true;
			return true;
		}
		
		public bool LoadCharacters(string fileName, DomNode node, Texture defaultTexture, int defaultBaseline, float defaultU, float defaultV, int defaultWidth, int defaultHeight, int defaultTexWidth, int defaultTexHeight) {
			Texture tex = null;
			int baseLine, width, height, texWidth, texHeight;
			float u, v;
			
			if( !node["relTex"].Equals("") ) {
				tex = Texture.Cache(node["relTex"].RelativeTo(fileName));
				if( tex == null )
					GameDebugger.Log(LogLevel.Error, "Could not load font image '{0}'", node["relTex"].RelativeTo(fileName));
			}
			else if( !node["tex"].Equals("") ) {
				tex = Texture.Cache(node["tex"].ToPath());
				if( tex == null )
					GameDebugger.Log(LogLevel.Error, "Could not load font image '{0}'", node["tex"].ToPath());
			}
			else
				tex = defaultTexture;
			
			if( tex == null )
				return false;
			
			baseLine = node["baseline"].ToInt32(-10000);
			if( baseLine <= -10000 )
				baseLine = defaultBaseline;
			
			width = node["width"].ToInt32(-1);
			if( width < 0 )
				width = defaultWidth;
			
			height = node["height"].ToInt32(-1);
			if( height < 0 )
				height = defaultHeight;
			
			texWidth = node["texWidth"].ToInt32(-1);
			if( texWidth < 0 )
				texWidth = defaultTexWidth;

			texHeight = node["texHeight"].ToInt32(-1);
			if( texHeight < 0 )
				texHeight = defaultTexHeight;
			
			u = (float)node["u"].ToInt32(-1);
			if( u < 0.0f )
				u = defaultU;
			
			v = (float)node["v"].ToInt32(-1);
			if( v < 0.0f )
				v = defaultV;
			
			CharInfo chr;
			foreach(DomNode child in node) {
				if( child.Name.ToLower().Equals("char") ) {
					chr = new CharInfo(child, tex, baseLine, u, v, width, height, texWidth, texHeight);
					m_Chars[chr.Symbol] = chr;
				}
				else if( child.Name.ToLower().Equals("characters") )
					if( !LoadCharacters(fileName, child, tex, baseLine, u, v, width, height, texWidth, texHeight) )
						return false;
			}
			
			return true;
		}

		public void Unload() {
			DefaultCharacter = null;
			foreach(CharInfo chr in m_Chars.Values)
				chr.Texture = null; // just to make garbage collecting a bit faster (maybe?)
			m_Chars.Clear();
		}
		#endregion Loading and constructors

		#region Text info
		public void GetTextSize(out float width, out float height, string str) {
			TextInfo info = GetTextInfo(str);
			width = info.Width;
			height = info.Height;
		}
		
		public void GetTextSize(out int width, out int height, string format, params object[] param_list) {
			GetTextSize(out width, out height, String.Format(format, param_list));
		}

		public TextInfo GetTextInfo(string text) {
			return GetTextInfo(text, true);
		}

		public TextInfo GetTextInfo(string text, bool includeLines) {
			TextInfo oti = GetUnscaledTextInfo(text);
			if( m_Scale.X == 1f && m_Scale.Y == 1f )
				return oti;
			TextInfo ti = new TextInfo();
			ti.Width = oti.Width * m_Scale.X;
			ti.Height = oti.Height * m_Scale.Y;
			if( includeLines ) {
				ti.LineWidth = new float[oti.LineWidth.Length];
				for( int i = ti.LineWidth.Length - 1; i >= 0; i-- )
					ti.LineWidth[i] = oti.LineWidth[i] * m_Scale.X;
			}
			return ti;
		}

		public TextInfo GetUnscaledTextInfo(string text) {
			TextInfo info = m_TextInfo[text];
			if( info == null ) {
				info = new TextInfo();
				
				// Calculate info for the given text:
				//   width as a whole and for every line
				//   height for the whole text
				char symbol;
				float width;
				float x = width = 0.0f, y = 0.0f;
				int i, sl = text.Length, escape = 0, bl = 0;
				List<float> widths = new List<float>();
				
				for( i = 0; i < sl; i++ ) {
					symbol = text[i];
					if( symbol == '\r' || symbol == '\t' )
						continue;
					if( escape > 0 ) {
						if( escape == 1 ) {
							if( symbol == '<' ) {
								escape = 0;
								continue;
							}
							else if( symbol == '|' ) {
								escape = 0;
								continue;
							}
							else if( symbol == '>' ) {
								escape = 0;
								continue;
							}
							else if( (symbol >= '0' && symbol <= '9') || (symbol >= 'a' && symbol <= 'f') || (symbol >= 'A' && symbol <= 'F') ) {
								escape++;
								bl++;
								continue;
							}
							else
								escape = -1;
						}
						else if( escape <= 8 ) {
							if( (symbol >= '0' && symbol <= '9') || (symbol >= 'a' && symbol <= 'f') || (symbol >= 'A' && symbol <= 'F') ) {
								escape++;
								bl++;
								continue;
							}
							else
								escape = -1;
						}
						else
							escape = -1;
					}
					
					if( escape < 0 ) {
						if( bl > 0 ) {
							switch( bl ) {
								case 3:
									escape = bl - 3;
									break;
								case 4:
								case 5:
									escape = bl - 4;
									break;
								case 6:
								case 7:
									escape = bl - 6;
									break;
								case 8:
									escape = bl - 8;
									break;
								default:
									escape = bl;
									break;
							}
						}
						
						if( escape > 0 ) {
							i -= escape + 1;
							escape = 0;
							continue;
						}
						escape = 0;
					}
					else if( symbol == '⌂' ) {
						if( escape == 0 ) {
							bl = 0;
							escape = 1;
							continue;
						}
						else
							escape = 0;
					}
					
					if( symbol == '\n' ) {
						y += LineHeight;
						if( width < x )
							width = x;
						widths.Add(x);
						x = 0.0f;
					}
					else {
						x += this[text[i]].Width;
					}
				}
				if( x > 0.0f )
					y += LineHeight;
				
				if( width < x )
					width = x;
				widths.Add(x);
				
				info.Width = width;
				info.Height = y;
				info.LineWidth = widths.ToArray();
				
				m_TextInfo[text] = info;
			}
			return info;
		}
		#endregion Text info

		#region Print text
		public void StartTextOutput() {
			if( !Loaded ) Load(GameConfig.DefaultFont.ToPath());
			GL.PushAttrib(AttribBits.EnableBit);
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
		}
		
		public void EndTextOutput() {
			GL.PopAttrib();
		}
		
		public int OutText(int x, int y, string str) {
			if( !Loaded ) return x;
			int i, sl = str.Length;
			CharInfo chr;
			for( i = 0; i < sl; i++ ) {
				chr = this[str[i]];
				chr.Render(x, y);
				x += chr.Width;
			}
			return x;
		}
		
		public void Print(float x, float y, string str) {
			if( !Loaded ) Load(GameConfig.DefaultFont.ToPath());
			if( !Loaded ) return;
			
			float cx = x, cy = y;
			int i, sl, line = 0;
			char symbol;
			CharInfo chr;
			TextInfo info = GetTextInfo(str);
			
			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			
			if( View.YAxisIsUp )
				cy -= (BaseLine - LineHeight) * m_Scale.Y;
			else
				cy += BaseLine * m_Scale.Y;
			GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);
			sl = str.Length;
			int escape = 0, align = 0;
			StringBuilder sb = new StringBuilder();
			
			for( i = 0; i < sl; i++ ) {
				symbol = str[i];
				if( symbol == '\r' || symbol == '\t' )
					continue;
				if( escape > 0 ) {
					if( escape == 1 ) {
						if( symbol == '<' ) {
							escape = 0;
							cx = x;
							align = 0;
							continue;
						}
						else if( symbol == '|' ) {
							escape = 0;
							cx = x + (info.Width - info.LineWidth[line]) * 0.5f;
							align = 1;
							continue;
						}
						else if( symbol == '>' ) {
							escape = 0;
							cx = x + (info.Width - info.LineWidth[line]);
							align = 2;
							continue;
						}
						else if( (symbol >= '0' && symbol <= '9') || (symbol >= 'a' && symbol <= 'f') || (symbol >= 'A' && symbol <= 'F') ) {
							escape++;
							sb.Append(symbol);
							continue;
						}
						else
							escape = -1;
					}
					else if( escape <= 8 ) {
						if( (symbol >= '0' && symbol <= '9') || (symbol >= 'a' && symbol <= 'f') || (symbol >= 'A' && symbol <= 'F') ) {
							escape++;
							sb.Append(symbol);
							continue;
						}
						else
							escape = -1; // -1 means that there is something in the buffer
					}
					else
						escape = -1;
				}
				
				if( escape < 0 ) {
					byte r = 255, g = 255, b = 255, a = 255;
					uint color;
					if( sb.Length > 0 ) {
						switch( sb.Length ) {
							case 3:
								color = Convert.ToUInt32(sb.ToString(), 16);
								r = (byte)((color >> 8) & 0xF);
								g = (byte)((color >> 4) & 0xF);
								b = (byte)((color) & 0xF);
								r += (byte)(r << 4);
								g += (byte)(g << 4);
								b += (byte)(b << 4);
								escape = sb.Length - 3;
								GL.Color4(r, g, b, (byte)255);
								break;
							case 4:
							case 5:
								color = Convert.ToUInt32(sb.ToString(), 16);
								r = (byte)((color >> 12) & 0xF);
								g = (byte)((color >> 8) & 0xF);
								b = (byte)((color >> 4) & 0xF);
								a = (byte)((color) & 0xF);
								r += (byte)(r << 4);
								g += (byte)(g << 4);
								b += (byte)(b << 4);
								a += (byte)(a << 4);
								escape = sb.Length - 4;
								GL.Color4(r, g, b, a);
								break;
							case 6:
							case 7:
								color = Convert.ToUInt32(sb.ToString(), 16);
								r = (byte)((color >> 16) & 0xFF);
								g = (byte)((color >> 8) & 0xFF);
								b = (byte)((color) & 0xFF);
								escape = sb.Length - 6;
								GL.Color4(r, g, b, (byte)255);
								break;
							case 8:
								color = Convert.ToUInt32(sb.ToString(), 16);
								r = (byte)((color >> 24) & 0xFF);
								g = (byte)((color >> 16) & 0xFF);
								b = (byte)((color >> 8) & 0xFF);
								a = (byte)((color) & 0xFF);
								escape = sb.Length - 8;
								GL.Color4(r, g, b, a);
								break;
							default:
								escape = sb.Length;
								break;
						}
					}
					
					// positive "escape" means that we have to get back a little bit
					if( escape > 0 ) {
						i -= escape + 1;
						escape = 0;
						continue;
					}
					escape = 0;
				}
				else if( symbol == '⌂' ) {
					if( escape == 0 ) {
						sb.Clear();
						escape = 1;
						continue;
					}
					else
						escape = 0;
				}
				
				if( symbol == '\n' ) {
					line++;
					switch (align) {
						case 0: cx = x; break;
						case 1: cx = x + (info.Width - info.LineWidth[line]) * 0.5f; break;
						case 2: cx = x + (info.Width - info.LineWidth[line]); break;
					}
					if( View.YAxisIsUp )
						cy -= LineHeight * m_Scale.Y;
					else
						cy += LineHeight * m_Scale.Y;
					continue;
				}
				
				chr = this[symbol];
				chr.Render((int)cx, (int)cy, m_Scale.X, m_Scale.Y);
				cx += m_Scale.X * chr.Width;
			}
		}
		
		public void Print(float x, float y, string format, params object[] param_list) {
			Print(x, y, String.Format(format, param_list));
		}
		
		public void Print(float x, float y, object obj) {
			Print(x, y, obj.ToString());
		}

		public void PrintCentered(float x, float y, string str) {
			TextInfo ti = GetTextInfo(str, false);
			Print(x - ti.Width * 0.5f, y, str);
		}
		public void PrintCentered(float x, float y, string format, params object[] param_list) {
			PrintCentered(x, y, String.Format(format, param_list));
		}
		public void PrintCentered(float x, float y, object obj) {
			PrintCentered(x, y, obj.ToString());
		}

		public void PrintRight(float x, float y, string str) {
			TextInfo ti = GetTextInfo(str, false);
			Print(x - ti.Width, y, str);
		}
		public void PrintRight(float x, float y, string format, params object[] param_list) {
			PrintRight(x, y, String.Format(format, param_list));
		}
		public void PrintRight(float x, float y, object obj) {
			PrintRight(x, y, obj.ToString());
		}
		
		#endregion Print text
	}
}
