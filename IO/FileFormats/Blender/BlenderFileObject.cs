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
			ulong fieldAddress = address;
			BlenderFileStructure structInfo = sdna.StructureIndex[typeName];
			string fieldName;
			int dimStartIndex, i;
			string[] dimensionSizesText;
			int[] dimensionSizes;
			object val;
			
			m_Address = address;
			m_Size = structInfo.Size;
			m_TypeName = structInfo.Name;
			foreach( BlenderFileField fieldInfo in structInfo.Fields ) {
				fieldName = fieldInfo.Name;
				dimensionSizes = null;
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
			}
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
							if( field.BaseType == BlenderFieldObjectFieldType.Method )
								throw new Exception("Unexpected pointer pointing to another pointer, which in turn points to a method");
							if( field.BaseType == BlenderFieldObjectFieldType.Pointer )
								throw new Exception("Unexpected pointer pointing to another pointer");
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

		public BlenderFieldObjectFieldType GetFieldBaseType(string fieldName) {
			BlenderFileObjectField field;
			if( !m_Fields.TryGetValue(fieldName, out field) )
				return BlenderFieldObjectFieldType.None;
			return field.BaseType;
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
		
		public void AddField(BlenderFieldObjectFieldType baseType, string typeName, string name, object val, ulong address, int size) {
			AddField(name, new BlenderFileObjectField {
				BaseType = baseType,
				Address = address,
				Size = size,
				TypeName = typeName,
				Value = val
			});
		}
	}
}
