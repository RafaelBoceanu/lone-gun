using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class CommonBehavior : MonoBehaviour
    {
        EnemyAI enAI_main;

        // checks for each waypoint
        [HideInInspector]
        public bool _initCheck;
        [HideInInspector]
        public bool _lookAtTarget;
        [HideInInspector]
        public bool _overrideAnimation;
        Quaternion targetRot;

        // main waypoints
        public int indexWaypoint;
        public List<WaypointsBase> waypoints = new List<WaypointsBase>();

        // wait time for waypoints
        public bool circularList;
        bool descendingList;
        float _waitTime;

        void Start()
        {
            enAI_main = GetComponent<EnemyAI>();
        }

        public void DecreaseAlertLevels()
        {
            if (enAI_main.charStats.alertLevel > 0)
            {
                enAI_main.alertTimer += Time.deltaTime * enAI_main.alertMultiplier;

                if (enAI_main.alertTimer > enAI_main.alertTimerIncrement * 2)
                {
                    enAI_main.charStats.alertLevel--;
                    enAI_main.alertTimer = 0;
                }
            }

            if (enAI_main.charStats.alertLevel == 0)
            {
                if (enAI_main.aiStates != EnemyAI.AIstates.patrol)
                {
                    enAI_main.AI_State_Normal();
                }
            }
        }

        public void PatrolBehaviour()
        {
            if (waypoints.Count > 0)
            {
                WaypointsBase curWaypoint = waypoints[indexWaypoint];

                if (!enAI_main.goToPos)
                {
                    enAI_main.charStats.MoveToPosition(curWaypoint.targetDestination.position);
                    enAI_main.goToPos = true;
                }
                else
                {
                    float distanceToTarget = Vector3.Distance(transform.position, curWaypoint.targetDestination.position);

                    if (distanceToTarget < enAI_main.playerControl.stopDistance)
                    {
                        CheckWaypoint(curWaypoint, 0);
                    }
                }
            }
        }

        public void CheckWaypoint(WaypointsBase wp, int listCase)
        {
            #region InitCheck
            if (!_initCheck)
            {
                _lookAtTarget = wp.lookTowards;
                _overrideAnimation = wp.overrideAnimation;
                _initCheck = true;
            }
            #endregion

            if (!wp.stopList)
            {
                switch (listCase)
                {
                    case 0:
                        WaitTimerForEachWP(wp, waypoints);
                        break;
                    case 1:
                        WaitTimerForExtraBehaviour(wp, enAI_main.alertBehavior.onAlertExtraBehaviours);
                        break;
                }
            }

            #region LookTowards
            if (_lookAtTarget)
            {
                enAI_main.playerControl.moveToPosition = false;

                float speedToRotate;

                if (wp.speedToLook < .1f)
                {
                    speedToRotate = 2;
                }
                else
                {
                    speedToRotate = wp.speedToLook;
                }

                Vector3 direction = wp.targetToLookTo.position - transform.position;
                direction.y = 0;

                float angle = Vector3.Angle(transform.forward, direction);

                if (angle > .1f)
                {
                    targetRot = Quaternion.LookRotation(direction);
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * speedToRotate);
                }
                else
                {
                    _lookAtTarget = false;
                }
            }
            #endregion

            #region AnimationOverride
            if (_overrideAnimation)
            {
                if (wp.animationRoutines.Length > 0)
                {
                    for (int i = 0; i < wp.animationRoutines.Length; i++)
                    {
                        enAI_main.charStats.CallFunctionWithString(wp.animationRoutines[i], 0);
                    }
                }
                else
                {
                    Debug.Log("Warning! Animation Override Check but there's no Routines assigned!");
                }

                _overrideAnimation = false;
            }
            #endregion
        }

        void WaitTimerForEachWP(WaypointsBase wp, List<WaypointsBase> listOfWp)
        {
            if (listOfWp.Count > 1)
            {
                #region WaitTime
                _waitTime += Time.deltaTime;

                if (_waitTime > wp.waitTime)
                {
                    if (circularList)
                    {
                        if (listOfWp.Count - 1 > indexWaypoint)
                        {
                            indexWaypoint++;
                        }
                        else
                        {
                            indexWaypoint = 0;
                        }
                    }
                    else
                    {
                        if (!descendingList)
                        {
                            if (listOfWp.Count - 1 == indexWaypoint)
                            {
                                descendingList = true;
                                indexWaypoint--;
                            }
                            else
                            {
                                indexWaypoint++;
                            }
                        }
                        else
                        {
                            if (indexWaypoint > 0)
                            {
                                indexWaypoint--;
                            }
                            else
                            {
                                descendingList = false;
                                indexWaypoint++;
                            }
                        }
                    }

                    _initCheck = false;
                    enAI_main.goToPos = false;
                    _waitTime = 0;
                }

                #endregion
            }
        }

        void WaitTimerForExtraBehaviour(WaypointsBase wp, List<WaypointsBase> listOfWp)
        {
            if (listOfWp.Count > 1)
            {
                #region WaitTime
                _waitTime += Time.deltaTime;

                if (_waitTime > wp.waitTime)
                {
                    if (circularList)
                    {
                        if (listOfWp.Count - 1 > enAI_main.alertBehavior.indexBehaviour)
                        {
                            enAI_main.alertBehavior.indexBehaviour++;
                        }
                        else
                        {
                            enAI_main.alertBehavior.indexBehaviour = 0;
                        }
                    }
                    else
                    {
                        if (!descendingList)
                        {
                            if (listOfWp.Count - 1 == enAI_main.alertBehavior.indexBehaviour)
                            {
                                descendingList = true;
                                enAI_main.alertBehavior.indexBehaviour--;
                            }
                            else
                            {
                                enAI_main.alertBehavior.indexBehaviour++;
                            }
                        }
                        else
                        {
                            if (enAI_main.alertBehavior.indexBehaviour > 0)
                            {
                                enAI_main.alertBehavior.indexBehaviour--;
                            }
                            else
                            {
                                descendingList = false;
                                enAI_main.alertBehavior.indexBehaviour++;
                            }
                        }
                    }


                    _initCheck = false;
                    enAI_main.goToPos = false;
                    _waitTime = 0;
                }

                #endregion
            }
        }
    }
}
