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
using System.Collections.Generic;
using System.Dynamic;

namespace IGE.IO.FileFormats.Blender {
	public class BlenderFileObject : DynamicObject {
		protected ulong m_Address;
		protected int m_Size;
		protected string m_TypeName;
		protected Dictionary<string, BlenderFileObjectField> m_Fields = new Dictionary<string, BlenderFileObjectField>();

		public BlenderFileObject(BinaryReader r, ulong address, BlenderPointerSize ptrSize, Endian endianness, BlenderSDNAFileBlock sdna, int sdnaIndex)
			: this(r, address, ptrSize, endianness, sdna, sdna.Structures[sdnaIndex].Name)
		{
		}
		
		public BlenderFileObject(BinaryReader r, ulong address, BlenderPointerSize ptrSize, Endian endianness, BlenderSDNAFileBlock sdna, string typeName) {
			ulong fieldAddress = address, valAddress;
			BlenderFileStructure structInfo = sdna.StructureIndex[typeName];
			string fieldName;
			LoadType loadType;
			int dimStartIndex, stringLength, readSize, i;
			string[] dimensionSizesText;
			int[] dimensionSizes;
			dynamic val, ptr;
			Type arrayElementType, arrayType;
			
			m_Address = address;
			m_Size = structInfo.Size;
			m_TypeName = structInfo.Name;
			foreach( BlenderFileField fieldInfo in structInfo.Fields ) {
				fieldName = fieldInfo.Name;
				dimensionSizes = null;
				stringLength = 0;

				if( fieldName.EndsWith("]", StringComparison.InvariantCulture) ) {
					dimStartIndex = fieldName.IndexOf('[');
					if( dimStartIndex <= 0 )
						throw new Exception(String.Format("Invalid field name format: {0}", fieldName));
					dimensionSizesText = fieldName.Substring(dimStartIndex + 1, fieldName.Length - dimStartIndex - 2).Split(new String[] { "][" }, StringSplitOptions.None);
					dimensionSizes = new int[dimensionSizesText.Length];
					for( i = dimensionSizesText.Length - 1; i >= 0; i-- )
						dimensionSizes[i] = int.Parse(dimensionSizesText[i]);
					fieldName = fieldName.Substring(0, dimStartIndex);
				}

				switch(fieldInfo.Type) {
					case "char":
						if( dimensionSizes.Length > 0 ) {
							stringLength = dimensionSizes[dimensionSizes.Length - 1];
							int[] newDimSizes = (dimensionSizes.Length > 1) ? new int[dimensionSizes.Length - 1] : null;
							if( newDimSizes != null ) {
								for( i = dimensionSizes.Length - 2; i >= 0; i-- )
									newDimSizes[i] = dimensionSizes[i];
							}
							dimensionSizes = newDimSizes;
							loadType = LoadType.String;
						}
						else
							loadType = LoadType.Char;
						break;
					case "uchar": loadType = LoadType.Byte; break;
					case "short": loadType = LoadType.Int16; break;
					case "ushort": loadType = LoadType.UInt16; break;
					case "int": loadType = LoadType.Int32; break;
					case "uint": loadType = LoadType.UInt32; break;
					case "long": loadType = LoadType.Int64; break;
					case "ulong": loadType = LoadType.UInt64; break;
					case "float": loadType = LoadType.Single; break;
					case "double": loadType = LoadType.Double; break;
					default:
						if( fieldName.StartsWith("(*", StringComparison.InvariantCulture) && fieldName.EndsWith(")()", StringComparison.InvariantCulture) ) {
							fieldName = fieldName.Substring(2, fieldInfo.Name.Length - 5);
							loadType = LoadType.Method;
						}
						else if( fieldName.StartsWith("*", StringComparison.InvariantCulture) ) {
							fieldName = fieldName.Substring(1);
							loadType = LoadType.Pointer;
						}
						else
							loadType = LoadType.Object;
						break;
				}
				
				valAddress = fieldAddress;
				
				if( dimensionSizes == null ) {
					val = ReadFieldValue(r, ptrSize, endianness, sdna, loadType, fieldInfo.Type, fieldName, valAddress, stringLength, out readSize);
					if( loadType == LoadType.Method || loadType == LoadType.Pointer )
						AddField(fieldName, fieldInfo.Type, valAddress, readSize, null, val);
					else
						AddField(fieldName, fieldInfo.Type, valAddress, readSize, val, null);
					valAddress += (ulong)readSize;
				}
				else {
					switch(loadType) {
						case LoadType.Char: arrayElementType = typeof(char); break;
						case LoadType.Byte: arrayElementType = typeof(byte); break;
						case LoadType.Int16: arrayElementType = typeof(short); break;
						case LoadType.UInt16: arrayElementType = typeof(ushort); break;
						case LoadType.Int32: arrayElementType = typeof(int); break;
						case LoadType.UInt32: arrayElementType = typeof(uint); break;
						case LoadType.Int64: arrayElementType = typeof(long); break;
						case LoadType.UInt64: arrayElementType = typeof(ulong); break;
						case LoadType.Single: arrayElementType = typeof(float); break;
						case LoadType.Double: arrayElementType = typeof(double); break;
						case LoadType.String: arrayElementType = typeof(string); break;
						case LoadType.Method: arrayElementType = null; break;
						case LoadType.Pointer: goto case LoadType.Object;
						case LoadType.Object: arrayElementType = typeof(BlenderFileObject); break;
						default: throw new Exception("Unexpected value type");
					}
					if( arrayElementType != null ) {
						arrayType =  arrayElementType.MakeArrayType(dimensionSizes.Length);
						val = Activator.CreateInstance(arrayType, dimensionSizes);
					}
					else {
						arrayType = null;
						val = null;
					}
					
					// TODO: load data into arrays
					
				}
				
				fieldAddress = valAddress;
				/*
				if( fieldName.StartsWith("(*", StringComparison.InvariantCulture) && fieldName.EndsWith(")()", StringComparison.InvariantCulture) ) {
					AddField(
						BlenderFieldObjectFieldType.Method,
						fieldInfo.Type,
						fieldName.Substring(2, fieldInfo.Name.Length - 5),
						(ptrSize == BlenderPointerSize.Ptr64) ? r.ReadUInt64(endianness) : r.ReadUInt32(endianness),
						fieldAddress,
						fieldInfo.Size
					);
					fieldAddress += (ptrSize == BlenderPointerSize.Ptr64) ? 8UL : 4UL;
				}
				else if( fieldName.StartsWith("*", StringComparison.InvariantCulture) ) {
					AddField(
						BlenderFieldObjectFieldType.Pointer,
						fieldInfo.Type,
						fieldName.Substring(1),
						(ptrSize == BlenderPointerSize.Ptr64) ? r.ReadUInt64(endianness) : r.ReadUInt32(endianness),
						fieldAddress,
						fieldInfo.Size
					);
					fieldAddress += (ptrSize == BlenderPointerSize.Ptr64) ? 8UL : 4UL;
				}
				*/
			}
		}
		
