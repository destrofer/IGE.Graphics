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

namespace IGE.IO.FileFormats.Blender {
	public class BlenderFileBlock {
		public string Code;
		public int Size;
		public ulong OldMemoryAddress;
		public int SDNAIndex;
		public int Count;
		public long DataPosition;
		public bool IsParsable = false;
		public BlenderFileObject[] Data = null;
		public byte[] OriginalData = null;
		
		public BlenderFileBlock(BinaryReader r, BlenderFile file) {
			Code = r.ReadFixedSizeString(4, Encoding.ASCII);
			Size = r.ReadInt32(file.Header.Endianness);
			OldMemoryAddress = (file.Header.PointerSize == BlenderPointerSize.Ptr32) ? (ulong)r.ReadUInt32(file.Header.Endianness) : r.ReadUInt64(file.Header.Endianness);
			SDNAIndex = r.ReadInt32(file.Header.Endianness);
			Count = r.ReadInt32(file.Header.Endianness);
			DataPosition = r.BaseStream.Position;
		}
		
		public void Load(BinaryReader r, BlenderFile file) {
			IsParsable = (
				Count > 0 && Size > 0 && Size == Count * file.SDNA.Structures[SDNAIndex].Size
				&& !Code.Equals("DNA1", StringComparison.Ordinal)
				&& !Code.Equals("ENDB", StringComparison.Ordinal)
			);

			OriginalData = r.ReadBytes(Size); // original data is needed to resolve multidimensional pointers later
			
			if( !IsParsable ) {
				GameDebugger.Log(LogLevel.VerboseDebug, "LOADING BYTES ONLY {0} size={1} addr=0x{2:X16} SDNA={3} count={4}", Code, Size, OldMemoryAddress, SDNAIndex, Count);
				return;
			}
			
			r.BaseStream.Position = DataPosition;
			ulong addr = OldMemoryAddress;
			Data = new BlenderFileObject[Count];
			GameDebugger.Log(LogLevel.VerboseDebug, "LOADING STRUCT {0} size={1} addr=0x{2:X16} SDNA={3} count={4}", Code, Size, OldMemoryAddress, SDNAIndex, Count);
			for( int i = 0; i < Count; i++ ) {
				Data[i] = new BlenderFileObject(r, file, addr, SDNAIndex);
				addr += (ulong)file.SDNA.Structures[SDNAIndex].Size;
			}
		}

		/// <summary>
		/// Processes pointers in loaded data
		/// </summary>
		/// <param name="r"></param>
		/// <param name="file"></param>
		public void Process(BlenderFile file) {
			if( !IsParsable )
				return;
			foreach( BlenderFileObject data in Data )
				data.ResolvePointers(file);
		}
		
		public void Log() {
			if( IsParsable )
				for( int i = 0; i < Data.Length; i++ )
					Data[i].Log("", 0);
		}
	}
}
