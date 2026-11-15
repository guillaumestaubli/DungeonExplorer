using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;

using SFML.System;
using SFML.Graphics;

using GameLib;
using GameLib.World;
using GameLib.Core;
using GameLib.World.Tiles;
using GameEditor.Windows.Controls;

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowWorldEditor.xaml
    /// </summary>
    public partial class WindowWorldEditor : Window
    {
        private bool _isCursorInRenderArea = false;
        private bool _dragMove_Running = false;
        private float _viewZoomFactor = 1.0f;
        private Vector2i _dragMove_LastMousePosition = new Vector2i(-1, -1);
        private Vector2f _viewPosition { get { return _view.Center - _view.Size / 2.0f; } }
        private Vector2f _renderAreaSize = new(0.0f, 0.0f);
        private View _view;
        private RenderWindow _renderArea;
        private System.Windows.Forms.Control _win32SfmlSurface;
        private DisplaySettings _displaySettings = new();
        private Control? _currentToolbar = null;

        /// <summary>
        /// Initialize a new instance of the world editor with a world to open
        /// </summary>
        /// <param name="path">Path of the .dungeon file to open</param>
        public WindowWorldEditor(string path)
        {
            InitializeComponent();

            Dungeon.LoadFromFile(path);

            this.Loaded += WindowWorldEditor_Loaded;
            this.Closing += WindowWorldEditor_Closing;
            this.KeyDown += WindowWorldEditor_KeyDown;

            _win32SfmlSurface = new System.Windows.Forms.Control();
            _win32SfmlSurface.Resize += Win32SfmlSurface_Resize;
            _win32SfmlSurface.MouseEnter += Win32SfmlSurface_MouseEnter;
            _win32SfmlSurface.MouseLeave += Win32SfmlSurface_MouseLeave;

            this.SfmlHost.Child = _win32SfmlSurface;

            _renderArea = new RenderWindow(_win32SfmlSurface.Handle);
            _renderArea.SetFramerateLimit(0);
            _renderArea.SetVerticalSyncEnabled(true);
            _renderArea.SetKeyRepeatEnabled(true);
            _renderArea.MouseWheelScrolled += RenderArea_MouseWheelScrolled;

            _renderAreaSize = new(_win32SfmlSurface.Size.Width, _win32SfmlSurface.Size.Height);
            _view = new View(new FloatRect(new Vector2f(0.0f, 0.0f), _renderAreaSize));

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        #region RENDERING

        /// <summary>
        /// Called each time the window is drawn (equivalent to a frame)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (_renderArea != null)
            {
                _renderArea.DispatchEvents();

                RenderArea_ProcessMouseAndKeyboardInputs();

                _renderArea.Clear(SFML.Graphics.Color.White);

                DrawGround(Dungeon.CurrentLevel);

                _renderArea.Display();
            }

            return;
        }

        /// <summary>
        /// Draw only the visible part of the map to increase performances on large maps
        /// </summary>
        /// <param name="level">level to draw</param>
        private void DrawGround(DungeonLevel level)
        {
            FloatRect viewBounds = new FloatRect(_viewPosition, _view.Size);

            for (int x = 0; x < level.GridSize.X; ++x)
            {
                for (int y = 0; y < level.GridSize.Y; ++y)
                {
                    Tile tile = level.Tiles[x, y];

                    FloatRect? intersection = tile.GlobalBounds.FindIntersection(viewBounds);

                    if (intersection != null)
                    {
                        if (_displaySettings.ShowTileTexture)
                            _renderArea.Draw(tile.D_Background, tile.BackgroundRenderStates);

                        if (_displaySettings.ShowTileBorder)
                            _renderArea.Draw(tile.D_Border);

                        if (_displaySettings.ShowTileIndex)
                            _renderArea.Draw(tile.D_GridIndex);
                    }
                }
            }
        }

        #endregion

        #region SFML_RENDER_AREA

        /// <summary>
        /// Handle the MouseEnter event on the SFML render area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win32SfmlSurface_MouseEnter(object? sender, EventArgs e)
        {
            _isCursorInRenderArea = true;
        }

        /// <summary>
        /// Handle the MouseLeave event on the SFML render area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win32SfmlSurface_MouseLeave(object? sender, EventArgs e)
        {
            _isCursorInRenderArea = false;
        }

        /// <summary>
        /// Process the user inputs on the keyboard and the mouse
        /// </summary>
        private void RenderArea_ProcessMouseAndKeyboardInputs()
        {
            if (!_isCursorInRenderArea)
                return;

            bool isLCtrlPressed = SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.LControl);
            bool isLeftMouseDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left);

            RenderArea_HandleDragMove(isLCtrlPressed, isLeftMouseDown);
        }

        /// <summary>
        /// Handle the drag-move action on the world map
        /// </summary>
        /// <param name="isLCtrlPressed">Left Ctrl pressed</param>
        /// <param name="isLeftMouseDown">Left Button pressed</param>
        private void RenderArea_HandleDragMove(bool isLCtrlPressed, bool isLeftMouseDown)
        {
            if (isLCtrlPressed && isLeftMouseDown)
            {
                _win32SfmlSurface.Cursor = System.Windows.Forms.Cursors.Hand;

                Vector2i currentMousePosition = SFML.Window.Mouse.GetPosition(_renderArea);

                if (!_dragMove_Running)
                {
                    _dragMove_Running = true;
                    _dragMove_LastMousePosition = currentMousePosition;
                }
                else
                {
                    Vector2i delta = currentMousePosition - _dragMove_LastMousePosition;

                    _view.Move((Vector2f)delta * -1.0f);
                    _renderArea.SetView(_view);

                    _dragMove_LastMousePosition = currentMousePosition;
                }
            }
            else
            {
                _win32SfmlSurface.Cursor = System.Windows.Forms.Cursors.Default;

                _dragMove_Running = false;
            }
        }

        /// <summary>
        /// Handle the MouseWheelScrolled on the SFML render area.
        /// LCtrl + Scroll => Zoom in/out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderArea_MouseWheelScrolled(object? sender, SFML.Window.MouseWheelScrollEventArgs e)
        {
            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.LControl))
            {
                if (e.Delta < 0) // zoom out
                {
                    _view.Zoom(GameEditorConfig.ZOOM_OUT_FACTOR);
                    _viewZoomFactor *= GameEditorConfig.ZOOM_OUT_FACTOR;
                }
                else if (e.Delta > 0) // zoom in
                {
                    _view.Zoom(GameEditorConfig.ZOOM_IN_FACTOR);
                    _viewZoomFactor *= GameEditorConfig.ZOOM_IN_FACTOR;
                }

                _renderArea.SetView(_view);
            }

            return;
        }

        /// <summary>
        /// Handle the resize of the SFML render area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win32SfmlSurface_Resize(object? sender, EventArgs e)
        {
            _renderArea.Size = new Vector2u((uint)_win32SfmlSurface.Size.Width, (uint)_win32SfmlSurface.Size.Height);

            _renderAreaSize = new Vector2f(_win32SfmlSurface.Width, _win32SfmlSurface.Height);

            _view = new View(new FloatRect(_viewPosition, _renderAreaSize));
            _view.Zoom(_viewZoomFactor);
            _renderArea.SetView(_view);

            return;
        }

        #endregion

        #region WINDOW_EVENTS

        /// <summary>
        /// Handle the Loaded event.
        /// Initialize the display tab of the nav.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowWorldEditor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeDisplayMenu();

            return;
        }

        /// <summary>
        /// Handle the Closing event.
        /// Ask the user if he wants to save before quitting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowWorldEditor_Closing(object? sender, CancelEventArgs e)
        {


            return;
        }

        /// <summary>
        /// Handle the KeyDown event.
        /// LCtrl + S => Save project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowWorldEditor_KeyDown(object sender, KeyEventArgs e)
        {
            // LCtrl + S => Save
            if(Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
            {
                GameLib.Files.FilesManager.SaveProject();
            }
        }

        

        #endregion

        #region NAV_TAB_PROJECT

        /// <summary>
        /// Handle the Click event on Btn_Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            GameLib.Files.FilesManager.SaveProject();
        }

        /// <summary>
        /// Handle the Click event on Btn_AddTexture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddTextures_Click(object sender, RoutedEventArgs e)
        {
            WindowAddRessource windowAddRessource = new WindowAddRessource();

            _ = windowAddRessource.ShowDialog();
        }

        /// <summary>
        /// Handle the Click event on Btn_InitializeTiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_InitializeTiles_Click(object sender, RoutedEventArgs e)
        {
            WindowInitializeTiles windowInitializeTiles = new WindowInitializeTiles();

            _ = windowInitializeTiles.ShowDialog();
        }

        #endregion

        #region NAV_TAB_DISPLAY

        /// <summary>
        /// Handle the Checked event on the CheckBoxes of the display tab of the navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkBox_DisplayMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not CheckBox)
                return;

            CheckBox checkBox = (CheckBox)sender;

            Type displaySettingsType = _displaySettings.GetType();

            PropertyInfo[] properties = displaySettingsType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? propertyValue = property.GetValue(_displaySettings);

                if (propertyValue == null)
                    continue;

                if (property.Name == checkBox.Name)
                {
                    property.SetValue(_displaySettings, true);
                }

            }
        }

        /// <summary>
        /// Handle the Unchecked event on the CheckBoxes of the display tab of the navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkBox_DisplayMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not CheckBox)
                return;

            CheckBox checkBox = (CheckBox)sender;

            Type displaySettingsType = _displaySettings.GetType();

            PropertyInfo[] properties = displaySettingsType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? propertyValue = property.GetValue(_displaySettings);

                if (propertyValue == null)
                    continue;

                if (property.Name == checkBox.Name)
                {
                    property.SetValue(_displaySettings, false);
                }

            }
        }

        /// <summary>
        /// Create checkboxes for the display settings using System.Reflection.
        /// Must update itemsPerPanel if any setting is added in DisplaySettings class
        /// </summary>
        private void InitializeDisplayMenu()
        {
            StackPanel[] panels = [StckPanel_DisplayTile, StckPanel_DisplayProps, StckPanel_DisplayEntities, StckPanel_DisplayPlayer];
            int[] itemsPerPanel = [5, 3, 4, 3];
            int panelIndex = 0;
            int itemCounter = 0;

            Type displaySettingsType = _displaySettings.GetType();

            PropertyInfo[] properties = displaySettingsType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object? propertyValue = property.GetValue(_displaySettings);

                if (propertyValue == null)
                    continue;

                DisplaySettingDisplayName? propertyDisplayName = property.GetCustomAttribute<DisplaySettingDisplayName>();

                CheckBox checkBox = new()
                {
                    Name = property.Name,
                    Content = propertyDisplayName == null ? property.Name : propertyDisplayName.DisplayName,
                    IsChecked = (bool)propertyValue
                };

                checkBox.Checked += ChkBox_DisplayMenu_Checked;
                checkBox.Unchecked += ChkBox_DisplayMenu_Unchecked;

                panels[panelIndex].Children.Add(checkBox);

                itemCounter++;

                if (itemCounter >= itemsPerPanel[panelIndex])
                {
                    panelIndex++;
                    itemCounter = 0;
                }
            }
        }

        #endregion

        #region NAV_TAB_WORLD

        /// <summary>
        /// Handle the Click event on Btn_EditWorldSize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_EditWorldSize_Click(object sender, RoutedEventArgs e)
        {
            string text = "Not implemented";
            string caption = "DungeonExplorer - GameEditor - Info";

            _ = MessageBox.Show(text, caption, MessageBoxButton.OK);
        }

        /// <summary>
        /// Handle the Click event on Btn_DrawWorld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DrawWorld_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new ControlDrawWorldToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        #endregion

        #region NAV_TAB_TILE



        #endregion

        #region NAV_TAB_PROPS



        #endregion

        #region NAV_TAB_ENTITIES



        #endregion

        #region NAV_TAB_PLAYER



        #endregion

        

    }

    public class DisplaySettings
    {
        [DisplaySettingDisplayName("Show Tile Border")]
        public bool ShowTileBorder { get; set; } = true;

        [DisplaySettingDisplayName("Show Tile Index")]
        public bool ShowTileIndex { get; set; } = true;

        [DisplaySettingDisplayName("Show Tile Type")]
        public bool ShowTileType { get; set; } = false;

        [DisplaySettingDisplayName("Show Tile Texture")]
        public bool ShowTileTexture { get; set; } = true;

        [DisplaySettingDisplayName("Show Wall Hitbox")]
        public bool ShowWallHitbox { get; set; } = false;

        [DisplaySettingDisplayName("Show Props")]
        public bool ShowProps { get; set; } = true;
        [DisplaySettingDisplayName("Show Props Rectangle Hitbox")]
        public bool ShowPropsRectangleHitbox { get; set; } = false;
        [DisplaySettingDisplayName("Show Props Pixel Perfect Hitbox")]
        public bool ShowPropsPixelPerfectHitbox { get; set; } = false;

        [DisplaySettingDisplayName("Show Entities")]
        public bool ShowEntities { get; set; } = true;
        [DisplaySettingDisplayName("Show Entities Rectangle Hitbox")]
        public bool ShowEntitiesRectangleHitbox { get; set; } = false;
        [DisplaySettingDisplayName("Show Entities Pixel Perfect Hitbox")]
        public bool ShowEntitiesPixelPerfectHitbox { get; set; } = false;
        [DisplaySettingDisplayName("Show Entities Paths")]
        public bool ShowEntitiesPaths { get; set; } = false;

        [DisplaySettingDisplayName("Show Player")]
        public bool ShowPlayer { get; set; } = true;
        [DisplaySettingDisplayName("Show Player Rectangle Hitbox")]
        public bool ShowPlayerRectangleHitbox { get; set; } = false;
        [DisplaySettingDisplayName("Show Player Pixel Perfect Hitbox")]
        public bool ShowPlayerPixelPerfectHitbox { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class DisplaySettingDisplayName : Attribute
    {
        public string DisplayName { get; }

        public DisplaySettingDisplayName(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
