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
using System.Windows.Shapes;

namespace GameEditor.Windows.Texture.ViewTexture
{
    /// <summary>
    /// Logique d'interaction pour WindowViewPropTextures.xaml
    /// </summary>
    public partial class WindowViewPropTextures : Window
    {
        public WindowViewPropTextures()
        {
            InitializeComponent();

            this.Loaded += WindowViewPropTextures_Loaded;
        }

        private void WindowViewPropTextures_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayTexturesList();
        }

        private void DisplayTexturesList()
        {
            PropTexture[] textures = RessourcesManager.PropTextures.ToArray();

            this.StackPanel_List.Children.Clear();

            for (int i = 0; i < textures.Length; ++i)
            {
                PropTexture texture = textures[i];

                Grid grid = new();

                RowDefinition row = new();
                row.Height = new(1, GridUnitType.Star);

                grid.RowDefinitions.Add(row);

                ColumnDefinition col0 = new();
                col0.Width = new(1, GridUnitType.Star);

                ColumnDefinition col1 = new();
                col1.Width = new(1, GridUnitType.Star);

                ColumnDefinition col2 = new();
                col2.Width = new(1, GridUnitType.Star);

                ColumnDefinition col3 = new();
                col2.Width = new(1, GridUnitType.Star);

                grid.ColumnDefinitions.Add(col0);
                grid.ColumnDefinitions.Add(col1);
                grid.ColumnDefinitions.Add(col2);
                grid.ColumnDefinitions.Add(col3);

                Label lblName = new()
                {
                    Content = texture.Name,
                    Margin = new(5, 5, 5, 5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(lblName, 0);
                Grid.SetRow(lblName, 0);

                Label lblSize = new()
                {
                    Content = $"{texture.Image.Size.X} x {texture.Image.Size.X} px",
                    Margin = new(5, 5, 5, 5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                Grid.SetColumn(lblSize, 1);
                Grid.SetRow(lblSize, 0);

                Button btnView = new()
                {
                    Content = "View",
                    Tag = texture.Name,
                    Margin = new(5, 5, 5, 5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                btnView.Click += BtnView_Clicked;
                Grid.SetColumn(btnView, 2);
                Grid.SetRow(btnView, 0);

                Button btnDelete = new()
                {
                    Content = "Delete",
                    Tag = texture.Name,
                    Margin = new(5, 5, 5, 5),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                btnDelete.Click += BtnDelete_Clicked;
                Grid.SetColumn(btnDelete, 3);
                Grid.SetRow(btnDelete, 0);

                grid.Children.Add(lblName);
                grid.Children.Add(lblSize);
                grid.Children.Add(btnView);
                grid.Children.Add(btnDelete);

                this.StackPanel_List.Children.Add(grid);
            }
        }

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not Button)
                return;

            Button btn = (Button)sender;

            if (btn.Tag == null)
                return;

            PropTexture texture = RessourcesManager.GetPropTexture(btn.Tag.ToString() ?? "");

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = new Uri(texture.Path);

            if (this.Img_Texture.RenderSize.Width < this.Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)this.Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)this.Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            this.Img_Texture.Source = bitmapImage;
        }

        private void BtnDelete_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not Button)
                return;

            Button btn = (Button)sender;

            if (btn.Tag == null)
                return;

            PropTexture texture = RessourcesManager.GetPropTexture(btn.Tag.ToString() ?? "");

            if (texture.Name == RessourcesManager.PropTextureNotFound.Name)
                return;

            string caption = "DungeonExplorer - WorldEditor - View Prop Textures";
            string message = $"Do you really want to delete \"{texture.Name}\" ?";

            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                this.Img_Texture.Source = null;

                bool success = RessourcesManager.DeletePropTextureFromProject(texture);

                if (success)
                {
                    message = $"The texture {texture.Name} has been successfully deleted from project";

                    _ = MessageBox.Show(message, caption, MessageBoxButton.OK);
                }
                else
                {
                    message = $"An error occured when trying to delete {texture.Name} from project";

                    _ = MessageBox.Show(message, caption, MessageBoxButton.OK);
                }

                DisplayTexturesList();
            }
        }
    }
}