		protected dynamic ReadFieldValue(BinaryReader r, BlenderPointerSize ptrSize, Endian endianness, BlenderSDNAFileBlock sdna, LoadType loadType, string typeName, string fieldName, ulong address, int stringLength, out int readSize) {
			switch(loadType) {
				case LoadType.Char: readSize = 1; return Encoding.ASCII.GetChars(r.ReadBytes(1))[0];
				case LoadType.Byte: readSize = 1; return r.ReadByte();
				case LoadType.Int16: readSize = 2; return r.ReadInt16(endianness);
				case LoadType.UInt16: readSize = 2; return r.ReadUInt16(endianness);
				case LoadType.Int32: readSize = 4; return r.ReadInt32(endianness);
				case LoadType.UInt32: readSize = 4; return r.ReadUInt32(endianness);
				case LoadType.Int64: readSize = 8; return r.ReadInt64(endianness);
				case LoadType.UInt64: readSize = 8; return r.ReadUInt64(endianness);
				case LoadType.Single: readSize = 4; return r.ReadSingle(); // TODO: make extension to load float with specified endianness
				case LoadType.Double: readSize = 8; return r.ReadDouble(); // TODO: make extension to load double with specified endianness
				case LoadType.String: readSize = stringLength; return r.ReadFixedSizeString(stringLength, Encoding.ASCII);
				case LoadType.Method: goto case LoadType.Pointer;
				case LoadType.Pointer:
					readSize = (ptrSize == BlenderPointerSize.Ptr64) ? 8 : 4;
					return (ptrSize == BlenderPointerSize.Ptr64) ? r.ReadUInt64() : r.ReadUInt32();
				case LoadType.Object:
					readSize = sdna.StructureIndex[typeName].Size;
					return new BlenderFileObject(r, address, ptrSize, endianness, sdna, typeName);
			}
			throw new Exception("Unexpected value type");
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			BlenderFileObjectField field;
			result = null;
			if( !m_Fields.TryGetValue(binder.Name, out field) )
				return false;
			result = field.Value;
			return true;
		}
		
		public ulong GetAddress() {
			return m_Address;
		}
		
		public int GetSize() {
			return m_Size;
		}
		
		public string GetTypeName() {
			return m_TypeName;
		}
		
		public bool TryFindValue(ulong address, out object val) {
			val = null;
			if( m_Address <= address && address < m_Address + (ulong)m_Size ) {
				foreach( BlenderFileObjectField field in m_Fields.Values ) {
					if( field.Value is BlenderFileObject ) {
						if( ((BlenderFileObject)field.Value).TryFindValue(address, out val) )
							return true;
					}
					else {
						if( field.Address == address ) {
							val = field.Value;
							return true;
						}
						if( field.Address < address && address < field.Address + (ulong)field.Size )
							throw new Exception("Unexpected pointer pointing to an address inside a field");
					}
				}
			}
			return false;
		}

		public bool IsPointerType(string fieldName) {
			BlenderFileObjectField field;
			if( !m_Fields.TryGetValue(fieldName, out field) )
				throw new Exception(String.Format("Dynamic field '{0}' not found", fieldName));
			return field.Pointer != null;
		}
		
		public string GetFieldType(string fieldName) {
			BlenderFileObjectField field;
			if( !m_Fields.TryGetValue(fieldName, out field) )
				return null;
			return field.TypeName;
		}
		
		public void AddField(string name, BlenderFileObjectField field) {
			m_Fields.Add(name, field);
		}
		
		public void AddField(string name, string typeName, ulong address, int size, dynamic val, dynamic pointer) {
			AddField(name, new BlenderFileObjectField {
				TypeName = typeName,
				Address = address,
				Size = size,
				Value = val,
				Pointer = pointer
			});
		}

		public enum LoadType {
			Char,
			Byte,
			Int16,
			UInt16,
			Int32,
			UInt32,
			Int64,
			UInt64,
			Single,
			Double,
			String,
			Pointer,
			Object,
			Method
		};
	}
}
