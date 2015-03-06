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

using IGE;
using IGE.Platform;

namespace IGE.GUI {
	public class UI : Component {
		public static UIDefaults Defaults = null;
	
		public static Component Focus {
			get { return m_Focus; }
			set { Component.SetFocus(value); }
		}
	
		static UI() {
			Defaults = new UIDefaults();
			GameDebugger.EngineLog(LogLevel.Debug, "Trying to load UI defaults from '{0}'", GameConfig.UIDefaultsFile.ToPath());
			try {
				Defaults.Load(GameConfig.UIDefaultsFile.ToPath());
				GameDebugger.EngineLog(LogLevel.Debug, "UI defaults loaded successfully");
			}
			catch(FileNotFoundException) {
				GameDebugger.EngineLog(LogLevel.Debug, "UI defaults file is not found.");
				GameDebugger.EngineLog(LogLevel.Debug, "Using hardcoded UI defaults.");
			}
			catch(Exception ex) {
				GameDebugger.EngineLog(LogLevel.Debug, "Error while trying to load UI defaults: {0}", ex.ToString());
				GameDebugger.EngineLog(LogLevel.Debug, "Using hardcoded UI defaults.");
			}
		}
	
		public UI() {
			Area = new Rectangle(0, 0, Application.MainWindow.Width, Application.MainWindow.Height);
		}
		
		~UI() {
		}
		
		public override void SetDefaultStyle() {
		}
		
		public void Render() {
			Render(0, 0);
		}
		
		/// <summary>
		/// Handles input device types for which bit flags are enabled and
		/// in case if input was handled disables that bit. For instance if your
		/// game decides that UI should not handle mouse (mouse should not
		/// affect ui), then you can pass a variable with InputHandlingFlags.Mouse
		/// bit disabled. This way hovering, clicking and doing other actions with
		/// mouse over UI components will not have any effect, but if there is an
		/// "input" component that has a focus, it will capture keyboard button
		/// presses and disable the InputHandlingFlags.Keyboard bit in that variable.
		/// </summary>
		/// <param name="handlingFlags">Bit flags of input device types to do handling for. May disable bits in case if input was "intercepted".</param>
		public void HandleInput(ref InputHandlingFlags handlingFlags) {
			Component.HandleInput(this, ref handlingFlags);
		}
		
		/// <summary>
		/// Handles all input devices and returns flags with bits enabled for
		/// device types that were not handled. For instance if returned flags
		/// have InputHandlingFlags.Keyboard bit enabled, then it means that
		/// UI did not have any components that "intercepted" keyboard input
		/// and the game may safely use keyboard input for it's own controls.
		/// Otherwise it might result in conflict, like entering letter "w" at
		/// the same time as moving the character forward.
		/// </summary>
		/// <returns>Bit flags of unhandled input device types</returns>
		public InputHandlingFlags HandleInput() {
			InputHandlingFlags flags = InputHandlingFlags.All;
			HandleInput(ref flags);
			return flags;
		}
	}
}