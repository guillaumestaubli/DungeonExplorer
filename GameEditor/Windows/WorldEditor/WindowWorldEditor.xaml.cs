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

using GameLib.World;
using GameLib.World.Tiles;
using GameLib.World.Props;
using GameLib.Entities;

using GameEditor.Windows.Texture.AddTexture;
using GameEditor.Windows.WorldEditor.Toolbar.Entity;
using GameEditor.Windows.WorldEditor.Toolbar.Level;
using GameEditor.Windows.WorldEditor.Toolbar.Player;
using GameEditor.Windows.WorldEditor.Toolbar.Prop;
using GameEditor.Windows.WorldEditor.Toolbar.Tile;
using GameEditor.Windows.Texture.ViewTexture;

namespace GameEditor.Windows.WorldEditor
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
        private Stopwatch _renderAreaClock;
        private long _renderAreaClockElapsedMs = 0;
        private DisplaySettings _displaySettings = new();
        private Control? _currentToolbar = null;

        private bool _playerControl = false;

        /// <summary>
        /// Initialize a new instance of the world editor with a world to open
        /// </summary>
        /// <param name="path">Path of the .world file to open</param>
        public WindowWorldEditor(string path)
        {
            InitializeComponent();

            World.LoadFromFile(path);

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
            _renderArea.MouseButtonReleased += RenderArea_MouseButtonReleased;

            _renderAreaSize = new(_win32SfmlSurface.Size.Width, _win32SfmlSurface.Size.Height);
            _view = new View(new FloatRect(new Vector2f(0.0f, 0.0f), _renderAreaSize));

            _renderAreaClock = new();
            _renderAreaClock.Start();

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
            if (_renderArea == null)
                return;

            _renderArea.DispatchEvents();

            RenderArea_ProcessMouseAndKeyboardInputs();

            _renderAreaClockElapsedMs = _renderAreaClock.ElapsedMilliseconds;
            _renderAreaClock.Restart();

            World.RenderFrame(_renderAreaClockElapsedMs);

            _renderArea.Clear(new SFML.Graphics.Color(100, 100, 100));

            DrawGround(World.CurrentLevel);
            DrawProps(World.CurrentLevel);
            DrawPlayer(World.Player);

            _renderArea.Display();

            if (_playerControl && _currentToolbar is ControlPlayerToolbar)
                ((ControlPlayerToolbar)_currentToolbar).LoadPlayer(World.Player);

            return;
        }

        /// <summary>
        /// Draw only the visible part of the map to increase performances on large maps
        /// </summary>
        /// <param name="level">level to draw</param>
        private void DrawGround(Level level)
        {
            FloatRect viewBounds = new FloatRect(_viewPosition, _view.Size);

            for (int x = 0; x < level.GridSize.X; ++x)
            {
                for (int y = 0; y < level.GridSize.Y; ++y)
                {
                    Tile tile = level.Tiles[x, y];

                    FloatRect? intersection = tile.GlobalBounds.FindIntersection(viewBounds);

                    if (intersection == null)
                        continue;

                    if (_displaySettings.ShowTileTexture)
                        _renderArea.Draw(tile.D_Background, tile.BackgroundRenderStates);

                    if (_displaySettings.ShowTileBorder)
                        _renderArea.Draw(tile.D_Border);

                    if ((tile.Type == TileType.Wall || tile.Type == TileType.Door) && _displaySettings.ShowWallHitbox)
                        _renderArea.Draw(tile.D_Hitbox);

                    if (_displaySettings.ShowTileIndex)
                        _renderArea.Draw(tile.D_GridIndex);

                    if (_displaySettings.ShowTileType)
                        _renderArea.Draw(tile.D_TileType);
                }
            }
        }

        private void DrawProps(Level level)
        {
            FloatRect viewBounds = new FloatRect(_viewPosition, _view.Size);

            foreach (Prop prop in level.Props)
            {
                FloatRect? intersection = prop.GlobalBounds.FindIntersection(viewBounds);

                if (intersection == null)
                    continue;

                if (_displaySettings.ShowProps)
                    _renderArea.Draw(prop.D_Prop, prop.D_Prop_RenderStates);

                if (_displaySettings.ShowPropsRectangleHitbox)
                    _renderArea.Draw(prop.D_RectBorders);

                if (_displaySettings.ShowPropsPixelPerfectHitbox)
                    _renderArea.Draw(prop.D_PixelPerfectBorders);
            }
        }

        private void DrawPlayer(Player player)
        {
            FloatRect viewBounds = new FloatRect(_viewPosition, _view.Size);

            FloatRect? intersection = player.GlobalBounds.FindIntersection(viewBounds);

            if (intersection == null)
                return;

            if (_displaySettings.ShowPlayer)
                _renderArea.Draw(player.D_Player, player.D_Player_RenderStates);

            if (_displaySettings.ShowPlayerRectangleHitbox)
                _renderArea.Draw(player.D_RectBounds);

            if (_displaySettings.ShowPlayerPixelPerfectHitbox)
                _renderArea.Draw(player.D_PixelPerfectBounds);

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

            bool LCTRL_Pressed = SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.LControl);
            bool LMouse_Pressed = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left);

            RenderArea_HandleDragMove(LCTRL_Pressed, LMouse_Pressed);

            if (_playerControl)
            {
                World.MovePlayer();
                CenterViewOnPlayer();
            }
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

                    delta = (Vector2i)((Vector2f)delta * _viewZoomFactor);

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

        /// <summary>
        /// Handle the MouseButtonReleased event of the SFML render area depending of the current toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderArea_MouseButtonReleased(object? sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (_currentToolbar == null)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || _dragMove_Running)
                return;

            if (_currentToolbar is DrawTileToolbar)
            {
                DrawSelectedTextureOnClickedTile(e);
                return;
            }

            if (_currentToolbar is AddPropToolbar)
            {
                DrawSelectedPropOnClickedTile(e);
                return;
            }

            if (_currentToolbar is ViewTileToolbar)
            {
                LoadSelectedTileOnTileDetailsToolbar(e);
                return;
            }

            if (_currentToolbar is ViewPropToolbar)
            {
                LoadSelectedPropOnPropDetailsToolbar(e);
                return;
            }
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
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.S))
            {
                GameLib.Files.FilesManager.SaveProject();
            }
        }



        #endregion

        #region NAV_TAB_PROJECT

        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            GameLib.Files.FilesManager.SaveProject();
        }

        private void Btn_WorldMap_Click(object sender, RoutedEventArgs e)
        {
            WindowWorldMap windowWorldMap = new();

            _ = windowWorldMap.ShowDialog();
        }

        private void ComboBox_AddTexture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBox_AddTexture.SelectedItem == null)
                return;

            ComboBoxItem item = (ComboBoxItem)this.ComboBox_AddTexture.SelectedItem;

            Window? window = null;

            switch (item.Tag.ToString())
            {
                case "TILE":
                    window = new WindowAddTileTexture();
                    break;
                case "PROP":
                    window = new WindowAddPropTexture();
                    break;
                case "PLAYER":
                    window = new WindowAddPlayerTexture();
                    break;
                default:
                    window = null;
                    break;
            }

            if (window != null)
                _ = window.ShowDialog();

            this.ComboBox_AddTexture.SelectedIndex = 0;
        }

        private void ComboBox_ViewTextures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ComboBox_AddTexture.SelectedItem == null)
                return;

            ComboBoxItem item = (ComboBoxItem)this.ComboBox_ViewTextures.SelectedItem;

            Window? window = null;

            switch (item.Tag.ToString())
            {
                case "TILE":
                    window = new WindowViewTileTextures();
                    break;
                case "PROP":
                    window = new WindowViewPropTextures();
                    break;
                case "PLAYER":
                    window = new WindowViewPlayerTextures();
                    break;
                default:
                    window = null;
                    break;
            }

            if (window != null)
                _ = window.ShowDialog();

            this.ComboBox_AddTexture.SelectedIndex = 0;
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
            int[] itemsPerPanel = [5, 3, 4, 4];
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

        #region NAV_TAB_LEVEL

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

        private void Btn_ViewLevel_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new ViewLevelToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        #endregion

        #region NAV_TAB_TILE

        /// <summary>
        /// Handle the Click event on Btn_DrawWorld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DrawTile_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new DrawTileToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        /// <summary>
        /// Handle the Click event on Btn_InspectTile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_InspectTile_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new ViewTileToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        /// <summary>
        /// Handle the Click event on Btn_EditTile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_EditTile_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new EditTileToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        #endregion

        #region NAV_TAB_PROPS

        private void Btn_AddProp_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new AddPropToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        private void Btn_InspectProp_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new ViewPropToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        private void Btn_EditProp_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new EditPropToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        #endregion

        #region NAV_TAB_ENTITIES

        private void Btn_AddEntity_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new AddEntityToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        private void Btn_InspectEntity_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new ViewEntityToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        private void Btn_EditEntity_Click(object sender, RoutedEventArgs e)
        {
            _currentToolbar = new EditEntityToolbar();

            this.ToolbarContainer.Child = _currentToolbar;
        }

        #endregion

        #region NAV_TAB_PLAYER

        private void Btn_ViewPlayer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_EditPlayer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ControlPlayer_Click(object sender, RoutedEventArgs e)
        {
            _playerControl = !_playerControl;

            if (!_playerControl)
                return;

            _currentToolbar = new ControlPlayerToolbar();
            this.ToolbarContainer.Child = _currentToolbar;
            ((ControlPlayerToolbar)_currentToolbar).LoadPlayer(World.Player);

            CenterViewOnPlayer();
        }

        #endregion


        /// <summary>
        /// Draw the selected texture on the clicked tile if a texture is selected
        /// </summary>
        /// <param name="e">EventArgs of the base event</param>
        private void DrawSelectedTextureOnClickedTile(SFML.Window.MouseButtonEventArgs e)
        {
            if (_currentToolbar == null)
                return;

            DrawTileToolbar toolbar = (DrawTileToolbar)_currentToolbar;

            if (toolbar.CurrentTileTexture == null)
                return;

            if (toolbar.CurrentTileType == null)
                return;

            Vector2i gridIndex = World.ConvertWorldCoordsToGridIndex(_renderArea.MapPixelToCoords(e.Position));

            Tile tile = World.GetTile(gridIndex);

            tile.TileTexture = toolbar.CurrentTileTexture;
            tile.Type = (TileType)toolbar.CurrentTileType;

            World.UpdateTile(tile, gridIndex);
        }

        /// <summary>
        /// Add the selected prop on the clicked tile if a prop is selected
        /// </summary>
        /// <param name="e">EventArgs of the base event</param>
        private void DrawSelectedPropOnClickedTile(SFML.Window.MouseButtonEventArgs e)
        {
            if (_currentToolbar == null)
                return;

            AddPropToolbar toolbar = (AddPropToolbar)_currentToolbar;

            if (toolbar.CurrentPropTexture == null)
                return;

            Vector2i gridIndex = World.ConvertWorldCoordsToGridIndex(_renderArea.MapPixelToCoords(e.Position));

            Prop prop = new(gridIndex, World.CurrentLevel.TileRenderSize, toolbar.CurrentPropTexture);

            World.AddProp(prop);
        }

        private void LoadSelectedTileOnTileDetailsToolbar(SFML.Window.MouseButtonEventArgs e)
        {
            if (_currentToolbar == null)
                return;

            Vector2i gridIndex = World.ConvertWorldCoordsToGridIndex(_renderArea.MapPixelToCoords(e.Position));

            Tile tile = World.GetTile(gridIndex);

            ((ViewTileToolbar)_currentToolbar).LoadTile(tile);
        }

        private void LoadSelectedPropOnPropDetailsToolbar(SFML.Window.MouseButtonEventArgs e)
        {
            if (_currentToolbar == null)
                return;

            Vector2i gridIndex = World.ConvertWorldCoordsToGridIndex(_renderArea.MapPixelToCoords(e.Position));

            Prop? prop = World.GetProp(gridIndex);

            if (prop == null)
                return;

            ((ViewPropToolbar)_currentToolbar).LoadProp(prop);
        }

        private void CenterViewOnPlayer()
        {
            _view.Center = World.Player.RenderPosition;
            _renderArea.SetView(_view);
        }


    }
}
