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

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowWorldEditor.xaml
    /// </summary>
    public partial class WindowWorldEditor : Window
    {
        private const float ZOOM_FACTOR = 0.1f;

        private float _currentZoom = 1.0f;
        private Vector2f _viewPosition = new(0.0f, 0.0f);
        private Vector2f _renderAreaSize = new(0.0f, 0.0f);
        private View _view;
        private readonly RenderWindow _renderArea;
        private readonly System.Windows.Forms.Control _win32SfmlSurface;

        /// <summary>
        /// 
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
            _win32SfmlSurface.MouseWheel += RenderArea_MouseWheel;

            this.SfmlHost.Child = _win32SfmlSurface;

            _renderArea = new RenderWindow(_win32SfmlSurface.Handle);
            _renderArea.SetVerticalSyncEnabled(true);

            _renderAreaSize = new(_win32SfmlSurface.Size.Width, _win32SfmlSurface.Size.Height);
            _viewPosition = new(0.0f, 0.0f);
            _view = new View(new FloatRect(_viewPosition, _renderAreaSize));

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            this.ScrlBar_SfmlRenderAreaHorizontal.LargeChange = Dungeon.CurrentLevel.TileRenderSize.X;
            this.ScrlBar_SfmlRenderAreaHorizontal.SmallChange = Dungeon.CurrentLevel.TileRenderSize.X;

            this.ScrlBar_SfmlRenderAreaVertical.LargeChange = Dungeon.CurrentLevel.TileRenderSize.X;
            this.ScrlBar_SfmlRenderAreaVertical.SmallChange = Dungeon.CurrentLevel.TileRenderSize.X;
        }

        #region EVENTS

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

                _renderArea.Clear(SFML.Graphics.Color.Black);

                DrawGround(Dungeon.CurrentLevel);

                _renderArea.Display();
            }

            return;
        }

        /// <summary>
        /// Occurs when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowWorldEditor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRenderAreaScrollBars();

            return;
        }

        /// <summary>
        /// Occurs before the window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowWorldEditor_Closing(object? sender, CancelEventArgs e)
        {
            return;
        }

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
            _view.Zoom(_currentZoom);

            _renderArea.SetView(_view);

            UpdateRenderAreaScrollBars();

            return;
        }

        /// <summary>
        /// Occurs when the user scrolls in the render area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrlBar_SfmlRenderAreaVertical_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Vector2f delta = new()
            {
                X = 0.0f,
                Y = (float)this.ScrlBar_SfmlRenderAreaVertical.Value - _viewPosition.Y
            };

            _view.Move(delta);
            _viewPosition += delta;

            _renderArea.SetView(_view);
        }

        /// <summary>
        /// Occurs when the user scrolls in the render area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrlBar_SfmlRenderAreaHorizontal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Vector2f delta = new()
            {
                X = (float)this.ScrlBar_SfmlRenderAreaHorizontal.Value - _viewPosition.X,
                Y = 0.0f
            };

            _view.Move(delta);
            _viewPosition += delta;

            _renderArea.SetView(_view);
        }

        /// <summary>
        /// Handles the mouse scroll in the render area :
        /// > zoom in/out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderArea_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            // For zoom in/out : MouseWheel + LeftCtrl
            if (e.Button == System.Windows.Forms.MouseButtons.None && e.Delta != 0 && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta < 0) // zoom -
                {
                    _currentZoom += ZOOM_FACTOR;
                }
                else if (e.Delta > 0) // zoom +
                {
                    // prevent the zoom inversion when the float value goes beyond the maximum of decimals of a float
                    if (_currentZoom - ZOOM_FACTOR >= 0.1f)
                        _currentZoom -= ZOOM_FACTOR;
                }

                _view = new View(new FloatRect(_viewPosition, _renderAreaSize));
                _view.Zoom(_currentZoom);
                _renderArea.SetView(_view);

                UpdateRenderAreaScrollBars();
            }
        }

        #endregion

        /// <summary>
        /// Update the min/max values of the render area scrollbars so that they are still correct when the area is resized or the user zoom (in/out) in the render area
        /// </summary>
        private void UpdateRenderAreaScrollBars()
        {
            float d_hor = _renderAreaSize.X - _view.Size.X;
            float d_ver = _renderAreaSize.Y - _view.Size.Y;

            float hor_min = -1.0f * d_hor / 2.0f;
            float hor_max = Dungeon.CurrentLevel.RenderSize.X - _renderAreaSize.X + (d_hor / 2.0f);

            float vert_min = -1.0f * d_ver / 2.0f;
            float vert_max = Dungeon.CurrentLevel.RenderSize.Y - _renderAreaSize.Y + (d_ver / 2.0f);

            this.ScrlBar_SfmlRenderAreaHorizontal.Minimum = hor_min;
            this.ScrlBar_SfmlRenderAreaHorizontal.Maximum = hor_max;

            this.ScrlBar_SfmlRenderAreaVertical.Minimum = vert_min;
            this.ScrlBar_SfmlRenderAreaVertical.Maximum = vert_max;

            double p = 0.25;
            this.ScrlBar_SfmlRenderAreaHorizontal.ViewportSize = ScrlBar_SfmlRenderAreaHorizontal.Maximum * p / (1.0 - p);
            this.ScrlBar_SfmlRenderAreaVertical.ViewportSize = ScrlBar_SfmlRenderAreaVertical.Maximum * p / (1.0 - p);
        }

        /// <summary>
        /// Draw only the visible part of the map to increase performances on large maps
        /// </summary>
        /// <param name="level">level to draw</param>
        private void DrawGround(DungeonLevel level)
        {
            int x_min = (int)(_viewPosition.X / level.TileRenderSize.X);
            int x_max = (int)((_viewPosition.X + _view.Size.X) / level.TileRenderSize.X) + 1;

            if (x_min < 0)
                x_min = 0;

            if (x_max >= level.GridSize.X)
                x_max = level.GridSize.X;

            int y_min = (int)(_viewPosition.Y / level.TileRenderSize.Y);
            int y_max = (int)((_viewPosition.Y + _view.Size.Y) / level.TileRenderSize.Y) + 1;

            if (y_min < 0)
                y_min = 0;

            if (y_max >= level.GridSize.Y)
                y_max = level.GridSize.Y;

            for (int x = x_min; x < x_max; ++x)
            {
                for (int y = y_min; y < y_max; ++y)
                {
                    foreach (IDrawable drawable in level.Tiles[x, y].Drawables)
                    {
                        _renderArea.Draw(drawable);
                    }
                }
            }
        }
    }
}
