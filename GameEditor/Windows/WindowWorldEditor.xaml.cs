using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
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

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowWorldEditor.xaml
    /// </summary>
    public partial class WindowWorldEditor : Window
    {
        private Vector2f _viewPosition = new Vector2f(0.0f, 0.0f);
        private Vector2f _viewSize = new Vector2f(0.0f, 0.0f);
        private View _view;
        private RenderWindow _renderWindow;
        private System.Windows.Forms.Control _win32SfmlSurface;

        public WindowWorldEditor()
        {
            InitializeComponent();

            this.Loaded += WindowWorldEditor_Loaded;
            this.Closing += WindowWorldEditor_Closing;

            _win32SfmlSurface = new System.Windows.Forms.Control();
            SfmlHost.Child = _win32SfmlSurface;

            _renderWindow = new RenderWindow(_win32SfmlSurface.Handle);
            _renderWindow.SetVerticalSyncEnabled(true);
            
            _view = new View();

            CompositionTarget.Rendering += CompositionTarget_Rendering;

            _win32SfmlSurface.Resize += Win32SfmlSurface_Resize;

        }

        private void WindowWorldEditor_Loaded(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void WindowWorldEditor_Closing(object? sender, CancelEventArgs e)
        {
            return;
        }

        private void Win32SfmlSurface_Resize(object? sender, EventArgs e)
        {
            _viewPosition = new Vector2f(0.0f, 0.0f);
            _viewSize = new Vector2f(_win32SfmlSurface.Width, _win32SfmlSurface.Height);

            _view = new View(new FloatRect(_viewPosition, _viewSize));

            _renderWindow.SetView(_view);

            return;
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (_renderWindow != null)
            {
                _renderWindow.DispatchEvents();

                _renderWindow.Clear(SFML.Graphics.Color.Green);

                _renderWindow.Display();
            }

            return;
        }

        
    }
}
