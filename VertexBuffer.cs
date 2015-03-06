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
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public class VertexBuffer : IDisposable {
		protected bool m_Compiled = false;
		protected int m_VboId = 0;
		protected int m_IboId = 0;
		protected List<BufferArray> m_Arrays = new List<BufferArray>();
		protected BufferArray m_IboArray = null;
		
		public int VboId { get { return m_VboId; } }
		public int IboId { get { return m_IboId; } }
		// public List<BufferArray> Arrays { get { return m_Arrays; } }
		public bool Disposed { get { return m_VboId == 0; } }
		
		public VertexBuffer() {
			m_VboId = GL.Supports.VBO ? GL.GenBuffer() : -1;
		}
		
		~VertexBuffer() {
			Dispose(false);
		}
		
		public void Dispose() {
			Dispose(true);
		}
		
		protected void Dispose(bool manual) {
			if( manual ) {
				if( m_VboId != 0 ) {
					Unbind();
					if( m_VboId != -1 )
						GL.DeleteBuffer(ref m_VboId);
					if( m_IboId != -1 )
						GL.DeleteBuffer(ref m_IboId);
					m_VboId = 0;
					m_IboId = 0;
					m_Arrays = null;
					m_IboArray = null;
				}
			}
		}

		/// <summary>
		/// Use this method to add a client state array
		/// </summary>
		/// <param name="arrayType"></param>
		/// <param name="unitElementCount"></param>
		/// <param name="data"></param>
		public void AddArray(ClientStateCap arrayType, int unitElementCount, object data) {
			if( !m_Compiled && m_VboId != 0 )
				m_Arrays.Add(new BufferArray(arrayType, unitElementCount, 0, 0, data));
		}

		/// <summary>
		/// Use this method to add a vertex attribute array
		/// </summary>
		/// <param name="attributeIdx"></param>
		/// <param name="unitElementCount"></param>
		/// <param name="normalized"></param>
		/// <param name="data"></param>
		public void AddArray(int attributeIdx, int unitElementCount, bool normalized, object data) {
			if( !m_Compiled && m_VboId != 0 )
				m_Arrays.Add(new BufferArray(attributeIdx, unitElementCount, normalized, 0, 0, data));
		}

		/// <summary>
		/// Use this method to add an array of vertex indices (IBO)
		/// </summary>
		/// <param name="data">Array of vertex indices. Must be of type either byte[], short[] or int[]</param>
		public void AddArray(object data) {
			if( !m_Compiled && m_VboId != 0 )
				m_IboArray = new BufferArray(data);
		}
		
		public void Compile() {
			if( !m_Compiled && m_VboId != 0 ) {
				if( m_VboId != -1 ) {
					if( m_IboArray != null ) {
						m_IboId = GL.GenBuffer();
						if( m_IboId != 0 ) {
							GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_IboId);
							GL.BufferData(BufferTarget.ElementArrayBuffer, m_IboArray.Size, ref m_IboArray.DataArray, BufferUsage.StaticDraw);
							m_IboArray.DataArray = null;
						}
						else {
							// if failed to create IBO we don't store the VBO data, since we'll have to do rendering using old technique
							m_Compiled = true;
							return;
						}
					}
					
					int internalOffset = 0;
					foreach(BufferArray arr in m_Arrays) {
						arr.InternalOffset = arr.Offset + internalOffset;
						internalOffset += arr.Size;
					}
					
					GL.BindBuffer(BufferTarget.ArrayBuffer, m_VboId);

					GL.BufferData(BufferTarget.ArrayBuffer, internalOffset, BufferUsage.StaticDraw);
					foreach(BufferArray arr in m_Arrays) {
						GL.BufferSubData(BufferTarget.ArrayBuffer, arr.InternalOffset, arr.Size, ref arr.DataArray);
						arr.DataArray = null; // the data is no longer needed so it may be freed from the memory, but it must be kept in case of IBO buffer creation failure: we'll fall back to slow rendering
					}
					
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				}
				
				m_Compiled = true;
			}
		}
		
		public void Bind() {
			if( m_VboId != 0 ) {
				if( !m_Compiled )
					Compile();
				if( m_VboId != -1 ) {
					GL.BindBuffer(BufferTarget.ArrayBuffer, m_VboId);
					foreach (BufferArray arr in m_Arrays) {
						if( arr.AttributeIdx == -1 ) {
							GL.EnableClientState(arr.ClientArrayType);
							switch(arr.ClientArrayType) {
								case ClientStateCap.ColorArray: GL.ColorPointer(arr.UnitElementCount, arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.EdgeFlagArray: GL.EdgeFlagPointer(arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.FogCoordArray: GL.FogCoordPointer(arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.IndexArray: GL.IndexPointer(arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.NormalArray: GL.NormalPointer(arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.SecondaryColorArray: GL.SecondaryColorPointer(arr.UnitElementCount, arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.TextureCoordArray: GL.TexCoordPointer(arr.UnitElementCount, arr.DataType, arr.Stride, arr.InternalOffset); break;
								case ClientStateCap.VertexArray: GL.VertexPointer(arr.UnitElementCount, arr.DataType, arr.Stride, arr.InternalOffset); break;
							}
						}
						else {
							GL.EnableVertexAttribArray(arr.AttributeIdx);
							GL.VertexAttribPointer(arr.AttributeIdx, arr.UnitElementCount, arr.DataType, arr.Normalized ? (byte)1 : (byte)0, arr.Stride, arr.InternalOffset);
						}						
					}
				}
				
				if( m_IboId != 0 ) {
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_IboId);
				}
			}
		}
		
		public void Unbind() {
			if( m_VboId != 0 ) {
				if( m_VboId != -1 ) {
					foreach (BufferArray arr in m_Arrays) {
						if( arr.AttributeIdx == -1 )
							GL.DisableClientState(arr.ClientArrayType);
						else
							GL.DisableVertexAttribArray(arr.AttributeIdx);
					}
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				}
				if( m_IboId != 0 )
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			}
		}
		
		public void Draw(BeginMode mode, int offset, int vertexCount) {
			if( m_VboId != 0 ) {
				if( !m_Compiled )
					Bind();
				if( m_IboArray != null ) {
					if( m_IboId != 0 )
						GL.DrawElements(mode, vertexCount, m_IboArray.DataType, offset);
					// TODO: render primitives using old opengl technique without using IBO/VBO
				}
				else
					GL.DrawArrays(mode, offset, vertexCount);
			}
		}
		
		public class BufferArray {
			public ArrayType ArrayType;
			
			/// <summary>
			/// Must be -1 if this is a client state array, or other number (attribute index) if it is a vertex attribute array
			/// </summary>
			public int AttributeIdx = -1;
			
			/// <summary>
			/// Type of the client state array if it is not a vertex attribute array
			/// </summary>
			public ClientStateCap ClientArrayType = ClientStateCap.VertexArray;

			/// <summary>
			/// Used only in vertex attrib arrays
			/// </summary>
			public bool Normalized = false;
			
			/// <summary>
			/// Offset in bytes, where the first unit of an array starts
			/// </summary>
			public int Offset = 0;
			
			/// <summary>
			/// Size of a single unit
			/// </summary>
			public int Stride = 0;
			
			/// <summary>
			/// Amount of array elements that make up information for a single unit (vertex)
			/// </summary>
			public int UnitElementCount = 4;
			
			/// <summary>
			/// Amount of elements in an array (###[].Length)
			/// </summary>
			public int ElementCount = 0;

			/// <summary>
			/// Offset of an array in the buffer
			/// </summary>
			internal int InternalOffset = 0;
			
			/// <summary>
			/// Size of an array in bytes
			/// </summary>
			internal int Size = 0;
			
			/// <summary>
			/// Type, which each array element represents
			/// </summary>
			public PointerToType DataType = PointerToType.Float;
			
			public object DataArray = null;
			
			public BufferArray(int attributeIdx, int unitElementCount, bool normalized, int stride, int offset, object data) {
				ArrayType = ArrayType.AttribArray;
				
				AttributeIdx = attributeIdx;
				UnitElementCount = unitElementCount;
				Normalized = normalized;
				Stride = stride;
				Offset = offset;
				DataArray = data;
				
				string typeName = data.GetType().Name;
				
				switch(typeName) {
					case "Single[]":
						DataType = PointerToType.Float;
						ElementCount = ((float[])data).Length;
						Size = ElementCount * sizeof(float);
						break;

					case "UInt32[]":
						DataType = PointerToType.UnsignedInt;
						ElementCount = ((uint[])data).Length;
						Size = ElementCount * sizeof(uint);
						break;
						
					case "Int32[]":
						DataType = PointerToType.Int;
						ElementCount = ((int[])data).Length;
						Size = ElementCount * sizeof(int);
						break;
						
					case "UInt16[]":
						DataType = PointerToType.UnsignedShort;
						ElementCount = ((ushort[])data).Length;
						Size = ElementCount * sizeof(ushort);
						break;
						
					case "Int16[]":
						DataType = PointerToType.Short;
						ElementCount = ((short[])data).Length;
						Size = ElementCount * sizeof(short);
						break;

					case "Byte[]":
						DataType = PointerToType.UnsignedByte;
						ElementCount = ((byte[])data).Length;
						Size = ElementCount * sizeof(byte);
						break;
				
					case "SByte[]":
						DataType = PointerToType.Byte;
						ElementCount = ((sbyte[])data).Length;
						Size = ElementCount * sizeof(sbyte);
						break;
				
					case "Double[]":
						DataType = PointerToType.Double;
						ElementCount = ((double[])data).Length;
						Size = ElementCount * sizeof(double);
						break;
				
					/* case "HalfFloat[]":
						DataType = PointerToType.HalfFloat;
						ElementCount = ((halffloat[])data).Length;
						Size = ElementCount * sizeof(halffloat);
						break;
					*/
				}
				// TODO: think of a way to safely set data type to Int2101010Rev
			}
			
			public BufferArray(ClientStateCap arrayType, int unitElementCount, int stride, int offset, object data)
				: this(-1, unitElementCount, false, stride, offset, data)
			{
				ClientArrayType = arrayType;
				ArrayType = ArrayType.ClientStateArray;
			}
			
			public BufferArray(object data)
				: this(-1, 0, false, 0, 0, data)
			{
				string typeName = data.GetType().Name;
				if( !(typeName.Equals("UInt32[]") || typeName.Equals("UInt16[]") || typeName.Equals("Byte[]")) )
					throw new UserFriendlyException("Index buffer array may be only of byte[], ushort[] or uint[] type", "Invalid data type usage in IBO array");
				ArrayType = ArrayType.IboArray;
			}
		}
		
		public enum ArrayType : byte {
			ClientStateArray,
			AttribArray,
			IboArray,
		}
	}
}
