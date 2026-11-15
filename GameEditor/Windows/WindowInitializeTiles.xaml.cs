using GameEditor.Logging;
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
using System.Windows.Shapes;

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowInitializeTiles.xaml
    /// </summary>
    public partial class WindowInitializeTiles : Window
    {
        private Dictionary<TileType, string> _tileTypes = new()
        {
            {TileType.Ground, "Ground Texture" },
            {TileType.Wall, "Wall Texture" },
            {TileType.Door, "Door texture" }
        };

        private TileType? _tileType = null;
        private TileTexture? _texture = null;

        public WindowInitializeTiles()
        {
            InitializeComponent();

            this.Loaded += WindowInitializeTiles_Loaded;
        }

        private void WindowInitializeTiles_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<TileType, string> tileType in _tileTypes)
            {
                ComboBoxItem item = new()
                {
                    Name = $"TileType_{(int)tileType.Key}",
                    Content = tileType.Value,
                    Tag = (int)tileType.Key
                };

                this.ComboBox_TileType.Items.Add(item);
            }
        }

        private void ComboBox_TileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tileType = null;

            ComboBoxItem? item = (ComboBoxItem?)this.ComboBox_TileType.SelectedItem;

            if (item == null)
            {
                UpdateListBoxTextures(null);
                return;
            }

            string? tag = item.Tag.ToString();

            if (tag == null)
            {
                UpdateListBoxTextures(null);
                return;
            }

            int index = 0;

            if (!int.TryParse(tag, out index))
            {
                UpdateListBoxTextures(null);
                return;
            }

            _tileType = (TileType)index;

            UpdateListBoxTextures(_tileType);
        }

        private void LstBox_Textures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Img_Texture.Source = null;
            _texture = null;

            ListBoxItem? item = (ListBoxItem?)this.LstBox_Textures.SelectedItem;

            if (item == null)
                return;

            if (item.Tag == null)
                return;

            if (_tileType == null)
                return;

            TileTexture texture = RessourcesManager.GetTileTexture(item.Tag.ToString() ?? string.Empty, (TileType)_tileType);

            if (texture.Name == RessourcesManager.TileTextureNotFound.Name)
                return;

            _texture = texture;

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(texture.Path);

            if (Img_Texture.RenderSize.Width < Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_Texture.Source = bitmapImage;
        }

        private void Btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            if (_tileType == null)
                return;

            if (_texture == null)
                return;

            RessourcesManager.ApplyTextureToAllTiles(_texture, (TileType)_tileType);

            this.Close();
        }

        private void UpdateListBoxTextures(TileType? tileType)
        {
            this.LstBox_Textures.Items.Clear();
            this.Img_Texture.Source = null;

            if (tileType == null)
                return;

            TextureBase[] textures = [];

            switch (tileType)
            {
                case TileType.Ground:
                    {
                        textures = RessourcesManager.GroundTextures.ToArray();
                        break;
                    }
                case TileType.Wall:
                    {
                        textures = RessourcesManager.WallTextures.ToArray();
                        break;
                    }
                case TileType.Door:
                    {
                        textures = RessourcesManager.DoorTextures.ToArray();
                        break;
                    }
            }

            foreach(TextureBase texture in textures)
            {
                ListBoxItem item = new()
                {
                    Name = texture.Name,
                    Content = texture.Name,
                    Tag = texture.Name
                };

                this.LstBox_Textures.Items.Add(item);
            }

        }
    }
}
