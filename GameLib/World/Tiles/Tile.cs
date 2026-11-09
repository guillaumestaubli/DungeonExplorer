using GameLib.Files.SaveFiles;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.World.Tiles
{
    public class Tile
    {
        #region ATTTRIBUTES

        private Guid _id;
        private TileType _type;
        private Vector2i _gridIndex;
        private Vector2f _renderPosition;
        private Vector2f _renderSize;
        private bool _showBorders;
        private bool _showIndex;
        private Color _borderColor;

        private VertexArray _vaBackground;
        private VertexArray _vaBorder;
        private Text _txtIndex;

        #endregion

        #region GET_SET

        public Guid ID { get { return _id; } }
        public TileType Type { get { return _type; } }
        public Vector2i GridIndex { get { return _gridIndex; } }
        public FloatRect GlobalBounds { get { return new FloatRect(_renderPosition, _renderSize); } }
        public bool ShowBorders
        {
            get { return _showBorders; }
            set
            {
                _showBorders = value;
            }
        }
        public bool ShowIndex
        {
            get { return _showIndex; }
            set
            {
                _showIndex = value;
            }
        }
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
            }
        }


        public VertexArray D_Background { get { return _vaBackground; } }
        public VertexArray D_Border { get { return _vaBorder; } }
        public Text D_GridIndex { get { return _txtIndex; } }

        #endregion

        public Tile(TileType type, Vector2i gridIndex, Vector2f renderSize)
        {
            _id = Guid.NewGuid();
            _type = type;
            _gridIndex = gridIndex;
            _renderSize = renderSize;
            _renderPosition = new Vector2f(gridIndex.X * _renderSize.X, gridIndex.Y * _renderSize.Y);
            _showBorders = false;
            _showIndex = false;
            _borderColor = Color.Black;

            _vaBackground = new VertexArray(PrimitiveType.TriangleStrip, 4);
            _vaBorder = new VertexArray(PrimitiveType.LineStrip, 5);
            _txtIndex = new Text(GameLibConfig.Fonts.Arial, $"[{_gridIndex.X};{_gridIndex.Y}]", 12u)
            {
                Position = _renderPosition + new Vector2f(2.0f, 2.0f),
                FillColor = Color.Black
            };

            UpdateVertexArrayBorder();
            UpdateVertexArrayTile();
        }

        public Tile(TileSave save)
        {
            _id = new Guid(save.ID);
            _type = (TileType)save.Type;
            _gridIndex = new(save.GridIndex_X, save.GridIndex_Y);
            _renderSize = new(save.RenderSize_X, save.RenderSize_Y);
            _renderPosition = new(_gridIndex.X * _renderSize.X, _gridIndex.Y * _renderSize.Y);
            _showBorders = false;
            _showIndex = false;
            _borderColor = Color.Black;

            _vaBackground = new VertexArray(PrimitiveType.TriangleStrip, 4);
            _vaBorder = new VertexArray(PrimitiveType.LineStrip, 5);
            _txtIndex = new Text(GameLibConfig.Fonts.Arial, $"[{_gridIndex.X};{_gridIndex.Y}]", 12u)
            {
                Position = _renderPosition + new Vector2f(2.0f, 2.0f),
                FillColor = Color.Black
            };

            UpdateVertexArrayBorder();
            UpdateVertexArrayTile();
        }

        private void UpdateVertexArrayTile()
        {
            _vaBackground[0] = new Vertex
            {
                Position = _renderPosition,
                Color = Color.Green
            };

            _vaBackground[1] = new Vertex
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = Color.Green
            };

            _vaBackground[2] = new Vertex
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = Color.Green
            };

            _vaBackground[3] = new Vertex
            {
                Position = _renderPosition + _renderSize,
                Color = Color.Green
            };
        }

        private void UpdateVertexArrayBorder()
        {
            _vaBorder[0] = new Vertex
            {
                Position = _renderPosition,
                Color = _borderColor
            };

            _vaBorder[1] = new Vertex
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = _borderColor
            };

            _vaBorder[2] = new Vertex
            {
                Position = _renderPosition + _renderSize,
                Color = _borderColor
            };

            _vaBorder[3] = new Vertex
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = _borderColor
            };

            _vaBorder[4] = new Vertex
            {
                Position = _renderPosition,
                Color = _borderColor
            };
        }

    }
}
