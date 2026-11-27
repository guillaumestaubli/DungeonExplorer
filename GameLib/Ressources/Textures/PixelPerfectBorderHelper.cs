using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Ressources.Textures
{
    internal static class PixelPerfectBorderHelper
    {
        /// <summary>
        /// Generate a list of points representing the border of the prop
        /// </summary>
        /// <param name="img">Image representing the prop</param>
        /// <returns></returns>
        public static Vector2f[] GeneratePixelPerfectBorderPoints(Image img)
        {
            List<Vector2u> pointsBorder = [];
            IntRect bounds = new(new(0, 0), (Vector2i)img.Size);

            Vector2f[] outPoints = GeneratePixelPerfectBorderPoints(img, bounds);

            return outPoints;
        }

        public static Vector2f[] GeneratePixelPerfectBorderPoints(Image img, IntRect bounds)
        {
            List<Vector2u> pointsBorder = [];

            List<Vector2u> topPoints = GetTopPoints(img, bounds).OrderBy(x => x.X).Distinct().ToList();
            List<Vector2u> rightPoints = GetRightPoints(img, bounds).OrderBy(x => x.Y).Distinct().ToList();
            List<Vector2u> bottomPoints = GetBottomPoints(img, bounds).OrderByDescending(x => x.X).Distinct().ToList();
            List<Vector2u> leftPoints = GetLeftPoints(img, bounds).OrderByDescending(x => x.Y).Distinct().ToList();

            pointsBorder.AddRange(topPoints);
            pointsBorder.AddRange(rightPoints);
            pointsBorder.AddRange(bottomPoints);
            pointsBorder.AddRange(leftPoints);

            pointsBorder = pointsBorder.Distinct().ToList();

            Vector2f[] outPoints = new Vector2f[pointsBorder.Count];

            for (int i = 0; i < pointsBorder.Count; ++i)
            {
                // pointsBorder[i] - bounds.Position =>
                // used for spritesheets to remove the texture tile position from the local points
                // makes each collection of points look like it's a whole texture
                outPoints[i] = (Vector2f)pointsBorder[i] - (Vector2f)bounds.Position;
            }

            return outPoints;
        }

        /// <summary>
        /// Check if a pixel is transparent using the alpha component of the image
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns>True if pixel.A < 255</returns>
        private static bool IsTransparent(Color pixel)
        {
            return (int)pixel.A < 255;
        }

        /// <summary>
        /// Get the border points from the top of the image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="bounds">Part of the image to check</param>
        /// <returns></returns>
        private static List<Vector2u> GetTopPoints(Image img, IntRect bounds)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u startIndex = (Vector2u)bounds.Position;
            Vector2u endIndex = startIndex + (Vector2u)bounds.Size;
            Vector2u coords = new(0u, 0u);

            // check each column from top to bottom
            for (coords.X = startIndex.X; coords.X < endIndex.X; ++coords.X)
            {
                for (coords.Y = startIndex.Y; coords.Y < endIndex.Y; ++coords.Y)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the top pixel of the column is the border
                    if (coords.Y == 0 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.Y == endIndex.Y - 1 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(0, 1));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the bottom of the image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="bounds">Part of the image to check</param>
        /// <returns></returns>
        private static List<Vector2u> GetBottomPoints(Image img, IntRect bounds)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u startIndex = (Vector2u)bounds.Position;
            Vector2u endIndex = startIndex + (Vector2u)bounds.Size;
            Vector2u coords = new(0u, 0u);

            // check each column from bottom to top
            for (coords.X = startIndex.X; coords.X < endIndex.X; ++coords.X)
            {
                for (coords.Y = endIndex.Y - 1; coords.Y >= startIndex.Y; --coords.Y)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the bottom pixel of the column is the border
                    if (coords.Y == endIndex.Y - 1 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.Y == startIndex.Y && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(0, -1));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the left of the image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="bounds">Part of the image to check</param>
        /// <returns></returns>
        private static List<Vector2u> GetLeftPoints(Image img, IntRect bounds)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u startIndex = (Vector2u)bounds.Position;
            Vector2u endIndex = startIndex + (Vector2u)bounds.Size;
            Vector2u coords = new(0u, 0u);

            // check each line from left to right
            for (coords.Y = startIndex.Y; coords.Y < endIndex.Y; ++coords.Y)
            {
                for (coords.X = startIndex.X; coords.X < endIndex.X; ++coords.X)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the left pixel of the line is the border
                    if (coords.X == startIndex.X && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.X == endIndex.X - 1 && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(1, 0));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        /// <summary>
        /// Get the border points from the right of the image
        /// </summary>
        /// <param name="img"></param>
        /// <param name="bounds">Part of the image to check</param>
        /// <returns></returns>
        private static List<Vector2u> GetRightPoints(Image img, IntRect bounds)
        {
            List<Vector2u> pointsBorder = [];

            Vector2u startIndex = (Vector2u)bounds.Position;
            Vector2u endIndex = startIndex + (Vector2u)bounds.Size;
            Vector2u coords = new(0u, 0u);

            // check each line from right to left
            for (coords.Y = startIndex.Y; coords.Y < endIndex.Y; ++coords.Y)
            {
                for (coords.X = endIndex.X - 1; coords.X >= 0; --coords.X)
                {
                    Color pixel = img.GetPixel(coords);

                    // means the right pixel of the line is the border
                    if (coords.X == endIndex.X - 1 && !IsTransparent(pixel))
                    {
                        pointsBorder.Add(coords);
                        break;
                    }

                    // means no border is present in this column (all the pixels are transparent)
                    if (coords.X == startIndex.X && IsTransparent(pixel))
                        break;

                    // get the adjacent pixel
                    Vector2u nextCoords = (Vector2u)((Vector2i)coords + new Vector2i(-1, 0));
                    Color nextPixel = img.GetPixel(nextCoords);

                    // add the adjacent point to the list if this is the border
                    if (!IsTransparent(nextPixel))
                    {
                        pointsBorder.Add(nextCoords);
                        break;
                    }
                }
            }

            return pointsBorder;
        }

        public static Vector2f[] GetGlobalPointsPixelPerfectBorder(Vector2f[] localPoints, Vector2f renderPosition, Vector2f scale)
        {
            Vector2f[] outArray = new Vector2f[localPoints.Length];

            for (int i = 0; i < localPoints.Length; ++i)
            {
                outArray[i] = new()
                {
                    X = (localPoints[i].X * scale.X) + renderPosition.X,
                    Y = (localPoints[i].Y * scale.Y) + renderPosition.Y
                };
            }

            return outArray;
        }

        public static Vector2f[] GetGlobalPointsPixelPerfectBorder(Vector2f[] localPoints, Vector2f renderPosition)
        {
            Vector2f[] outArray = new Vector2f[localPoints.Length];

            for (int i = 0; i < localPoints.Length; ++i)
            {
                outArray[i] = new()
                {
                    X = (localPoints[i].X) + renderPosition.X,
                    Y = (localPoints[i].Y) + renderPosition.Y
                };
            }

            return outArray;
        }

    }
}
