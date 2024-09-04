using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class AlliesBehavior : MonoBehaviour
    {
        EnemyAI enAI_main;

        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }

        public void AlertAllies()
        {
            if (enAI_main.AlliesNear.Count > 0)
            {
                for (int i = 0; i < enAI_main.AlliesNear.Count; i++)
                {
                    if (enAI_main.AlliesNear[i].aiStates == EnemyAI.AIstates.patrol)
                    {
                        enAI_main.AlliesNear[i].AI_State_HasTarget();
                        enAI_main.AlliesNear[i].target = enAI_main.target;
                        enAI_main.AlliesNear[i].charStats.alertLevel = 10;
                    }
                }
            }
        }

        public void AlertEveryoneInsideRange(float range)
        {
            LayerMask mask = 1 << gameObject.layer;

            Collider[] cols = Physics.OverlapSphere(transform.position, range, mask);

            for (int i = 0; i < cols.Length; i++)
            {
                EnemyAI otherAi = cols[i].transform.GetComponent<EnemyAI>();

                if (otherAi.aiStates == EnemyAI.AIstates.patrol)
                {
                    otherAi.AI_State_HasTarget();
                    otherAi.target = enAI_main.target;
                    otherAi.charStats.alertLevel = 10;
                }
            }
        }

        public void DecreaseAlliesMorale(int amount)
        {
            if (enAI_main.AlliesNear.Count > 0)
            {
                for (int i = 0; i < enAI_main.AlliesNear.Count; i++)
                {
                    enAI_main.AlliesNear[i].charStats.morale -= amount;
                }
            }
        }

        public void IncreaseAlliesSupression(int amount)
        {
            if (enAI_main.AlliesNear.Count > 0)
            {
                for (int i = 0; i < enAI_main.AlliesNear.Count; i++)
                {
                    enAI_main.AlliesNear[i].charStats.supressionLevel += amount;
                }
            }
        }
    }
}
