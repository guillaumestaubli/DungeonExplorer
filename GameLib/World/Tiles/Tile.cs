using GameLib.Files.SaveFiles;
using GameLib.Ressources;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private Color _borderColor;
        private TileTexture _tileTexture;

        private VertexArray _vaBackground;
        private RenderStates _backgroundRenderStates;
        private VertexArray _vaBorder;
        private VertexArray _vaRectHitbox;
        private Text _txtIndex;
        private Text _txtType;

        #endregion

        #region GET_SET

        public Guid ID { get { return _id; } }
        public TileType Type { get { return _type; } set { _type = value; } }
        public Vector2i GridIndex { get { return _gridIndex; } }
        public Vector2f RenderPosition { get { return _renderPosition; } }
        public Vector2f RenderSize { get { return _renderSize; } }
        public FloatRect GlobalBounds { get { return new FloatRect(_renderPosition, _renderSize); } }
        public RenderStates BackgroundRenderStates { get { return _backgroundRenderStates; } }

        public TileTexture TileTexture
        {
            get { return _tileTexture; }
            set
            {
                _tileTexture = value;
                _vaBackground = GenerateVertexArrayBackground();
                _backgroundRenderStates = new(_tileTexture.Texture);
            }
        }

        public VertexArray D_Background { get { return _vaBackground; } }
        public VertexArray D_Border { get { return _vaBorder; } }
        public VertexArray D_Hitbox { get { return _vaRectHitbox; } }
        public Text D_GridIndex { get { return _txtIndex; } }
        public Text D_TileType { get { return _txtType; } }

        #endregion

        public Tile(TileType type, Vector2i gridIndex, Vector2f renderSize)
        {
            _id = Guid.NewGuid();
            _type = type;
            _gridIndex = gridIndex;
            _renderSize = renderSize;
            _renderPosition = new Vector2f(gridIndex.X * _renderSize.X, gridIndex.Y * _renderSize.Y);
            _borderColor = Color.White;
            _tileTexture = RessourcesManager.TileTextureNotFound;

            _vaBackground = GenerateVertexArrayBackground();
            _backgroundRenderStates = new(_tileTexture.Texture);
            _vaBorder = GenerateVertexArrayBorder();
            _vaRectHitbox = GenerateVertexArrayHitbox();
            _txtIndex = GenerateTxtTileIndex();
            _txtType = GenerateTxtTileType();
        }

        public Tile(TileSave save, Vector2f renderSize)
        {
            _id = new Guid(save.ID);
            _type = (TileType)save.Type;
            _gridIndex = new(save.GridIndex_X, save.GridIndex_Y);
            _renderSize = new(renderSize.X, renderSize.Y);
            _renderPosition = new(_gridIndex.X * _renderSize.X, _gridIndex.Y * _renderSize.Y);
            _borderColor = Color.White;
            _tileTexture = RessourcesManager.GetTileTexture(save.Texture, _type);

            _vaBackground = GenerateVertexArrayBackground();
            _backgroundRenderStates = new(_tileTexture.Texture);
            _vaBorder = GenerateVertexArrayBorder();
            _vaRectHitbox = GenerateVertexArrayHitbox();
            _txtIndex = GenerateTxtTileIndex();
            _txtType = GenerateTxtTileType();
        }

        private Text GenerateTxtTileIndex()
        {
            Text txt = new Text(GameLibConfig.Fonts.Arial, $"[{_gridIndex.X};{_gridIndex.Y}]", 12u)
            {
                Position = _renderPosition + new Vector2f(5.0f, 5.0f),
                FillColor = Color.Black,
                OutlineColor = Color.White,
                OutlineThickness = 0.75f
            };

            return txt;
        }

        private Text GenerateTxtTileType()
        {
            Text txt = new(GameLibConfig.Fonts.Arial, $"[{_type.ToString()}]", 12u)
            {
                Position = _renderPosition + new Vector2f(5.0f, 20.0f),
                FillColor = Color.Black,
                OutlineColor = Color.White,
                OutlineThickness = 0.75f
            };

            return txt;
        }

        private VertexArray GenerateVertexArrayBackground()
        {
            VertexArray array = new(PrimitiveType.TriangleStrip, 4u);

            array[0] = new Vertex
            {
                Position = _renderPosition,
                Color = Color.White,
                TexCoords = new Vector2f(0.0f, 0.0f)
            };

            array[1] = new Vertex
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = Color.White,
                TexCoords = new Vector2f(0.0f, _tileTexture.Texture.Size.Y)
            };

            array[2] = new Vertex
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = Color.White,
                TexCoords = new Vector2f(_tileTexture.Texture.Size.X, 0.0f)
            };

            array[3] = new Vertex
            {
                Position = _renderPosition + _renderSize,
                Color = Color.White,
                TexCoords = (Vector2f)_tileTexture.Texture.Size
            };

            return array;
        }

        private VertexArray GenerateVertexArrayBorder()
        {
            VertexArray array = new(PrimitiveType.LineStrip, 5u);

            array[0] = new Vertex
            {
                Position = _renderPosition,
                Color = _borderColor
            };

            array[1] = new Vertex
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = _borderColor
            };

            array[2] = new Vertex
            {
                Position = _renderPosition + _renderSize,
                Color = _borderColor
            };

            array[3] = new Vertex
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = _borderColor
            };

            array[4] = new Vertex
            {
                Position = _renderPosition,
                Color = _borderColor
            };

            return array;
        }

        private VertexArray GenerateVertexArrayHitbox()
        {
            VertexArray array = new(PrimitiveType.LineStrip, 5u);

            array[0] = new Vertex
            {
                Position = _renderPosition,
                Color = GameLibConfig.Textures.Color_WallHitbox
            };

            array[1] = new Vertex
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = GameLibConfig.Textures.Color_WallHitbox
            };

            array[2] = new Vertex
            {
                Position = _renderPosition + _renderSize,
                Color = GameLibConfig.Textures.Color_WallHitbox
            };

            array[3] = new Vertex
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = GameLibConfig.Textures.Color_WallHitbox
            };

            array[4] = new Vertex
            {
                Position = _renderPosition,
                Color = GameLibConfig.Textures.Color_WallHitbox
            };

            return array;
        }

    }
}
