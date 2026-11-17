using GameLib.Files.SaveFiles;
using GameLib.Ressources;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.World.Props
{
    public class Prop
    {
        private Guid _id;
        private Vector2i _gridIndex;
        private Vector2f _renderSize;
        private Vector2f _renderPosition;
        private PropTexture _texture;

        private VertexArray _vaProp;
        private RenderStates _renderStates;
        private VertexArray _vaRectBorders;
        private VertexArray _vaPixelPerfectBorders;

        #region GET_SET

        public Guid Id { get { return _id; } }
        public Vector2i GridIndex { get { return _gridIndex; } }
        public Vector2f RenderSize { get { return _renderSize; } }
        public Vector2f RenderPosition { get { return _renderPosition; } }
        public PropTexture Texture { get { return _texture; } }

        public VertexArray D_Prop { get { return _vaProp; } }
        public RenderStates D_Prop_RenderStates { get { return _renderStates; } }
        public VertexArray D_RectBorders { get { return _vaRectBorders; } }
        public VertexArray D_PixelPerfectBorders { get { return _vaPixelPerfectBorders; } }

        public FloatRect GlobalBounds { get { return new FloatRect(_renderPosition, _renderSize); } }

        #endregion

        public Prop(Vector2i gridIndex, Vector2f renderSize, PropTexture texture)
        {
            _id = Guid.NewGuid();
            _gridIndex = gridIndex;

            _renderSize = renderSize;

            float r_x = _gridIndex.X * Dungeon.CurrentLevel.TileRenderSize.X;
            float r_y = _gridIndex.Y * Dungeon.CurrentLevel.TileRenderSize.Y;
            _renderPosition = new(r_x, r_y);

            _texture = texture;

            _vaProp = CreateVertexArrayProp();
            _renderStates = CreatePropRenderStates();
            _vaRectBorders = CreateVertexArrayRectBorders();
            _vaPixelPerfectBorders = CreateVertexArrayPixelPerfectBorders();
        }

        public Prop(PropSave save)
        {
            _id = new Guid(save.ID);
            _gridIndex = new(save.GridIndex_X, save.GridIndex_Y);
            _renderSize = new(save.RenderSize_X, save.RenderSize_Y);
            _renderPosition = new(save.RenderPosition_X, save.RenderPosition_Y);
            _texture = RessourcesManager.GetPropTexture(save.TextureName);
            _vaProp = CreateVertexArrayProp();
            _renderStates = CreatePropRenderStates();
            _vaRectBorders = CreateVertexArrayRectBorders();
            _vaPixelPerfectBorders = CreateVertexArrayPixelPerfectBorders();
        }

        private RenderStates CreatePropRenderStates()
        {
            return new(_texture.Texture);
        }

        private VertexArray CreateVertexArrayProp()
        {
            Color color = Color.White;

            VertexArray array = new(PrimitiveType.TriangleStrip, 4);

            array[0] = new()
            {
                Position = new(_renderPosition.X, _renderPosition.Y),
                TexCoords = new(0.0f, 0.0f),
                Color = color
            };

            array[1] = new()
            {
                Position = new(_renderPosition.X, _renderPosition.Y + _renderSize.Y),
                TexCoords = new(0.0f, _texture.Image.Size.Y),
                Color = color
            };

            array[2] = new()
            {
                Position = new(_renderPosition.X + _renderSize.X, _renderPosition.Y),
                TexCoords = new(_texture.Image.Size.X, 0.0f),
                Color = color
            };

            array[3] = new()
            {
                Position = new(_renderPosition.X + _renderSize.X, _renderPosition.Y + _renderSize.Y),
                TexCoords = (Vector2f)_texture.Image.Size,
                Color = color
            };

            return array;
        }

        private VertexArray CreateVertexArrayRectBorders()
        {
            Color color = Color.Green;

            VertexArray array = new(PrimitiveType.LineStrip, 5);

            array[0] = new()
            {
                Position = new(_renderPosition.X, _renderPosition.Y),
                Color = color
            };

            array[1] = new()
            {
                Position = new(_renderPosition.X, _renderPosition.Y + _renderSize.Y),
                Color = color
            };

            array[2] = new()
            {
                Position = new(_renderPosition.X + _renderSize.X, _renderPosition.Y + _renderSize.Y),
                Color = color
            };

            array[3] = new()
            {
                Position = new(_renderPosition.X + _renderSize.X, _renderPosition.Y),
                Color = color
            };

            array[4] = new()
            {
                Position = new(_renderPosition.X, _renderPosition.Y),
                Color = color
            };

            return array;
        }

        private VertexArray CreateVertexArrayPixelPerfectBorders()
        {
            return new(PrimitiveType.LineStrip, 0);
        }
    }
}
