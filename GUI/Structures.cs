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

namespace IGE.GUI {
	public struct HitTestResult : IEquatable<HitTestResult> {
		public static readonly HitTestResult None = new HitTestResult { Target = null, X = 0, Y = 0 };
		public Component Target;
		public int X;
		public int Y;
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				if (Target != null)
					hashCode += 1000000007 * Target.GetHashCode();
				hashCode += 1000000009 * X.GetHashCode();
				hashCode += 1000000021 * Y.GetHashCode();
			}
			return hashCode;
		}
		
		public override bool Equals(object obj) {
			return (obj is HitTestResult) && Equals((HitTestResult)obj);
		}

		public bool Equals(HitTestResult other) {
			return other.Target == Target && other.X == X && other.Y == Y;
		}
		
		public static bool operator ==(HitTestResult lhs, HitTestResult rhs) {
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(HitTestResult lhs, HitTestResult rhs) {
			return !(lhs == rhs);
		}

	}
}
