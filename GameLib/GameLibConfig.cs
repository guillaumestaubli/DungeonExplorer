using SFML.Graphics;
using SFML.Window;
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

            public static Color Color_RectBounds { get; set; } = Color.Green;
            public static Color Color_PixelPerfectBounds { get; set; } = Color.Magenta;
            public static Color Color_WallHitbox { get; set; } = Color.Yellow;
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

        public static class Controls
        {
            public const Keyboard.Key KeyMoveNorth = Keyboard.Key.W;
            public const Keyboard.Key KeyMoveSouth = Keyboard.Key.S;
            public const Keyboard.Key KeyMoveWest = Keyboard.Key.A;
            public const Keyboard.Key KeyMoveEast = Keyboard.Key.D;

            public const Keyboard.Key KeyMoveSprint = Keyboard.Key.LShift;

            public const float MoveOffsetPerFrame = 2.0f; // [px/frame]
            public const float SprintRatio = 1.75f;
        }
    }
}
