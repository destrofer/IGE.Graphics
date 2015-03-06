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
using System.Collections.Generic;

using IGE.IO;
using IGE.GUI;

namespace IGE.Graphics {
	public class ComplexImage : ISizedRenderable2D {
		protected ComplexImageComponent[] m_Components = null;
		
		public ComplexImage() {
		}
		
		public ComplexImage(string fileName) : base() {
			using(StructuredTextFile file = GameFile.LoadFile<StructuredTextFile>(fileName)) {
				Load(fileName, file.Root);
			}
		}
		
		public ComplexImage(string fileName, DomNode node) : base() {
			Load(fileName, node);
		}
		
		public virtual void Load(string fileName, DomNode node) {
			List<ComplexImageComponent> components = new List<ComplexImageComponent>();
			foreach( DomNode child in node )
				if( child.Name.ToLower().Equals("component") )
					components.Add(ComplexImageComponent.Load(fileName, child));
			m_Components = (components.Count > 0) ? components.ToArray() : null;
		}
		
		public void Render(int x, int y, int width, int height) {
			if( m_Components == null || width <= 0 || height <= 0 )
				return;
			foreach( ComplexImageComponent comp in m_Components )
				comp.Render(x, y, width, height);
		}
	}
}
