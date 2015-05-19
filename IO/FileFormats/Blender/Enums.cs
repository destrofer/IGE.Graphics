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

namespace IGE.IO.FileFormats.Blender {
	public enum BlenderPointerSize : byte {
		Ptr32,
		Ptr64
	}

	public enum BlenderInternalType : byte {
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
		Object,
		Method
	};
	
	public enum BlenderPointerState : byte {
		/// <summary>
		/// Pointer is new and was not tried to resolve yet.
		/// </summary>
		Unknown,
		
		/// <summary>
		/// Object or field could not be found at specified address.
		/// </summary>
		Failed,
		
		/// <summary>
		/// Pointer is being currently resolved. if encountered a pointer with this state when resolving then it means there is a pointer loop.
		/// </summary>
		Resolving,
		
		/// <summary>
		/// Pointer is resolved. No real use for that state since resolved pointers are replaced by values they have been resolved to.
		/// </summary>
		Resolved,
	}
}
