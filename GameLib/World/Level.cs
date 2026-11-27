using GameLib.Files.SaveFiles;
using GameLib.World.Props;
using GameLib.World.Tiles;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.World
{
    public class Level
    {
        #region ATTTRIBUTES

        private Guid _id;
        private int _index;
        private string _name;
        private Vector2i _gridSize;
        private Vector2f _tileRenderSize;
        private Tile[,] _tiles;
        private List<Prop> _props;

        #endregion

        #region GET_SET

        public Guid ID { get { return _id; } }
        public int Index { get { return _index; } }
        public string Name { get { return _name; } }

        public Vector2i GridSize { get { return _gridSize; } }
        public Vector2f TileRenderSize { get { return _tileRenderSize; } }
        public Tile[,] Tiles { get { return _tiles; } set { _tiles = value; } }
        public List<Prop> Props { get { return _props; } set { _props = value; } }
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

        public Level(int index, Vector2i gridSize, Vector2f tileRenderSize, string name)
        {
            _id = Guid.NewGuid();
            _index = index;
            _name = name;
            _gridSize = gridSize;
            _tileRenderSize = tileRenderSize;
            _tiles = new Tile[gridSize.X, gridSize.Y];
            _props = [];

            for (int x = 0; x < gridSize.X; ++x)
            {
                for (int y = 0; y < gridSize.Y; ++y)
                {
                    _tiles[x, y] = new Tile(TileType.Void, new Vector2i(x, y), _tileRenderSize);
                }
            }

        }

        internal Level(LevelSave save)
        {
            _id = new Guid(save.ID);
            _index = save.Index;
            _name = save.Name;
            _gridSize = new(save.GridSize_X, save.GridSize_Y);
            _tileRenderSize = new(save.TileRenderSize_X, save.TileRenderSize_Y);
            _tiles = new Tile[_gridSize.X, _gridSize.Y];
            _props = [];

            for (int x = 0; x < _gridSize.X; ++x)
            {
                for (int y = 0; y < _gridSize.Y; ++y)
                {
                    _tiles[x, y] = new(save.Tiles[x, y], _tileRenderSize);
                }
            }

            for (int i = 0; i < save.Props.Length; ++i)
            {
                _props.Add(new(save.Props[i]));
            }
        }

        public Tile GetTile(Vector2i gridIndex)
        {
            if (gridIndex.X < 0 || gridIndex.X >= _gridSize.X || gridIndex.Y < 0 || gridIndex.Y >= _gridSize.Y)
                throw new IndexOutOfRangeException("The index given is out of the level grid range");

            return _tiles[gridIndex.X, gridIndex.Y];
        }

        public void UpdateTile(Tile tile, Vector2i gridIndex)
        {
            _tiles[gridIndex.X, gridIndex.Y] = tile;
        }
    }
}
