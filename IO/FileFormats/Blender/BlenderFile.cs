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
 * 
 * Blender file loading is built according to 3rd party file format documentation:
 * http://www.atmind.nl/blender/mystery_ot_blend.html
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;

namespace IGE.IO.FileFormats.Blender {
	public class BlenderFile {
		public BlenderSDNAFileBlock SDNA = null;
		
		public BlenderFile(string fileName) {
			try {
				using(BinaryReader r = new BinaryReader(new FileStream(fileName, FileMode.Open))) {
					BlenderFileHeader bfh = new BlenderFileHeader(r);
					if( bfh.Identifier.Equals("BLENDER", StringComparison.Ordinal) ) {
						GameDebugger.Log(LogLevel.VerboseDebug, "{0} ptr={1} endianness={2} version={3}", bfh.Identifier, bfh.PointerSize, bfh.Endianness, bfh.VersionNumber);
						BlenderFileBlockHeader bfbh;
						
						Dictionary<string, List<BlenderFileBlockHeader>> BlockCodeIndex = new Dictionary<string, List<BlenderFileBlockHeader>>();
						Dictionary<int, List<BlenderFileBlockHeader>> BlockSDNAIndex = new Dictionary<int, List<BlenderFileBlockHeader>>();
						List<BlenderFileBlockHeader> allBlockHeaders = new List<BlenderFileBlockHeader>();
						List<BlenderFileBlockHeader> indexList;
						
						do {
							bfbh = new BlenderFileBlockHeader(r, bfh.PointerSize, bfh.Endianness);
							GameDebugger.Log(LogLevel.VerboseDebug, "{0} size={1} addr=0x{2:X16} SDNA={3} count={4}", bfbh.Code, bfbh.Size, bfbh.OldMemoryAddress, bfbh.SDNAIndex, bfbh.Count);
							
							allBlockHeaders.Add(bfbh);
							
							// add to index by block code
							if( !BlockCodeIndex.TryGetValue(bfbh.Code, out indexList) ) {
								indexList = new List<BlenderFileBlockHeader>();
								BlockCodeIndex.Add(bfbh.Code, indexList);
							}
							indexList.Add(bfbh);

							// add to index by SDNA type
							if( !BlockSDNAIndex.TryGetValue(bfbh.SDNAIndex, out indexList) ) {
								indexList = new List<BlenderFileBlockHeader>();
								BlockSDNAIndex.Add(bfbh.SDNAIndex, indexList);
							}
							indexList.Add(bfbh);
							
							if( bfbh.Size > 0 )
								r.BaseStream.Position = r.BaseStream.Position + bfbh.Size;
						} while(!bfbh.Code.Equals("ENDB", StringComparison.Ordinal));
						
						if( BlockCodeIndex.TryGetValue("DNA1", out indexList) ) {
							r.BaseStream.Position = indexList[0].DataPosition;
							SDNA = new BlenderSDNAFileBlock(r, bfh.PointerSize, bfh.Endianness);
						}
						else
							throw new Exception("DNA1 block not found");
						
						List<BlenderFileObject[]> loadedData = new List<BlenderFileObject[]>();
						BlenderFileObject[] arr;
						
						int i;
						ulong addr;
						foreach( BlenderFileBlockHeader bh in allBlockHeaders ) {
							if( bh.Count <= 0 || bh.Size <= 0 || bh.Code.Equals("ENDB", StringComparison.Ordinal) || bh.Code.Equals("ENDB", StringComparison.Ordinal) )
								continue;
							r.BaseStream.Position = bh.DataPosition;
							addr = bh.OldMemoryAddress;
							arr = new BlenderFileObject[bh.Count];
							GameDebugger.Log(LogLevel.VerboseDebug, "LOADING {0} size={1} addr=0x{2:X16} SDNA={3} count={4}", bh.Code, bh.Size, bh.OldMemoryAddress, bh.SDNAIndex, bh.Count);
							for( i = 0; i < bh.Count; i++ ) {
								arr[i] = new BlenderFileObject(r, addr, bfh.PointerSize, bfh.Endianness, SDNA, bh.SDNAIndex);
								addr += (ulong)SDNA.Structures[bh.SDNAIndex].Size;
							}
							loadedData.Add(arr);
						}
						
						foreach( BlenderFileObject[] data in loadedData ) {
							for( i = 0; i < data.Length; i++ )
								data[i].Log("", 0);
						}
					}
					else
						throw new Exception("Not a .blend file");
				}
			}
			catch(Exception ex) {
				throw new UserFriendlyException(String.Format("Error loading '{0}'", fileName), "One of data files has an invalid format or is damaged.", ex);
			}
		}
	}
}
