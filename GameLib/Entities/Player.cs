using GameLib.Ressources;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.Entities
{
    public class Player
    {
        private Guid _id;
        private Direction _direction;
        private Action _action;

        private Vector2f _renderPosition;
        private Vector2f _renderSize;

        private PlayerTexture _texture;

        private VertexArray _vaPlayer;
        private VertexArray _vaRectBounds;
        private VertexArray _vaPixelPerfectBounds;

        #region GET_SET

        public Guid ID { get { return _id; } }
        public Direction Direction { get { return _direction; } }
        public Action Action { get { return _action; } }
        public Vector2f RenderPosition { get { return _renderPosition; } }
        public Vector2f RenderSize { get { return _renderSize; } }
        public FloatRect GlobalBounds { get { return new(_renderPosition, _renderSize); } }
        public PlayerTexture Texture { get { return _texture; } }

        public VertexArray D_Player { get { return _vaPlayer; } }
        public VertexArray D_RectBounds { get { return _vaRectBounds; } }
        public VertexArray D_PixelPerfectBounds { get { return _vaPixelPerfectBounds; } }

        #endregion

        public Player(Vector2f position, Vector2f size, PlayerTexture texture)
        {
            _id = Guid.NewGuid();
            _direction = Direction.South;
            _action = Action.Idle;

            _renderPosition = position;
            _renderSize = size;

            _texture = texture;

            _vaPlayer = GenerateVertexArrayPlayer();
            _vaRectBounds = GenerateVertexArrayRectBounds();
            _vaPixelPerfectBounds = GenerateVertexArrayPixelPerfectBounds();
        }

        private VertexArray GenerateVertexArrayPlayer()
        {
            return new VertexArray(PrimitiveType.TriangleStrip, 0);
        }

        private VertexArray GenerateVertexArrayRectBounds()
        {
            VertexArray array = new(PrimitiveType.LineStrip, 5);

            array[0] = new()
            {
                Position = _renderPosition,
                Color = GameLibConfig.Textures.Color_RectBounds
            };

            array[1] = new()
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = GameLibConfig.Textures.Color_RectBounds
            };

            array[2] = new()
            {
                Position = _renderPosition + _renderSize,
                Color = GameLibConfig.Textures.Color_RectBounds
            };

            array[3] = new()
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = GameLibConfig.Textures.Color_RectBounds
            };

            array[4] = new()
            {
                Position = _renderPosition,
                Color = GameLibConfig.Textures.Color_RectBounds
            };

            return array;
        }

        private VertexArray GenerateVertexArrayPixelPerfectBounds()
        {
            return new VertexArray(PrimitiveType.LineStrip, 0);
        }
    }
}
