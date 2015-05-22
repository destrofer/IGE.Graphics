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
		protected BlenderFileObjectField m_FirstField = null;

		public bool IsMyAddress(ulong address) {
			return m_Address == address;
		}

		public bool IsAddressInRange(ulong address) {
			return m_Address <= address && address < m_Address + (ulong)m_Size;
		}
		
		public BlenderFileObject(BinaryReader r, BlenderFile file, ulong address, int sdnaIndex)
			: this(r, file, address, file.SDNA.Structures[sdnaIndex].Name)
		{
		}
		
		public BlenderFileObject(BinaryReader r, BlenderFile file, ulong address, string typeName) {
			ulong fieldAddress = address, valAddress;
			BlenderFileStructure structInfo = file.SDNA.StructureIndex[typeName];
			string fieldName;
			BlenderInternalType loadType;
			int dimStartIndex, stringLength, readSize, totalReadSize, i;
			string[] dimensionSizesText;
			int[] dimensionSizes;
			object[] dimensionSizesForReflection;
			dynamic val, v;
			int pointerDimensions;
			
			m_Address = address;
			m_Size = structInfo.Size;
			m_TypeName = structInfo.Name;
			foreach( BlenderFileField fieldInfo in structInfo.Fields ) {
				fieldName = fieldInfo.Name;
				dimensionSizes = null;
				stringLength = 0;
				pointerDimensions = 0;

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

				while( pointerDimensions < fieldName.Length && fieldName[pointerDimensions] == '*' )
					pointerDimensions++;
				if( pointerDimensions > 0 )
					fieldName = fieldName.Substring(pointerDimensions);

				switch(fieldInfo.Type) {
					case "char":
						if( dimensionSizes != null ) {
							stringLength = dimensionSizes[dimensionSizes.Length - 1];
							int[] newDimSizes = (dimensionSizes.Length > 1) ? new int[dimensionSizes.Length - 1] : null;
							if( newDimSizes != null ) {
								for( i = dimensionSizes.Length - 2; i >= 0; i-- )
									newDimSizes[i] = dimensionSizes[i];
							}
							dimensionSizes = newDimSizes;
							loadType = BlenderInternalType.String;
						}
						else
							loadType = BlenderInternalType.Char;
						break;
					case "uchar": loadType = BlenderInternalType.Byte; break;
					case "short": loadType = BlenderInternalType.Int16; break;
					case "ushort": loadType = BlenderInternalType.UInt16; break;
					case "int": loadType = BlenderInternalType.Int32; break;
					case "uint": loadType = BlenderInternalType.UInt32; break;
					case "int64_t": goto case "long";
					case "long": loadType = BlenderInternalType.Int64; break;
					case "uint64_t": goto case "ulong";
					case "ulong": loadType = BlenderInternalType.UInt64; break;
					case "float": loadType = BlenderInternalType.Single; break;
					case "double": loadType = BlenderInternalType.Double; break;
					default:
						if( fieldName.StartsWith("(*", StringComparison.InvariantCulture) && fieldName.EndsWith(")()", StringComparison.InvariantCulture) ) {
							fieldName = fieldName.Substring(2, fieldInfo.Name.Length - 5);
							loadType = BlenderInternalType.Method;
						}
						else if( pointerDimensions > 0 || !fieldInfo.Type.Equals("void", StringComparison.Ordinal) )
							loadType = BlenderInternalType.Object;
						else
							throw new Exception("Unexpected non-pointer void");
						break;
				}
				
				if( dimensionSizes == null ) {
					val = ReadFieldValue(r, file, loadType, fieldInfo.Type, fieldAddress, stringLength, pointerDimensions, out readSize);
					
					if( loadType == BlenderInternalType.Method )
						AddField(fieldName, loadType, fieldInfo.Type, fieldAddress, readSize, pointerDimensions > 0, null);
					else
						AddField(fieldName, loadType, fieldInfo.Type, fieldAddress, readSize, pointerDimensions > 0, val);
					
					fieldAddress += (ulong)readSize;
				}
				else {
					val = null;
					
					dimensionSizesForReflection = new object[dimensionSizes.Length];
					for( i = dimensionSizes.Length - 1; i >= 0; i-- )
						dimensionSizesForReflection[i] = dimensionSizes[i];
					
					val = Activator.CreateInstance(typeof(object).MakeArrayType(dimensionSizes.Length), dimensionSizesForReflection);
					
					int[] idx = new int[dimensionSizes.Length];

					valAddress = fieldAddress;
					totalReadSize = 0;
					
					do {
						v = ReadFieldValue(r, file, loadType, fieldInfo.Type, valAddress, stringLength, pointerDimensions, out readSize);
						
						totalReadSize += readSize;
						valAddress += (ulong)readSize;
						
						val.SetValue(v, idx);
						
						// go to next possible index in array of current indexes
						for( i = idx.Length - 1; i >= 0 && ++idx[i] >= dimensionSizes[i]; i-- )
							idx[i] = 0;
					} while(i >= 0);
					
					AddField(fieldName, loadType, fieldInfo.Type, fieldAddress, totalReadSize, pointerDimensions > 0, val);
					
					fieldAddress = valAddress;
				}
			}
		}
		
		protected dynamic ReadFieldValue(BinaryReader r, BlenderFile file, BlenderInternalType loadType, string typeName, ulong address, int stringLength, int pointerDimensions, out int readSize) {
			if( pointerDimensions > 0 ) {
				readSize = (file.Header.PointerSize == BlenderPointerSize.Ptr64) ? 8 : 4;
				return new BlenderPointer((file.Header.PointerSize == BlenderPointerSize.Ptr64) ? r.ReadUInt64() : (ulong)r.ReadUInt32(), pointerDimensions);
			}
			switch(loadType) {
				// case InternalType.Char: readSize = 1; return Encoding.ASCII.GetChars(r.ReadBytes(1))[0];
				case BlenderInternalType.Char: readSize = 1; return r.ReadSByte();
				case BlenderInternalType.Byte: readSize = 1; return r.ReadByte();
				case BlenderInternalType.Int16: readSize = 2; return r.ReadInt16(file.Header.Endianness);
				case BlenderInternalType.UInt16: readSize = 2; return r.ReadUInt16(file.Header.Endianness);
				case BlenderInternalType.Int32: readSize = 4; return r.ReadInt32(file.Header.Endianness);
				case BlenderInternalType.UInt32: readSize = 4; return r.ReadUInt32(file.Header.Endianness);
				case BlenderInternalType.Int64: readSize = 8; return r.ReadInt64(file.Header.Endianness);
				case BlenderInternalType.UInt64: readSize = 8; return r.ReadUInt64(file.Header.Endianness);
				case BlenderInternalType.Single: readSize = 4; return r.ReadSingle(file.Header.Endianness);
				case BlenderInternalType.Double: readSize = 8; return r.ReadDouble(file.Header.Endianness);
				case BlenderInternalType.String: readSize = stringLength; return r.ReadFixedSizeString(stringLength, Encoding.ASCII);
				case BlenderInternalType.Method:
					readSize = (file.Header.PointerSize == BlenderPointerSize.Ptr64) ? 8 : 4;
					return (file.Header.PointerSize == BlenderPointerSize.Ptr64) ? r.ReadUInt64() : (ulong)r.ReadUInt32();
				case BlenderInternalType.Object:
					readSize = file.SDNA.StructureIndex[typeName].Size;
					return new BlenderFileObject(r, file, address, typeName);
			}
			throw new Exception("Unexpected value type");
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			BlenderFileObjectField field;
			result = null;
			if( !m_Fields.TryGetValue(binder.Name, out field) )
				return false;
			result = field.Value;
			if( result is BlenderPointer )
				throw new UserFriendlyException("Tried to read an unresolved pointer field", "Failed to load game data");
			Type type = result.GetType();
			if( type.IsArray && type.GetElementType().Equals(typeof(BlenderPointer)) )
				throw new UserFriendlyException("Tried to read an unresolved pointer array field", "Failed to load game data");
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

		public delegate bool WalkFieldsFunc(string fieldName, BlenderFileObjectField field);
		
		/// <summary>
		/// Calls a callback delegate on every field in a structure.
		/// 
		/// Please not that the order of callback calls is not necessary the same as order of fields in the SDNA.
		/// </summary>
		/// <param name="recursive">When <b>true</b> the method will also walk through fields inside snternal substructures and substructure arrays. Otherwise it will call the callback delegate only once for every substructure and array.</param>
		/// <param name="callback"></param>
		/// <returns><b>true</b> if array walking was finished wihout interruption. <b>false</b> if walking was interrupted by callback (one of calls returned <b>false</b>)</returns>
		public bool WalkFields(bool recursive, WalkFieldsFunc callback) {
			if( callback == null )
				return true; // no callback - no walking
			foreach(KeyValuePair<string, BlenderFileObjectField> pair in m_Fields) {
				if( recursive && !pair.Value.IsPointer && pair.Value.Type == BlenderInternalType.Object ) {
					if( pair.Value.Value != null ) {
						if( pair.Value.Value.GetType().IsArray ) {
							foreach( BlenderFileObject sub in pair.Value.Value )
								if( !sub.WalkFields(recursive, callback) )
									return false;
						}
						else if( !pair.Value.Value.WalkFields(recursive, callback) )
							return false;
					}
				}
				else if( !callback(pair.Key, pair.Value) )
					return false;
			}
			return true;
		}
		
		public BlenderFileObjectField GetFirstField() {
			return m_FirstField;
		}
		
		public void ResolvePointers(BlenderFile file) {
			WalkFields(true, (name, field) => {
				if( !field.IsPointer )
					return true;
				
				// ResolvePointerField(field, file);
				return true;
			});
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <param name="file"></param>
		public void ResolvePointerField(BlenderFileObjectField field, BlenderFile file) {
			// TODO: add support for array of pointers
			BlenderPointer ptr = (BlenderPointer)field.Value;
			if( ptr.State == BlenderPointerState.Resolved ) {
				field.Value = ptr.Value;
				return;
			}
			if( ptr.State == BlenderPointerState.Resolving || ptr.State == BlenderPointerState.Failed ) {
				field.Value = null;
				return;
			}
			
			// This is needed to avoid infinite loops
			ptr.State = BlenderPointerState.Resolving;
			field.Value = ptr;
			
			dynamic resolvedValue = null;
			// TODO: resolve the pointer
			
			field.Value = resolvedValue;
		}
		
		/// <summary>
		/// Tries to find a field in the structure by its address 
		/// </summary>
		/// <param name="address">Address of the field</param>
		/// <param name="lazy">If set to <b>true</b> the method will return the field instead of the first object from the object array contained in that field. This applies only when field contains array of objects and given address is the same as address of the field itself.</param>
		/// <param name="loadedData">List of all objects loaded from the file. Needed for the recursive pointer resolving.</param>
		/// <returns></returns>
		public object ResolvePointer(ulong address, bool lazy, List<BlenderFileObject[]> loadedData) {
			// int[] dimSizes, idx;
			// int i;
			// BlenderFileObject obj;
			
			if( m_Address <= address && address < m_Address + (ulong)m_Size ) {
				foreach( BlenderFileObjectField field in m_Fields.Values ) {
					if( field.Address > address || address >= field.Address + (ulong)field.Size )
						continue;
					if( field.Type == BlenderInternalType.Object && field.Value != null ) {
						if( lazy && field.Address == address )
							return field;
						if( field.Value.GetType().IsArray ) {
							/*
							dimSizes = GetArrayDimensions(field.Value.Value);

							// walk through all array elements
							idx = new int[dimSizes.Length];
							do {
								obj = field.Value.Value.GetValue(idx);
								
								if( obj.IsMyAddress(address) )
									return obj;
								if( obj.IsAddressInRange(address) )
									return obj.ResolvePointer(address, lazy, loadedData);
								
								// go to next possible index in array of current indexes
								for( i = idx.Length - 1; i >= 0 && ++idx[i] >= dimSizes[i]; i-- )
									idx[i] = 0;
							} while(i >= 0);
							*/
							foreach( BlenderFileObject obj in field.Value.Value ) {
								if( obj.IsMyAddress(address) )
									return obj;
								if( obj.IsAddressInRange(address) )
									return obj.ResolvePointer(address, lazy, loadedData);
							}
							
							return null;
						}
						else
							return ((BlenderFileObject)field.Value).ResolvePointer(address, lazy, loadedData);
					}
					if( field.Address == address )
						return field;
				}
			}
			return false;
		}
		
		public string GetFieldType(string fieldName) {
			BlenderFileObjectField field;
			if( !m_Fields.TryGetValue(fieldName, out field) )
				return null;
			return field.TypeName;
		}
		
		public void AddField(string name, BlenderFileObjectField field) {
			m_Fields.Add(name, field);
			if( m_FirstField == null )
				m_FirstField = field;
		}
		
		public void AddField(string name, BlenderInternalType internalType, string typeName, ulong address, int size, bool isPointer, dynamic val) {
			AddField(name, new BlenderFileObjectField {
			    Type = internalType, 
				TypeName = typeName,
				Address = address,
				Size = size,
				Value = val,
				IsPointer = isPointer
			});
		}

		public void Log(string index, int level) {
			string indent = new string(' ', level * 2), typeName;
			Type type;
			int[] dimSizes, idx;
			int i;
			dynamic v;
			
			GameDebugger.Log(LogLevel.VerboseDebug, "{0}{1}{2} (0x{3:X16}) {{", indent, index, m_TypeName, m_Address);
			
			foreach( KeyValuePair<string, BlenderFileObjectField> field in m_Fields ) {
				typeName = (field.Value.Type == BlenderInternalType.String) ? "string" : field.Value.TypeName;
				
				if( field.Value.Type == BlenderInternalType.Method )
					GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2}()", indent, field.Value.TypeName, field.Key);
				else if( field.Value.Value != null ) {
					type = field.Value.Value.GetType();
					if( type.IsArray ) {
						dimSizes = GetArrayDimensions(field.Value.Value);
						GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2}{3}[{4}] = {{", indent, typeName, field.Value.IsPointer ? "*" : "", field.Key, String.Join("][", dimSizes));

						// walk through all array elements
						idx = new int[dimSizes.Length];
						do {
							v = field.Value.Value.GetValue(idx);
							if( field.Value.IsPointer ) {
								if( v is BlenderPointer )
									GameDebugger.Log(LogLevel.VerboseDebug, "{0}    [{1}] = 0x{2:X16} ({3})", indent, String.Join(",", idx), v.Address, v.State);
								else
									GameDebugger.Log(LogLevel.VerboseDebug, "{0}    [{1}] = {2} (Resolved)", indent, String.Join(",", idx), v.GetType().Name);
							}
							else if( field.Value.Type == BlenderInternalType.Object )
								v.Log(String.Format("[{0}] = ", String.Join(",", idx)), level + 2);
							else
								GameDebugger.Log(LogLevel.VerboseDebug, "{0}    [{1}] = {2}", indent, String.Join(",", idx), field.Value.Value.GetValue(idx));
							// go to next possible index in array of current indexes
							for( i = idx.Length - 1; i >= 0 && ++idx[i] >= dimSizes[i]; i-- )
								idx[i] = 0;
						} while(i >= 0);

						GameDebugger.Log(LogLevel.VerboseDebug, "{0}  }}", indent);
					}
					else {
						if( field.Value.IsPointer ) {
							if( field.Value.Value is BlenderPointer )
								GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2}{3} = 0x{4:X16} ({5})", indent, typeName, field.Value.Value.Multidimensional ? "multidim *" : "*", field.Key, field.Value.Value.Address, field.Value.Value.State);
							else
								GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2}{3} = {4} (Resolved)", indent, typeName, field.Value.Value.Multidimensional ? "multidim *" : "*", field.Key, field.Value.Value.GetType().Name);
						}
						else if( field.Value.Type == BlenderInternalType.Object )
							field.Value.Value.Log(String.Format("{0} {1} = ", typeName, field.Key), level + 1);
						else
							GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2} = {3}", indent, typeName, field.Key, field.Value.Value);
					}
				}
				else
					GameDebugger.Log(LogLevel.VerboseDebug, "{0}  {1} {2}{3} = NULL", indent, typeName, field.Value.IsPointer ? "*" : "", field.Key);
			}
			GameDebugger.Log(LogLevel.VerboseDebug, "{0}}}", indent, m_TypeName, index, m_Size);
		}
		
		public int[] GetArrayDimensions(Array arr) {
			int[] dimSizes = new int[arr.GetType().GetArrayRank()];
			for( int i = 0; i < dimSizes.Length; i++ )
				dimSizes[i] = arr.GetLength(i);
			return dimSizes;
		}
	}
}
