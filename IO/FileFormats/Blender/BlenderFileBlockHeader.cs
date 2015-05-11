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
	public class BlenderFileBlockHeader {
		public string Code;
		public int Size;
		public ulong OldMemoryAddress;
		public int SDNAIndex;
		public int Count;
		public long DataPosition;
		
		public BlenderFileBlockHeader(BinaryReader r, BlenderPointerSize pointerSize, Endian endianness) {
			Code = r.ReadFixedSizeString(4, Encoding.ASCII);
			Size = r.ReadInt32(endianness);
			OldMemoryAddress = (pointerSize == BlenderPointerSize.Ptr32) ? (ulong)r.ReadUInt32(endianness) : r.ReadUInt64(endianness);
			SDNAIndex = r.ReadInt32(endianness);
			Count = r.ReadInt32(endianness);
			DataPosition = r.BaseStream.Position;
		}
	}
}
