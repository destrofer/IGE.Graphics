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
		public BlenderFileHeader Header;
		public BlenderSDNAFileBlock SDNA = null;
		public List<BlenderFileBlock> Blocks = null;
		
		public BlenderFile(string fileName) {
			try {
				using(BinaryReader r = new BinaryReader(new FileStream(fileName, FileMode.Open))) {
					Header = new BlenderFileHeader(r);
					if( Header.Identifier.Equals("BLENDER", StringComparison.Ordinal) ) {
						GameDebugger.Log(LogLevel.VerboseDebug, "{0} ptr={1} endianness={2} version={3}", Header.Identifier, Header.PointerSize, Header.Endianness, Header.VersionNumber);
						BlenderFileBlock block;
						
						Dictionary<string, List<BlenderFileBlock>> BlockCodeIndex = new Dictionary<string, List<BlenderFileBlock>>();
						Dictionary<int, List<BlenderFileBlock>> BlockSDNAIndex = new Dictionary<int, List<BlenderFileBlock>>();
						Blocks = new List<BlenderFileBlock>();
						List<BlenderFileBlock> indexList;
						
						do {
							block = new BlenderFileBlock(r, this);
							GameDebugger.Log(LogLevel.VerboseDebug, "{0} size={1} addr=0x{2:X16} SDNA={3} count={4}", block.Code, block.Size, block.OldMemoryAddress, block.SDNAIndex, block.Count);
							
							Blocks.Add(block);
							
							// add to index by block code
							if( !BlockCodeIndex.TryGetValue(block.Code, out indexList) ) {
								indexList = new List<BlenderFileBlock>();
								BlockCodeIndex.Add(block.Code, indexList);
							}
							indexList.Add(block);

							// add to index by SDNA type
							if( !BlockSDNAIndex.TryGetValue(block.SDNAIndex, out indexList) ) {
								indexList = new List<BlenderFileBlock>();
								BlockSDNAIndex.Add(block.SDNAIndex, indexList);
							}
							indexList.Add(block);
							
							if( block.Size > 0 )
								r.BaseStream.Position = r.BaseStream.Position + block.Size;
						} while(!block.Code.Equals("ENDB", StringComparison.Ordinal));
						
						if( BlockCodeIndex.TryGetValue("DNA1", out indexList) ) {
							r.BaseStream.Position = indexList[0].DataPosition;
							SDNA = new BlenderSDNAFileBlock(r, this);
						}
						else
							throw new Exception("DNA1 block not found");
						
						foreach( BlenderFileBlock blk in Blocks )
							blk.Load(r, this);

						foreach( BlenderFileBlock blk in Blocks )
							blk.Process(this);
						
						foreach( BlenderFileBlock blk in Blocks )
							blk.Log();
						
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
