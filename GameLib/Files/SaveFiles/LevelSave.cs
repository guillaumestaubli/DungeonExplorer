using GameLib.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Files.SaveFiles
{
    public class LevelSave
    {
        public string ID { get; set; } = "#";
        public int Index { get; set; } = -1;
        public string Name { get; set; } = "#";
        public int GridSize_X { get; set; } = -1;
        public int GridSize_Y { get; set;} = -1;
        public float TileRenderSize_X { get; set; } = -1.0f;
        public float TileRenderSize_Y { get; set; } = -1.0f;

        public TileSave[,] Tiles { get; set; } = new TileSave[0, 0];

        internal LevelSave()
        {

        }

        internal LevelSave(DungeonLevel level)
        {
            ID = level.ID.ToString();
            Index = level.Index;
            Name = level.Name;
            GridSize_X = level.GridSize.X;
            GridSize_Y = level.GridSize.Y;
            TileRenderSize_X = level.TileRenderSize.X;
            TileRenderSize_Y = level.TileRenderSize.Y;

            Tiles = new TileSave[level.GridSize.X, level.GridSize.Y];

            for(int x = 0; x < level.GridSize.X; ++x)
            {
                for(int y = 0;  y < level.GridSize.Y; ++y)
                {
                    Tiles[x,y] = new(level.Tiles[x,y]);
                }
            }
        }
    }
}
