using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TownTests
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

        private Grid StartBy3x3AndOnlyOnePlaceTown()
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

            bytes[1, 1].Items["town"] = Structs.TownCenter;
            bytes[1, 0].Items["town"] = Structs.House;
            for(int i = 0; i < 3; i++)
            {
                for(int y = 0; y < 3; y++)
                {
                    bytes[i, y].Items["town"] = Structs.House;
                }
                i++;
            }

            Grid grid = new Grid(bytes);
            return grid;
        }

        [Test]
        public void TownSpawnHouseTestCenter()
        {
            Grid grid = StartBy3x3AndOnlyOnePlaceTown();

            ITown town = new Town(grid, 1, 1);
            (int x, int y) returned = town.CreateHouse();
            (int x, int y) expected = (1, 2);

            Assert.AreEqual(expected.x, returned.x);
            Assert.AreEqual(expected.y, returned.y);
        }

        [Test]
        public void TownSpawnHouseTestConrer()
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
            bytes[0, 2].Items["town"] = Structs.TownCenter;
            Grid grid = new Grid(bytes);
            Town town = new Town(grid, 0, 2);

            (int x, int y) resultPosition = town.CreateHouse();
            Assert.IsTrue(resultPosition.x >= 0 && resultPosition.y >= 0, "the first creating false");

            resultPosition = town.CreateHouse();
            Assert.IsTrue(resultPosition.x >= 0 && resultPosition.y >= 0, "the first creating false");

            resultPosition = town.CreateHouse();
            Assert.IsTrue(resultPosition.x >= 0 && resultPosition.y >= 0, "the first creating false");
        }

        [UnityTest]
        public IEnumerator TownTestAllEmpty()
        {
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            yield return new WaitForSeconds(0.1f);

            GridSystem gridSystem = gridObject.GetComponent<GridSystem>();
            yield return null;

            gridSystem.grid.CreateTown(0, 0);
            Town town = new Town(gridSystem.grid, 0, 0);

            Object[] neighbors = gridSystem.grid.GetNearestCells(town.x, town.y);
            yield return null;

            Assert.AreEqual(3, neighbors.Length);
            yield return null;

            MonoBehaviour.Destroy(menu);
            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(gridObject);
        }

        [UnityTest]
        public IEnumerator TownTestsCreating()
        {
            MonoBehaviour.Instantiate(Resources.Load("Menu"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            yield return new WaitForSeconds(0.1f);

            GridSystem gridSystem = gridObject.GetComponent<GridSystem>();
            yield return null;

            TownExpand town = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Town"))
                                                                .GetComponent<TownExpand>();
            town.town = new Town(gridSystem.grid, 0, 0);
            town.GetComponent<TownMoney>().Creator =
                            new Castle(new EmptyCastle(), town.gameObject);
            town.SpeedOfExpand = 0.1f;



            int nearestBuildings = 0;
            foreach (Object @object in gridSystem.grid.GetNearestCells(town.town.x, town.town.y))
            {
                if (@object.Items["town"] == Structs.House)
                {
                    nearestBuildings++;
                }
            }
            Assert.AreEqual(0, nearestBuildings);
            yield return new WaitForSecondsRealtime(0.15f);

            nearestBuildings = 0;
            foreach (Object @object in gridSystem.grid.GetNearestCells(town.town.x, town.town.y))
            {
                if (@object.Items["town"] == Structs.House)
                {
                    nearestBuildings++;
                }
            }
            Assert.AreEqual(1, nearestBuildings);
            yield return new WaitForSecondsRealtime(0.15f);

            nearestBuildings = 0;
            foreach (Object @object in gridSystem.grid.GetNearestCells(town.town.x, town.town.y))
            {
                if (@object.Items["town"] == Structs.House)
                {
                    nearestBuildings++;
                }
            }
            Assert.AreEqual(2, nearestBuildings);
            yield return new WaitForSecondsRealtime(0.15f);

            nearestBuildings = 0;
            foreach (Object @object in gridSystem.grid.GetNearestCells(town.town.x, town.town.y))
            {
                if (@object.Items["town"] == Structs.House)
                {
                    nearestBuildings++;
                }
            }
            Assert.AreEqual(3, nearestBuildings);
            yield return new WaitForSecondsRealtime(0.01f);

            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(town.gameObject);
        }

        [UnityTest]
        public IEnumerator MoneyTakerTest()
        {
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));

            GameObject city = new GameObject();
            city.AddComponent<PacifierTownMoney>();
            GameObject castle = new GameObject();
            castle.transform.position = new Vector3(1, 1);

            GameObject moneyTakerObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("MoneyTaker"));
            moneyTakerObject.transform.SetParent(city.transform);
            MoneyTake moneyTaker = moneyTakerObject.GetComponent<MoneyTake>();
            moneyTaker.Castle = new Castle(new EmptyCastle(), castle);
            yield return new WaitForSeconds(1.4f);

            float distanceToCastle = GetFloatDistance(
                                            castle.transform.position,
                                            moneyTaker.transform.position);
            float distanceToTown = GetFloatDistance(
                                            city.transform.position,
                                            moneyTaker.transform.position);
            Assert.Greater(distanceToTown, distanceToCastle);
            yield return new WaitForSeconds(1.4f);

             distanceToCastle = GetFloatDistance(
                                            castle.transform.position,
                                            moneyTaker.transform.position);
             distanceToTown = GetFloatDistance(
                                            city.transform.position,
                                            moneyTaker.transform.position);
            Assert.Greater(distanceToCastle, distanceToTown);


            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(moneyTakerObject);
            MonoBehaviour.Destroy(city);
        }
        
        private float GetFloatDistnceInOneAxis(float first, float second)
        {
            return 
                Mathf.Abs(first - second);
        }

        private float GetFloatDistance(Vector3 first, Vector3 second)
        {
            return
                GetFloatDistnceInOneAxis(first.x, second.y)
                + GetFloatDistnceInOneAxis(first.y, second.y);
        }
    }
}
