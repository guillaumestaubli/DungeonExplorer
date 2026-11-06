using GameLib.World.Tiles;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.World
{
    public class DungeonLevel
    {
        #region ATTTRIBUTES

        private Vector2i _gridSize;
        private Vector2f _tileRenderSize;
        private Tile[,] _tiles;


        #endregion

        #region GET_SET

        public Vector2i GridSize { get { return _gridSize; } }
        public Vector2f TileRenderSize { get { return _tileRenderSize; } }
        public Tile[,] Tiles { get { return _tiles; } set { _tiles = value; } }
        public Vector2f RenderSize
        {
            get
            {
                return new Vector2f
                {
                    X = _gridSize.X * _tileRenderSize.X,
                    Y = _gridSize.Y * _tileRenderSize.Y
                };
            }
        }

        #endregion

        public DungeonLevel(Vector2i gridSize, Vector2f tileRenderSize)
        {
            _gridSize = gridSize;
            _tileRenderSize = tileRenderSize;
            _tiles = new Tile[gridSize.X, gridSize.Y];

            for (int x = 0; x < gridSize.X; ++x)
            {
                for (int y = 0; y < gridSize.Y; ++y)
                {
                    _tiles[x, y] = new Tile(TileType.Void, new Vector2i(x, y), _tileRenderSize)
                    {
                        ShowBorders = true,
                        ShowIndex = true
                    };
                }
            }

        }
    }
}
