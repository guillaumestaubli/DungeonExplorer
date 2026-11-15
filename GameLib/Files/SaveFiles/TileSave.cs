using GameLib.World.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class TileSave
    {
        public string ID { get; set; } = "#";
        public int Type { get; set; } = -1;
        public string Texture { get; set; } = "#";
        public int GridIndex_X { get; set; } = -1;
        public int GridIndex_Y { get; set; } = -1;

        public TileSave()
        {

        }

        public TileSave(Tile tile)
        {
            ID = tile.ID.ToString();
            Type = (int)tile.Type;
            Texture = tile.TileTexture.Name;
            GridIndex_X = tile.GridIndex.X;
            GridIndex_Y = tile.GridIndex.Y;
        }
    }
}
