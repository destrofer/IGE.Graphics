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

using IGE.IO;
using IGE.Graphics.OpenGL;

namespace IGE.Graphics {
	public abstract class ShaderObject : IDisposable {
		protected int m_Id;
		protected ShaderObjectType m_ObjectType;
		protected Version m_Version = null;
		
		public List<ShaderAutoUniformInfo> StaticAutoUniforms = null;
		public List<ShaderAutoUniformInfo> DynamicAutoUniforms = null;
		
		public int Id { get { return m_Id; } }
		public ShaderObjectType ObjectType { get { return m_ObjectType; } }
		public string InfoLog { get { return (m_Id == 0) ? null : GL.GetInfoLog(m_Id); } }
		public bool Disposed { get { return m_Id == 0; } }
		public Version Version { get { return m_Version; } }
		
		public ShaderObject(ShaderObjectType objectType) {
			m_ObjectType = objectType;
			m_Id = GL.CreateShaderObject(objectType);
			if( m_Id == 0 )
				throw new UserFriendlyException(String.Format("GL.CreateShaderObject({0}) returned a zero result", objectType.ToString()), "Could not create a shader");
		}
		
		public ShaderObject(ShaderObjectType objectType, string source) : this(objectType) {
			SetSource(source);
			Compile();
		}
		
		public virtual void SetSource(string source) {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			GL.ShaderSource(m_Id, source);
		}
		
		public virtual void Compile() {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			GL.CompileShader(m_Id);
		}
		
		public virtual void AttachTo(Shader shader) {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			shader.Attach(this);
		}
		
		~ShaderObject() {
			Dispose(false);
		}
		
		public void Dispose() {
			Dispose(true);
		}
		
		protected virtual void Dispose(bool manual) {
			if( !Disposed ) {
				if( manual ) {
					if( m_Id != 0 )
						GL.DeleteObject(m_Id);
					m_Id = 0;
				}
				else
					GameDebugger.Log(LogLevel.Warning, "One of shader objects ({0}) was not properly disposed", m_ObjectType.ToString());
			}
		}
		
		public static ShaderObject Load(string fileName) {
			StructuredTextFile root = GameFile.LoadFile<StructuredTextFile>(fileName);
			return Load(root.Root, fileName);
		}
		
		public static ShaderObject Load(DomNode node, string fileName) {
			int[] intVal = new int[4];
			float[] floatVal = new float[4];
			string[] splitValue;
			string shaderTypeString = null;
			ShaderObject obj = null;
			Version ver, maxVer = Shader.VersionSupported;

			Version bestVersion = null;
			DomNode bestNode = null;
			
			List<ShaderAutoUniformInfo> StaticAutoUniforms = null;
			List<ShaderAutoUniformInfo> DynamicAutoUniforms = null;
			string code = null;
			
			foreach(DomAttribute attr in node.Attributes) {
				switch(attr.Name.ToLower()) {
					case "src":  return Load(attr.Value.ToPath());
					case "relsrc": return Load(attr.Value.RelativeTo(fileName));
					case "type": shaderTypeString = attr.Value.ToLower(); break;
				}
			}

			foreach( DomNode child in node ) {
				if( child.Name.ToLower().Equals("content") ) {
					if( !Version.TryParse(child["version"], out ver) )
						ver = new Version("0.0");
					if( (bestVersion == null || bestVersion < ver) && ver <= maxVer ) {
						bestNode = child;
						bestVersion = ver;
					}
				}
			}
			
			if( bestNode == null )
				return null;
			
			foreach( DomNode child in bestNode ) {
				switch( child.Name.ToLower() ) {
					case "code":
						code = child.Value;
						break;
					
					case "uniform":
						ShaderAutoUniformInfo aui = new ShaderAutoUniformInfo(null, ShaderAutoUniformClass.Unknown, null);
						bool typePresent = false;
						foreach( DomAttribute attr in child.Attributes ) {
							switch( attr.Name ) {
								case "name":
									aui.Name = attr.Value;
									break;
								
								case "type":
									typePresent = true;
									if( attr.Value.ToLower() == "unknown" || !ShaderAutoUniformClass.TryParse(attr.Value, true, out aui.Type) ) {
										aui.Type = ShaderAutoUniformClass.Unknown;
										// This is not a critical error, but it will affect how shader will work during render process so we have to log this minor error
										GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform with an unexpected type of '{1}'", fileName, attr.Value);
									}
									break;
								
								case "value":
									aui.Value = attr.Value;
									break;
							}
						}
						
						if( !typePresent || aui.Name == null || aui.Name.Equals("") ) {
							GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' that does not have all required attributes", fileName, aui.Name);
						}
						else if( aui.Type != ShaderAutoUniformClass.Unknown ) {
							if( aui.Value == null ) {
								if( aui.Type == ShaderAutoUniformClass.GameTime || aui.Type == ShaderAutoUniformClass.NonStopTime )
									aui.Value = "1";
								else
									GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' that has no value set", fileName, aui.Name);
							}
							bool result = false;
							
							if( aui.Value != null ) {
								switch (aui.Type) {
									case ShaderAutoUniformClass.Int:
										if( result = int.TryParse((string)aui.Value, out intVal[0]) )
											aui.Value = intVal[0];
										break;
										
									case ShaderAutoUniformClass.Float:
										if( result = float.TryParse((string)aui.Value, out floatVal[0]) )
											aui.Value = floatVal[0];
										break;
										
									case ShaderAutoUniformClass.Vec2:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 2 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = float.TryParse(splitValue[0], out floatVal[0]) )
												if( result = float.TryParse(splitValue[1], out floatVal[1]) )
													aui.Value = new Vector2(floatVal[0], floatVal[1]);
										}
										break;
										
									case ShaderAutoUniformClass.Vec3:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 3 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = float.TryParse(splitValue[0], out floatVal[0]) )
												if( result = float.TryParse(splitValue[1], out floatVal[1]) )
													if( result = float.TryParse(splitValue[2], out floatVal[2]) )
														aui.Value = new Vector3(floatVal[0], floatVal[1], floatVal[2]);
										}
										break;
										
									case ShaderAutoUniformClass.Vec4:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 4 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = float.TryParse(splitValue[0], out floatVal[0]) )
												if( result = float.TryParse(splitValue[1], out floatVal[1]) )
													if( result = float.TryParse(splitValue[2], out floatVal[2]) )
														if( result = float.TryParse(splitValue[3], out floatVal[3]) )
															aui.Value = new Vector4(floatVal[0], floatVal[1], floatVal[2], floatVal[3]);
										}
										break;
										
