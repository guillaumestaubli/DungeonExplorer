using GameLib.World.Tiles;
using SFML.Graphics;
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

namespace GameEditor.Windows.Controls.TileToolbar
{
    /// <summary>
    /// Logique d'interaction pour ControlViewTileToolbar.xaml
    /// </summary>
    public partial class ControlViewTileToolbar : UserControl
    {
        public ControlViewTileToolbar()
        {
            InitializeComponent();
        }

        public void LoadTile(Tile tile)
        {
            this.TxtBox_ID.Text = tile.ID.ToString();
            this.TxtBox_Type.Text = tile.Type.ToString();
            this.TxtBox_GridIndex.Text = $"[{tile.GridIndex.X};{tile.GridIndex.Y}]";
            this.TxtBox_RenderPosition.Text = $"[{tile.GlobalBounds.Position.X};{tile.GlobalBounds.Position.Y}]";
            this.TxtBox_RenderSize.Text = $"[{tile.GlobalBounds.Size.X};{tile.GlobalBounds.Size.Y}]";
            this.TxtBox_TextureName.Text = tile.TileTexture.Name;
            this.TxtBox_TexturePath.Text = tile.TileTexture.Path;

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(tile.TileTexture.Path);

            if (this.Img_Texture.RenderSize.Width < this.Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)this.Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)this.Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_Texture.Source = bitmapImage;
        }
    }
}
