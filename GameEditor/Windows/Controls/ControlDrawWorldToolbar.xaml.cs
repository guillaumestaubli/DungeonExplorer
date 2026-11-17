using GameEditor.Helper;
using GameLib.Ressources;
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

namespace GameEditor.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour ControlDrawWorldToolbar.xaml
    /// </summary>
    public partial class ControlDrawWorldToolbar : UserControl
    {
        private TileType? _tileType = null;
        private Image? _currentImgTexture = null;
        private TileTexture? _currentTileTexture = null;

        #region GET_SET

        public TileType? CurrentTileType { get { return _tileType; } }
        public TileTexture? CurrentTileTexture { get { return _currentTileTexture; } }

        #endregion

        public ControlDrawWorldToolbar()
        {
            InitializeComponent();

            this.Loaded += ControlDrawWorldToolbar_Loaded;
        }

        private void ControlDrawWorldToolbar_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<TileType, string> tileType in TextureHelper.DisplayTileTypes)
            {
                ComboBoxItem item = new()
                {
                    Name = $"TileType_{(int)tileType.Key}",
                    Content = tileType.Value + " texture",
                    Tag = (int)tileType.Key
                };

                this.ComboBox_Type.Items.Add(item);
            }
        }

        private void ComboBox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tileType = null;

            ComboBoxItem? item = (ComboBoxItem?)this.ComboBox_Type.SelectedItem;

            if (item == null)
            {
                UpdateTextureList(null);
                return;
            }

            string? tag = item.Tag.ToString();

            if (tag == null)
            {
                UpdateTextureList(null);
                return;
            }

            int index = 0;

            if (!int.TryParse(tag, out index))
            {
                UpdateTextureList(null);
                return;
            }

            _tileType = (TileType)index;

            UpdateTextureList(_tileType);

            _currentImgTexture = null;
            _currentTileTexture = null;
        }

        private void UpdateTextureList(TileType? tileType)
        {
            this.WrapPanel_Textures.Children.Clear();

            if (tileType == null)
                return;

            TextureBase[] textures = RessourcesManager.GetTileTextures((TileType)tileType);

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

            if (_tileType == null)
                return;

            if (_currentImgTexture != null)
            {
                ((Border)_currentImgTexture.Parent).BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2C2C2"));
            }

            _currentImgTexture = (Image)sender;
            _currentTileTexture = RessourcesManager.GetTileTexture((string)_currentImgTexture.Tag, (TileType)_tileType);

            ((Border)_currentImgTexture.Parent).BorderBrush = Brushes.Red;
        }
    }
}
