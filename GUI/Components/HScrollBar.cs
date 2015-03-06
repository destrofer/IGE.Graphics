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
	public class HScrollBar : ScrollBar {
		public HScrollBar(int x, int y, int width, int height, int min, int max) : base(x, y, width, height, min, max) {
		}
		
		public HScrollBar(int x, int y, int width, int height) : base(x, y, width, height) {
		}

		public override void SetDefaultStyle() {
			m_Background = UI.Defaults.HScroll.BarBackground;
		}
		
		public override UIEventType ScrollEventType { get { return UIEventType.HScroll; } }
	}
}
