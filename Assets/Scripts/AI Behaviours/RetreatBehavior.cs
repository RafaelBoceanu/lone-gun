using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown
{
    public class RetreatBehavior : MonoBehaviour
    {
        EnemyAI enAI_main;

        public float delayTillNewBehaviour = 3;
        float _timerTillNewBehaviour;

        bool hasAction;
        bool exitLevel;

        Vector3 targetDestination;
        public List<Transform> retreatPositions = new List<Transform>();
        RetreatActionsBase currentRetreatPosition;
        public List<Transform> exitLevelPositions = new List<Transform>();

        LevelManager lvlManager;

        void Start()
        {
            lvlManager = LevelManager.GetInstance();
            enAI_main = GetComponent<EnemyAI>();

            exitLevelPositions = lvlManager.exitPositions;
        }

        public void RetreatAction()
        {
            if (!hasAction) // decide action
            {
                // search for retreat positions not previously visited
                retreatPositions.Clear();

                for (int i = 0; i < lvlManager.retreatPositions.Count; i++)
                {
                    if (!lvlManager.retreatPositions[i].visited)
                    {
                        retreatPositions.Add(lvlManager.retreatPositions[i].retreatPositions);
                    }
                }

                // if some were found
                if (retreatPositions.Count > 0)
                {
                    // sort positions based on distance
                    enAI_main.attackBehavior.SortPosition(retreatPositions);

                    // the closest retreat position
                    currentRetreatPosition = lvlManager.ReturnRetreatPosition(retreatPositions[0]);

                    // if it's not in use, so someone isn't already going to call reinforcements there
                    if (currentRetreatPosition.inUse == false)
                    {
                        currentRetreatPosition.inUse = true; // put it in use

                        // want to go there
                        targetDestination = currentRetreatPosition.retreatPositions.position;
                        enAI_main.charStats.MoveToPosition(targetDestination);

                        exitLevel = false;
                    }
                    else // if it's in use
                    {
                        // some player is going for reinforcements in that position

                    }

                }
                else // if there are no retreat positions
                {
                    // then exit the level

                    // sort the exit positions by distance
                    enAI_main.attackBehavior.SortPosition(exitLevelPositions);

                    // want to exit the level on the first position in the sorted list
                    targetDestination = exitLevelPositions[0].position;
                    enAI_main.charStats.MoveToPosition(targetDestination);
                    exitLevel = true;
                }

                hasAction = true; // now there is an action
            }
            else // therefore..
            {
                // call the correct function
                if (exitLevel)
                {
                    ExitLevel();
                }
                else
                {
                    RetreatToPosition();
                }
            }
        }

        void RetreatToPosition()
        {
            // check the distance to the retreat position
            if (Vector3.Distance(transform.position, targetDestination) < 1)
            {
                // when reached, the retreat position is visited
                currentRetreatPosition.visited = true;

                // if it has any units in the reinforcements list
                if (currentRetreatPosition.reinforcements.Count > 0)
                {
                    // then alert everyone on the list
                    for (int i = 0; i < currentRetreatPosition.reinforcements.Count; i++)
                    {
                        if (currentRetreatPosition.reinforcements[i].aiStates == EnemyAI.AIstates.patrol ||
                            currentRetreatPosition.reinforcements[i].aiStates == EnemyAI.AIstates.search)
                        {
                            currentRetreatPosition.reinforcements[i].AI_State_HasTarget();
                            currentRetreatPosition.reinforcements[i].target = enAI_main.target;
                            currentRetreatPosition.reinforcements[i].charStats.alertLevel = 10;
                        }
                    }
                }
                else // if there isn't anyone on the list
                {
                    // try to alert any characters in range
                    enAI_main.alliesBehavior.AlertEveryoneInsideRange(20);
                }

                // then probably want the characters to return to battle
                enAI_main.charStats.morale = 100;
                enAI_main.AI_State_HasTarget();

                // reset the variable here
                hasAction = false;
                exitLevel = false;
            }
        }

        void ExitLevel()
        {
            // if character is exiting level
            if (Vector3.Distance(transform.position, targetDestination) < 1)
            {
                // deactivate the game object for now
                transform.GetComponent<EnemyUI>().enUI.SetActive(false);
                gameObject.SetActive(false);
                Debug.Log("Buh-bye");
            }
        }
    }
}
