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
using IGE.Graphics;
using IGE.Graphics.OpenGL;

namespace IGE.IO {
	public abstract class ModelFile : GameFile {
		public Material[] Materials;
		public ModelFileObject[] Objects;
		public int[] RootObjectIndexes;

		public ModelFile(Stream file) : base(file) {
		}
		
		[FileFormat()]
		public static Type DefaultContentDetect(string filename, Stream file) {
			BinaryReader br = new BinaryReader(file);
			if( file.Length > 4 ) {
				string head = Encoding.ASCII.GetString(br.ReadBytes(4));
				file.Seek(0, SeekOrigin.Begin);
				if( head == "MMD\x0" ) return typeof(MazanovModelFile);
			}
			
			return null;
		}
	}
	
	public class ModelFileObject {
		public string Name = "";
		public Matrix4 Transform = Matrix4.Identity;
		public int[] Children = null;
		public Vector3[] Vertices = null;
		public Color4[] Colors = null;
		public Vector2[] TexCoords = null;
		public Vector3[] Normals = null;
		public ModelFileFace[] Faces = null;
		public ModelFileWeightMap[] WeightMaps = null;
	}
	
	public struct ModelFileFace {
		public int[] VertexIndexes;
		public int[] NormalIndexes;
		public int MaterialIndex;
		
		public ModelFileFace(int v1, int v2, int v3, int n1, int n2, int n3, int material) {
			VertexIndexes = new int[3] { v1, v2, v3 };
			NormalIndexes = new int[3] { n1, n2, n3 };
			MaterialIndex = material;
		}
	}
	
	public struct ModelFileWeightMap {
		/// <summary>
		/// Index of an object in the model that affects movement of listed vertices of the current model's object.
		/// </summary>
		public int LinkedObjectIndex;
		public Matrix4 Transform;
		public int[] VertexIndexes;
		public float[] VertexWeights;
	}
}
