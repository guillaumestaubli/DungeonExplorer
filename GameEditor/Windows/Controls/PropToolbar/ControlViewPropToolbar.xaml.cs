using GameLib.World.Props;
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

namespace GameEditor.Windows.Controls.PropToolbar
{
    /// <summary>
    /// Logique d'interaction pour ControlViewPropToolbar.xaml
    /// </summary>
    public partial class ControlViewPropToolbar : UserControl
    {
        public ControlViewPropToolbar()
        {
            InitializeComponent();
        }

        public void LoadProp(Prop prop)
        {
            this.TxtBox_ID.Text = prop.Id.ToString();
            this.TxtBox_GridIndex.Text = $"[{prop.GridIndex.X};{prop.GridIndex.Y}]";
            this.TxtBox_RenderPosition.Text = $"[{prop.GlobalBounds.Position.X};{prop.GlobalBounds.Position.Y}]";
            this.TxtBox_RenderSize.Text = $"[{prop.GlobalBounds.Size.X};{prop.GlobalBounds.Size.Y}]";

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(prop.Texture.Path);

            if (this.Img_Texture.RenderSize.Width < this.Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)this.Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)this.Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_Texture.Source = bitmapImage;
        }
    }
}
