using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameEditor.Logging;
using GameLib.Files;
using GameLib.World;
using GameLib.World.Tiles;

namespace GameEditor.Windows
{
    /// <summary>
    /// Logique d'interaction pour WindowAddRessource.xaml
    /// </summary>
    public partial class WindowAddRessource : Window
    {
        private Dictionary<TileType, string> _tileTypes = new()
        {
            {TileType.Void, "Void Texture" },
            {TileType.Ground, "Ground Texture" },
            {TileType.Wall, "Wall Texture" },
            {TileType.Door, "Door texture" }
        };

        private List<string> _textureFiles = [];

        private TileType? _tileType = null;

        public WindowAddRessource()
        {
            InitializeComponent();

            this.Loaded += WindowAddRessource_Loaded;
        }

        private void WindowAddRessource_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<TileType, string> tileType in _tileTypes)
            {
                ComboBoxItem item = new()
                {
                    Name = $"TileType_{(int)tileType.Key}",
                    Content = tileType.Value,
                    Tag = (int)tileType.Key
                };

                ComboBox_TextureCategory.Items.Add(item);
            }
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Filter = $"Image file (*.png) | *.png",
                FilterIndex = 1,
                RestoreDirectory = true,
                Multiselect = true
            };

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                _textureFiles.Clear();
                _textureFiles.AddRange(dialog.FileNames);

                UpdateListBoxFileNames();
            }
            else
            {
                _textureFiles.Clear();
                UpdateListBoxFileNames();
            }
        }

        private void UpdateListBoxFileNames()
        {
            LstBox_Files.Items.Clear();

            for (int i = 0; i < _textureFiles.Count; ++i)
            {
                ListBoxItem item = new()
                {
                    Content = _textureFiles[i],
                    Name = $"item_{i}",
                    Tag = i
                };

                LstBox_Files.Items.Add(item);
            }
        }

        private void LstBox_Files_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem? item = (ListBoxItem?)LstBox_Files.SelectedItem;

            if (item == null)
            {
                SetTexturePreview(null);
                return;
            }

            int index = 0;

            if (!int.TryParse(item.Tag.ToString(), out index))
            {
                SetTexturePreview(null);
                return;
            }

            string filename = _textureFiles[index];

            SetTexturePreview(filename);
        }


        private void SetTexturePreview(string? filename)
        {
            if (filename == null)
            {
                Img_Texture.Source = null;
                return;
            }

            BitmapImage bitmapImage = new();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filename);

            if (Img_Texture.RenderSize.Width < Img_Texture.RenderSize.Height)
                bitmapImage.DecodePixelWidth = (int)Img_Texture.RenderSize.Width;
            else
                bitmapImage.DecodePixelHeight = (int)Img_Texture.RenderSize.Height;

            bitmapImage.EndInit();

            Img_Texture.Source = bitmapImage;
        }

        private void ComboBox_TextureCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tileType = null;

            ComboBoxItem? item = (ComboBoxItem?)this.ComboBox_TextureCategory.SelectedItem;

            if (item == null)
                return;

            string? tag = item.Tag.ToString();

            if (tag == null)
                return;

            int index = 0;

            if (!int.TryParse(tag, out index))
                return;

            _tileType = (TileType)index;
        }

        private void Btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            if (_tileType == null)
                return;

            if (_textureFiles.Count <= 0)
                return;

            List<string> errorFiles = [];

            foreach (string file in _textureFiles)
            {
                IOResult ior = FilesManager.CopyTileTextureInProject(file, (TileType)_tileType);

                if(ior != IOResult.OK)
                {
                    errorFiles.Add(file);

                    Logger.Error($"{nameof(WindowAddRessource)}.{nameof(Btn_Validate_Click)}", $"An error occured during the copy of the file : {file}", null);
                }
            }

            if(errorFiles.Count > 0)
            {
                string text = "Errors occured for the file : \n";

                foreach(string file in errorFiles)
                {
                    text += $"> {file}\n";
                }

                string caption = "GameEditor - WindowAddRessource";

                _ = System.Windows.MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            GameLib.Files.FilesManager.SaveProject();

            this.Close();
        }
    }
}
