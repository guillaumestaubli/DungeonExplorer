using GameLib.Ressources;
using GameLib.Ressources.Textures;
using GameLib.World.Tiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameEditor.Windows.WorldEditor.Toolbar.Prop
{
    /// <summary>
    /// Logique d'interaction pour AddPropToolbar.xaml
    /// </summary>
    public partial class AddPropToolbar : UserControl
    {
        private Image? _currentImgTexture = null;
        private PropTexture? _currentPropTexture = null;

        #region GET_SET

        public PropTexture? CurrentPropTexture { get { return _currentPropTexture; } }

        #endregion

        public AddPropToolbar()
        {
            InitializeComponent();

            this.Loaded += ControlAddPropToolbar_Loaded;
        }

        private void ControlAddPropToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            this.WrapPanel_Textures.Children.Clear();

            TextureBase[] textures = RessourcesManager.PropTextures.ToArray();

            foreach (TextureBase texture in textures)
            {
                Image img = new()
                {
                    Width = 128,
                    Height = 128,
                    Tag = texture.Name
                };

                BitmapImage bitmapImage = new();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(texture.Path);

                if (img.RenderSize.Width < img.RenderSize.Height)
                    bitmapImage.DecodePixelWidth = (int)img.RenderSize.Width;
                else
                    bitmapImage.DecodePixelHeight = (int)img.RenderSize.Height;

                bitmapImage.EndInit();

                img.Source = bitmapImage;
                img.MouseDown += ImgTexture_MouseDown;

                Border border = new()
                {
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2C2C2")),
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(10),
                    Child = img
                };

                this.WrapPanel_Textures.Children.Add(border);
            }
        }

        private void ImgTexture_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            if (sender is not Image)
                return;

            if (_currentImgTexture != null)
            {
                ((Border)_currentImgTexture.Parent).BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2C2C2"));
            }

            _currentImgTexture = (Image)sender;
            _currentPropTexture = RessourcesManager.GetPropTexture((string)_currentImgTexture.Tag);

            ((Border)_currentImgTexture.Parent).BorderBrush = Brushes.Red;
        }
    }
}
