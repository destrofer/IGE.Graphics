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
using IGE.IO;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	/// <summary>
	/// </summary>
	public class SpriteLibrary : IDisposable {
		protected List<Texture> Textures = new List<Texture>();
		protected Dictionary<string, SpriteFrameset> Framesets = new Dictionary<string, SpriteFrameset>();
		
		public bool Disposed { get { return Textures == null; } }
		
		public SpriteFrameset this[string id] { get { return Framesets[id]; } }
		
		public SpriteLibrary(string fileName) {
			Load(fileName);
		}
		
		public void Load(string fileName) {
			XmlFile xml = GameFile.LoadFile<XmlFile>(fileName);
			Load(xml.Root, fileName);
		}
		
		public void Load(DomNode data, string fileName) {
			SpriteLibrary.FramesetData framesetData;
			SpriteLibrary.FrameData frameData, quadFrameData;
			List<SpriteLibrary.FrameData> framesData = new List<SpriteLibrary.FrameData>();
			List<SpriteLibrary.FramesetData> framesetsData = new List<SpriteLibrary.FramesetData>();
			List<QuadSorter.Quad> quads = new List<QuadSorter.Quad>();
			QuadSorter.Quad quad;
			int frameIndex = 0, framesetIndex, width, height;
			Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
			string bitmapPath;
			Bitmap bitmap;
			BitmapFile bitmapFile;
			Texture texture;
			float quadWidth, quadHeight;
			bool quadFound;
			
			// load xml and related bitmaps
			foreach( DomNode node in data ) {
				if( !node.Name.Equals("frameset", StringComparison.OrdinalIgnoreCase) ) {
					GameDebugger.Log(LogLevel.Notice, "Skipping node '{0}' since 'frameset' is expected", node.Name);
					continue;
				}
				framesetData = new SpriteLibrary.FramesetData();
				
				if( node.Attributes["id"].Equals("") )
					throw new UserFriendlyException("Cannot load sprite library since one or more framesets have no 'id' attribute.", "There are errors in game data files.");
				framesetData.Id = node.Attributes["id"];

				bitmapPath = null;
				if( !node.Attributes["imgRel"].Equals("") )
					bitmapPath = node.Attributes["imgRel"].RelativeTo(fileName);
				else if( !node.Attributes["img"].Equals("") )
					bitmapPath = node.Attributes["img"].ToPath();
				if( bitmapPath != null ) {
					if( !bitmaps.TryGetValue(bitmapPath, out bitmap) ) {
						bitmapFile = GameFile.LoadFile<BitmapFile>(bitmapPath);
						bitmap = bitmapFile.Bitmap;
						bitmaps.Add(bitmapPath, bitmap);
					}
					framesetData.Bitmap = bitmap;
				}
				
				foreach( DomNode child in node ) {
					if( !child.Name.Equals("frame", StringComparison.OrdinalIgnoreCase) ) {
						GameDebugger.Log(LogLevel.Notice, "Skipping node '{0}' since 'frame' is expected", child.Name);
						continue;
					}
					frameData = new SpriteLibrary.FrameData();
					
					bitmapPath = null;
					if( !child.Attributes["imgRel"].Equals("") )
						bitmapPath = child.Attributes["imgRel"].RelativeTo(fileName);
					else if( !child.Attributes["img"].Equals("") )
						bitmapPath = child.Attributes["img"].ToPath();
					if( bitmapPath != null ) {
						if( !bitmaps.TryGetValue(bitmapPath, out bitmap) ) {
							bitmapFile = GameFile.LoadFile<BitmapFile>(bitmapPath);
							bitmap = bitmapFile.Bitmap;
							bitmaps.Add(bitmapPath, bitmap);
						}
						frameData.Bitmap = bitmap;
					}
					else {
						if( framesetData.Bitmap == null )
							throw new UserFriendlyException("Cannot load sprite library since one or more frames have no 'img' attribute.", "There are errors in game data files.");
						frameData.Bitmap = framesetData.Bitmap;
					}
					
					if( !child.Attributes["id"].Equals("") )
						frameData.Id = child.Attributes["id"];
					if( !child.Attributes["x"].Equals("") )
						frameData.X = int.Parse(child.Attributes["x"]);
					if( !child.Attributes["y"].Equals("") )
						frameData.Y = int.Parse(child.Attributes["y"]);
					if( !child.Attributes["u"].Equals("") )
						frameData.U = int.Parse(child.Attributes["u"]);
					if( !child.Attributes["v"].Equals("") )
						frameData.V = int.Parse(child.Attributes["v"]);
					if( !child.Attributes["width"].Equals("") )
						frameData.Width = int.Parse(child.Attributes["width"]);
					if( !child.Attributes["height"].Equals("") )
						frameData.Height = int.Parse(child.Attributes["height"]);
					if( !child.Attributes["duration"].Equals("") )
						frameData.Duration = child.Attributes["duration"].ToDouble(0.1);
					if( !child.Attributes["next"].Equals("") )
						frameData.NextFrame = child.Attributes["next"].ToInt32(0);
					if( !child.Attributes["event"].Equals("") )
						frameData.Event = child.Attributes["event"];
					
					if( frameData.Width <= 0 )
						frameData.Width = frameData.Bitmap.Width;
					if( frameData.Height <= 0 )
						frameData.Height = frameData.Bitmap.Height;
					
					framesetData.Frames.Add(frameData);
					framesData.Add(frameData);
					
					quadWidth = (float)(frameData.Width + 2); // 2 is the padding of 1 px from every side
					quadHeight = (float)(frameData.Height + 2);
					quadFound = false;
					
					// we need only unique pictures so we will check every quad already added
					foreach( QuadSorter.Quad q in quads ) {
						if( q.Width == quadWidth && q.Height == quadHeight ) {
							quadFrameData = ((List<SpriteLibrary.FrameData>)q.Data)[0];
							if( quadFrameData.Bitmap == frameData.Bitmap && quadFrameData.U == frameData.U && quadFrameData.V == frameData.V ) {
								// same width, height, u, v and bitmap ... this frame is not the first to use that graphic
								((List<SpriteLibrary.FrameData>)q.Data).Add(frameData);
								quadFound = true;
								break;
							}
						}
					}
					if( !quadFound ) {
						quad = new QuadSorter.Quad();
						quad.Data = new List<SpriteLibrary.FrameData>();
						((List<SpriteLibrary.FrameData>)quad.Data).Add(frameData);
						quad.Width = quadWidth; 
						quad.Height = quadHeight;
						quads.Add(quad);
					}
				}
				framesetsData.Add(framesetData);
			}
			
			// sort and build containers
			QuadSorter sorter = new QuadSorter(quads, 256f, 256f);
			
			// create textures
			frameIndex = 0;
			foreach( QuadSorter.QuadContainer container in sorter.Containers ) {
				width = Texture.ConvertToSuitableDimension((int)container.Width);
				height = Texture.ConvertToSuitableDimension((int)container.Height);
				bitmap = new Bitmap(width, height, 4);
				bitmap.Format = BitmapFormat.BGRA;
				foreach( QuadSorter.Quad q in container.Quads ) {
					frameData = ((List<SpriteLibrary.FrameData>)q.Data)[0];
					bitmap.CopyPixels(frameData.Bitmap, (int)q.X + 1, (int)q.Y + 1, frameData.U, frameData.V, frameData.Width, frameData.Height);
				}
				
				/*
				// Uncomment this block if the sprite managing algorithm needs debugging
				bitmapFile = new BitmapFile();
				bitmapFile.Bitmap = bitmap;
				bitmapFile.Save(String.Format("test/spltex-{0}.png", frameIndex++));
				*/
				
				texture = new Texture(bitmap.Width, bitmap.Height, bitmap.Pixels, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
				Textures.Add(texture);
				
				foreach( QuadSorter.Quad q in container.Quads ) {
					foreach( FrameData fd in (List<FrameData>)q.Data ) {
						fd.Frame = new SpriteFrame(fd.Id, new Image(texture, (int)q.X + 1, (int)q.Y + 1, fd.Width, fd.Height, fd.X, fd.Y), fd.Duration, fd.NextFrame, fd.Event);
					}
				}
			}
			
			framesetIndex = 0;
			foreach( FramesetData fsdata in framesetsData ) {
				List<SpriteFrame> frames = new List<SpriteFrame>();
				frameIndex = 0;
				foreach( FrameData fdata in fsdata.Frames ) {
					fdata.Frame.Index = frameIndex++;
					frames.Add(fdata.Frame);
				}
				SpriteFrameset frameset = new SpriteFrameset(fsdata.Id, frames);
				frameset.Index = framesetIndex++;
				Framesets.Add(frameset.Id, frameset);
			}
		}
		
		public void Dispose() {
			if( Textures != null ) {
				foreach( Texture tex in Textures )
					tex.Dispose();
				Textures = null;
			}
		}
		
		public class FramesetData {
			public string Id = null;
			public Bitmap Bitmap = null;
			public List<FrameData> Frames = new List<FrameData>();
		}
		
		public class FrameData {
			public string Id = null;
			public Bitmap Bitmap = null;
			public SpriteFrame Frame = new SpriteFrame();
			public int X = 0;
			public int Y = 0;
			public int U = 0;
			public int V = 0;
			public int Width = 0;
			public int Height = 0;
			public double Duration = 0.1;
			public string Event = null;
			public int NextFrame = 0;
		}
	}
}
