using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    /// Logique d'interaction pour WindowCreateDungeon.xaml
    /// </summary>
    public partial class WindowCreateDungeon : Window
    {
        private string _projectBaseDir = GameEditorConfig.Path_Base_Projects_Dir;

        private string _projectName = string.Empty;
        private string _projectDir = string.Empty;
        private int _firstLevel_X = 0;
        private int _firstLevel_Y = 0;

        public WindowCreateDungeon()
        {
            InitializeComponent();

            this.Loaded += WindowCreateDungeon_Loaded;
        }

        private void WindowCreateDungeon_Loaded(object sender, RoutedEventArgs e)
        {
            this.TxtBox_Dir.Text = GameEditorConfig.Path_Base_Projects_Dir;
        }

        private void Btn_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new()
            {
                InitialDirectory = GameEditorConfig.Path_Base_Projects_Dir,
                Multiselect = false,
                DefaultDirectory = GameEditorConfig.Path_Base_Projects_Dir
            };

            bool? result = dialog.ShowDialog();

            if(result != null && result == true)
            {
                _projectBaseDir = dialog.FolderName;
                this.TxtBox_Dir.Text = _projectBaseDir;
            }
            else
            {
                _projectBaseDir = GameEditorConfig.Path_Base_Projects_Dir;
                this.TxtBox_Dir.Text = _projectBaseDir;
            }
        }

        private void TxtBox_Validation_Number_Integer(object? sender, TextCompositionEventArgs e)
        {
            if(sender == null || sender is not TextBox)
            {
                e.Handled = false;
                return;
            }

            string fullInput = ((TextBox)sender).Text + e.Text;

            if (fullInput.Length == 0)
                fullInput = "0";

            bool success = int.TryParse(fullInput, CultureInfo.InvariantCulture, out _);

            e.Handled = !success;

            // e.Handled = false; => write new char
            // e.Handled = true;  => don't write new char
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }

        private void Btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            if (!FetchValuesFromInputs())
            {
                string message =
                    $"An error occured during {nameof(WindowCreateDungeon)}.{nameof(FetchValuesFromInputs)}. " +
                    $"Impossible to fetch user inputs.";

                string caption = $"DungeonExplorer - GameEditor";

                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            GameEditorConfig.DungeonCreation.ProjectName = _projectName;
            GameEditorConfig.DungeonCreation.ProjectDir = _projectDir;
            GameEditorConfig.DungeonCreation.Level_X = _firstLevel_X;
            GameEditorConfig.DungeonCreation.Level_Y = _firstLevel_Y;

            this.DialogResult = true;
            this.Close();
        }

        private bool FetchValuesFromInputs()
        {
            bool outSuccess = true;

            _projectName = this.TxtBox_Name.Text;

            if(_projectName.Length == 0)
            {
                Debug.WriteLine($"> ERROR : WindowCreateDungeon.FetchValuesFromInputs : _projectName.Length == 0");
                outSuccess = false;
            }

            _projectDir = _projectBaseDir + "\\" + _projectName;

            if(_projectDir.Length == 0)
            {
                Debug.WriteLine($"> ERROR : WindowCreateDungeon.FetchValuesFromInputs : _projectDir.Length == 0");
                outSuccess = false;
            }

            if (Directory.Exists(_projectDir))
            {
                Debug.WriteLine($"> ERROR : WindowCreateDungeon.FetchValuesFromInputs : directory with name \"{_projectDir}\" already exists");
                outSuccess = false;
            }

            if(!int.TryParse(this.TxtBox_Level_X.Text, CultureInfo.InvariantCulture, out _firstLevel_X))
            {
                Debug.WriteLine($"> ERROR : WindowCreateDungeon.FetchValuesFromInputs : failed to parse first level X value to int");
                outSuccess = false;
            }

            if(!int.TryParse(this.TxtBox_Level_Y.Text, CultureInfo.InvariantCulture, out _firstLevel_Y))
            {
                Debug.WriteLine($"> ERROR : WindowCreateDungeon.FetchValuesFromInputs : failed to parse first level Y value to int");
                outSuccess = false;
            }

            return outSuccess;
        }
    }
}
