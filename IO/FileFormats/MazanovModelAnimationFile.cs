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
using System.Text;

using IGE;
using IGE.IO;
using IGE.Graphics;
using IGE.Graphics.OpenGL;

namespace IGE.IO {
	[FileFormat("mma")]
	public class MazanovModelAnimationFile : ModelAnimationFile {
		public MazanovModelAnimationFile(Stream file) : base(file) {
			BinaryReader r = new BinaryReader(file);
			
			double spf = 1.0 / 30.0; // default is 30 fps (sfp = 1 / fps)
			// int version = 0;
			int objectCount = r.ReadInt32();
			int frameCount = r.ReadInt32();
			int i, j;
			ModelAnimationFileObject currentObject;
			ModelAnimationFileFrame currentFrame;
			
			if( objectCount == 0 || frameCount == 0 )
				return;
				
			Duration = (double)frameCount * spf;
			Objects = new ModelAnimationFileObject[objectCount];
			for( i = 0; i < objectCount; i++ ) {
				Objects[i] = currentObject = new ModelAnimationFileObject();
				currentObject.Name = r.ReadFixedSizeString(32, Encoding.ASCII);
				currentObject.Frames = new ModelAnimationFileFrame[frameCount];
				for( j = 0; j < frameCount; j++ ) {
					currentFrame = new ModelAnimationFileFrame();
					currentFrame.Time = (double)j * spf;
					currentFrame.TransformMatrix = r.ReadMatrix4();
					currentObject.Frames[j] = currentFrame;
				}
			}
		}
	}
}
