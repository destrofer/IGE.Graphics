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

namespace IGE.Graphics {
	public class EnterFrameEventArgs {
		protected string m_FrameEventName;
		public string FrameEventName { get { return m_FrameEventName; } }
		
		protected object m_FrameData;
		public object FrameData { get { return FrameData; } }
		
		protected double m_Speed;
		public double Speed { get { return m_Speed; } }
		
		public EnterFrameEventArgs(string event_name, object frame_data, double speed) {
			m_FrameEventName = event_name;
			m_FrameData = frame_data;
			m_Speed = speed;
		}
	}

	public delegate int EnterFrameEvent(object issuer, EnterFrameEventArgs e);
}
