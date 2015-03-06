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

using IGE;
using IGE.Platform;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public class Sprite : IAnimated {
		protected double m_FrameTime;
		public double FrameTime { get { return m_FrameTime; } set { m_FrameTime = value; } }
		
		protected int m_CurrentFrame;
		public virtual int CurrentFrame {
			get { return m_CurrentFrame; }
			set { m_CurrentFrame = value; m_FrameTime = 0.0; }
		}

		protected int m_FrameCount;
		public int FrameCount { get { return m_FrameCount; } }

		protected SpriteFrameset m_Frames;
		public SpriteFrameset Frames { get { return m_Frames; } set { m_Frames = value; m_FrameCount = (m_Frames == null) ? 0 : m_Frames.Count; m_CurrentFrame = 0; m_FrameTime = 0.0; } }
		
		public EnterFrameEvent FrameEnter;
		public EnterFrameEvent FrameEvent;
		
		public SpriteFrame this[int index] { get { return m_Frames[index]; } }
		public SpriteFrame this[string id] { get { return m_Frames[id]; } }
		
		protected bool m_XFlipped;
		public bool XFlipped { get { return m_XFlipped; } set { m_XFlipped = value; } }
		
		protected bool m_YFlipped;
		public bool YFlipped { get { return m_YFlipped; } set { m_YFlipped = value; } }
		
		public Sprite(SpriteFrameset frames) {
			m_CurrentFrame = 0;
			m_FrameTime = 0.0;
			m_FrameCount = 0;
			Frames = frames;
		}
		
		public Sprite() : this(null) {
		}
		
		public virtual void Animate() {
			Animate(1.0);
		}
		
		public virtual void Animate(double speed) {
			int i, j;
			bool frame_changed;
			
			m_FrameTime += Application.InternalTimer.LastFrameTime * speed;
			
			do {
				frame_changed = false;
				if( m_FrameTime < 0.0 ) {
					// reverse animation is slower than forward but possible :)
					j = -1;
					for( i = 0; i < m_FrameCount; i++ ) {
						if( m_Frames[i].NextFrame == m_CurrentFrame ) {
							j = i;
							break;
						}
					}
					m_CurrentFrame = (j < 0) ? (m_CurrentFrame - 1) : j;
					if( m_CurrentFrame < 0 )
						m_CurrentFrame = m_FrameCount - 1;	
					
					m_FrameTime += m_Frames[m_CurrentFrame].Duration;
					
					frame_changed = true;
				}
				else if( m_FrameTime >= m_Frames[m_CurrentFrame].Duration ) {
					m_FrameTime -= m_Frames[m_CurrentFrame].Duration;
					
					if( m_Frames[m_CurrentFrame].NextFrame >= 0 && m_Frames[m_CurrentFrame].NextFrame < m_FrameCount )
						m_CurrentFrame = m_Frames[m_CurrentFrame].NextFrame;
					else {
						m_CurrentFrame++;
						if( m_CurrentFrame >= m_FrameCount )
							m_CurrentFrame = 0;
					}
					
					frame_changed = true;
				}
				
				if( frame_changed ) {
					string event_name = m_Frames[m_CurrentFrame].EventName;
					if( event_name != null ) {
						if( FrameEvent != null ) {
							FrameEvent(this, new EnterFrameEventArgs(event_name, null, speed));
							if( m_Frames == null || m_CurrentFrame < 0 || m_CurrentFrame >= m_FrameCount )
								return;
						}
					}
					if( FrameEnter != null ) {
						FrameEnter(this, new EnterFrameEventArgs(event_name, null, speed));
						if( m_Frames == null || m_CurrentFrame < 0 || m_CurrentFrame >= m_FrameCount )
							return;
					}
				}
			} while(frame_changed);
		}

		public virtual void Render() { Render(0, 0); }
		public virtual void Render(int x, int y) {
			if( m_Frames == null || m_CurrentFrame < 0 || m_CurrentFrame >= m_FrameCount ) {
				// if frame does not exist render a red X instead :)
				GL.PushAttrib(AttribBits.CurrentBit | AttribBits.TextureBit);
				
				Texture.Unbind();
				GL.Color4(1.0f, 0.0f, 0.0f, 1.0f);
				GL.Begin(BeginMode.Lines);
					GL.Vertex2(x - 8.0f, y - 8.0f);
					GL.Vertex2(x + 8.0f, y + 8.0f);
					GL.Vertex2(x + 8.0f, y - 8.0f);
					GL.Vertex2(x - 8.0f, y + 8.0f);
				GL.End();
				
				GL.PopAttrib();
			}
			
			m_Frames[m_CurrentFrame].Render(x, y, m_XFlipped, m_YFlipped);
		}
	}
}
