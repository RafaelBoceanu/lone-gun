using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class EnemiesManager : MonoBehaviour
    {
        public List<CharacterStats> AllEnemies = new List<CharacterStats>();
        public List<CharacterStats> EnemiesAvailableToChase = new List<CharacterStats>();
        public List<CharacterStats> EnemiesOnPatrol = new List<CharacterStats>();

        public bool showBehaviours;
        public bool resetAll;
        public bool universalAlert;
        public bool everyoneWhoCanChase;
        public bool patrolsOnly;
        public Transform debugPOI;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (resetAll)
            {
                for (int i = 0; i < AllEnemies.Count; i++)
                {
                    AllEnemies[i].ChangeToNormal();
                }

                resetAll = false;
            }

            if (universalAlert)
            {
                for (int i = 0; i < AllEnemies.Count; i++)
                {
                    AllEnemies[i].ChangeToAlert(debugPOI.position);
                }

                universalAlert = false;
            }

            if (everyoneWhoCanChase)
            {
                for (int i = 0; i < EnemiesAvailableToChase.Count; i++)
                {
                    EnemiesAvailableToChase[i].ChangeToAlert(debugPOI.position);
                }

                everyoneWhoCanChase = false;
            }

            if (patrolsOnly)
            {
                for (int i = 0; i < EnemiesOnPatrol.Count; i++)
                {
                    EnemiesOnPatrol[i].ChangeToAlert(debugPOI.position);
                }

                patrolsOnly = false;
            }

            if (showBehaviours)
            {
                for (int i = 0; i < AllEnemies.Count; i++)
                {
                    AllEnemies[i].GetComponent<EnemyUI>().EnableDisableUI();
                }

                showBehaviours = false;
            }
        }

        public void UpdateListOfChaseEnemies()
        {
            if (AllEnemies.Count > 0)
            {
                for (int i = 0; i < AllEnemies.Count; i++)
                {
                    if (AllEnemies[i].GetComponent<EnemyAI>().canChase)
                    {
                        if (!EnemiesAvailableToChase.Contains(AllEnemies[i]))
                        {
                            EnemiesAvailableToChase.Add(AllEnemies[i]);
                        }
                    }
                }
            }
        }
    }
}