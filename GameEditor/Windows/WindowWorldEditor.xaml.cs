using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SFML.System;
using SFML.Graphics;

using GameLib;
using GameLib.World;
using GameLib.Core;
using GameLib.World.Tiles;

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowWorldEditor.xaml
    /// </summary>
    public partial class WindowWorldEditor : Window
    {
        private Vector2f _viewPosition { get { return _view.Center - _view.Size / 2.0f; } }
        private Vector2f _renderAreaSize = new(0.0f, 0.0f);
        private View _view;
        private float _viewZoomFactor = 1.0f;
        private readonly RenderWindow _renderArea;
        private readonly System.Windows.Forms.Control _win32SfmlSurface;

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

                _renderArea.Clear(SFML.Graphics.Color.Black);

                DrawGround(Dungeon.CurrentLevel);

                DrawDebug();

                _renderArea.Display();
            }

            return;
        }

        #region DEBUG

        private void DrawDebug()
        {
            CircleShape cs_viewCenter = new(5.0f, 32)
            {
                Origin = new Vector2f(5.0f, 5.0f),
                Position = _view.Center,
                FillColor = SFML.Graphics.Color.Magenta
            };

            _renderArea.Draw(cs_viewCenter);

            CircleShape cs_topLeftCorner = new(5.0f, 32)
            {
                Origin = new Vector2f(5.0f, 5.0f),
                Position = _viewPosition,
                FillColor = SFML.Graphics.Color.Blue
            };

            _renderArea.Draw(cs_topLeftCorner);

        }

        #endregion

        #region EVENTS_RENDER_AREA

        private void Win32SfmlSurface_MouseEnter(object? sender, EventArgs e)
        {
            _isCursorInRenderArea = true;
        }

        private void Win32SfmlSurface_MouseLeave(object? sender, EventArgs e)
        {
            _isCursorInRenderArea = false;
        }

        private bool _isCursorInRenderArea = false;
        private bool _dragMove_Running = false;
        private Vector2i _dragMove_LastMousePosition = new Vector2i(-1, -1);
        private void RenderArea_ProcessMouseAndKeyboardInputs()
        {
            if (!_isCursorInRenderArea)
                return;

            bool isLCtrlPressed = SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.LControl);
            bool isLeftMouseDown = SFML.Window.Mouse.IsButtonPressed(SFML.Window.Mouse.Button.Left);

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

        private void RenderArea_MouseButtonPressed(object? sender, SFML.Window.MouseButtonEventArgs e)
        {
            return;
        }

        private void RenderArea_MouseButtonReleased(object? sender, SFML.Window.MouseButtonEventArgs e)
        {
            return;
        }

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

        #endregion

        #region EVENTS_WPF_WINDOW

        private void WindowWorldEditor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            return;
        }

        private void WindowWorldEditor_Closing(object? sender, CancelEventArgs e)
        {
            return;
        }

        #endregion

        #region EVENTS_WINDOW_CONTROLS

        /// <summary>
        /// Occurs when the render area is resized in the window
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
                        _renderArea.Draw(tile.D_Background);
                        _renderArea.Draw(tile.D_Border);
                        _renderArea.Draw(tile.D_GridIndex);
                    }
                }
            }
        }
    }
}
