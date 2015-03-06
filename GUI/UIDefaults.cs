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
using IGE.IO;
using IGE.Graphics;

namespace IGE.GUI {
	public class UIDefaults {
		public UIWindowDefaults Window;
		public UIScrollbarDefaults VScroll;
		public UIScrollbarDefaults HScroll;
		public UITextDefaults Label;
		public UIButtonDefaults Button;
		public UIMenuDefaults Menu;
		public UIMenuDefaults PopupMenu;
		
		public UIDefaults() {
			Window = new UIWindowDefaults();
			VScroll = new UIScrollbarDefaults();
			HScroll = new UIScrollbarDefaults();
			Label = new UITextDefaults();
			Button = new UIButtonDefaults();
			Menu = new UIMenuDefaults();
			PopupMenu = new UIMenuDefaults();
			
			Reset();
		}
		
		public UIDefaults(string fileName) : this() {
			Load(fileName);
		}
		
		public UIDefaults(string fileName, DomNode node) : this() {
			Load(fileName, node);
		}
		
		public void Reset() {
			Window.Background = new SolidBackground(Color4.FromBytes(64, 64, 64, 255));
			Window.Border = new ThinBorder(Color4.FromBytes(128, 128, 128, 255));
			
			VScroll.BarBackground = new SolidBackground(Color4.FromBytes(96, 96, 96, 255));
			VScroll.BoxBackground = new SolidBackground(Color4.FromBytes(128, 128, 128, 255));
			VScroll.BoxBorder = new ThinBorder(Color4.FromBytes(64, 64, 64, 255));
			
			HScroll.BarBackground = new SolidBackground(Color4.FromBytes(96, 96, 96, 255));
			HScroll.BoxBackground = new SolidBackground(Color4.FromBytes(128, 128, 128, 255));
			HScroll.BoxBorder = new ThinBorder(Color4.FromBytes(64, 64, 64, 255));
			
			Label.Color = Color4.White;
			Label.Font = Font.Cache(GameConfig.DefaultFont.ToPath());
			
			Button.Font = Font.Cache(GameConfig.DefaultFont.ToPath());
			
			Button.States.Normal.Background = new GradientBackground(false, Color4.FromBytes(120, 120, 120, 255), Color4.FromBytes(72, 72, 72, 255));
			Button.States.Normal.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			Button.States.Normal.Color = Color4.White;
			
			Button.States.Focus.Background = new GradientBackground(false, Color4.FromBytes(112, 112, 112, 255), Color4.FromBytes(80, 80, 80, 255));
			Button.States.Focus.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			Button.States.Focus.Color = Color4.White;
			
			Button.States.Hover.Background = new GradientBackground(false, Color4.FromBytes(104, 104, 104, 255), Color4.FromBytes(88, 88, 88, 255));
			Button.States.Hover.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			Button.States.Hover.Color = Color4.White;
			
			Button.States.MouseDown.Background = new GradientBackground(false, Color4.FromBytes(72, 72, 72, 255), Color4.FromBytes(120, 120, 120, 255));
			Button.States.MouseDown.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			Button.States.MouseDown.Color = Color4.White;
			
			Menu.Background = new SolidBackground(Color4.FromBytes(64, 64, 64, 255));
			Menu.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			Menu.Font = Font.Cache(GameConfig.DefaultFont.ToPath());
			
			Menu.MenuPadding.Left = 5;
			Menu.MenuPadding.Right = 5;
			Menu.MenuPadding.Top = 5;
			Menu.MenuPadding.Bottom = 5;
			
			Menu.ItemPadding.Left = 2;
			Menu.ItemPadding.Right = 2;
			Menu.ItemPadding.Top = 2;
			Menu.ItemPadding.Bottom = 2;
			
			Menu.ItemSpacing.Size = 20;
			Menu.ItemSpacing.Space = 2;
			
			Menu.ItemStates.Normal.Background = null;
			Menu.ItemStates.Normal.Border = null;
			Menu.ItemStates.Normal.Color = Color4.White;
			
			Menu.ItemStates.Focus.Background = new SolidBackground(Color4.FromBytes(72, 72, 72, 255));
			Menu.ItemStates.Focus.Border = new ThinBorder(Color4.FromBytes(80, 80, 80, 255));
			Menu.ItemStates.Focus.Color = Color4.White;
			
			Menu.ItemStates.Hover.Background = new SolidBackground(Color4.FromBytes(80, 80, 80, 255));
			Menu.ItemStates.Hover.Border = null;
			Menu.ItemStates.Hover.Color = Color4.White;
			
			Menu.ItemStates.MouseDown.Background = new SolidBackground(Color4.FromBytes(80, 80, 80, 255));
			Menu.ItemStates.MouseDown.Border = null;
			Menu.ItemStates.MouseDown.Color = Color4.White;
			
			PopupMenu.Background = new SolidBackground(Color4.FromBytes(64, 64, 64, 255));
			PopupMenu.Border = new ThinBorder(Color4.FromBytes(96, 96, 96, 255));
			PopupMenu.Font = Font.Cache(GameConfig.DefaultFont.ToPath());
			
			PopupMenu.MenuPadding.Left = 5;
			PopupMenu.MenuPadding.Right = 5;
			PopupMenu.MenuPadding.Top = 5;
			PopupMenu.MenuPadding.Bottom = 5;
			
			PopupMenu.ItemPadding.Left = 2;
			PopupMenu.ItemPadding.Right = 2;
			PopupMenu.ItemPadding.Top = 2;
			PopupMenu.ItemPadding.Bottom = 2;
			
			PopupMenu.ItemSpacing.Size = 20;
			PopupMenu.ItemSpacing.Space = 2;
			
			PopupMenu.ItemStates.Normal.Background = null;
			PopupMenu.ItemStates.Normal.Border = null;
			PopupMenu.ItemStates.Normal.Color = Color4.White;
			
			PopupMenu.ItemStates.Focus.Background = new SolidBackground(Color4.FromBytes(72, 72, 72, 255));
			PopupMenu.ItemStates.Focus.Border = new ThinBorder(Color4.FromBytes(80, 80, 80, 255));
			PopupMenu.ItemStates.Focus.Color = Color4.White;
			
			PopupMenu.ItemStates.Hover.Background = new SolidBackground(Color4.FromBytes(80, 80, 80, 255));
			PopupMenu.ItemStates.Hover.Border = null;
			PopupMenu.ItemStates.Hover.Color = Color4.White;
			
			PopupMenu.ItemStates.MouseDown.Background = new SolidBackground(Color4.FromBytes(80, 80, 80, 255));
			PopupMenu.ItemStates.MouseDown.Border = null;
			PopupMenu.ItemStates.MouseDown.Color = Color4.White;
		}
		
