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
using IGE.Graphics;
using IGE.Graphics.OpenGL;

namespace IGE {
	/// <summary>
	/// </summary>
	public class Planet {
		public const int MaxLevel = 3;
		public const double DetailSize = 8.0; // 2.0, 4.0, 8.0, ....

		public const double HALF_PI = Math.PI * 0.5;
		public const double PI = Math.PI;
		public const double DOUBLE_PI = Math.PI * 2.0;
		
		public Vector3 Location = Vector3.Zero;
		public Vector3 Rotation = Vector3.Zero;
		public double Radius = 1.0;
		public double Amplitude = 1.0;
		public Simplex Noise = null;
		
		public Planet() {
			Noise = new Simplex();
		}
		
		public void Render() {
			double coefficient = (double)View.Width / (2.0 * Math.Tan(HALF_PI * 0.5)); // 90 degrees fov (HALF_PI)
			double quality = 32.0 * Radius * coefficient / 500.0;
		
			GL.PushMatrix();
			GL.Translate(ref Location);
			GL.Rotate(Rotation.X, 1f, 0f, 0f);
			GL.Rotate(Rotation.Y, 0f, 1f, 0f);
			GL.Rotate(Rotation.Z, 0f, 0f, 1f);
		    
		    // north half
		    RenderSection(0, true, quality, 0, HALF_PI, HALF_PI, 0, 0, 0);
		    RenderSection(0, true, quality, HALF_PI, HALF_PI, PI, 0, HALF_PI, 0);
		    RenderSection(0, true, quality, PI, HALF_PI, PI+HALF_PI, 0, PI, 0);
		    RenderSection(0, true, quality, PI+HALF_PI, HALF_PI, DOUBLE_PI, 0, PI+HALF_PI, 0);
		
		    // south half
		    RenderSection(0, true, quality, 0, -HALF_PI, 0, 0, HALF_PI, 0);
		    RenderSection(0, true, quality, HALF_PI, -HALF_PI, HALF_PI, 0, PI, 0);
		    RenderSection(0, true, quality, PI, -HALF_PI, PI, 0, PI+HALF_PI, 0);
		    RenderSection(0, true, quality, PI+HALF_PI, -HALF_PI, PI+HALF_PI, 0, DOUBLE_PI, 0);

			GL.PopMatrix();
		}
		
		public void RenderSection(int level, bool polar, double quality, double lng1, double lat1, double lng2, double lat2, double lng3, double lat3) {
			if( level > MaxLevel )
				return;

			double lng4, lat4, lng5, lat5, lng6, lat6;
			
			double
				lngCos1 = Math.Cos(lng1), lngSin1 = Math.Sin(lng1), latCos1 = Math.Cos(lat1), latSin1 = Math.Sin(lat1),
				lngCos2 = Math.Cos(lng2), lngSin2 = Math.Sin(lng2), latCos2 = Math.Cos(lat2), latSin2 = Math.Sin(lat2),
				lngCos3 = Math.Cos(lng3), lngSin3 = Math.Sin(lng3), latCos3 = Math.Cos(lat3), latSin3 = Math.Sin(lat3);
			
			double h1 = Noise.SphericalNoise(DetailSize, lngCos1, lngSin1, latCos1, latSin1) * 0.5;
			double h2 = Noise.SphericalNoise(DetailSize, lngCos2, lngSin2, latCos2, latSin2) * 0.5;
			double h3 = Noise.SphericalNoise(DetailSize, lngCos3, lngSin3, latCos3, latSin3) * 0.5;
				
			double r1 = h1 * Amplitude + Radius;
			double r2 = h2 * Amplitude + Radius;
			double r3 = h3 * Amplitude + Radius;
			if( h1 < 0.0 || h1 > 1.0 )
				GameDebugger.Add(h1);
			
			double x1 = lngSin1 * latCos1 * r1, y1 = latSin1 * r1, z1 = lngCos1 * latCos1 * r1;
			double x2 = lngSin2 * latCos2 * r2, y2 = latSin2 * r2, z2 = lngCos2 * latCos2 * r2;
			double x3 = lngSin3 * latCos3 * r3, y3 = latSin3 * r3, z3 = lngCos3 * latCos3 * r3;
			
			if( level == MaxLevel ) {
				GL.Begin(BeginMode.Triangles);
				GL.Color4((float)h1, (float)h1, (float)h1, 1.0f); GL.Vertex3((float)x1, (float)y1, (float)z1);
				GL.Color4((float)h3, (float)h3, (float)h3, 1.0f); GL.Vertex3((float)x3, (float)y3, (float)z3);
				GL.Color4((float)h2, (float)h2, (float)h2, 1.0f); GL.Vertex3((float)x2, (float)y2, (float)z2);
				GL.End();
				/*GL.Begin(BeginMode.LineStrip);
				GL.Color4((float)h1 * 0.5f, (float)h1 * 0.5f, (float)h1 * 0.5f, 1.0f); GL.Vertex3((float)x1, (float)y1, (float)z1);
				GL.Color4((float)h3 * 0.5f, (float)h3 * 0.5f, (float)h3 * 0.5f, 1.0f); GL.Vertex3((float)x3, (float)y3, (float)z3);
				GL.Color4((float)h2 * 0.5f, (float)h2 * 0.5f, (float)h2 * 0.5f, 1.0f); GL.Vertex3((float)x2, (float)y2, (float)z2);
				GL.End();*/
			}
			else {
				if( polar ) {
					lng4 = (lng2 + lng3) * 0.5; lat4 = lat2;
					lng5 = lng3; lat5 = (lat1 + lat3) * 0.5;
					lng6 = lng2; lat6 = (lat1 + lat2) * 0.5;
				}
				else {
					lng4 = (lng2 + lng3) * 0.5; lat4 = (lat2 + lat3) * 0.5;
					lng5 = (lng1 + lng3) * 0.5; lat5 = (lat1 + lat3) * 0.5;
					lng6 = (lng2 + lng1) * 0.5; lat6 = (lat2 + lat1) * 0.5;
				}
				quality *= 0.5;
				RenderSection(level + 1, polar, quality, lng1, lat1, lng6, lat6, lng5, lat5);
				RenderSection(level + 1, false, quality, lng5, lat5, lng4, lat4, lng3, lat3);
				RenderSection(level + 1, false, quality, lng4, lat4, lng5, lat5, lng6, lat6);
				RenderSection(level + 1, false, quality, lng6, lat6, lng2, lat2, lng4, lat4);
			}
		}
	}
}
