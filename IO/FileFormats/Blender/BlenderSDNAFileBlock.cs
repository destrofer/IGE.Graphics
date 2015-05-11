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

namespace IGE.IO.FileFormats.Blender {
	public class BlenderSDNAFileBlock {
		public BlenderFileStructure[] Structures = null;
		public Dictionary<string, BlenderFileStructure> StructureIndex = null;
		
		public BlenderSDNAFileBlock(BinaryReader r, BlenderPointerSize pointerSize, Endian endianness) {
			int i, j, idx, nameCount, typeCount, structCount, fieldCount;
			string id;
			long basePos, pos;
			int[] typeSizes;
			string[] names, typeNames;
			BlenderFileStructure st;
			BlenderFileField fld;
			
			Encoding ascii = Encoding.ASCII;
			
			basePos = r.BaseStream.Position;
			
			id = r.ReadFixedSizeString(4, ascii);
			if( !id.Equals("SDNA", StringComparison.Ordinal) )
				throw new Exception("SDNA block identifier expected");

			id = r.ReadFixedSizeString(4, Encoding.ASCII);
			if( !id.Equals("NAME", StringComparison.Ordinal) )
				throw new Exception("NAME block identifier expected");
			
			nameCount = r.ReadInt32(endianness);
			names = new string[nameCount];
			
			for( i = 0; i < nameCount; i++ ) {
				names[i] = r.ReadZeroTerminatedString(ascii);
				GameDebugger.Log(LogLevel.VerboseDebug, "names[{0}]={1}", i, names[i]);
			}
			
			// align to 4 bytes
			pos = r.BaseStream.Position;
			if( ((pos - basePos) & 3) != 0 )
				r.BaseStream.Position = basePos + ((pos - basePos) / 4 + 1) * 4;
			
			id = r.ReadFixedSizeString(4, Encoding.ASCII);
			if( !id.Equals("TYPE", StringComparison.Ordinal) )
				throw new Exception("TYPE block identifier expected");
			
			typeCount = r.ReadInt32(endianness);
			typeNames = new string[typeCount];
			typeSizes = new int[typeCount];
			
			for( i = 0; i < typeCount; i++ ) {
				typeNames[i] = r.ReadZeroTerminatedString(ascii);
				GameDebugger.Log(LogLevel.VerboseDebug, "typeNames[{0}]={1}", i, typeNames[i]);
			}
			
			// align to 4 bytes
			pos = r.BaseStream.Position;
			if( ((pos - basePos) & 3) != 0 )
				r.BaseStream.Position = basePos + ((pos - basePos) / 4 + 1) * 4;

			id = r.ReadFixedSizeString(4, Encoding.ASCII);
			if( !id.Equals("TLEN", StringComparison.Ordinal) )
				throw new Exception("TLEN block identifier expected");

			for( i = 0; i < typeCount; i++ ) {
				typeSizes[i] = r.ReadInt16(endianness);
				GameDebugger.Log(LogLevel.VerboseDebug, "typeSizes[{0}]={1}", i, typeSizes[i]);
			}
			
			// align to 4 bytes
			pos = r.BaseStream.Position;
			if( ((pos - basePos) & 3) != 0 )
				r.BaseStream.Position = basePos + ((pos - basePos) / 4 + 1) * 4;
			
			id = r.ReadFixedSizeString(4, Encoding.ASCII);
			if( !id.Equals("STRC", StringComparison.Ordinal) )
				throw new Exception("STRC block identifier expected");
			
			structCount = r.ReadInt32(endianness);
			Structures = new BlenderFileStructure[structCount];
			StructureIndex = new Dictionary<string, BlenderFileStructure>();
			for( i = 0; i < structCount; i++ ) {
				st = new BlenderFileStructure();
				idx = r.ReadInt16(endianness);
				st.Name = typeNames[idx];
				st.Size = typeSizes[idx];
				
				fieldCount = r.ReadInt16(endianness);
				st.Fields = new BlenderFileField[fieldCount];
				
				GameDebugger.Log(LogLevel.VerboseDebug, "struct #{0} {1} (size={2}, fields={3})", i, st.Name, st.Size, fieldCount);

				for( j = 0; j < fieldCount; j++ ) {
					fld = new BlenderFileField();
					idx = r.ReadInt16(endianness);
					fld.Type = typeNames[idx];
					fld.Size = typeSizes[idx];
					fld.Name = names[r.ReadInt16(endianness)];
					st.Fields[j] = fld;
					GameDebugger.Log(LogLevel.VerboseDebug, "    {0} {1} (size={2})", fld.Type, fld.Name, fld.Size);
				}
				Structures[i] = st;
				StructureIndex.Add(st.Name, st);
			}
		}
	}

	public struct BlenderFileStructure {
		public string Name;
		public int Size;
		public BlenderFileField[] Fields;
	}
	
	public struct BlenderFileField {
		public string Name;
		public string Type;
		public int Size;
	}
}