		public void Load(string fileName) {
			using( StructuredTextFile xml = GameFile.LoadFile<StructuredTextFile>(fileName) ) {
				Load(fileName, xml.Root);
			}
		}
		
		public void Load(string fileName, DomNode node) {
			Reset();
			
			string relName;
			foreach(DomNode child in node) {
				relName = String.Format("{0}~{1}", fileName, child.Name);
				switch( child.Name.ToLower() ) {
					case "window": Window.Load( relName, child ); break;
					case "vscroll": VScroll.Load( relName, child ); break;
					case "hscroll": HScroll.Load( relName, child ); break;
					case "label": Label.Load( relName, child ); break;
					case "button": Button.Load( relName, child ); break;
					case "menu": Menu.Load( relName, child ); break;
					case "popup-menu": PopupMenu.Load( relName, child ); break;
				}
			}
		}
		
		#region Basic resource loaders
		
		public static ISizedRenderable2D LoadBackground(string fileName, DomNode node) {
			switch( node["type"].ToLower() ) {
				case "complex-image": return new ComplexImage(fileName, node);
				case "gradient": return new GradientBackground(node);
				case "solid": return new SolidBackground(node);
			}
			return null; // no background if type is unknown
		}
		
		public static ISizedRenderable2D LoadBorder(string fileName, DomNode node) {
			switch( node["type"].ToLower() ) {
				case "thin": return new ThinBorder(node);
			}
			return null; // no border if type is unknown
		}
		
