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
	[FileFormat("mmd")]
	public class MazanovModelFile : ModelFile {
		public MazanovModelFile(Stream file) : base(file) {
			BinaryReader r = new BinaryReader(file);
			
			int version = 0;
			int materialCount;
			int objectCount;
			int rootObjectCount;
			int childCount;
			int vertexCount;
			int normalCount;
			int faceCount;
			int colorMapCount;
			int weightMapCount;
			int weightMapVertexCount;
			
			int[][] vertexToNormalLinks = null;
			string textureFileName;
			Material currentMaterial;
			ModelFileObject currentObject;
			ModelFileWeightMap currentWeightMap;
			int i, j, k;
			
			materialCount = r.ReadInt32();
			if( materialCount == 4476237 ) { // newer version has a header "MMD\0"
				version = r.ReadInt32();
				materialCount = r.ReadInt32();
			}
			
			if( materialCount > 0 ) {
				Materials = new Material[materialCount];
				for( i = 0; i < materialCount; i++ ) {
					Materials[i] = currentMaterial = new Material();
					textureFileName = r.ReadFixedSizeString(256, Encoding.ASCII);
					if( textureFileName != "" )
						currentMaterial.Textures = new Texture[] { Texture.Cache(textureFileName.RelativeTo("models/").ToPath()) };
					
					currentMaterial.Properties = new Material.MaterialProperties[] {
						new Material.MaterialProperties(FaceName.Front, r.ReadColor4(), r.ReadColor4(), r.ReadColor4(), r.ReadColor4(), r.ReadSingle(), 1f)
					};
					currentMaterial.Properties[0].Opacity = currentMaterial.Properties[0].DiffuseColor.A;
				}
			}
			
			objectCount = r.ReadInt32();
			rootObjectCount = r.ReadInt32();
			
			if( rootObjectCount > 0 ) {
				RootObjectIndexes = new int[rootObjectCount];
				for( i = 0; i < rootObjectCount; i++ )
					RootObjectIndexes[i] = r.ReadInt32();
			}
			
			if( objectCount > 0 ) {
				Objects = new ModelFileObject[objectCount];
				vertexToNormalLinks = new int[objectCount][];
				for( i = 0; i < objectCount; i++ ) {
					Objects[i] = currentObject = new ModelFileObject();
					
					currentObject.Name = r.ReadFixedSizeString(32, Encoding.ASCII);
					
					childCount = r.ReadInt32();
					if( childCount > 0 ) {
						currentObject.Children = new int[childCount];
						for( j = 0; j < childCount; j++ )
							currentObject.Children[j] = r.ReadInt32();
					}
					else {
						currentObject.Children = null;
					}
					
					currentObject.Transform = r.ReadMatrix4();
					currentObject.Transform.M41 *= 0.01f;
					currentObject.Transform.M42 *= 0.01f;
					currentObject.Transform.M43 *= 0.01f;
					
					vertexCount = r.ReadInt32();
					normalCount = r.ReadInt32();
					faceCount = r.ReadInt32();
					if( version > 0 ) {
						colorMapCount = (r.ReadInt32() != 0) ? 1 : 0;
						weightMapCount = r.ReadInt32();
					}
					else {
						colorMapCount = 0;
						weightMapCount = 0;
					}
					
					if( vertexCount > 0 ) {
						currentObject.Vertices = new Vector3[vertexCount];
						currentObject.TexCoords = new Vector2[vertexCount];
						for( j = 0; j < vertexCount; j++ ) {
							currentObject.Vertices[j] = r.ReadVector3();
							Vector3.Multiply(ref currentObject.Vertices[j], 0.01f, ref currentObject.Vertices[j]);
						}
						for( j = 0; j < vertexCount; j++ )
							currentObject.TexCoords[j] = r.ReadVector2();
					}
					
					if( normalCount > 0 ) {
						currentObject.Normals = new Vector3[vertexCount];
						for( j = 0; j < normalCount; j++ )
							currentObject.Normals[j] = r.ReadVector3();
					}
					
					if( faceCount > 0 ) {
						currentObject.Faces = new ModelFileFace[faceCount];
						for( j = 0; j < faceCount; j++ ) {
							currentObject.Faces[j] = new ModelFileFace(
								r.ReadInt32(), r.ReadInt32(), r.ReadInt32(),
								r.ReadInt32(), r.ReadInt32(), r.ReadInt32(),
								r.ReadInt32()
							);
						}
					}
					
					if( colorMapCount > 0 ) {
						currentObject.Colors = new Color4[vertexCount];
						for( j = 0; j < vertexCount; j++ )
							currentObject.Colors[j] = r.ReadColor4();
					}
					
					if( weightMapCount > 0 ) {
						currentObject.WeightMaps = new ModelFileWeightMap[weightMapCount];
						for( j = 0; j < weightMapCount; j++ ) {
							currentWeightMap = new ModelFileWeightMap();
							currentWeightMap.LinkedObjectIndex = r.ReadInt32();
							weightMapVertexCount = r.ReadInt32();
							if( weightMapVertexCount <= 0 ) // no need to do anything since weight map is empty
								continue;
							currentWeightMap.VertexIndexes = new int[weightMapVertexCount];
							currentWeightMap.VertexWeights = new float[weightMapVertexCount];
							for( k = 0; k < weightMapVertexCount; k++ )
								currentWeightMap.VertexIndexes[k] = r.ReadInt32();
							for( k = 0; k < weightMapVertexCount; k++ )
								currentWeightMap.VertexWeights[k] = r.ReadSingle();
							currentObject.WeightMaps[j] = currentWeightMap;
						}
					}
				}
			}
		}
	}
}
