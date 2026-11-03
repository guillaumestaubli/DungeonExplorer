using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLib.World.Tiles
{
    public class Tile
    {
        #region ATTTRIBUTES

        /// <summary>
        /// Unique identifier of the tile
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Position of the tile in the tiles array of the level
        /// </summary>
        private Vector2i _gridIndex;

        /// <summary>
        /// Position of the tile on the screen. Origin is top/left corner
        /// </summary>
        private Vector2f _screenPosition;

        /// <summary>
        /// Type of the tile
        /// </summary>
        private TileType _type;

        #endregion

        #region GET_SET

        /// <summary>
        /// Unique identifier of the tile
        /// </summary>
        public Guid ID { get { return _id; } }

        /// <summary>
        /// Position of the tile in the tiles array of the level
        /// </summary>
        public Vector2i GridIndex { get { return _gridIndex; } }

        /// <summary>
        /// Position of the tile on the screen. Origin is top/left corner
        /// </summary>
        public Vector2f ScreenPosition { get { return _screenPosition; } }

        /// <summary>
        /// Type of the tile
        /// </summary>
        public TileType Type { get { return _type; } }

        #endregion

    }
}
