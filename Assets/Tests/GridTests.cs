using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GridTests
    {

        private Grid StartBy3x3()
        {
            Object[,] bytes = new Object[3, 3]
                                    {
                                    {
                                        new Object(0, 0),
                                        new Object(0, 1),
                                        new Object(0, 2)
                                    },
                                    {
                                        new Object(1, 0),
                                        new Object(1, 1),
                                        new Object(1, 2)
                                    },
                                    {
                                        new Object(2, 0),
                                        new Object(2, 1),
                                        new Object(2, 2)
                                    }
                                    };
            Grid grid = new Grid(bytes);
            return grid;
        }

        private void AssertPositionsEqual(Object[] expected, Object[] result)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].x, result[i].x);
                Assert.AreEqual(expected[i].y, result[i].y);
            }
        }

        [Test]
        public void NeighborsTestInCornerLeftUp()
        {
            Grid grid = StartBy3x3();

            Object[] expectedNeighbors = new Object[] {
                new Object(0, 1),
                new Object(1, 0),
                new Object(1, 1)
            };
            Object[] resultNeighbors = grid.GetNearestCells(0, 0);

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestInCornerRightUp()
        {
            Grid grid = StartBy3x3();

            Object[] expectedNeighbors = new Object[] {
                new Object(1, 0),
                new Object(1, 1),
                new Object(2, 1)
            };
            Object[] resultNeighbors = grid.GetNearestCells(2, 0);

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestInCornerLeftDown()
        {
            Grid grid = StartBy3x3();

            Object[] expectedNeighbors = new Object[] {
                new Object(0, 1),
                new Object(1, 1),
                new Object(1, 2)
            };
            Object[] resultNeighbors = grid.GetNearestCells(0, 2);

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestInCornerRightDown()
        {
            Grid grid = StartBy3x3();

            Object[] expectedNeighbors = new Object[] {
                new Object(1, 1),
                new Object(1, 2),
                new Object(2, 1)
            };
            Object[] resultNeighbors = grid.GetNearestCells(2, 2);

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestOnBorderLeft()
        {
            Grid grid = StartBy3x3();

            Object[] resultNeighbors = grid.GetNearestCells(0, 1);
            Object[] expectedNeighbors = new Object[] 
            {
                new Object(0, 0),
                new Object(0, 2),
                new Object(1, 0),
                new Object(1, 1),
                new Object(1, 2),
            };

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestOnBorderRight()
        {
            Grid grid = StartBy3x3();

            Object[] resultNeighbors = grid.GetNearestCells(2, 1);
            Object[] expectedNeighbors = new Object[]
            {
                new Object(1, 0),
                new Object(1, 1),
                new Object(1, 2),
                new Object(2, 0),
                new Object(2, 2),
            };

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestOnBorderUp()
        {
            Grid grid = StartBy3x3();

            Object[] resultNeighbors = grid.GetNearestCells(1, 0);
            Object[] expectedNeighbors = new Object[] 
            {
                new Object(0, 0),
                new Object(0, 1),
                new Object(1, 1),
                new Object(2, 0),
                new Object(2, 1),
            };

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestOnBorderDown()
        {
            Grid grid = StartBy3x3();

            Object[] resultNeighbors = grid.GetNearestCells(1, 2);
            Object[] expectedNeighbors = new Object[]
            {
                new Object(0, 1),
                new Object(0, 2),
                new Object(1, 1),
                new Object(2, 1),
                new Object(2, 2),
            };

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }

        [Test]
        public void NeighborsTestOnCenter()
        {
            Grid grid = StartBy3x3();

            Object[] expectedNeighbors = new Object[]
            {
                new Object(0, 0),
                new Object(0, 1),
                new Object(0, 2),
                new Object(1, 0),
                new Object(1, 2),
                new Object(2, 0),
                new Object(2, 1),
                new Object(2, 2)
            };
            Object[] resultNeighbors = grid.GetNearestCells(1, 1);

            Assert.AreEqual(expectedNeighbors.Length, resultNeighbors.Length);
            AssertPositionsEqual(expectedNeighbors, resultNeighbors);
        }
    }
}
