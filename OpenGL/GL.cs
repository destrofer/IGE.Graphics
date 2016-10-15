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
using IGE;

using IGE.Platform;

namespace IGE.Graphics.OpenGL {
	public static partial class GL {
		public static readonly Version ExpectedDriverVersion = new Version(9, 0, 0);
		public static IGraphicsDriver Driver = null;

		static GL() {
			Driver = DriverManager.LoadDriver<IGraphicsDriver>(GameConfig.GraphicsDriver, ExpectedDriverVersion);
			Driver.Initialize();
			RuntimeImport();
		}
		
		public static void EnsureLoaded() {
		}
		
		/// <summary>
		/// To properly load extension functions this method must be called AFTER OpenGL rendering context has been created and pixel format chosen!
		/// </summary>
		public static void RuntimeImport() {
			API.RuntimeImport(typeof(IGE.Graphics.OpenGL.GL.Delegates), GetProcAddressInternal, null);
		}
		
		private static IntPtr GetProcAddressInternal(string lpszProc, object param) {
			return Driver.GetGLProcAddress(lpszProc);
		}
		
		public static class Supports {
			public static bool VBO { get { return GL.Delegates.GenBuffersARB != null; } }
		}
		
		public static void ResetContext() {
			Driver.ResetContext();
		}
		
		public static IOpenGLWindow CreateWindow(INativeWindow parent, int x, int y, int width, int height) {
			return Driver.CreateWindow(parent, x, y, width, height);
		}
		
		public static bool SetBufferSwapInterval(int interval) {
			return Driver.SetBufferSwapInterval(interval);
		}
		
		public static int GetBufferSwapInterval() {
			return Driver.GetBufferSwapInterval();
		}
	}
}