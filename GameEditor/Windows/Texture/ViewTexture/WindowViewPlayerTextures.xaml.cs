using GameLib.Ressources;
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
using System.Windows.Shapes;

namespace GameEditor.Windows.Texture.ViewTexture
{
    /// <summary>
    /// Logique d'interaction pour WindowViewPlayerTextures.xaml
    /// </summary>
    public partial class WindowViewPlayerTextures : Window
    {
        public WindowViewPlayerTextures()
        {
            InitializeComponent();

            this.Loaded += WindowViewPlayerTextures_Loaded;
        }

        private void WindowViewPlayerTextures_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayTexturesList();
        }

        private void DisplayTexturesList()
        {
            PlayerTexture[] textures = RessourcesManager.PlayerTextures.ToArray();

            this.StackPanel_List.Children.Clear();

            Grid grid = new();

            AddRows(ref grid, 1);
            AddColumns(ref grid, 8);

            Label titleName = CreateLabel("Name", 0, 0);
            Label titleTexSize = CreateLabel("Size", 0, 1);
            Label titleAction = CreateLabel("Action", 0, 2);
            Label titleTileSize = CreateLabel("Tile", 0, 3);
            Label titleAnim = CreateLabel("Duration", 0, 4);
            Label titleLines = CreateLabel("Lines", 0, 5);

            grid.Children.Add(CreateLabel("Name", 0, 0));
            grid.Children.Add(CreateLabel("Size", 0, 1));
            grid.Children.Add(CreateLabel("Action", 0, 2));
            grid.Children.Add(CreateLabel("Tile", 0, 3));
            grid.Children.Add(CreateLabel("Duration", 0, 4));
            grid.Children.Add(CreateLabel("Lines", 0, 5));

            int row = 0;

            for (int i = 0; i < textures.Length; ++i)
            {
                PlayerTexture texture = textures[i];

                AddRows(ref grid, 1);

                ++row;

                Label lblName = CreateLabel(texture.Name, row, 0);
                Label lblTexSize = CreateLabel($"{texture.Image.Size.X} x {texture.Image.Size.X} px", row, 1);
                Label lblAction = CreateLabel(texture.Action.ToString(), row, 2);
                Label lblTileSize = CreateLabel($"{texture.TileSize.X} x {texture.TileSize.Y} px", row, 3);
                Label lblAnim = CreateLabel($"{texture.AnimationDurationMs} ms", row, 4);
                Label lblLinesCount = CreateLabel($"{texture.Lines.Length} lines", row, 5);

                Button btnView = CreateViewButton(texture, row, 6);
                Button btnDelete = CreateDeleteButton(texture, row, 7);

                grid.Children.Add(lblName);
                grid.Children.Add(lblTexSize);
                grid.Children.Add(lblAction);
                grid.Children.Add(lblTileSize);
                grid.Children.Add(lblAnim);
                grid.Children.Add(lblLinesCount);
                grid.Children.Add(btnView);
                grid.Children.Add(btnDelete);

                AddRows(ref grid, 1);

                ++row;

                grid.Children.Add(CreateLabel("Line", row, 1));
                grid.Children.Add(CreateLabel("Direction", row, 2));
                grid.Children.Add(CreateLabel("Textures", row, 3));

                for (int j = 0; j < texture.Lines.Length; ++j)
                {
                    PlayerTextureLine line = texture.Lines[j];

                    AddRows(ref grid, 1);

                    ++row;

                    Label lblIndex = CreateLabel(line.LineIndex.ToString(), row, 1);
                    Label lblDir = CreateLabel(line.Direction.ToString(), row, 2);
                    Label lblCount = CreateLabel(line.TexturesCount.ToString(), row, 3);

                    grid.Children.Add(lblIndex);
                    grid.Children.Add(lblDir);
                    grid.Children.Add(lblCount);
                }

            }

            this.StackPanel_List.Children.Add(grid);
        }

        private void AddRows(ref Grid grid, int rows)
        {
            for (int i = 0; i < rows; ++i)
            {
                RowDefinition rowDef = new();
                rowDef.Height = new(1, GridUnitType.Auto);

                grid.RowDefinitions.Add(rowDef);
            }
        }

        private void AddColumns(ref Grid grid, int columns)
        {
            for (int i = 0; i < columns; ++i)
            {
                ColumnDefinition colDef = new();
                colDef.Width = new(1, GridUnitType.Auto);

                grid.ColumnDefinitions.Add(colDef);
            }
        }

        private Label CreateLabel(string content, int row = 0, int col = 0)
        {
            Label lbl = new()
            {
                Content = content,
                Margin = new(5, 5, 5, 5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Grid.SetColumn(lbl, col);
            Grid.SetRow(lbl, row);

            return lbl;
        }

        private Button CreateViewButton(PlayerTexture texture, int row = 0, int col = 0)
        {
            Button btn = new()
            {
                Content = "View",
                Tag = texture.Name,
                Margin = new(5, 5, 5, 5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            btn.Click += BtnView_Clicked;
            Grid.SetColumn(btn, col);
            Grid.SetRow(btn, row);

            return btn;
        }

        private Button CreateDeleteButton(PlayerTexture texture, int row = 0, int col = 0)
        {
            Button btn = new()
            {
                Content = "Delete",
                Tag = texture.Name,
                Margin = new(5, 5, 5, 5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            btn.Click += BtnDelete_Clicked;
            Grid.SetColumn(btn, col);
            Grid.SetRow(btn, row);

            return btn;
        }

        private void BtnView_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender == null || sender is not Button)
                return;

            Button btn = (Button)sender;

            if (btn.Tag == null)
                return;

            PlayerTexture texture = RessourcesManager.GetPlayerTexture(btn.Tag.ToString() ?? "");

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
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

            PlayerTexture texture = RessourcesManager.GetPlayerTexture(btn.Tag.ToString() ?? "");

            if (texture.Name == RessourcesManager.PlayerTextureNotFound.Name)
                return;

            string caption = "DungeonExplorer - WorldEditor - View Player Textures";
            string message = $"Do you really want to delete \"{texture.Name}\" ?";

            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // TODO : delete texture from project

                System.Diagnostics.Debug.WriteLine($"> delete texture {texture.Name}. Coming soon ...");

                DisplayTexturesList();
            }
        }
    }
}
