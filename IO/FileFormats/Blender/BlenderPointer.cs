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
	public struct BlenderPointer : IEquatable<BlenderPointer> {
		public readonly ulong Address;
		public BlenderPointerState State;
		public dynamic Value;
		public bool Multidimensional;
		
		public static readonly BlenderPointer Null = new BlenderPointer(0UL);

		public BlenderPointer(ulong address) : this(address, false) {
		}
		
		public BlenderPointer(ulong address, bool multidimensional) {
			Address = address;
			State = (address == 0UL) ? BlenderPointerState.Resolved : BlenderPointerState.Unknown;
			Value = null;
			Multidimensional = multidimensional;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj) {
			if (obj is BlenderPointer)
				return Equals((BlenderPointer)obj);
			return false;
		}
		
		public bool Equals(BlenderPointer other) {
			return Address == other.Address;
		}
		
		public override int GetHashCode() {
			return Address.GetHashCode();
		}
		
		public static bool operator == (BlenderPointer left, BlenderPointer right) {
			return left.Equals(right);
		}
		
		public static bool operator != (BlenderPointer left, BlenderPointer right) {
			return !left.Equals(right);
		}
		#endregion
	}
}
