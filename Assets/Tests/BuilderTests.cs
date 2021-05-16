using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BuilderTests
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
        public IEnumerator BuilderCanBuildTest()
        {
            GameObject camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Main Camera"));
            GameObject gridObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GridForTests"));
            GameObject menu = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Menu"));

            GameObject town = new GameObject();
            town.transform.position = new Vector3(0, 0);
            yield return null;

            GameObject.FindGameObjectWithTag("Chooser").GetComponent<ChooseSquad>()
                            .Choose(new ChoosedSquad(null, 2));

            Builder builder = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Builder"))
                                                                    .GetComponent<Builder>();
            builder.TownPrefub = Resources.Load<GameObject>("SimpleTown");
            builder.SetParams(
                            gridObject.GetComponent<GridSystem>(),
                            builder.transform.position,
                            new Castle(
                                    new EmptyCastle(),
                                    town),
                            0
                            );

            Assert.AreEqual(0,
                            GameObject.FindObjectsOfType<TownTag>().Length);

            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(1,
                            GameObject.FindObjectsOfType<TownTag>().Length);

            MonoBehaviour.Destroy(camera);
            MonoBehaviour.Destroy(gridObject);
            MonoBehaviour.Destroy(menu);
            if (builder)
            {
                MonoBehaviour.Destroy(builder.gameObject);
            }
        }
    }
}
