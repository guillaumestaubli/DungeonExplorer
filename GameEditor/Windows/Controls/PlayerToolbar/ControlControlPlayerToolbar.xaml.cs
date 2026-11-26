using GameLib.Entities;
using GameLib.World.Props;
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

namespace GameEditor.Windows.Controls.PlayerToolbar
{
    /// <summary>
    /// Logique d'interaction pour ControlControlPlayerToolbar.xaml
    /// </summary>
    public partial class ControlControlPlayerToolbar : UserControl
    {
        public ControlControlPlayerToolbar()
        {
            InitializeComponent();
        }

        public void LoadPlayer(Player player)
        {
            this.TxtBox_ID.Text = player.ID.ToString();
            this.TxtBox_Action.Text = player.Action.ToString();
            this.TxtBox_Direction.Text = player.Direction.ToString();
            this.TxtBox_RenderSize.Text = $"[{player.RenderSize.X};{player.RenderSize.Y}]";
            this.TxtBox_RenderPosition.Text = $"[{player.RenderPosition.X};{player.RenderPosition.Y}]";

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(player.Textures[player.Action].Path);

            if (this.Img_Texture.RenderSize.Width < this.Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)this.Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)this.Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_Texture.Source = bitmapImage;
        }
    }
}
