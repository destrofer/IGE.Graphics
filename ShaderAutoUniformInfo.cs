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

namespace IGE.Graphics {
	public struct ShaderAutoUniformInfo {
		public string Name;
		public ShaderAutoUniformClass Type;
		public object Value;
		
		public ShaderAutoUniformInfo(string name, ShaderAutoUniformClass type, object val) {
			Name = name;
			Type = type;
			Value = val;
		}
	}
	
	public enum ShaderAutoUniformClass {
		Unknown,
		Int,
		Float,
		Vec2,
		Vec3,
		Vec4,
		Point2,
		Point3,
		Point4,
		Color4,
		Sampler1D,
		Sampler2D,
		Sampler3D,
		Sampler4D,
		NonStopTime,
		GameTime,
	}
}
