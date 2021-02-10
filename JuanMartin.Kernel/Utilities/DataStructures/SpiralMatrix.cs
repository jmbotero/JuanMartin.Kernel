using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuanMartin.Kernel.Utilities;

namespace JuanMartin.Kernel.Utilities.DataStructures
{
    public class SpiralMatrix
    {
        [Flags]
        public enum Corner
        {
            topRight = 1,
            topLeft = 2,
            bottomRight = 3,
            bottomLeft = 4
        }

        public enum Direction
        {
            northwest = 1,
            north = 2,
            northeast = 4,
            east = 8,
            southeast = 16,
            south = 32,
            southwest = 64,
            west = 128,
            none = 256
        }

        public enum Point
        {
            center,
            northeast,
            southeast,
            southwest,
            northwest
        }

        private long _diagonalSum;

        public static int Dimension { get; private set; }

        public int[,] Matrix { get; }

        public long DiagonalSum { get => _diagonalSum; private set => _diagonalSum = value; }

        public SpiralMatrix(int dimension)
        {
            if (dimension < 3 && !UtilityMath.IsEven(dimension))
                throw new ArgumentOutOfRangeException("A spiral matrix can only de defined for at least three dimensions and the dimension count must not be even.");

            DiagonalSum = 0;
            Dimension = dimension;
            Matrix = new int[dimension, dimension];
            var value = 1;

            var x = dimension / 2;
            var y = dimension / 2;
            var next = Direction.south;
            var position = Point.center;

            while (value <= dimension * dimension)
            {
                Matrix[x, y] = value;

                if (IsDiagonal(x, y))
                {
                    switch (position)
                    {
                        case Point.center:
                        case Point.northeast:
                            {
                                x++;
                                next = Direction.south;
                                position = Point.southeast;
                                break;
                            }
                        case Point.southeast:
                            {
                                x--;
                                next = Direction.west;
                                position = Point.southwest;
                                break;
                            }
                        case Point.southwest:
                            {
                                y--;
                                next = Direction.north;
                                position = Point.northwest;
                                break;
                            }
                        case Point.northwest:
                            {
                                x++;
                                next = Direction.east;
                                position = Point.northeast;
                                break;
                            }

                        default:
                            throw new Exception("Unexpected Case");
                    }
                    DiagonalSum += value;
                }
                else
                {
                    switch (next)
                    {
                        case Direction.east:
                            {
                                x++;
                                next = Direction.east;
                                break;
                            }
                        case Direction.south:
                            {
                                y++;
                                next = Direction.south;
                                break;
                            }
                        case Direction.west:
                            {
                                x--;
                                next = Direction.west;
                                break;
                            }
                        case Direction.north:
                            {
                                y--;
                                next = Direction.north;
                                break;
                            }

                        default:
                            throw new Exception("Unexpected Case");
                    }
                }

                value++;
            }
        }

        public int[] GetDiagonal(Corner start)
        {
            var diagonal = new List<int>();

            //0,d--0,0
            // |    |
            //d,d--d,0
            switch (start)
            {
                case Corner.topLeft:
                    {
                        for (int i = 0; i < Dimension; i++)
                        {
                            diagonal.Add(Matrix[i, Dimension - 1 - i]);
                        }
                        break;
                    }
                case Corner.topRight:
                    {
                        for (int i = Dimension - 1; i >= 0; i--)
                        {
                            diagonal.Add(Matrix[i, i]);
                        }
                        break;
                    }
                default:
                    throw new Exception("Only two, top-left and top-right, diagonals are available");
            }

            return diagonal.ToArray();
        }

        public static bool IsDiagonal(int x, int y)
        {
            return x == y || x + y == Dimension - 1;
        }
    }
}
