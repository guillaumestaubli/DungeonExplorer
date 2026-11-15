using GameLib.World.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEditor.Helper
{
    public static class TextureHelper
    {
        public static readonly Dictionary<TileType, string> DisplayTileTypes = new()
        {
            {TileType.Ground, "Ground" },
            {TileType.Wall, "Wall" },
            {TileType.Door, "Door" }
        };
    }
}
