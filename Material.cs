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
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using IGE.IO;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public class Material : IDisposable {
		public Texture[] Textures = null;
		public MaterialProperties[] Properties = null;
		public Shader Shader = null;
		
		protected bool m_Disposed = false;
		public bool Disposed { get { return m_Disposed; } }
		
		public object this[string uniformName] {
			set {
				if( Shader != null )
					Shader[uniformName] = value;
			}
		}
		
		public Material() {
		}
		
		public int GetAttribLocation(string attribName) {
			return (Shader == null) ? -1 : Shader.GetAttribLocation(attribName);
		}
		
		public void Bind() {
			Bind(1.0f);
		}
		
		public void Bind(float opacity) {
			if( Textures == null || Textures.Length == 0 )
				Texture.Unbind();
			else {
				for( int i = Textures.Length - 1; i >= 0; i-- )
					if( Textures[i] != null )
						Textures[i].Bind(i);
			}

			if( Properties != null ) {
				foreach( MaterialProperties prop in Properties )
					prop.Apply(opacity);
			}
			
			if( Shader == null )
				Shader.Unbind();
			else
				Shader.Bind();
		}
		
		public static void Unbind() {
			Shader.Unbind();
			Texture.Unbind();
		}
		
		public static Material Load(string fileName) {
			StructuredTextFile root = GameFile.LoadFile<StructuredTextFile>(fileName);
			return Load(root.Root, fileName);
		}
		
		public static Material Load(DomNode node, string fileName) {
			foreach(DomAttribute attr in node.Attributes) {
				switch(attr.Name.ToLower()) {
					case "src":  return Load(attr.Value.ToPath());
					case "relsrc": return Load(attr.Value.RelativeTo(fileName));
				}
			}
			
			int unit, maxUnit = -1;
			string path;
			Texture tex;
			MaterialProperties prop;
			Dictionary<int, Texture> textures = null;
			List<MaterialProperties> properties = new List<MaterialProperties>();
			Material obj = new Material();
						
			foreach( DomNode child in node ) {
				switch( child.Name.ToLower() ) {
					case "textures":
						foreach( DomNode texNode in child ) {
							if( !texNode.Name.ToLower().Equals("texture") )
								continue;
							if( int.TryParse(texNode["unit"], out unit) ) {
								if( unit >= 0 ) {
									if( !texNode["relsrc"].Equals("") )
										path = texNode["relsrc"].RelativeTo(fileName);
									else
										path = texNode["src"].ToPath();
									tex = Texture.Cache(path);
									if( tex != null ) {
										if( maxUnit < unit )
											maxUnit = unit;
										if( textures == null )
											textures = new Dictionary<int, Texture>();
										textures.Add(unit, tex);
									}
									else {
										GameDebugger.Log(LogLevel.Error, "Failed loading texture '{0}' specified in '{1}'", path, fileName);
									}
								}
							}
						}
						break;
					
					case "properties":
						prop = new MaterialProperties();
						
						if( !FaceName.TryParse(child["face"], true, out prop.Face) ) {
							prop.Face = FaceName.FrontAndBack;
							GameDebugger.Log(LogLevel.Error, "Failed parsing material face '{0}' for properties in '{1}'", child["face"], fileName);
						}
						
						foreach( DomNode propNode in child ) {
							switch( propNode.Name.ToLower() ) {
								case "ambient":
									if( !Color4.TryParse(propNode["color"], out prop.AmbientColor) ) {
										prop.AmbientColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
										GameDebugger.Log(LogLevel.Error, "Failed parsing ambient color '{0}' in '{1}'", propNode["color"], fileName);
									}
									else
										prop.AmbientColor.A = 1.0f;
									break;
								case "diffuse":
									if( !Color4.TryParse(propNode["color"], out prop.DiffuseColor) ) {
										prop.DiffuseColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
										GameDebugger.Log(LogLevel.Error, "Failed parsing diffuse color '{0}' in '{1}'", propNode["color"], fileName);
									}
									break;
								case "specular":
									if( !Color4.TryParse(propNode["color"], out prop.SpecularColor) ) {
										prop.SpecularColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
										GameDebugger.Log(LogLevel.Error, "Failed parsing specular color '{0}' in '{1}'", propNode["color"], fileName);
									}
									else
										prop.SpecularColor.A = 1.0f;
									break;
								case "emission":
									if( !Color4.TryParse(propNode["color"], out prop.EmissionColor) ) {
										prop.EmissionColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
										GameDebugger.Log(LogLevel.Error, "Failed parsing emission color '{0}' in '{1}'", propNode["color"], fileName);
									}
									else
										prop.EmissionColor.A = 1.0f;
									break;
								case "shininess":
									if( !float.TryParse(propNode["value"], out prop.Shininess) ) {
										prop.Shininess = 0.0f;
										GameDebugger.Log(LogLevel.Error, "Failed parsing shininess '{0}' in '{1}'", propNode["value"], fileName);
									}
									break;
								case "opacity":
									if( !float.TryParse(propNode["value"], out prop.Opacity) ) {
										prop.Opacity = 1.0f;
										GameDebugger.Log(LogLevel.Error, "Failed parsing opacity '{0}' in '{1}'", propNode["value"], fileName);
									}
									break;
							}
						}
						properties.Add(prop);
						break;
					
					case "shader":
						if( obj.Shader != null )
							obj.Shader.Dispose();
						obj.Shader = Shader.Load(child, fileName);
						if( obj.Shader == null )
							GameDebugger.Log(LogLevel.Error, "Failed loading shader in '{0}'", fileName);
						break;
				}
			}
			
			if( maxUnit >= 0 ) {
				obj.Textures = new Texture[maxUnit + 1];
				foreach( KeyValuePair<int, Texture> texPair in textures )
					obj.Textures[texPair.Key] = texPair.Value;
			}
			
			if( properties.Count > 0 )
				obj.Properties = properties.ToArray();
			
			return obj;
		}
		
		
		~Material() {
			Dispose(false);
		}
		
		public void Dispose() {
			Dispose(true);
		}
		
		protected virtual void Dispose(bool manual) {
			if( !m_Disposed ) {
				if( manual ) {
					if( Textures != null ) {
						foreach(Texture tex in Textures) {
							if( tex != null && !tex.Disposed )
								tex.Dispose();
						}
					}
					if( Shader != null && !Shader.Disposed )
						Shader.Dispose();
				}
				else
					GameDebugger.Log(LogLevel.Warning, "One of materials was not properly disposed");
				
				m_Disposed = true;
			}
		}
		
		public class MaterialProperties {
			public FaceName Face;
			public Color4 AmbientColor;
			public Color4 DiffuseColor;
			public Color4 SpecularColor;
			public Color4 EmissionColor;
			public float Shininess;
			public float Opacity;
			
			public MaterialProperties() {
				Face = FaceName.Front;
				AmbientColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
				DiffuseColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
				SpecularColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
				EmissionColor = new Color4(0.0f, 0.0f, 0.0f, 1.0f);
				Shininess = 0.0f;
				Opacity = 1.0f;
			}
			
			public MaterialProperties(FaceName face, Color4 ambient, Color4 diffuse, Color4 specular, Color4 emission, float shininess, float opacity) {
				Face = face;
				AmbientColor = ambient;
				DiffuseColor = diffuse;
				SpecularColor = specular;
				EmissionColor = emission;
				Shininess = shininess;
				Opacity = opacity;
			}
			
			public void Apply(float opacity) {
				DiffuseColor.A = Opacity * opacity;
				GL.Color4(ref DiffuseColor);
				GL.Material(Face, MaterialParamName.Ambient, ref AmbientColor);
				GL.Material(Face, MaterialParamName.Diffuse, ref DiffuseColor);
				GL.Material(Face, MaterialParamName.Specular, ref SpecularColor);
				GL.Material(Face, MaterialParamName.Emission, ref EmissionColor);
				GL.Material(Face, MaterialParamName.Shininess, Shininess);
			}
		}
	}
}