		public static Font LoadFont(string fileName, DomNode node) {
			Font font = null;
			if( !node["relSrc"].Equals("") )
				font = Font.Cache(node["relSrc"].RelativeTo(fileName));
			else if( !node["src"].Equals("") )
				font = Font.Cache(node["src"].ToPath());
			return (font == null) ? Font.Cache(GameConfig.DefaultFont.ToPath()) : font;
		}

		public static Color4 LoadColor(string fileName, DomNode node, Color4 defaultColor) {
			Color4 result;
			if( Color4.TryParse(node["color"], out result) )
				return result;
			return defaultColor;
		}
		
		#endregion Basic resource loaders
		public struct UIOffsetDefaults {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
			
			public readonly static UIOffsetDefaults Default = new UIOffsetDefaults {
				Left = 0,
				Top = 0,
				Right = 0,
				Bottom = 0
			};
			
			public void Load(string fileName, DomNode node) {
				Left = node["left"].ToInt32(0);
				Top = node["top"].ToInt32(0);
				Right = node["right"].ToInt32(0);
				Bottom = node["bottom"].ToInt32(0);
			}
		}
		
		public struct UISpacingDefaults {
			public int Size;
			public int Space;
			
			public readonly static UISpacingDefaults Default = new UISpacingDefaults {
				Size = 0,
				Space = 0
			};
			
			public void Load(string fileName, DomNode node) {
				Size = node["size"].ToInt32(0);
				Space = node["space"].ToInt32(0);
			}
		}
		
		public struct UIWindowDefaults {
			public ISizedRenderable2D Background;
			public ISizedRenderable2D Border;
			
			public readonly static UIWindowDefaults Default = new UIWindowDefaults {
				Background = null,
				Border = null
			};
			
			public void Load(string fileName, DomNode node) {
				foreach(DomNode child in node) {
					switch( child.Name.ToLower() ) {
						case "background": Background = UIDefaults.LoadBackground(fileName, child); break;
						case "border": Border = UIDefaults.LoadBorder(fileName, child); break;
					}
				}
			}
		}
		
		public struct UITextDefaults {
			public Color4 Color;
			public Font Font;
			
			public readonly static UITextDefaults Default = new UITextDefaults {
				Color = Color4.White,
				Font = null
			};
			
			public void Load(string fileName, DomNode node) {
				foreach(DomNode child in node) {
					switch( child.Name.ToLower() ) {
						case "color": Color = UIDefaults.LoadColor(fileName, child, Color4.White); break;
						case "font": Font = UIDefaults.LoadFont(fileName, child); break;
					}
				}
			}
		}
		
		public struct UIScrollbarDefaults {
			public ISizedRenderable2D BarBackground;
			public ISizedRenderable2D BoxBackground;
			public ISizedRenderable2D BoxBorder;
			
			public readonly static UIScrollbarDefaults Default = new UIScrollbarDefaults {
				BarBackground = null,
				BoxBackground = null,
				BoxBorder = null
			};
			
			public void Load(string fileName, DomNode node) {
				foreach(DomNode child in node) {
					switch( child.Name.ToLower() ) {
						case "bar-background": BarBackground = UIDefaults.LoadBackground(fileName, child); break;
						case "box-background": BoxBackground = UIDefaults.LoadBackground(fileName, child); break;
						case "box-border": BoxBorder = UIDefaults.LoadBorder(fileName, child); break;
					}
				}
			}
		}
		
		public struct UIComponentStateDefaults {
			public ISizedRenderable2D Background;
			public ISizedRenderable2D Border;
			public Color4 Color;
			
			public readonly static UIComponentStateDefaults Default = new UIComponentStateDefaults {
				Background = null,
				Border = null,
				Color = Color4.White
			};
			
			public void Load(string fileName, DomNode node) {
				foreach(DomNode child in node) {
					switch( child.Name.ToLower() ) {
						case "background": Background = UIDefaults.LoadBackground(fileName, child); break;
						case "border": Border = UIDefaults.LoadBorder(fileName, child); break;
						case "color": Color = UIDefaults.LoadColor(fileName, child, Color4.White); break;
					}
				}
			}
		}

