using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib
{
    public static class GameLibConfig
    {
        public static class Global
        {
            public const bool FlushLogsOnStartup = true;
            public const string Ext_DungeonBaseFile = "dungeon";

            private static bool _isInPlayMode = true;
            public static bool IsInPlayMode { get { return _isInPlayMode; } set { _isInPlayMode = value; } }
            public static bool IsInEditMode { get { return !_isInPlayMode; } set { _isInPlayMode = !value; } }
        }

        public static class Textures
        {
            public const string Path_Img_TextureNotFound = @"DefaultRessources\texture_not_found.png";
        }

        public static class Fonts
        {
            private const string Path_Font_Arial = @"DefaultRessources\arial.ttf";
            private const string Path_Font_CalibriLight = @"DefaultRessources\calibri_light.ttf";
            private const string Path_Font_Consola = @"DefaultRessources\consolas.ttf";

            public static readonly Font Arial;
            public static readonly Font CalibriLight;
            public static readonly Font Consola;

            static Fonts()
            {
                Arial = new Font(Path_Font_Arial);
                CalibriLight = new Font(Path_Font_CalibriLight);
                Consola = new Font(Path_Font_Consola);
            }
        }
    }
}
