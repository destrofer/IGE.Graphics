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
using IGE.Platform;

namespace IGE.Graphics {
	// TODO: make shaders cacheable like textures
	public class Shader : IDisposable {
		private static Version m_VersionSupported = null;
		private static string m_VersionInfo = null;
		public static string VersionInfo {
			get {
				if( m_VersionInfo == null ) {
					m_VersionInfo = GL.GetString(GlStringName.ShadingLanguageVersion);
					if( m_VersionInfo == null )
						m_VersionInfo = "0.0 GLSL_NOT_SUPPORTED";
				}
				return m_VersionInfo;
			}
		}
		public static Version VersionSupported {
			get {
				if( m_VersionSupported == null ) {
					string[] glslVersion = VersionInfo.Split(' ');
					if( !Version.TryParse(glslVersion[0], out m_VersionSupported) )
						m_VersionSupported = new Version("0.0");
				}
				return m_VersionSupported;
			}
		}
		
		protected int m_Id = 0;
		protected VertexShader m_VertexShader = null;
		protected FragmentShader m_FragmentShader = null;
		protected Version m_Version = null;
		
		public int Id { get { return m_Id; } }
		public VertexShader VertexShader { get { return m_VertexShader; } }
		public FragmentShader FragmentShader { get { return m_FragmentShader; } }
		public string InfoLog { get { return (m_Id == 0) ? null : GL.GetInfoLog(m_Id); } }
		public bool Disposed { get { return m_Id == 0; } }
		public Version Version { get { return m_Version; } }

		public List<ShaderAutoUniformInfo> StaticAutoUniforms = null;
		public List<ShaderAutoUniformInfo> DynamicAutoUniforms = null;
		
		protected Dictionary<string, int> m_UniformLocations = null;
		
		public object this[string uniformName] {
			set {
				if( m_Id == 0 )
					return;
				int uniformLocation;
				if( m_UniformLocations == null )
					m_UniformLocations = new Dictionary<string, int>();
				if( !m_UniformLocations.TryGetValue(uniformName, out uniformLocation) )
					m_UniformLocations[uniformName] = uniformLocation = GL.GetUniformLocation(m_Id, uniformName);
				if( uniformLocation < 0 )
					return;
				if( value is float ) GL.Uniform1(uniformLocation, (float)value);
				else if( value is Vector2 ) GL.Uniform2(uniformLocation, (Vector2)value);
				else if( value is Vector3 ) GL.Uniform3(uniformLocation, (Vector3)value);
				else if( value is Color4 ) GL.Uniform4(uniformLocation, (Color4)value);
				else if( value is Vector4 ) GL.Uniform4(uniformLocation, (Vector4)value);
				else if( value is int ) GL.Uniform1(uniformLocation, (int)value);
				else if( value is Point2 ) GL.Uniform2(uniformLocation, (Point2)value);
				else if( value is Point3 ) GL.Uniform3(uniformLocation, (Point3)value);
				else if( value is Point4 ) GL.Uniform4(uniformLocation, (Point4)value);
			}
		}
		
		public int GetAttribLocation(string attribName) {
			return (m_Id == 0) ? -1 : GL.GetAttribLocation(m_Id, attribName);
		}
		
		public Shader() {
			m_Id = GL.CreateProgramObject();
			if( m_Id == 0 )
				throw new UserFriendlyException("GL.CreateProgramObject() returned a zero result", "Could not create a shader");
		}
		
		public Shader(ShaderObject singleShaderProgram) : this() {
			Attach(singleShaderProgram);
			Link();
		}
		
		public Shader(ShaderObject[] shaderPrograms) : this() {
			foreach( ShaderObject obj in shaderPrograms )
				Attach(obj);
			Link();
		}
		
		public Shader(VertexShader vertexProgram, FragmentShader fragmentProgram) : this() {
			Attach(vertexProgram);
			Attach(fragmentProgram);
			Link();
		}
		
		public Shader(string vertexProgramSource, string fragmentProgramSource)
			: this(new VertexShader(vertexProgramSource), new FragmentShader(fragmentProgramSource)) {
		}
		
