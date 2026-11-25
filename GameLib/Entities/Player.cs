using GameLib.Ressources;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Timers;
using static GameLib.GameLibConfig;

namespace GameLib.Entities
{
    public class Player
    {
        private bool _initialized = false;
        private Guid _id;
        private Direction _direction;
        private Action _action;

        private Vector2f _renderPosition;
        private Vector2f _renderSize;

        private Dictionary<Action, PlayerTexture> _textures;
        private Vector2f[] _pixelPerfectBorderPoints = [];

        private Timer _texTimer;
        private double _texTimerInterval = 200; // [ms]
        private int _texCurrentIndex = 0;

        private VertexArray _vaPlayer;
        private RenderStates _vaPlayerRenderStates;
        private VertexArray _vaRectBounds;
        private VertexArray _vaPixelPerfectBounds;

        #region GET_SET

        public Guid ID { get { return _id; } }
        public Direction Direction { get { return _direction; } }
        public Action Action { get { return _action; } }
        public Vector2f RenderPosition { get { return _renderPosition; } }
        public Vector2f RenderSize { get { return _renderSize; } }
        public FloatRect GlobalBounds { get { return new(_renderPosition, _renderSize); } }
        public Dictionary<Action, PlayerTexture> Textures { get { return _textures; } }
        public Vector2f[] PixelPerfectBorderPoints { get { return _pixelPerfectBorderPoints; } }

        public VertexArray D_Player { get { return _vaPlayer; } }
        public RenderStates D_Player_RenderStates { get { return _vaPlayerRenderStates; } }
        public VertexArray D_RectBounds { get { return _vaRectBounds; } }
        public VertexArray D_PixelPerfectBounds { get { return _vaPixelPerfectBounds; } }

        #endregion

        public Player()
        {
            _id = Guid.NewGuid();
            _direction = Direction.None;
            _action = Action.None;

            _renderPosition = new(0.0f, 0.0f);
            _renderSize = new(0.0f, 0.0f);

            PlayerTextureLine line = new(0, Direction.None, 1);

            PlayerTexture texture = new(GameLibConfig.Textures.Path_Img_TextureNotFound, Action.None, [line]);

            _textures = new()
            {
                { Action.None, texture }
            };

            _texTimer = new(_texTimerInterval);

            _vaPlayer = GenerateVertexArrayPlayer();
            _vaPlayerRenderStates = new(texture.Texture);
            _vaRectBounds = GenerateVertexArrayRectBounds();
            _vaPixelPerfectBounds = GenerateVertexArrayRectBounds();
        }

        public Player(Vector2f position, Vector2f size, Dictionary<Action, PlayerTexture> textures)
        {
            _initialized = true;
            _id = Guid.NewGuid();
            _direction = Direction.South;
            _action = Action.Idle;

            _renderPosition = position;
            _renderSize = size;

            _textures = textures;

            _texTimer = new(_texTimerInterval);
            _texTimer.Elapsed += OnTexTimerElapsed;
            _texTimer.AutoReset = true;
            _texTimer.Enabled = true;

            _vaPlayer = GenerateVertexArrayPlayer();

            PlayerTexture texture;

            if (_textures.ContainsKey(_action))
                texture = _textures[_action];
            else
            {
                PlayerTextureLine line = new(0, Direction.None, 1);
                texture = new(GameLibConfig.Textures.Path_Img_TextureNotFound, Action.None, [line]);
            }

            _vaPlayerRenderStates = new(texture.Texture);

            _vaRectBounds = GenerateVertexArrayRectBounds();
            _vaPixelPerfectBounds = GenerateVertexArrayPixelPerfectBounds();
        }

        /// <summary>
        /// Triggered each time the interval is reached. Update the _texCurrentIndex used to draw the correct texture part for the animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTexTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            ++_texCurrentIndex;

            if (!_textures.ContainsKey(_action))
                return;

            int texCount = _textures[_action].Lines.First(x => x.Direction == _direction).TexturesCount;

            if (_texCurrentIndex >= texCount)
                _texCurrentIndex = 0;
        }

        public void UpdateDrawables(long elapsedMs)
        {
            if (!_textures.ContainsKey(_action))
                return;

            Vector2f[] localPoints = _textures[_action].GetPixelPerfectBorderPoints(_direction, _texCurrentIndex);
            _pixelPerfectBorderPoints = RessourcesHelper.GetGlobalPointsPixelPerfectBorder(localPoints, _renderPosition);

            _vaPlayerRenderStates = new(_textures[_action].Texture);
            _vaPlayer = GenerateVertexArrayPlayer();
            _vaRectBounds = GenerateVertexArrayRectBounds();
            _vaPixelPerfectBounds = GenerateVertexArrayPixelPerfectBounds();
        }

