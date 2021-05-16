using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FightingTests
    {
        private (GameObject grid, GameObject camera) Start()
        {
            GameObject grid = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));

            return (grid, camera);
        }

        private (GameObject soldier1, GameObject soldier2) CreateSoldiers()
        {
            GameObject soldierObject = MonoBehaviour
                                .Instantiate(Resources.Load<GameObject>("Soldier"));
            soldierObject.GetComponent<ArmyCost>().enabled = false;
            soldierObject.GetComponentInChildren<Squad>().Creator
                        = new Castle(null, Camera.main.gameObject);

            GameObject otherSoldierObject = MonoBehaviour
                                .Instantiate(Resources.Load<GameObject>("Soldier"));
            otherSoldierObject.GetComponent<ArmyCost>().enabled = false;
            otherSoldierObject.GetComponentInChildren<Squad>().Creator
                        = new Castle(null, null);

            return (soldierObject, otherSoldierObject);
        }

        [UnityTest]
        public IEnumerator FightingGoEachOtherTest()
        {
            (GameObject grid, GameObject camera) objects = Start();
            (GameObject soldier1, GameObject soldier2) soldiers = CreateSoldiers();
            soldiers.soldier2.transform.position = soldiers.soldier1.transform.position
                                                    + new Vector3(1, 1);
            yield return new WaitForSeconds(0.1f);

            soldiers.soldier2.transform.position = soldiers.soldier1.transform.position
                                                    + new Vector3(0.75f, 0.75f);
            yield return new WaitForSeconds(1);

            Assert.AreEqual(soldiers.soldier1
                                .GetComponentInChildren<Squad>()
                                .transform.position,
                            soldiers.soldier2
                                .GetComponentInChildren<Squad>()
                                .transform.position);

            MonoBehaviour.Destroy(objects.grid);
            MonoBehaviour.Destroy(objects.camera);
            MonoBehaviour.Destroy(soldiers.soldier1);
            MonoBehaviour.Destroy(soldiers.soldier2);
        }

        [UnityTest]
        public IEnumerator FightingThreeGoingTest()
        {
            (GameObject grid, GameObject camera) objects = Start();
            Squad[] squads = MonoBehaviour.Instantiate(Resources.Load<GameObject>("ThreeSoldiers"))
                                    .GetComponentsInChildren<Squad>();
            yield return new WaitForSeconds(0.5f);

            Assert.AreEqual(squads[0].transform.position,
                            squads[1].transform.position);
            Assert.AreNotEqual(squads[0].transform.position,
                                squads[2].transform.position);

            MonoBehaviour.Destroy(objects.grid);
            MonoBehaviour.Destroy(objects.camera);
            foreach (Squad squad in squads)
            {
                MonoBehaviour.Destroy(squad.transform.parent.gameObject);
            }
        }

        [Test]
        public void Fighting100x100Test()
        {
            int selfUnits = Fighting.FightWith(unitsNum: 100,
                                                strength: 80,
                                                selfUnitsNum: 100,
                                                selfStrength: 80).unitsNum;
            Assert.AreEqual(80, selfUnits);
        }

        [Test]
        public void FightingStrengthIsBloking()
        {
            int selfUnits = Fighting.FightWith(unitsNum: 100,
                                                strength: 80,
                                                selfUnitsNum: 100,
                                                selfStrength: 90).unitsNum;
            Assert.AreEqual(90, selfUnits);
        }

        [Test]
        public void Fighting50x100()
        {
            int selfUnits = Fighting.FightWith(unitsNum: 100,
                                                strength: 74,
                                                selfUnitsNum: 50,
                                                selfStrength: 21).unitsNum;
            Assert.AreEqual(18, selfUnits);
        }

        [Test]
        public void Fighting4x40()
        {
            int selfUnits = Fighting.FightWith(unitsNum: 40,
                                                strength: 27,
                                                selfUnitsNum: 4,
                                                selfStrength: 1).unitsNum;
            Assert.AreEqual(0, selfUnits);
        }

        [Test]
        public void Fighting67x44FullStrenghtTest()
        {
            int selfUnits = Fighting.FightWith(unitsNum: 44,
                                                strength: 44,
                                                selfUnitsNum: 67,
                                                selfStrength: 58).unitsNum;
            Assert.AreEqual(59, selfUnits);
        }
    }
}