		public void Attach(ShaderObject shaderObj) {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			if( shaderObj.Disposed )
				throw new ObjectDisposedException(shaderObj.GetType().FullName);
			
			if( shaderObj is VertexShader )
				m_VertexShader = (VertexShader)shaderObj;
			else if( shaderObj is FragmentShader )
				m_FragmentShader = (FragmentShader)shaderObj;
			
			GL.AttachObject(m_Id, shaderObj.Id);
			
			if( m_UniformLocations != null )
				m_UniformLocations.Clear();
			
			// TODO: optimize so that there would not be more than one uniform with the same name in the list
			if( shaderObj.StaticAutoUniforms != null ) {
				if( StaticAutoUniforms == null )
					StaticAutoUniforms = new List<ShaderAutoUniformInfo>();
				StaticAutoUniforms.AddRange(shaderObj.StaticAutoUniforms);
			}
			if( shaderObj.DynamicAutoUniforms != null ) {
				if( DynamicAutoUniforms == null )
					DynamicAutoUniforms = new List<ShaderAutoUniformInfo>();
				DynamicAutoUniforms.AddRange(shaderObj.DynamicAutoUniforms);
			}
			if( m_Version == null || m_Version < shaderObj.Version )
				m_Version = shaderObj.Version;
		}
		
		public void Link() {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			GL.LinkProgram(m_Id);
			GL.UseProgramObject(m_Id);
			SetStaticAutoUniforms();
			GL.UseProgramObject(0);
		}
		
		public void SetStaticAutoUniforms() {
			if( StaticAutoUniforms != null )
				foreach( ShaderAutoUniformInfo aui in StaticAutoUniforms ) {
					// GameDebugger.Log("{0}={1}", aui.Name, aui.Value);
					this[aui.Name] = aui.Value;
				}
		}
		
		public void SetDynamicAutoUniforms() {
			if( DynamicAutoUniforms != null ) {
				foreach( ShaderAutoUniformInfo aui in DynamicAutoUniforms ) {
					switch( aui.Type ) {
						case ShaderAutoUniformClass.NonStopTime:
							// GameDebugger.Log("{0}={1}", aui.Name,  (float)((float)aui.Value * (float)GameEngine.Timer.Time));
							this[aui.Name] = (float)aui.Value * (float)Application.InternalTimer.Time;
							break;
						case ShaderAutoUniformClass.GameTime:
							// GameDebugger.Log("{0}={1}", aui.Name,  (float)((float)aui.Value * (float)GameEngine.Timer.Time));
							this[aui.Name] = (float)((float)aui.Value * (float)Application.Timer.Time);
							break;
						default:
							// GameDebugger.Log("{0}={1}", aui.Name, aui.Value);
							this[aui.Name] = aui.Value;
							break;
					}
				}
			}
		}
		
		public void Bind() {
			if( Disposed )
				throw new ObjectDisposedException(GetType().FullName);
			GL.UseProgramObject(m_Id);
			SetDynamicAutoUniforms();
		}
		
		public static void Unbind() {
			GL.UseProgramObject(0);
		}
		
		~Shader() {
			Dispose(false);
		}
		
		public void Dispose() {
			Dispose(true);
		}
		
		protected virtual void Dispose(bool manual) {
			if( m_Id != 0 ) {
				if( manual ) {
					if( m_Id != 0 )
						GL.DeleteObject(m_Id);
					if( m_VertexShader != null )
						m_VertexShader.Dispose();
					if( m_FragmentShader != null )
						m_FragmentShader.Dispose();
					m_VertexShader = null;
					m_FragmentShader = null;
					m_Id = 0;
				}
				else
					GameDebugger.Log(LogLevel.Warning, "One of shaders was not properly disposed");
			}
		}
		
		public static Shader Load(string fileName) {
			StructuredTextFile root = GameFile.LoadFile<StructuredTextFile>(fileName);
			return Load(root.Root, fileName);
		}
		
		public static Shader Load(DomNode node, string fileName) {
			foreach(DomAttribute attr in node.Attributes) {
				switch(attr.Name.ToLower()) {
					case "src": return Load(attr.Value.ToPath());
					case "relsrc": return Load(attr.Value.RelativeTo(fileName));
				}
			}

			List<ShaderObject> objectsToAttach = new List<ShaderObject>();
			foreach( DomNode child in node ) {
				switch( child.Name.ToLower() ) {
					case "shader":
						ShaderObject sobj = ShaderObject.Load(child, fileName);
						// GameDebugger.Log("Loaded {0}", sobj.GetType().Name);
						if( sobj == null ) {
							foreach( ShaderObject shaderObj in objectsToAttach )
								shaderObj.Dispose();
							return null; // shader program has a component that requires GLSL version that is higher than currently available or there was no code present
						}
						objectsToAttach.Add(sobj);
						break;
				}
			}
			
			Shader obj = new Shader();
			
			foreach( ShaderObject shaderObj in objectsToAttach )
				obj.Attach(shaderObj);
			obj.Link();
			
			return obj;
		}
	}
}
