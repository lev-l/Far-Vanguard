using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FindPathTests
    {
        private class EmptyCastle : ICashTaker
        {
            public bool CanBuild()
            {
                return true;
            }

            public bool CanTrain()
            {
                return true;
            }

            public int GiveMoney(int amount)
            {
                return 0;
            }

            public bool Pay(int cost)
            {
                return true;
            }

            public void AddFlagTo(Transform subject)
            {

            }
        }

        [UnityTest]
        public IEnumerator SimpleGoTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            CentralCastle castle = MonoBehaviour
                                    .Instantiate(Resources
                                    .Load<GameObject>("CenterCastle"))
                                    .GetComponent<CentralCastle>();
            MoveState move = new MoveState(new GameObject().transform,
                                            new TargetsStackContainer(new Vector2(5, 5)),
                                            new Castle(castle, castle.gameObject));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();
            List<(int x, int y)> closed = new List<(int x, int y)>();

            (int x, int y) returnedPosition = move.NextCell((5, 5),
                                                            (0, 0),
                                                            ways,
                                                            closed);
            Assert.AreEqual((1, 1), returnedPosition);

            returnedPosition = move.NextCell((5, 5),
                                            (1, 1),
                                            ways,
                                            closed);

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual((2, 2), returnedPosition);
        }

        [UnityTest]
        public IEnumerator WaysCorrectTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            CentralCastle castle = MonoBehaviour
                                    .Instantiate(Resources
                                    .Load<GameObject>("CenterCastle"))
                                    .GetComponent<CentralCastle>();
            MoveState move = new MoveState(new GameObject().transform,
                                new TargetsStackContainer(new Vector2(5, 5)),
                                new Castle(castle, castle.gameObject));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();
            List<(int x, int y)> closed = new List<(int x, int y)>();

            move.NextCell((5, 5), (0, 0), ways, closed);

            Assert.AreEqual((0, 0), ways[(1, 0)]);
            Assert.AreEqual((0, 0), ways[(1, 1)]);
            Assert.AreEqual((0, 0), ways[(0, 1)]);

            move.NextCell((5, 5), (1, 1), ways, closed);

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual((0, 0), ways[(1, 0)]);
            Assert.AreEqual((0, 0), ways[(1, 1)]);
            Assert.AreEqual((0, 0), ways[(0, 1)]);

            Assert.AreEqual((1, 1), ways[(2, 0)]);
            Assert.AreEqual((1, 1), ways[(2, 2)]);
            Assert.AreEqual((1, 1), ways[(0, 2)]);
        }

        [UnityTest]
        public IEnumerator WayCorrectBuildTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            EmptyCastle castle = new EmptyCastle();
            MoveState move = new MoveState(new GameObject().transform,
                                new TargetsStackContainer(new Vector2(5, 5)),
                                new Castle(castle, camera));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();

            (int x, int y)[] returnedWay = move.FindPath((5, 7));
            (int x, int y)[] expectedWay = new (int x, int y)[]
            {
                (0, 0),
                (1, 1),
                (2, 2),
                (3, 3),
                (4, 4),
                (5, 5),
                (5, 6),
                (5, 7)
            };

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual(expectedWay.Length, returnedWay.Length);
            
            for(int i = 0; i < expectedWay.Length; i++)
            {
                Assert.AreEqual(expectedWay[i], returnedWay[i]);
            }
        }

        [UnityTest]
        public IEnumerator MoveWithObstacleTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            CentralCastle castle = MonoBehaviour
                                    .Instantiate(Resources
                                    .Load<GameObject>("CenterCastle"))
                                    .GetComponent<CentralCastle>();
            MoveState move = new MoveState(new GameObject().transform,
                                new TargetsStackContainer(new Vector2(5, 5)),
                                new Castle(castle, castle.gameObject));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();
            GridSystem gridSystem = gridObject.GetComponent<GridSystem>();
            int y = 3;
            Dictionary<string, Structs> land = new Dictionary<string, Structs>();
            land.Add("land", Structs.Sea);
            land.Add("town", Structs.None);
            for(int x = 0; x < 4; x++)
            {
                gridSystem.grid.Cells[x, y] = new Object(land, x, y);
            }

            (int x, int y)[] returnedWay = move.FindPath((5, 7));
            (int x, int y)[] expectedWay = new (int x, int y)[]
            {
                (0, 0),
                (1, 1),
                (2, 2),
                (3, 2),
                (4, 3),
                (5, 4),
                (5, 5),
                (5, 6),
                (5, 7)
            };

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual(expectedWay.Length, returnedWay.Length);

            for (int i = 0; i < expectedWay.Length; i++)
            {
                Assert.AreEqual(expectedWay[i], returnedWay[i]);
            }
        }

        [UnityTest]
        public IEnumerator TurnsCorrectTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            CentralCastle castle = MonoBehaviour
                                    .Instantiate(Resources
                                    .Load<GameObject>("CenterCastle"))
                                    .GetComponent<CentralCastle>();
            MoveState move = new MoveState(new GameObject().transform,
                                new TargetsStackContainer(new Vector2(5, 5)),
                                new Castle(castle, castle.gameObject));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();
            GridSystem gridSystem = gridObject.GetComponent<GridSystem>();
            int y = 3;
            Dictionary<string, Structs> land = new Dictionary<string, Structs>();
            land.Add("land", Structs.Sea);
            land.Add("town", Structs.None);
            for (int x = 0; x < 4; x++)
            {
                gridSystem.grid.Cells[x, y] = new Object(land, x, y);
            }

            (int x, int y)[] returnedTurns = move.GetTurns(move.FindPath((0, 6)));
            (int x, int y)[] expectedTurns = new (int x, int y)[]
            {
                (0, 1),
                (1, 2),
                (3, 2),
                (1, 6),
                (0, 6),
            };

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual(expectedTurns.Length, returnedTurns.Length);

            for (int i = 0; i < expectedTurns.Length; i++)
            {
                Assert.AreEqual(expectedTurns[i], returnedTurns[i]);
            }
        }

        [UnityTest]
        public IEnumerator TurnsCorrectWithoutObstaclesTest()
        {
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            yield return null;
            CentralCastle castle = MonoBehaviour
                                    .Instantiate(Resources
                                    .Load<GameObject>("CenterCastle"))
                                    .GetComponent<CentralCastle>();
            MoveState move = new MoveState(new GameObject().transform,
                                new TargetsStackContainer(new Vector2(1, 1)),
                                new Castle(castle, castle.gameObject));
            yield return null;

            Dictionary<(int x, int y), (int x, int y)> ways =
                    new Dictionary<(int x, int y), (int x, int y)>();

            (int x, int y)[] returnedWay = move.GetTurns(move.FindPath((5, 7)));
            (int x, int y)[] expectedWay = new (int x, int y)[]
            {
                (0, 0),
                (5, 5),
                (5, 7)
            };

            TownsContainer.Towns = new Dictionary<(int x, int y), TownTag>();
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(menu);

            Assert.AreEqual(expectedWay.Length, returnedWay.Length);

            for (int i = 0; i < expectedWay.Length; i++)
            {
                Assert.AreEqual(expectedWay[i], returnedWay[i]);
            }
        }
    }
}
