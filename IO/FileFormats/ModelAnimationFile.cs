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

using IGE;
using IGE.Graphics;
using IGE.Graphics.OpenGL;

namespace IGE.IO {
	public abstract class ModelAnimationFile : GameFile {
		public ModelAnimationFileObject[] Objects = null;
		public ModelAnimationFileEvent[] Events = null;
		public double Duration = 0.0;
		
		public ModelAnimationFile(Stream file) : base(file) {
		}
	}
	
	public class ModelAnimationFileObject {
		public string Name = "";
		public ModelAnimationFileFrame[] Frames;
	}
	
	public struct ModelAnimationFileFrame {
		public double Time;
		public Matrix4 TransformMatrix;
	}
	
	public struct ModelAnimationFileEvent {
		public double Time;
		public string Id;
		public object[] Params;
	}
}