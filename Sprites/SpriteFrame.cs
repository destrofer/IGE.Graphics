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

using IGE.IO;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public struct SpriteFrame : IDisposable {
		public static SpriteFrame Empty = new SpriteFrame {Index = -1, Id = null, Image = null, Duration = 0.0, NextFrame = -1, EventName = null};
		
		public int Index;
		public string Id;
		public Image Image;
		public double Duration;
		public int NextFrame;
		public string EventName;
		
		public SpriteFrame(int index, string id, Image img, double duration, int next_frame, string event_name) {
			Index = index;
			Id = id;
			Image = img;
			Duration = duration;
			NextFrame = next_frame;
			EventName = event_name;
		}

		public SpriteFrame(string id, Image img, double duration, int next_frame, string event_name) : this(-1, id, img, duration, next_frame, event_name) {}
			
		public void Dispose() {
			this.Image = null;
		}
		
		public void Render() { if( this.Image != null ) this.Image.Render(); }
		public void Render(bool xflip) { if( this.Image != null ) this.Image.Render(xflip); }
		public void Render(bool xflip, bool yflip) { if( this.Image != null ) this.Image.Render(xflip, yflip); }
		public void Render(int x, int y ) { if( this.Image != null ) this.Image.Render(x, y); }
		public void Render(int x, int y, bool xflip ) { if( this.Image != null ) this.Image.Render(x, y, xflip); }
		public void Render(int x, int y, bool xflip, bool yflip ) { if( this.Image != null ) this.Image.Render(x, y, xflip, yflip); }
	}
}
