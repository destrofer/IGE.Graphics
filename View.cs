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
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	/// <summary>
	/// </summary>
	public static class View {
		private static Rectangle m_Area;
		private static float m_Left = -1f;
		private static float m_Top = 1f;
		private static float m_Right = 1f;
		private static float m_Bottom = -1f;
		
		public static Rectangle Area { get { return m_Area; } set { SetView(value); } }

		public static float Left { get { return m_Left; } }
		public static float Top { get { return m_Top; } }
		public static float Right { get { return m_Right; } }
		public static float Bottom { get { return m_Bottom; } }

		public static int Width { get { return m_Area.Width; } }
		public static int Height { get { return m_Area.Height; } }
		
		private static ViewMode m_CurrentMode = ViewMode.Mode3D;
		public static ViewMode CurrentMode {
			get { return m_CurrentMode; }
			set { SetMode(value); }
		}
		
		public static bool YAxisIsUp { get { return m_CurrentMode != ViewMode.UI; } }
		
		public static void SetView(int x, int y, int width, int height) {
			SetView(new Rectangle(x, y, width, height));
		}
		
		
		public static void ResetView() {
			if( Application.MainWindow != null && !Application.MainWindow.Disposed )
				SetView(Application.MainWindow.GetClientRect());
		}
		
		public static void SetView(Rectangle rect) {
			// GameDebugger.Log("Setting view: {0}", rect.ToString());
			GL.Viewport(rect);
			m_Area = rect;
		}

		// rendering related
		private static float m_FOV = 45.0f;
		public static float FOV { get { return m_FOV; } set { m_FOV = value; } }
		private static float m_MinDepth = 0.1f;
		public static float MinDepth { get { return m_MinDepth; } set { m_MinDepth = value; } }
		private static float m_MaxDepth = 2000.0f;
		public static float MaxDepth { get { return m_MaxDepth; } set { m_MaxDepth = value; } }

		public static void SetMode(ViewMode mode) {
			// TODO: move fov and depth to camera since in 3d only current camera should be used while in 2d it is ignored
			switch( mode ) {
					case ViewMode.Mode2D: SetMode2D(); break;
					case ViewMode.UI: SetModeUI(); break;
					default: SetMode3D(); break;
			}
		}

		public static void SetMode3D(float aspect_ratio) {
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			// GameDebugger.Log("{0} {1} {2} {3}", m_View, m_View.Width, m_View.Height, (m_View.Height != 0.0f) ? ((float)m_View.Width / (float)m_View.Height) : 1.0f);
			Matrix4 projection = Matrix4.Perspective(
				m_FOV * (float)Math.PI / 360.0f,	// by default FOV = 90.0f
				aspect_ratio,
				m_MinDepth, m_MaxDepth // min and max depth must be positive
			);
			GL.LoadMatrix(ref projection);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			m_CurrentMode = ViewMode.Mode3D;
			
			m_Left = -1f;
			m_Top = 1f;
			m_Right = 1f;
			m_Bottom = -1f;
		}

		public static void SetMode3D() {
			SetMode3D((m_Area.Height != 0) ? ((float)m_Area.Width / (float)m_Area.Height) : 1.0f);
		}

		public static void SetMode2D()
		{
			// this mode needs an integer alignment to look good
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			int hw = (int)m_Area.Width / 2;
			int hh = (int)m_Area.Height / 2;
			
			m_Left = (float)-hw;
			m_Right = (float)((int)m_Area.Width - hw);
			m_Top = (float)((int)m_Area.Height - hh);
			m_Bottom = (float)-hh;
			
			GL.Ortho(m_Left, m_Right, m_Bottom, m_Top, -100000f, 100000f);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			m_CurrentMode = ViewMode.Mode2D;
		}

		public static Vector2 CoordsUITo2D(Vector2 point) {
			return new Vector2(point.X - (float)(m_Area.Width / 2 - m_Area.X), (float)(m_Area.Height / 2 - m_Area.Y) - point.Y);
		}

		public static void SetModeUI()
		{
			// this mode needs an integer alignment to look good
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			m_Left = 0f;
			m_Right = (float)m_Area.Width;
			m_Top = 0f;
			m_Bottom = (float)m_Area.Height;

			GL.Ortho(m_Left, m_Right, m_Bottom, m_Top, -1f, 1f);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			m_CurrentMode = ViewMode.UI;
		}

		/*
		public static void DoCameraTransformations() {
			ResetCameraTransformations();
			m_Camera.InverseTransformView();
		}
		*/
		
		public static void ResetCameraTransformations() {
			GL.LoadIdentity();
		}
		
		/*
		public static void MouseEulerCameraControl() {
			Camera.RotationY -= (float)GameEngine.Mouse.DeltaX / 10.0f;
			Camera.RotationX -= (float)GameEngine.Mouse.DeltaY / 10.0f;
			if( Camera.RotationX < -90.0f ) Camera.RotationX = -90.0f;
			if( Camera.RotationX > 90.0f ) Camera.RotationX = 90.0f;
			if( Camera.RotationY <= -180.0f ) Camera.RotationY += 180.0f;
			if( Camera.RotationY > 180.0f ) Camera.RotationY -= 180.0f;
		}
		*/
	}

	public enum ViewMode {
		Mode3D,
		Mode2D,
		UI
	}
}
