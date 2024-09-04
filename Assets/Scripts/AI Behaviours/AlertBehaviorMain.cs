using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class AlertBehaviorMain : MonoBehaviour
    {
        public int indexBehaviour;
        public List<WaypointsBase> onAlertExtraBehaviours = new List<WaypointsBase>();

        Quaternion targetRot;
        public bool lookAtPOI;

        // general behaviour variables
        public float delayTillNewBehaviour = 3;
        float _timerTillNewBehaviour;

        public string[] alertLogic;

        EnemyAI enAI_main;

        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }

        public void AlertBehaviourMain()
        {
            if (!lookAtPOI)
            {
                Vector3 directionToLookTo = enAI_main.pointOfInterest - transform.position;
                directionToLookTo.y = 0;

                float angle = Vector3.Angle(transform.forward, directionToLookTo);

                if (angle > .1f)
                {
                    targetRot = Quaternion.LookRotation(directionToLookTo);
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 3);
                }
                else
                {
                    lookAtPOI = true;
                }
            }

            _timerTillNewBehaviour += Time.deltaTime;

            if (_timerTillNewBehaviour > delayTillNewBehaviour)
            {
                if (alertLogic.Length > 0)
                {
                    enAI_main.ChangeAIBehaviour(alertLogic[0], 0);
                }

                _timerTillNewBehaviour = 0;
            }
        }


        public void OnAlertExtraBehaviours()
        {
            if (onAlertExtraBehaviours.Count > 0)
            {
                WaypointsBase curBehaviour = onAlertExtraBehaviours[indexBehaviour];

                if (!enAI_main.goToPos)
                {
                    enAI_main.charStats.MoveToPosition(curBehaviour.targetDestination.position);
                    enAI_main.goToPos = true;
                }
                else
                {
                    float distanceToTarget = Vector3.Distance(transform.position, curBehaviour.targetDestination.position);

                    if (distanceToTarget < enAI_main.playerControl.stopDistance)
                    {
                        enAI_main.commonBehavior.CheckWaypoint(curBehaviour, 1);
                    }

                }
            }
        }
    }
}