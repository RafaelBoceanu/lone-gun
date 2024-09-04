using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class ChaseBehavior : MonoBehaviour
    {
        EnemyAI enAI_main;

        public float delayTillNewBehaviour = 3;
        float _timerTillNewBehaviour;

        // Start is called before the first frame update
        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }

        public void ChaseBehaviour()
        {
            if (enAI_main.target == null)
            {
                if (!enAI_main.goToPos)
                {
                    enAI_main.charStats.MoveToPosition(enAI_main.lastKnownPosition);
                    enAI_main.charStats.run = true;
                    enAI_main.goToPos = true;
                }
            }
            else
            {
                enAI_main.charStats.MoveToPosition(enAI_main.target.transform.position);
                enAI_main.charStats.run = true;
            }

            if (!enAI_main.sightRaycasts())
            {
                if (enAI_main.target)
                {
                    enAI_main.lastKnownPosition = enAI_main.target.transform.position;
                    enAI_main.target = null;
                }
                else
                {
                    float distanceFromTargetPosition = Vector3.Distance(transform.position, enAI_main.lastKnownPosition);

                    if (distanceFromTargetPosition < 2)
                    {
                        _timerTillNewBehaviour += Time.deltaTime;

                        if (_timerTillNewBehaviour > delayTillNewBehaviour)
                        {
                            enAI_main.AI_State_Search();
                            _timerTillNewBehaviour = 0;
                        }
                    }
                }
            }
        }
    }
}