		public struct UIComponentStateDefaultsCollection {
			public UIComponentStateDefaults Normal;
			public UIComponentStateDefaults Focus;
			public UIComponentStateDefaults Hover;
			public UIComponentStateDefaults MouseDown;
			
			public UIComponentStateDefaults this[UIComponentState state] {
				get {
					switch(state) {
						case UIComponentState.None: return Normal;
						case UIComponentState.Focus: return Focus;
						case UIComponentState.Hover: return Hover;
						case UIComponentState.MouseDown: return MouseDown;
					}
					return UIComponentStateDefaults.Default;
				}
				
				set {
					switch(state) {
						case UIComponentState.None: Normal = value; break;
						case UIComponentState.Focus: Focus = value; break;
						case UIComponentState.Hover: Hover = value; break;
						case UIComponentState.MouseDown: MouseDown = value; break;
					}
				}
			}
			
			public readonly static UIComponentStateDefaultsCollection Default = new UIComponentStateDefaultsCollection {
				Normal = UIComponentStateDefaults.Default,
				Focus = UIComponentStateDefaults.Default,
				Hover = UIComponentStateDefaults.Default,
				MouseDown = UIComponentStateDefaults.Default
			};
			
			public void Load(string fileName, DomNode node) {
				string relName;
				foreach(DomNode child in node) {
					relName = String.Format("{0}~{1}", fileName, child.Name);
					switch( child.Name.ToLower() ) {
						case "normal": Normal.Load(relName, child); break;
						case "focus": Focus.Load(relName, child); break;
						case "hover": Hover.Load(relName, child); break;
						case "mouse-down": MouseDown.Load(relName, child); break;
					}
				}
			}
		}
		
		public struct UIButtonDefaults {
			public UIComponentStateDefaultsCollection States;
			public Font Font;
			
			public readonly static UIButtonDefaults Default = new UIButtonDefaults {
				States = UIComponentStateDefaultsCollection.Default,
				Font = null
			};
			
			public void Load(string fileName, DomNode node) {
				string relName;
				foreach(DomNode child in node) {
					relName = String.Format("{0}~{1}", fileName, child.Name);
					switch( child.Name.ToLower() ) {
						case "states": States.Load(relName, child); break;
						case "font": Font = UIDefaults.LoadFont(fileName, child); break;
					}
				}
			}
		}
		
		public struct UIMenuDefaults {
			public ISizedRenderable2D Background;
			public ISizedRenderable2D Border;
			public UIOffsetDefaults MenuPadding;
			public UIOffsetDefaults ItemPadding;
			public UISpacingDefaults ItemSpacing;
			public ISizedRenderable2D Separator;
			public UIComponentStateDefaultsCollection ItemStates;
			public Font Font;

			public readonly static UIMenuDefaults Default = new UIMenuDefaults {
				Background = null,
				Border = null,
				MenuPadding = UIOffsetDefaults.Default,
				ItemPadding = UIOffsetDefaults.Default,
				ItemSpacing = UISpacingDefaults.Default,
				Separator = null,
				ItemStates = UIComponentStateDefaultsCollection.Default,
				Font = null
			};
			
			public void Load(string fileName, DomNode node) {
				foreach(DomNode child in node) {
					switch( child.Name.ToLower() ) {
						case "menu-background": Background = UIDefaults.LoadBackground(fileName, child); break;
						case "menu-border": Border = UIDefaults.LoadBorder(fileName, child); break;
						case "menu-padding": MenuPadding.Load(fileName, child); break;
						case "item-padding": ItemPadding.Load(fileName, child); break;
						case "item-spacing": ItemSpacing.Load(fileName, child); break;
						case "separator": Separator = UIDefaults.LoadBackground(fileName, child); break;
						case "item-states": ItemStates.Load(fileName, child); break;
						case "font": Font = UIDefaults.LoadFont(fileName, child); break;
					}
				}
			}
		}
	}
}
