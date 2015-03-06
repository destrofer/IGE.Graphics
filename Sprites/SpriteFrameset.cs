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
using System.Collections;
using System.Collections.Generic;

using IGE.IO;

namespace IGE.Graphics {
	public class SpriteFrameset : IDisposable {
		public int Index;
		
		protected string m_Id;
		public string Id { get { return m_Id; } }
		
		protected List<SpriteFrame> m_Frames;
		public List<SpriteFrame> Frames { get { return m_Frames; } }
		
		protected Dictionary<string, SpriteFrame> m_IdIndex;
		public Dictionary<string, SpriteFrame> IdIndex { get { return m_IdIndex; } }
		
		public SpriteFrame this[int idx] { get { return (idx >= 0 && idx < m_Count) ? m_Frames[idx] : SpriteFrame.Empty; } }
		public SpriteFrame this[string id] { get { return m_IdIndex.ContainsKey(id) ? m_IdIndex[id] : SpriteFrame.Empty; } }
		
		protected int m_Count;
		public int Count { get { return m_Count; } }
				
		public SpriteFrameset() {
			Index = -1;
			m_Id = null;
			m_Frames = new List<SpriteFrame>();
			m_IdIndex = new Dictionary<string, SpriteFrame>();
			m_Count = 0;
		}
				
		public SpriteFrameset(string id, List<SpriteFrame> frames) {
			Index = -1;
			m_Id = id;
			m_Frames = frames;
			m_IdIndex = new Dictionary<string, SpriteFrame>();
			m_Count = frames.Count;
			foreach( SpriteFrame frame in frames )
				if( frame.Id != null && !frame.Id.Equals("") )
					m_IdIndex.Add(frame.Id, frame);
		}
		
		public void Dispose() {
			m_Frames.Clear();
			m_IdIndex.Clear();
		}
	}
}