        /// <summary>
        /// Generate the VertexArray representing the Player
        /// </summary>
        /// <returns></returns>
        private VertexArray GenerateVertexArrayPlayer()
        {
            if (!_initialized || _textures.Count == 0 || !_textures.ContainsKey(_action))
                return new(PrimitiveType.TriangleStrip, 0);

            VertexArray array = new(PrimitiveType.TriangleStrip, 4);
            Color color = Color.White;

            PlayerTexture texture = _textures[_action];

            PlayerTextureLine line = texture.Lines.First(x => x.Direction == _direction);

            Vector2f texCoords = new()
            {
                X = _texCurrentIndex * texture.TileSize.X,
                Y = line.LineIndex * texture.TileSize.Y
            };

            array[0] = new()
            {
                Position = _renderPosition + new Vector2f(0.0f, 0.0f),
                Color = color,
                TexCoords = texCoords + new Vector2f(0.0f, 0.0f)
            };

            array[1] = new()
            {
                Position = _renderPosition + new Vector2f(0.0f, _renderSize.Y),
                Color = color,
                TexCoords = texCoords + new Vector2f(0.0f, texture.TileSize.Y)
            };

            array[2] = new()
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, 0.0f),
                Color = color,
                TexCoords = texCoords + new Vector2f(texture.TileSize.X, 0.0f)
            };

            array[3] = new()
            {
                Position = _renderPosition + new Vector2f(_renderSize.X, _renderSize.Y),
                Color = color,
                TexCoords = texCoords + new Vector2f(texture.TileSize.X, texture.TileSize.Y)
            };

            return array;
        }

        /// <summary>
        /// Generate the VertexArray representing the globalBounds of the Player
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Generate the VertexArray representing the pixel perfect bounds of the Player
        /// </summary>
        /// <returns></returns>
        private VertexArray GenerateVertexArrayPixelPerfectBounds()
        {
            if (!_initialized || _pixelPerfectBorderPoints.Length == 0)
                return new(PrimitiveType.LineStrip, 0);

            // add 1 more vertex to close the linestrip and form a loop
            VertexArray array = new VertexArray(PrimitiveType.LineStrip, (uint)_pixelPerfectBorderPoints.Length + 1);

            // draw each point of the line
            for (uint i = 0; i < _pixelPerfectBorderPoints.Length; ++i)
            {
                Vertex vertex = new()
                {
                    Position = _pixelPerfectBorderPoints[i],
                    Color = GameLibConfig.Textures.Color_PixelPerfectBounds
                };

                array[i] = vertex;
            }

            // draw the first point again to close the linestrip loop
            array[(uint)_pixelPerfectBorderPoints.Length] = new Vertex()
            {
                Position = _pixelPerfectBorderPoints[0],
                Color = GameLibConfig.Textures.Color_PixelPerfectBounds
            };

            return array;
        }

        /// <summary>
        /// Move the player of the specified offset. The direction is used to update the texture
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
        public void Move(Vector2f offset, Direction direction)
        {
            _renderPosition += offset;

            // update and check for errors in texture when changing player direction
            if (_direction != direction)
            {
                _direction = direction;

                int texCount = _textures[_action].Lines.First(x => x.Direction == _direction).TexturesCount;
                if (_texCurrentIndex >= texCount)
                    _texCurrentIndex = 0;

                InitializeTexTimer();
            }
        }

        public void SetAction(Action action = Action.Idle)
        {
            if (action == _action)
                return;

            _action = action;

            InitializeTexTimer();
        }

        private void InitializeTexTimer()
        {
            if (!_textures.ContainsKey(_action))
            {
                _texTimer = new(100);
                _texTimer.Elapsed += OnTexTimerElapsed;
                _texTimer.AutoReset = true;
                _texTimer.Enabled = true;

                return;
            }

            PlayerTexture texture = _textures[_action];

            PlayerTextureLine line = texture.Lines.First(x => x.Direction == _direction);

            _texTimer.Stop();
            _texTimer.Dispose();

            _texTimer = new(GameLibConfig.Textures.PlayerAnimationBaseDuration / line.TexturesCount);
            _texTimer.Elapsed += OnTexTimerElapsed;
            _texTimer.AutoReset = true;
            _texTimer.Enabled = true;

        }
    }
}