									case ShaderAutoUniformClass.Point2:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 2 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = int.TryParse(splitValue[0], out intVal[0]) )
												if( result = int.TryParse(splitValue[1], out intVal[1]) )
													aui.Value = new Point2(intVal[0], intVal[1]);
										}
										break;
										
									case ShaderAutoUniformClass.Point3:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 3 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = int.TryParse(splitValue[0], out intVal[0]) )
												if( result = int.TryParse(splitValue[1], out intVal[1]) )
													if( result = int.TryParse(splitValue[2], out intVal[2]) )
														aui.Value = new Point3(intVal[0], intVal[1], intVal[2]);
										}
										break;
										
									case ShaderAutoUniformClass.Point4:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 4 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = int.TryParse(splitValue[0], out intVal[0]) )
												if( result = int.TryParse(splitValue[1], out intVal[1]) )
													if( result = int.TryParse(splitValue[2], out intVal[2]) )
														if( result = int.TryParse(splitValue[3], out intVal[3]) )
															aui.Value = new Point4(intVal[0], intVal[1], intVal[2], intVal[3]);
										}
										break;
										
									case ShaderAutoUniformClass.Color4:
										splitValue = ((string)aui.Value).Split(',');
										if( splitValue.Length != 4 )
											GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value for '{2}' type", fileName, aui.Name, aui.Type);
										else {
											if( result = float.TryParse(splitValue[0], out floatVal[0]) )
												if( result = float.TryParse(splitValue[1], out floatVal[1]) )
													if( result = float.TryParse(splitValue[2], out floatVal[2]) )
														if( result = float.TryParse(splitValue[3], out floatVal[3]) )
															aui.Value = new Color4(floatVal[0], floatVal[1], floatVal[2], floatVal[3]);
										}
										break;
										
									case ShaderAutoUniformClass.Sampler1D:
										if( result = int.TryParse((string)aui.Value, out intVal[0]) )
											aui.Value = intVal[0];
										break;
										
									case ShaderAutoUniformClass.Sampler2D:
										if( result = int.TryParse((string)aui.Value, out intVal[0]) )
											aui.Value = intVal[0];
										break;
										
									case ShaderAutoUniformClass.Sampler3D:
										if( result = int.TryParse((string)aui.Value, out intVal[0]) )
											aui.Value = intVal[0];
										break;
										
									case ShaderAutoUniformClass.Sampler4D:
										if( result = int.TryParse((string)aui.Value, out intVal[0]) )
											aui.Value = intVal[0];
										break;
										
									case ShaderAutoUniformClass.NonStopTime:
										if( result = float.TryParse((string)aui.Value, out floatVal[0]) )
											aui.Value = floatVal[0];
										break;
										
									case ShaderAutoUniformClass.GameTime:
										if( result = float.TryParse((string)aui.Value, out floatVal[0]) )
											aui.Value = floatVal[0];
										break;
									
									default:
										GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with a type that is not fully implemented yet (no parser for the value)", fileName, aui.Name);
										break;
								}
							}
							
							if( result ) {
								if( aui.Type == ShaderAutoUniformClass.GameTime || aui.Type == ShaderAutoUniformClass.NonStopTime ) {
									if( DynamicAutoUniforms == null )
										DynamicAutoUniforms = new List<ShaderAutoUniformInfo>();
									DynamicAutoUniforms.Add(aui);
								}
								else {
									if( StaticAutoUniforms == null )
										StaticAutoUniforms = new List<ShaderAutoUniformInfo>();
									StaticAutoUniforms.Add(aui);
								}
							}
							else
								GameDebugger.Log(LogLevel.Error, "Shader object '{0}' has an auto uniform '{1}' with an invalid value", fileName, aui.Name);
						}
						
						break;
				}
			}

			if( code == null )
				return null;
			
			switch(shaderTypeString) {
				case "vertex": obj = new VertexShader(); break;
				case "fragment": obj = new FragmentShader(); break;
				default: throw new UserFriendlyException(String.Format("Shader node in '{1}' has a type attribute with an unrecognizable value '{0}' while only 'vertex' and 'fragment' are supported.", shaderTypeString, fileName), "There was an error in one of shader definition files.");
			}
			
			obj.m_Version = bestVersion;
			obj.SetSource(code);
			obj.Compile();
			obj.StaticAutoUniforms = StaticAutoUniforms;
			obj.DynamicAutoUniforms = DynamicAutoUniforms;
			
			return obj;
		}
	}
}
