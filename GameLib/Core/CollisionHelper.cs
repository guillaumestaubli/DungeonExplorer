using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

using SFML.System;

using GameLib.World;
using GameLib.World.Props;
using GameLib.World.Tiles;
using GameLib.Entities;

namespace GameLib.Core
{
    public static class CollisionHelper
    {
        public static Vector2f DetectAndCorrectPlayerCollisionsWhenMoving(Vector2f offset)
        {
            Player player = World.World.Player;
            Vector2i gridIndex = World.World.ConvertWorldCoordsToGridIndex(player.RenderPosition);
            Level level = World.World.CurrentLevel;

            List<Tile> tiles = GetWallsNearPlayer(gridIndex, level, offset);

            Vector2f[] playerPoints = World.World.Player.PixelPerfectBorderPoints;

            int xAxisCounter = 0;
            int yAxisCounter = 0;

            Vector2f xOffset = new(offset.X, 0.0f);
            Vector2f yOffset = new(0.0f, offset.Y);

            foreach (Tile tile in tiles)
            {
                Direction tileDirFromPlayer = GetTileDirectionFromPlayer(tile, player);

                foreach (Vector2f point in playerPoints)
                {
                    if (tile.GlobalBounds.Contains(point + xOffset))
                    {
                        if (
                            (tileDirFromPlayer == Direction.West && offset.X < 0.0f) ||
                            (tileDirFromPlayer == Direction.East && offset.X > 0.0f)
                            )
                        {
                            ++xAxisCounter;
                        }
                    }

                    if (tile.GlobalBounds.Contains(point + yOffset))
                    {
                        if (
                            (tileDirFromPlayer == Direction.North && offset.Y < 0.0f) ||
                            (tileDirFromPlayer == Direction.South && offset.Y > 0.0f)
                            )
                        {
                            ++yAxisCounter;
                        }
                    }
                }
            }

            if (xAxisCounter != 0)
                offset.X = 0.0f;

            if (yAxisCounter != 0)
                offset.Y = 0.0f;

            return offset;
        }

        private static List<Tile> GetWallsNearPlayer(Vector2i gridIndex, Level level, Vector2f offset)
        {
            List<Tile> tiles = [];

            int radius = GameLibConfig.Collisions.WallCollisionsCheckRadius;

            int x_min = gridIndex.X;
            int x_max = gridIndex.X + 1;
            int y_min = gridIndex.Y;
            int y_max = gridIndex.Y + 1;

            // x values
            if (offset.X < 0.0f)
            {
                x_min -= radius;
            }
            else if (offset.X > 0.0f)
            {
                x_max += radius;
            }

            if (x_min < 0)
                x_min = 0;

            if (x_max >= level.GridSize.X)
                x_max = level.GridSize.X - 1;

            // y values
            if (offset.Y < 0.0f)
            {
                y_min -= radius;
            }
            else if (offset.Y > 0.0f)
            {
                y_max += radius;
            }

            if (y_min < 0)
                y_min = 0;

            if (y_max >= level.GridSize.Y)
                y_max = level.GridSize.Y;

            // get tiles
            for (int x = x_min; x <= x_max; ++x)
            {
                for (int y = y_min; y <= y_max; ++y)
                {
                    Tile tile = level.Tiles[x, y];

                    if (tile.Type == TileType.Wall || tile.Type == TileType.Door)
                        tiles.Add(tile);
                }
            }

            return tiles;
        }

        private static Direction GetTileDirectionFromPlayer(Tile tile, Player player)
        {
            Vector2f tileCenter = tile.RenderPosition + (tile.RenderSize / 2.0f);
            Vector2f playerCenter = player.RenderPosition + (player.RenderSize / 2.0f);

            Direction xDir = Direction.None;
            Direction yDir = Direction.None;

            float xDeltaAbs = Math.Abs(tileCenter.X - playerCenter.X);
            float yDeltaAbs = Math.Abs(tileCenter.Y - playerCenter.Y);

            if (tileCenter.X < playerCenter.X)
            {
                xDir = Direction.West;
            }
            else
            {
                xDir = Direction.East;
            }

            if (tileCenter.Y < playerCenter.Y)
            {
                yDir = Direction.North;
            }
            else
            {
                yDir = Direction.South;
            }

            if (xDeltaAbs > yDeltaAbs) // => horizontal
            {
                return xDir;
            }
            else // => vertical
            {
                return yDir;
            }
        }
    }
}
