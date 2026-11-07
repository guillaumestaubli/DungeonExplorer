using GameLib.World.Tiles;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class TileSave
    {
        public string ID { get; set; } = "#";
        public int Type { get; set; } = -1;
        public int GridIndex_X { get; set; } = -1;
        public int GridIndex_Y { get; set; } = -1;
        public float RenderSize_X { get; set; } = -1.0f;
        public float RenderSize_Y { get; set; } = -1.0f;

        public TileSave()
        {

        }

        public TileSave(Tile tile)
        {
            ID = tile.ID.ToString();
            Type = (int)tile.Type;
            GridIndex_X = tile.GridIndex.X;
            GridIndex_Y = tile.GridIndex.Y;
            RenderSize_X = tile.GlobalBounds.Size.X;
            RenderSize_Y = tile.GlobalBounds.Size.Y;
        }
    }
}
