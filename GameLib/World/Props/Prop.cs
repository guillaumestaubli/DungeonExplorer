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
        private Vector2f _origin;
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
        public Vector2f Origin { get { return _origin; } }
        public Vector2f RenderSize { get { return _renderSize; } }
        public Vector2f RenderPosition { get { return _renderPosition; } }
        public PropTexture Texture { get { return _texture; } }

        public VertexArray D_Prop { get { return _vaProp; } }
        public RenderStates D_Prop_RenderStates { get { return _renderStates; } }
        public VertexArray D_RectBorders { get { return _vaRectBorders; } }
        public VertexArray D_PixelPerfectBorders { get { return _vaPixelPerfectBorders; } }

        public FloatRect GlobalBounds
        {
            get
            {
                float oX = _renderPosition.X;
                float oY = _renderPosition.Y;
                float dX = _renderSize.X / 2.0f;
                float dY = _renderSize.Y / 2.0f;

                Vector2f pos = new(oX - dX, oY - dY);

                return new(pos, _renderSize);
            }
        }

        #endregion

        public Prop(Vector2i gridIndex, Vector2f renderSize, PropTexture texture)
        {
            _id = Guid.NewGuid();
            _gridIndex = gridIndex;

            float o_x = renderSize.X / 2.0f;
            float o_y = renderSize.Y / 2.0f;
            _origin = new(o_x, o_y);

            _renderSize = renderSize;

            float r_x = (gridIndex.X * Dungeon.CurrentLevel.TileRenderSize.X) + (Dungeon.CurrentLevel.TileRenderSize.X / 2.0f);
            float r_y = (gridIndex.Y * Dungeon.CurrentLevel.TileRenderSize.Y) + (Dungeon.CurrentLevel.TileRenderSize.Y / 2.0f);
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
            _origin = new(save.Origin_X, save.Origin_Y);
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
            float oX = _renderPosition.X;
            float oY = _renderPosition.Y;

            float dX = _renderSize.X / 2.0f;
            float dY = _renderSize.Y / 2.0f;

            VertexArray array = new(PrimitiveType.TriangleStrip, 4);

            array[0] = new()
            {
                Position = new(oX - dX, oY - dY),
                TexCoords = new(0.0f, 0.0f),
                Color = Color.White
            };

            array[1] = new()
            {
                Position = new(oX - dX, oY + dY),
                TexCoords = new(0.0f, _texture.Image.Size.Y),
                Color = Color.White
            };

            array[2] = new()
            {
                Position = new(oX + dX, oY - dY),
                TexCoords = new(_texture.Image.Size.X, 0.0f),
                Color = Color.White
            };

            array[3] = new()
            {
                Position = new(oX + dX, oY + dY),
                TexCoords = (Vector2f)_texture.Image.Size,
                Color = Color.White
            };

            return array;
        }

        private VertexArray CreateVertexArrayRectBorders()
        {
            float oX = _renderPosition.X;
            float oY = _renderPosition.Y;

            float dX = _renderSize.X / 2.0f;
            float dY = _renderSize.Y / 2.0f;

            VertexArray array = new(PrimitiveType.LineStrip, 5);

            array[0] = new()
            {
                Position = new(oX - dX, oY - dY),
                Color = Color.Red
            };

            array[1] = new()
            {
                Position = new(oX - dX, oY + dY),
                Color = Color.Red
            };

            array[2] = new()
            {
                Position = new(oX + dX, oY + dY),
                Color = Color.Red
            };

            array[3] = new()
            {
                Position = new(oX + dX, oY - dY),
                Color = Color.Red
            };

            array[4] = new()
            {
                Position = new(oX - dX, oY - dY),
                Color = Color.Red
            };

            return array;
        }

        private VertexArray CreateVertexArrayPixelPerfectBorders()
        {
            return new(PrimitiveType.LineStrip, 0);
        }
    }
}
